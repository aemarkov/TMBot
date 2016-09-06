using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AutoMapper;
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
    /// <summary>
    /// Воркер для обновление цен трейдов. 
    /// Подробное описание воркера см. в базовом классе
    /// 
    /// ВАЖНО:
    /// По неизвестной причине, гении из TM засунули кулпенные
    /// предметы не в ОРДЕРЫ, а в ТРЕЙДЫ. Поэтому тут присутствует
    /// определенное количество ЗЛОЕБУЧЕЙ магии, которая
    /// объединяет ТРЕЙДЫ и ОРДЕРЫ
    /// 
    /// </summary>
    /// <typeparam name="TTMAPI"></typeparam>
    /// <typeparam name="TSteamAPI"></typeparam>
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

        //Получение ордеров
        protected override ICollection<Order> GetTMItems()
        {
            var orders = tmApi.GetOrders();
            return orders.Orders;
        }


        //Получает список трейдов
        protected ICollection<Trade> GetTrades()
        {
            //В трейдах еще указываются покупаемые предметы с разными статусами, поэтому
            //мы их игнорируем
            return tmApi.GetTrades().Where(x => x.ui_status == 3 || x.ui_status == 4).ToList();
        }

        // Получение предмета из БД
        protected override Item GetDbItem(ItemsRepository repository, Order api_item)
        {
            return repository.GetById(api_item.i_classid, api_item.i_instanceid);
        }

        //Получение моей цены
        protected override int GetItemMyPrice(Order api_item)
        {
            return api_item.o_price;
        }


        //Загружаем предметы с сайта
        protected override void GetItems(ItemsRepository repository)
        {
            //Загружаем ордеры
            base.GetItems(repository);

            //Дополнительно загружаем трейды и выбираем из инх те, которые относятся к продажам
            var trades = GetTrades();
            foreach (var trade in trades)
            {
                var item = CreateTradeOrderItem(trade, repository);
                Items.Add(item);
            }
        }

        //Создает модель из модели ТРЕЙДА
        private TradeItemViewModel CreateTradeOrderItem(Trade trade, ItemsRepository repository)
        {
            //Пиздец говнокод, копипаст дохуищи из BaseItemWorker + SellWorker 
            var item = Mapper.Map<Trade, TradeItemViewModel>(trade);

            item.MyPrice = (int) (trade.ui_price*100);

            var dbItem = repository.GetById(trade.i_classid, trade.ui_real_instance);
            if (dbItem != null)
            {
                Mapper.Map<Item, TradeItemViewModel>(dbItem, item);
            }
            else
            {
                //Такого предмета нет в БД, создадим новый
                dbItem = new Item()
                {
                    ClassId = item.ClassId,
                    InstanceId = item.IntanceId
                };

                repository.Create(dbItem);
            }

            item.ImageUrl = steamApi.GetImageUrl(item.ClassId);
            return item;
        }

    

        //Расчет новой цены
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

            if ((item.MyPrice!=tm_price+1) && ((tm_price > item.MyPrice) || (item.MyPrice > item.PriceLimit) || ((item.MyPrice - tm_price)/(float) item.MyPrice > PriceThreshold)))
            {
                myNewPrice = tm_price + 1;
                return true;
            }
            
            myNewPrice = item.MyPrice;
            return false;
        }

        //Получение минимальной цены на площадке
        protected override int? GetItemTMPrice(TradeItemViewModel item)
        {
            //Если ограничение по макс. цене 0 - то даже не ищем такие предметы
            if (item.PriceLimit == 0)
                return null;

            return PriceCounter.GetMaxOfferPrice<TTMAPI>(item.ClassId, item.IntanceId, item.PriceLimit);
        }

        //Обновление списка
        protected override void UpdateItems()
        {
            /* Делаем запрос ордеров и сравниваем с
             * текущим состоянием 
             * 
             * Если нет - добавляем, если удаленное есть - удаляем
             * 
             * Отдельно обрабатываем эти "трейды-ордеры"
             */

            var repository = new ItemsRepository();

            //Добавление новых и изменение статуса
            var orders = GetTMItems();
            var trades = GetTrades();

            foreach (var order in orders)
            {
                var listItem =
                    Items.FirstOrDefault(x => x.ClassId == order.i_classid && x.IntanceId == order.i_instanceid);
                if (listItem == null)
                {
                    //Добавляем
                    Items.Add(CreateTradeItem(order, repository));
                }
                
            }

            //Удаление старых ордеров
            //Не трогаем те ордеры, которые трейды
            for (int i = 0; i < Items.Count; i++)
            {
                if (String.IsNullOrEmpty(Items[i].ItemId))
                {
                    if (!orders.Any(
                            x => x.i_classid == Items[i].ClassId && x.i_instanceid == Items[i].IntanceId))
                        Items.RemoveAt(i);
                }
                else
                {
                    if (!trades.Any(x => x.ui_id == Items[i].ItemId))
                        Items.RemoveAt(i);
                }
            }

            //Обновление трейдов-ордеров
            foreach (var trade in trades)
            {
                var listItem =
                    Items.FirstOrDefault(x => x.ClassId == trade.i_classid && x.IntanceId == trade.ui_real_instance);
                if (listItem == null)
                {
                    //Добавляем
                    Items.Add(CreateTradeOrderItem(trade, repository));
                }
                else
                {
                    //Смена статуса
                    var newStatus = UiStatusToStatusConverter.Convert(trade.ui_status);
                    if (listItem.Status == ItemStatus.BOUGHT && newStatus == ItemStatus.BOUGHT_TAKE)
                        listItem.Status = ItemStatus.BOUGHT_TAKE;
                }
            }


        }

        //Обработка статуса
        protected override bool CheckStatusAndMakeRequest(TradeItemViewModel item)
        {
            if (item.Status != ItemStatus.ORDERING && item.Status != ItemStatus.BOUGHT_TAKE)
                return false;

            //Если статус - можете забрать вещь, но ItemRequest почему-то еще не сделан
            //надо сделать
            if (item.Status == ItemStatus.BOUGHT_TAKE)
            {
                //TOOD: ui_bid
                ItemRequestHelper.MakeBuyItemRequest(tmApi, item.BotId, item.ItemId);
                return false;
            }

            return true;
        }

        //Остановка
        public override void Stop()
        {
            base.Stop();
            var settings = SettingsManager.LoadSettings();
            settings.OrderMinThreshold = PriceThreshold;
            SettingsManager.SaveSettings(settings);
        }

        /// <summary>
        /// Обновляет цену
        /// </summary>
        /// <param name="itemid">ID предмета</param>
        /// <param name="price">новая цена</param>
        protected override void UpdatePrice(TradeItemViewModel item, int price)
        {
            tmApi.UpdateOrder(item.ClassId, item.IntanceId, price);
        }
    }
}