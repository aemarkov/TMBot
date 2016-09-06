﻿using System;
using System.Threading.Tasks;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Data;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Utilities
{
    /// <summary>
    /// Выполняет ItemRequest и все сопутствующие дела
    /// </summary>
    public static class ItemRequestHelper
    {
        private static readonly object _sellLock = new object();
        private static readonly object _buyLock = new object();

        /// <summary>
        /// Выполняет ItemRequest при продаже
        /// </summary>
        /// <param name="api"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static void MakeSellItemRequest(ITMAPI api, string itemId)
        {
            lock (_sellLock)
            {

                var item = ItemCollectionsContainer.GetInstance().FindTradeItem(itemId);

                //Проверяем статус
                if (item.Status == ItemStatus.SOLD_REQUEST || item.Status == ItemStatus.GIVEN)
                    return;

                //Меняем статус
                item.Status = ItemStatus.SOLD;

                try
                {
                    //Делаем ItemRequest, чтобы бот ТМ инициировал обмен в Стим
                    var itemrequest = makeRequest(api, "1", ItemRequestDirection.IN);
                    if (itemrequest == null)
                    {
                        return;
                    }

                    //Необходимо пометить это трейд, чтобы он не продавался
                    item.Status = ItemStatus.SOLD_REQUEST;

                }
                catch (Exception exp)
                {
                    Log.e($"Произошла ошибка при выполнении ItemRequest: {exp.Message}");
                }
            }
        }

        /// <summary>
        /// Выполняет ItemRequest при покупке
        /// </summary>
        /// <param name="api"></param>
        /// <param name="botid"></param>
        /// <returns></returns>
        public static void MakeBuyItemRequest(ITMAPI api, string botid, string itemId)
        {
            lock (_buyLock)
            {
                var item = ItemCollectionsContainer.GetInstance().FindOrderItem(itemId);

                //Проверяем статус
                if (item.Status == ItemStatus.BOUGHT_REQUEST || item.Status == ItemStatus.TAKEN)
                    return;

                //Меняем статус
                item.Status = ItemStatus.BOUGHT_TAKE;

                try
                {
                    //Делаем ItemRequest, чтобы бот ТМ инициировал обмен в Стим
                    var itemrequest = makeRequest(api, botid, ItemRequestDirection.OUT);
                    if (itemrequest == null)
                        return;

                    item.Status = ItemStatus.BOUGHT_REQUEST;

                }
                catch (Exception exp)
                {
                    Log.e($"Произошла ошибка при выполнении ItemRequest: {exp.Message}");
                    throw;
                }
            }
        }



        private static ItemRequestResponse makeRequest(ITMAPI api, string botid, ItemRequestDirection direction)
        {
            //Выполняем асинхронно, чтобы как можно скорее возвратиться в вызывающий метод
            //и он продолжил следить за сокетами
            var itemrequest = api.ItemRequest(direction, botid);

            if (itemrequest == null || !itemrequest.success)
            {
                Log.e($"Произошла ошибка при выполнении ItemRequest");
                return null;
            }

            Log.d($"Выполнен запрос на обмен, бот: {itemrequest.nick}, сообщение: {itemrequest.secret}");

            //Необходимо поместить ID бота и сообщение в список, который будет использоваться при поиске трейдов в стиме
            SteamTradeContainer.Trades.PushTrade(itemrequest);

            return itemrequest;
        }

    }
}