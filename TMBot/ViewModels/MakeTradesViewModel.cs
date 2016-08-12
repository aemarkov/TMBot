using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TMBot.API.Exceptions;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницы выставления ордеров
	/// </summary>
	public class MakeTradesViewModel
	{
		//Список предметов в инвентаре стим
		public ObservableCollection<InventoryItemViewModel> InventoryItems { get; private set; }

		public IAsyncCommand UpdateInventoryCommand { get; private set; }
		public IAsyncCommand BeginCommand { get; set; }


		// За какой процент цены выставлять
		public float PricePercentage { get; set; } = 1;

		public MakeTradesViewModel()
		{
			InventoryItems = new ObservableCollection<InventoryItemViewModel>();

			UpdateInventoryCommand = AsyncCommand.Create(update_inventory);
			BeginCommand = AsyncCommand.Create(begin);

			//load_inventory_items();
		}


		#region INVENTORY

		//Обновляет инвентарь
		private async Task update_inventory(object param)
		{
            await load_inventory_items();
		}


		//Загружает инвентарь стима
		private async Task load_inventory_items()
		{
			await load_inventory_game<CSSteamAPI, CSTMAPI>();
		}

		//Загружает инвентарь конкретной игры
		//Да, я понимаю, что надо было завернуть эту асинхронную операцию в 
		//INotifyPropertyChanged и биндится прямо к ней, а не
		//менять свойство из нее
		private async Task load_inventory_game<TSteamAPI, TTMAPI>() where TSteamAPI : ISteamAPI
																	 where TTMAPI : ITMAPI
		{
			Log.d("Получение инвентаря Steam...");

			ISteamAPI steamApi = SteamFactory.GetInstance<SteamFactory>().GetAPI<TSteamAPI>();
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			var inventory = await steamApi.GetSteamInventoryAsync();

            IList<Trade> trades = tmApi.GetTrades();

			//Число предметов, которые уже выставляются
			int sellingCount = 0;

			InventoryItems.Clear();

			//Составляем список инвентаря
			foreach (var item in inventory.rgInventory)
			{
				var rgItem = item.Value;
				var description = inventory.rgDescriptions[rgItem.classid + "_" + rgItem.instanceid];

				string imageUrl = "http://cdn.steamcommunity.com/economy/image/" + description.icon_url;

			    bool isSelling = trades.Any(x => x.i_classid == rgItem.classid && x.ui_real_instance == rgItem.instanceid);

				if (isSelling)
					sellingCount++;

				InventoryItemViewModel inventoryItemViewModel = new InventoryItemViewModel() {	Name = description.name,
																				ImageUrl = imageUrl,
																				IsSelling = isSelling,
																				ClassId = rgItem.classid,
																				IntanceId = rgItem.instanceid};
				InventoryItems.Add(inventoryItemViewModel);
			}

			Log.d("Загружено {0} предметов, выставляются: {1}", inventory.rgInventory.Count, sellingCount);
		}

		#endregion

		#region SELLING

		//Начинает выставлять
		private async Task begin(object param)
		{
			await begin_sell_game<CSTMAPI>(InventoryItems);
		}

		//Начинает выставлять предметы определенной площадки
		private async Task begin_sell_game<TTMAPI>(ICollection<InventoryItemViewModel> items) where TTMAPI : ITMAPI
		{
		    if (items == null)
		    {
		        Log.w("Предметы в инвентаре не загружены");
		        return;
		    }

		    Log.d("Выставляются предметы...");

		    if (items.Count == 0)
		    {
		        Log.w("Нет предметов для выставления: в инвентаре нет предметов");
		        return;
		    }


		    var tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
            int count = 0;

            foreach (var item in InventoryItems.Where(item => !item.IsSelling))
            {
                await FixedTimeCall.Call(() =>
                {
                    decimal price = PriceCounter.GetMinSellPrice<TTMAPI>(item.ClassId, item.IntanceId);
                    price = (decimal)PricePercentage * price;
                    tmApi.SetNewItem(item.ClassId, item.IntanceId, (int)price);
                    Log.d("Предмет {0}_{1} выставлен. за цену {2} коп.", item.ClassId, item.IntanceId, price);
                    count++;
                });
            }



            if (count==0)
				Log.w("Нет предметов для выставления: все уже выставляются");
		}

		#endregion
	}
}
