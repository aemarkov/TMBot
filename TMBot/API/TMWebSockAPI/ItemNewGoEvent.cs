using System;
using Newtonsoft.Json;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMWebSockAPI.Models;
using TMBot.Data;
using TMBot.Utilities;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Событие получения данных по каналу itemnew_go
    /// веб сокетов. Это событие вызывается
    /// 
    /// КОГДА У НАС КУПИЛИ ПРЕДМЕТ
    /// </summary>
    public class ItemNewGoEvent<TTMAPI> : ITMWebSocketObserver where TTMAPI: ITMAPI
    {
        private ITMAPI api;
        
        public ItemNewGoEvent()
        {
            api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
        }

        public void HandleEvebt(string data)
        {
            Log.d("ItemNewGo");

            var itemnew_go = JsonConvert.DeserializeObject<ItemStatusGoResponse>(data);

            try
            {
                //Делаем ItemRequest, чтобы бот ТМ инициировал обмен в Стим
                string botid = "1";
                var itemrequest = api.ItemRequest(ItemRequestDirection.IN, botid);

                if (itemrequest==null || !itemrequest.success)
                {
                    Log.e($"Произошла ошибка при выполнении ItemRequest");
                }

                Log.d($"Выполнен запрос на обмен, бот: {itemrequest.nick}, сообщение: {itemrequest.secret}");

                //Необходимо поместить ID бота и сообщение в список, который будет использоваться при поиске трейдов в стиме
                SteamTradeContainer.OutTrades.PushTrade(itemrequest);

                //Необходимо пометить это трейд, чтобы он не продавался

            }
            catch (Exception exp)
            {
                Log.e($"Произошла ошибка при выполнении ItemRequest: {exp.Message}");
            }

        }
    }
}