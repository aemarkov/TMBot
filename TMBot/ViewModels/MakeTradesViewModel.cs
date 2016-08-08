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
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницы выставления ордеров
	/// </summary>
	public class MakeTradesViewModel
	{
		//Список предметов в инвентаре стим
		public ObservableCollection<SteamInventoryItem> InventoryItems { get; private set; }

		//Список предметов, которые надо выставить на продажу
		private readonly IList<RgInventoryItem> _itemsToSell;

		public RelayCommands UpdateInventoryCommand { get; set; }
		public RelayCommands BeginCommand { get; set; }


		// За какой процент цены выставлять
		public float PricePercentage { get; set; } = 1;

		public MakeTradesViewModel()
		{
			InventoryItems = new ObservableCollection<SteamInventoryItem>();
			_itemsToSell = new List<RgInventoryItem>();

			UpdateInventoryCommand = new RelayCommands(x => update_inventory(x));
			BeginCommand = new RelayCommands(x => begin(x));

			load_inventory_items();
			Debug.WriteLine("Ffff");

		}


		#region INVENTORY

		//Обновляет инвентарь
		private void update_inventory(object param)
		{
			load_inventory_items();
		}


		//Загружает инвентарь стима
		private async void load_inventory_items()
		{
			await load_inventory_game<CSSteamAPI, CSTMAPI>();
		}

		//Загружает инвентарь конкретной игры
		private async Task load_inventory_game<TSteamAPI, TTMAPI>() where TSteamAPI : ISteamAPI
																	 where TTMAPI : ITMAPI
		{
			Log.d("Получение инвентаря Steam...");

			ISteamAPI steamApi = SteamFactory.GetInstance<SteamFactory>().GetAPI<TSteamAPI>();
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			var inventory = await steamApi.GetSteamInventoryAsync();
			IList<Trade> trades = tmApi.GetTrades();

			InventoryItems.Clear();
			_itemsToSell.Clear();


			//Составляем список инвентаря
			foreach (var item in inventory.rgInventory)
			{
				var rgItem = item.Value;
				var description = inventory.rgDescriptions[rgItem.classid + "_" + rgItem.instanceid];

				string imageUrl = "http://cdn.steamcommunity.com/economy/image/" + description.icon_url;

				bool isSelling = trades.Any(x => x.i_classid == rgItem.classid && x.ui_real_instance == rgItem.instanceid);
				if (!isSelling)
					_itemsToSell.Add(rgItem);

				decimal price;
				decimal? minPrice = PriceCounter.GetMinSellPrice<TTMAPI>(rgItem.classid, rgItem.instanceid);
				if (minPrice == null)
				{
					//Такого предмета на площадке нет, надо определять по стиму
					//TODO: определять цену по стиму
					price = -1;
					Log.e("Предмет {0}_{1} не найден на площадке", rgItem.classid, rgItem.instanceid);
				}
				else
					price = (int)minPrice;

				SteamInventoryItem inventoryItem = new SteamInventoryItem() {	Name = description.name,
																				ImageUrl = imageUrl,
																				IsSelling = isSelling,
																				ClassID_InstanceID =rgItem.classid+"_"+rgItem.instanceid,
																				TMPrice=price/100} ;
				InventoryItems.Add(inventoryItem);
			}

			Log.d("Загружено {0} предметов, не выставляются: {1}", inventory.rgInventory.Count, _itemsToSell.Count);
		}

		#endregion

		#region SELLING

		//Начинает выставлять
		private void begin(object param)
		{
			begin_sell_game<CSTMAPI>(_itemsToSell);
		}

		//Начинает выставлять предметы определенной площадки
		private void begin_sell_game<TTMAPI>(IList<RgInventoryItem> items_to_sell) where TTMAPI : ITMAPI
		{

			if(items_to_sell.Count==0)
			{
				Log.w("Нет предметов для выставления или все уже выставлены");
				return;
			}

			Log.d("Выставляются предметы...");

			var tm_api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			foreach (var item in items_to_sell)
			{
				//Определяем минимаьную цену такого же предмета на площадке
				decimal price;
				decimal? _price = PriceCounter.GetMinSellPrice<TTMAPI>(item.classid, item.instanceid);
				if (_price == null)
				{
					//Такого предмета на площадке нет, надо определять по стиму
					//TODO: определять цену по стиму
					price = -1;
					Log.e("Предмет {0}_{1} не найден на площадке", item.classid, item.instanceid);
				}
				else
					price = (int)_price;

				price = (decimal)PricePercentage * price;
				tm_api.SetNewItem(item.classid, item.instanceid, (int)price);

				Log.d("Предмет {0}_{1} выставлен. за цену {2} коп.", item.classid, item.instanceid, price);
			}
		}

		#endregion
	}
}
