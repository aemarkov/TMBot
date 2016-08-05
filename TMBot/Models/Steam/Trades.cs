using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.Models.Steam
{
	/// <summary>
	/// Список входящих и исходящих трейдов (обменов) в стим
	/// </summary>
	public class Trades
	{
		public Response response { get; set; }
	}

	/// <summary>
	/// Список входящих и исходящих трейдов (обменов) в стим
	/// </summary>
	public class Response
	{
		public List<TradeOffer> trade_offers_sent { get; set; }
		public List<TradeOffer> trade_offers_received { get; set; }
	}

	/// <summary>
	/// Входяшее или исходящее предложение обмена
	/// </summary>
	public class TradeOffer
	{
		public string tradeofferid { get; set; }

		/// <summary>
		/// ID аккаунта с кем происходит обмен
		/// </summary>
		public int accountid_other { get; set; }

		/// <summary>
		/// Сообщение, отправленное человеком\ботом
		/// </summary>
		public string message { get; set; }

		public int expiration_time { get; set; }
		public int trade_offer_state { get; set; }
		public List<TransferingItem> items_to_give { get; set; }
		public List<TransferingItem> items_to_receive { get; set; }
		public bool is_our_offer { get; set; }
		public int time_created { get; set; }
		public int time_updated { get; set; }
		public bool from_real_time_trade { get; set; }
		public int escrow_end_date { get; set; }
		public int confirmation_method { get; set; }
	}

	/// <summary>
	/// Передаваемый предмет
	/// </summary>
	public class TransferingItem
	{
		public string appid { get; set; }
		public string contextid { get; set; }
		public string assetid { get; set; }
		public string classid { get; set; }
		public string instanceid { get; set; }
		public string amount { get; set; }
		public bool missing { get; set; }
	}
}
