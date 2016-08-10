using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMBot.API;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.Models.Steam;
using TMBot.Models.TM;
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
		public ObservableCollection<InventoryItem> InventoryItems { get; private set; }

		//Список предметов, которые надо выставить на продажу
		//private readonly IList<RgInventoryItem> _itemsToSell;

		//public RelayCommands UpdateInventoryCommand { get; set; }
		public IAsyncCommand UpdateInventoryCommand { get; private set; }
		public RelayCommands BeginCommand { get; set; }


		// За какой процент цены выставлять
		public float PricePercentage { get; set; } = 1;

		public MakeTradesViewModel()
		{
			InventoryItems = new ObservableCollection<InventoryItem>();

			UpdateInventoryCommand = AsyncCommand.Create(update_inventory);
			BeginCommand = new RelayCommands(begin);

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

			await Task.Delay(3000);

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

				InventoryItem inventoryItem = new InventoryItem() {	Name = description.name,
																				ImageUrl = imageUrl,
																				IsSelling = isSelling,
																				ClassId = rgItem.classid,
																				IntanceId = rgItem.instanceid};
				InventoryItems.Add(inventoryItem);
			}

			Log.d("Загружено {0} предметов, выставляются: {1}", inventory.rgInventory.Count, sellingCount);
		}

		#endregion

		#region SELLING

		//Начинает выставлять
		private void begin(object param)
		{
			begin_sell_game<CSTMAPI>(InventoryItems);
		}

		//Начинает выставлять предметы определенной площадки
		private void begin_sell_game<TTMAPI>(ICollection<InventoryItem> items) where TTMAPI : ITMAPI
		{
			Log.d("Выставляются предметы...");

			if (items.Count == 0)
				Log.w("Нет предметов для выставления: в инвентаре нет предметов");

			var tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
			int count = 0;

			foreach (var item in InventoryItems)
			{
				if(item.IsSelling)
					continue;

				decimal price = item.TMPrice;
				price = (decimal)PricePercentage * price;
				tmApi.SetNewItem(item.ClassId, item.IntanceId, (int)price);

				Log.d("Предмет {0}_{1} выставлен. за цену {2} коп.", item.ClassId, item.IntanceId, price);
				count++;
			}

			if(count==0)
				Log.w("Нет предметов для выставления: все уже выставляются");
		}

		#endregion
	}
}
