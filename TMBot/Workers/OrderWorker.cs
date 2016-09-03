using System.Collections.Generic;
using System.Windows;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Data;
using TMBot.Database;
using TMBot.Models;
using TMBot.Settings;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Workers
{
    public class OrderWorker<TTMAPI, TSteamAPI> : BaseItemWorker<TTMAPI, TSteamAPI, Order> where TTMAPI : ITMAPI where TSteamAPI : ISteamAPI
    {
        public OrderWorker(SynchronizedObservableCollection<TradeItemViewModel>  items):base(items)
	    {
            var settings = SettingsManager.LoadSettings();
            PriceThreshold = settings.OrderMinThreshold;
        }


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
                //Если ограничение по максимальной цене - 0, то не меняем цену на предмет
                myNewPrice = item.MyPrice;
                return false;
            }

            if ((tm_price > item.MyPrice) || (item.MyPrice > item.PriceLimit) || ((item.MyPrice - tm_price)/(float) item.MyPrice > PriceThreshold))
            {
                myNewPrice = tm_price + 1;
                return true;
            }
            
            myNewPrice = item.MyPrice;
            return false;
        }

        protected override int? GetItemTMPrice(TradeItemViewModel item)
        {
            //Если ограничение по макс. цене 0 - то даже не ищем такие предметы
            if (item.PriceLimit == 0)
                return null;

            return PriceCounter.GetMaxOfferPrice<TTMAPI>(item.ClassId, item.IntanceId, item.PriceLimit);
        }

        //Остановка
        public override void Stop()
        {
            base.Stop();
            var settings = SettingsManager.LoadSettings();
            settings.OrderMinThreshold = PriceThreshold;
            SettingsManager.SaveSettings(settings);
        }
    }
}