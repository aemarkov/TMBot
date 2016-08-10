using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.Models.TM
{
	
	/// <summary>
	/// Информация о предмете в магазине
	/// </summary>
	public class ItemInfo
	{
		public string classid { get; set; }
		public string instanceid { get; set; }
		public object our_market_instanceid { get; set; }
		public string market_name { get; set; }
		public string name { get; set; }
		public string market_hash_name { get; set; }
		public string rarity { get; set; }
		public string quality { get; set; }
		public string type { get; set; }
		public string mtype { get; set; }
		public string slot { get; set; }
		public List<Description> description { get; set; }
		public List<Tag> tags { get; set; }
		public string hash { get; set; }
		public string min_price { get; set; }

		/// <summary>
		/// Продажи этого предмета
		/// </summary>
		public List<Offer> offers { get; set; }

		/// <summary>
		/// Ордеры на покупку этого прмдета
		/// </summary>
		public List<BuyOffer> buy_offers { get; set; }



		public class Description
		{
			public string type { get; set; }
			public string value { get; set; }
		}

		public class Tag
		{
			public string internal_name { get; set; }
			public string name { get; set; }
			public string category { get; set; }
			public string category_name { get; set; }
			public string color { get; set; }
			public List<Value> value { get; set; }
		}

		public class Value
		{
			public string name { get; set; }
			public string link { get; set; }
			public bool link_true { get; set; }
			public int link_updated { get; set; }
		}

		public class Offer
		{
			public decimal price { get; set; }
			public int count { get; set; }
			public int my_count { get; set; }
		}

		public class BuyOffer
		{
			public decimal o_price { get; set; }
			public int c { get; set; }
			public int my_count { get; set; }
		}
	}

	

	
}
