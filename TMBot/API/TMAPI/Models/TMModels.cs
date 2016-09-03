using System;
using System.Collections.Generic;

namespace TMBot.API.TMAPI.Models
{
	/// <summary>
	/// Ответ на запрос ключа для подписки на вебсокеты
	/// </summary>
	public class WebSocketAuth
	{
		public string wsAuth { get; set; }
		public bool success { get; set; }
	}

	/// <summary>
	/// Трейд (выставленный на продажу предмет)
	/// </summary>
	public class Trade
	{
		public string ui_id { get; set; }
		public string i_name { get; set; }
		public string i_market_name { get; set; }
		public string i_name_color { get; set; }
		public string i_rarity { get; set; }
		public object i_descriptions { get; set; }

		/// <summary>
		/// Статус трейда:
		/// 1 - выставляется
		/// 2 - Вы продали вещь и должны ее передать боту
		/// 3 - Ожидание передачи боту купленной вами вещи от продавца
		/// 4 - Вы можете забрать купленную вещь
		/// </summary>
		public int ui_status { get; set; }

		public string he_name { get; set; }
		public double ui_price { get; set; }
		public string i_classid { get; set; }

        [Obsolete]
		public string i_instanceid { get; set; }
		public string ui_real_instance { get; set; }
		public string i_quality { get; set; }
		public string i_market_hash_name { get; set; }
		public int i_market_price { get; set; }
		public int position { get; set; }
		public int min_price { get; set; }

		/// <summary>
		/// ID бота, с которым надо произвести обмен, чтобы получить
		/// этот предмет
		/// 
		/// 1 в случае передачи предмета ОТ ТЕБЯ БОТУ
		/// </summary>
		public string ui_bid { get; set; }
		public string ui_asset { get; set; }
		public string type { get; set; }
		public string ui_price_text { get; set; }
		public bool min_price_text { get; set; }
		public string i_market_price_text { get; set; }
		public int offer_live_time { get; set; }
		public string placed { get; set; }
	}

	
	/// <summary>
	/// Результат выставление предмета на продажу
	/// </summary>
	public class SetPriceResponse
	{
		public int result { get; set; }

		/// <summary>
		/// ID выставленного предмета, где-то нужно
		/// </summary>
		public int item_id { get; set; }
		public double price { get; set; }
		public string price_text { get; set; }
		public string status { get; set; }
		public int position { get; set; }
		public bool success { get; set; }
	}

	/// <summary>
	/// Ответ на ItemRequest
	/// (запрос на передачу боту\от бота)
	/// </summary>
	public class ItemRequestResponse
	{
		public bool success { get; set; }
		public string trade { get; set; }
		public string nick { get; set; }
		public int botid { get; set; }
		public string profile { get; set; }
		public string secret { get; set; }
		public List<string> items { get; set; }
	}

	/// <summary>
	/// Результат изменения цены ордера
	/// </summary>
	public class UpdateOrderResponse
	{
		public bool success { get; set; }
	}

	

	/// <summary>
	/// Резултат PingPong
	/// </summary>
	public class PingPongResponse
	{
		public bool success { get; set; }
		public string ping { get; set; }
	}
}
