using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.ViewModels.ViewModels
{
	/// <summary>
	/// Удобное представление объекта инвентаря стим
	/// </summary>
	public class SteamInventoryItem
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public bool IsSelling { get; set; }
		public string ClassID_InstanceID { get; set; }

		public decimal TMPrice { get; set; }
	}
}
