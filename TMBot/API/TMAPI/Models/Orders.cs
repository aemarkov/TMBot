using System.Collections.Generic;

namespace TMBot.API.TMAPI.Models
{
	public class OrdersList
	{
		public bool success { get; set; }
		public List<Order> Orders { get; set; }

		public class Order
		{
			public string i_classid { get; set; }
			public string i_instanceid { get; set; }
			public string i_market_hash_name { get; set; }
			public string i_market_name { get; set; }
			public string o_price { get; set; }
			public string o_state { get; set; }
		}
	}
}
