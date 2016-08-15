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
		public static int? GetMinSellPrice <TTMAPI>(string classid, string instanceid) where TTMAPI: ITMAPI
		{
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo itemInfo = tmApi.GetItemInfo(classid, instanceid);

			if (itemInfo?.offers == null)
			{
				//Товара нет на площадке
				Log.w("Товар {0}_{1} не найден на площадке",classid, instanceid);
				return null;
			}

			//TODO: Проверка ошибок api и null
		    var not_mine = itemInfo.offers.Where(x => x.my_count == 0);
		    if (!not_mine.Any())
		    {
		        Log.w("Товар {0}_{1} проадается только мной", classid, instanceid);
                return null;
		    }

		    return not_mine.Min(x => x.price);
		}

		/// <summary>
		/// Возвращает максимальную цену ордера на площадке
		/// </summary>
		/// <typeparam name="TTMAPI">Тип API, соответствуюший площадке</typeparam>
		/// <param name="classid">параметр предмета</param>
		/// <param name="instanceid">параметр предмета</param>
		/// <returns></returns>
		public static int? GetMaxOfferPrice<TTMAPI>(string classid, string instanceid) where TTMAPI : ITMAPI
		{
			ITMAPI tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();

			ItemInfo itemInfo = tmApi.GetItemInfo(classid, instanceid);

			if (itemInfo?.buy_offers == null)
			{
				//Товара нет на площадке
				Log.w("Ордер на товар {0}_{1} не найден на площадке", classid, instanceid);
				return null;
			}

			return itemInfo.buy_offers.Where(x => x.my_count == 0).Max(x => x.o_price);
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
