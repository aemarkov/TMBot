using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.ViewModels.ViewModels
{
	/// <summary>
	/// Удобное представление объекта инвентаря стим
	/// Комбинация важных параметров RgItem и RgDescription
	/// </summary>
	public class InventoryItemViewModel
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public bool IsSelling { get; set; }
		public string ClassId { get; set; }
		public string IntanceId { get; set; }
	}

    /// <summary>
    /// Представление предмета для покупки/продажия
    /// </summary>
    public class TradeViewModel : InventoryItemViewModel
    {
        public int TMPrice { get; set; }
        public int MyPrice { get; set; }
        public int PriceLimit { get; set; }
        public int? CountLimint { get; set; }
    }
}
