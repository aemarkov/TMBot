using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
		private IList<RgInventoryItem> items_to_sell;

		public RelayCommands UpdateInventoryCommand { get; set; }
		public RelayCommands BeginCommand { get; set; }


		// За какой процент цены выставлять
		public float PricePercentage { get; set; } = 1;

		public MakeTradesViewModel()
		{
			InventoryItems = new ObservableCollection<SteamInventoryItem>();
			items_to_sell = new List<RgInventoryItem>();

			UpdateInventoryCommand = new RelayCommands(x => update_inventory(x));
			BeginCommand = new RelayCommands(x => begin(x));

			load_inventory_items();

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

			ISteamAPI steam_api = SteamFactory.GetInstance<SteamFactory>().GetAPI<TSteamAPI>();
			ITMAPI tm_api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			var inventory = await steam_api.GetSteamInventoryAsync();
			IList<Trade> trades = tm_api.GetTrades();

			InventoryItems.Clear();
			items_to_sell.Clear();

			//Составляем список инвентаря
			foreach (var item in inventory.rgInventory)
			{
				var rg_item = item.Value;
				var description = inventory.rgDescriptions[rg_item.classid + "_" + rg_item.instanceid];

				string image_url = "http://cdn.steamcommunity.com/economy/image/" + description.icon_url;

				bool is_selling = trades.Any(x => x.i_classid == rg_item.classid && x.i_instanceid == rg_item.instanceid);
				if (!is_selling)
					items_to_sell.Add(rg_item);

				SteamInventoryItem inventory_item = new SteamInventoryItem() { Name = description.name, ImageUrl = image_url, IsSelling = is_selling };
				InventoryItems.Add(inventory_item);
			}

			Log.d("Загружено {0} предметов, не выставляются: {1}", inventory.rgInventory.Count, items_to_sell.Count);
		}

		#endregion

		#region SELLING

		//Начинает выставлять
		private void begin(object param)
		{
			begin_sell_game<CSTMAPI>(items_to_sell);
		}

		//Начинает выставлять предметы определенной площадки
		private void begin_sell_game<TTMAPI>(IList<RgInventoryItem> items_to_sell) where TTMAPI : ITMAPI
		{
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
