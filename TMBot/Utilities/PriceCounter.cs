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
        /// Определяет минимальную цену среди продаж на площадке,
        /// но не меньше заданной
        /// </summary>
        /// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
        /// <param name="classid">параметр предмета</param>
        /// <param name="instanceid">параметр предмета</param>
        /// <param name="minPrice">ограничение минимальной цены</param>
        /// <returns>Минимальную цену предмета или null, если цена не найдена</returns>
        public static int? GetMinSellPrice <TTMAPI>(string classid, string instanceid, int minPrice) where TTMAPI: ITMAPI
		{
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo itemInfo = tmApi.GetItemInfo(classid, instanceid);

			if (itemInfo?.offers == null)
			{
				//Товара нет на площадке
				Log.w("Товар {0}_{1} не найден на площадке",classid, instanceid);
				return null;
			}

			//Находим товары, чья стоимость больше ограничения и которые продаю НЕ Я
		    var trades = itemInfo.offers.Where(x => x.my_count == 0 && x.price >= minPrice).ToList();
		    if (trades.Count==0)
		    {
		        Log.w($"Трейды товара {classid}_{instanceid} с ценой больше {minPrice} не найдены");
                return null;
		    }

		    return trades.Min(x => x.price);
		}

        /// <summary>
        ///  Определяет минимальную цену среди продаж на площадке без ограничения
        /// минимальной цены
        /// </summary>
        /// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
        /// <param name="classid">параметр предмета</param>
        /// <param name="instanceid">параметр предмета</param>
        /// <returns>Минимальную цену предмета или null, если цена не найдена</returns>
        public static int? GetMinSellPrice<TTMAPI>(string classid, string instanceid) where TTMAPI : ITMAPI
	    {
	        return GetMinSellPrice<TTMAPI>(classid, instanceid, 0);
	    }

        /// <summary>
        /// Возвращает максимальную цену ордера на площадке, но не больше
        /// заданной
        /// </summary>
        /// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
        /// <param name="classid">параметр предмета</param>
        /// <param name="instanceid">параметр предмета</param>
        /// <param name="maxPrice">максимальная цена</param>
        /// <returns></returns>
        public static int? GetMaxOfferPrice<TTMAPI>(string classid, string instanceid, int maxPrice) where TTMAPI : ITMAPI
		{
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo itemInfo = tmApi.GetItemInfo(classid, instanceid);

			if (itemInfo?.buy_offers == null)
			{
				//Товара нет на площадке
				Log.w("Ордер на товар {0}_{1} не найден на площадке", classid, instanceid);
				return null;
			}

            //Находим ордеры, чья стоимость меньше ограничения и которые выставлены НЕ МНОЙ
            var orders = itemInfo.buy_offers.Where(x => x.my_count == 0 && x.o_price <= maxPrice).ToList();
            if (orders.Count==0)
            {
                Log.w($"Ордеры товара {classid}_{instanceid} с ценой меньше {maxPrice} не найдены");
                return null;
            }

            return orders.Max(x => x.o_price);
		}

	    /// <summary>
	    /// Возвращает максимальную цену ордера на площадке без ограничения
	    /// цены
	    /// </summary>
	    /// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
	    /// <param name="classid">параметр предмета</param>
	    /// <param name="instanceid">параметр предмета</param>
	    /// <returns></returns>
	    public static int? GetMaxOfferPrice<TTMAPI>(string classid, string instanceid) where TTMAPI : ITMAPI
	    {
	        return GetMaxOfferPrice<TTMAPI>(classid, instanceid, int.MaxValue);
	    }


        public static int GetSteamMinSellPrice(string classid, string instanceid)
	    {
            //TODO: Проверить по стиму
            Log.e("Steam price search not implemented");
	        return -1;
	    }


        public static int GetSteamMaxOfferPrice(string classid, string instanceid)
        {
            Log.e("Steam price search not implemented");
            return -1;
        }
    }
}
