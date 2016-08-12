using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;

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
		public static int GetMinSellPrice <TTMAPI>(string classid, string instanceid) where TTMAPI: ITMAPI
		{
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo itemInfo = tmApi.GetItemInfo(classid, instanceid);

			if (itemInfo?.offers == null)
			{
				//Товара нет на площадке, определяем по стиму
				//TODO: Проверить по стиму
				Log.e("Товар {0}_{1} не найден на площадке",classid, instanceid);
				return -1;
			}

			//TODO: Проверка ошибок api и null
			return itemInfo.offers.Min(x => x.price);
		}

		/// <summary>
		/// Возвращает максимальную цену ордера на площадке
		/// </summary>
		/// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
		/// <param name="classid">параметр предмета</param>
		/// <param name="instanceid">параметр предмета</param>
		/// <returns></returns>
		public static int GetMaxOfferPrice<TTMAPI>(string classid, string instanceid) where TTMAPI : ITMAPI
		{
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo itemInfo = tmApi.GetItemInfo(classid, instanceid);

			if (itemInfo?.buy_offers == null)
			{
				//Товара нет на площадке, определяем по стиму
				Log.e("Ордер на товар {0}_{1} не найден на площадке", classid, instanceid);
				return -1;
			}

			return itemInfo.buy_offers.Max(x => x.o_price);
		}

	}
}
