using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.Models.TM;

namespace TMBot.Utilities
{
	/// <summary>
	/// Расчитывает цену предмета
	/// </summary>
	public static class PriceCounter
	{
		/// <summary>
		/// Определяет минимальную цену среди продаж на площадке
		/// </summary>
		/// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
		/// <param name="classid">параметр предмета</param>
		/// <param name="instanceid">параметр предмета</param>
		/// <returns></returns>
		public static decimal? GetMinSellPrice <TTMAPI>(string classid, string instanceid) where TTMAPI: ITMAPI
		{
			ITMAPI tm_api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo item_info = tm_api.GetItemInfo(classid, instanceid);

			if (item_info.offers == null)
				return null;

			decimal min_price = item_info.offers.First().price;
			foreach(var offer in item_info.offers)
			{
				decimal price = offer.price;
				if (price < min_price)
					min_price = price;
			}

			return min_price;
		}

		/// <summary>
		/// Возвращает максимульную цену ордера на площадке
		/// </summary>
		/// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
		/// <param name="classid">параметр предмета</param>
		/// <param name="instanceid">параметр предмета</param>
		/// <returns></returns>
		public static decimal? GetMaxOfferPrice<TTMAPI>(string classid, string instanceid) where TTMAPI : ITMAPI
		{
			ITMAPI tm_api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo item_info = tm_api.GetItemInfo(classid, instanceid);

			if (item_info.offers == null)
				return null;

			decimal max_price = item_info.buy_offers.First().o_price;
			foreach (var buy_offer in item_info.buy_offers)
			{
				decimal price = buy_offer.o_price;
				if (price > max_price)
					max_price = price;
			}

			return max_price;
		}

	}
}
