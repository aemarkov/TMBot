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
	public class InventoryItem
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public bool IsSelling { get; set; }
		public string ClassId { get; set; }
		public string IntanceId { get; set; }
	}

    /// <summary>
    /// Представление товара при продажах\ордерах
    /// </summary>
    public class TradeItem : InventoryItem
    {
        /// <summary>
        /// Мин/макс цена на площадке
        /// </summary>
        public decimal TMPrice { get; set; }

        /// <summary>
        /// Выставленная ботом цена
        /// </summary>
        public decimal MyPrice { get; set; }

        /// <summary>
        /// Ограничения по цене
        /// </summary>
        public decimal PriceLimit { get; set; }
    }
}
