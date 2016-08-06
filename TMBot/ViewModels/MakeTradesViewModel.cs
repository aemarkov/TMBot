using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницы выставления ордеров
	/// </summary>
	public class MakeTradesViewModel
	{
		public ObservableCollection<SteamInventoryItem> InventoryItems { get; set; }

		public MakeTradesViewModel()
		{

		}

		private void load_inventory_items()
		{

		}
	}
}
