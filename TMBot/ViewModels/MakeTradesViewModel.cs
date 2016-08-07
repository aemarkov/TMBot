using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницы выставления ордеров
	/// </summary>
	public class MakeTradesViewModel
	{
		public ObservableCollection<SteamInventoryItem> InventoryItems { get; private set; }

		public MakeTradesViewModel()
		{
			InventoryItems = new ObservableCollection<SteamInventoryItem>();
			load_inventory_items();

		}

		private async void load_inventory_items()
		{
			ISteamAPI api = SteamFactory.GetInstance<SteamFactory>().GetAPI<CSSteamAPI>();
			var inventory = await api.GetSteamInventoryAsync();

			InventoryItems.Clear();

			foreach(var item in inventory.rgInventory)
			{
				var rg_item = item.Value;
				var description = inventory.rgDescriptions[rg_item.classid+"_"+rg_item.instanceid];

				string url = "http://cdn.steamcommunity.com/economy/image/" + description.icon_url;

				SteamInventoryItem inventory_item = new SteamInventoryItem() { Name = description.name, ImageUrl = url };
				InventoryItems.Add(inventory_item);
			}
		}
	}
}
