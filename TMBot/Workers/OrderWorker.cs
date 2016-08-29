using System.Collections.Generic;
using System.Windows;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Database;
using TMBot.Models;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Workers
{
    public class OrderWorker<TTMAPI, TSteamAPI> : BaseItemWorker<TTMAPI, TSteamAPI, Order> where TTMAPI : ITMAPI where TSteamAPI : ISteamAPI
    {
        protected override void ShowErrorMessage(string error_reason)
        {
            MessageBox.Show($"Не удалось начать покупки: {error_reason}", "Не удалось начать покупки", MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        protected override ICollection<Order> GetTMItems()
        {
            var orders = tmApi.GetOrders();
            return orders.Orders;
        }

        protected override Item GetDbItem(ItemsRepository repository, Order api_item)
        {
            return repository.GetById(api_item.i_classid, api_item.i_instanceid);
        }

        protected override int GetItemMyPrice(Order api_item)
        {
            return api_item.o_price;
        }

        protected override bool GetItemNewPrice(TradeItemViewModel item, int tm_price, ref int myNewPrice)
        {
            /* Если максимальная цена больше текущей, то увеличиваем на 1 коп
             * нашу цену. 
             * 
             * Если максимальная - наша, то уменьшаем цену (до макс+1 коп)
             * только если разница больше заданных %
             */

            if (item.PriceLimit == 0)
            {
                myNewPrice = item.MyPrice;
                return false;
            }

            if (tm_price > item.MyPrice || ((item.MyPrice - tm_price) /(float) item.MyPrice > OffsetPercentage))
                myNewPrice = tm_price + 1;
            else
            {
                myNewPrice = item.MyPrice;

                if (myNewPrice > item.PriceLimit)
                    myNewPrice = item.PriceLimit;

                return false;
            }

            if (myNewPrice > item.PriceLimit)
                myNewPrice = item.PriceLimit;

            return true;
        }

        protected override int? GetItemTMPrice(TradeItemViewModel item)
        {
            return PriceCounter.GetMaxOfferPrice<TTMAPI>(item.ClassId, item.IntanceId);
        }
    }
}