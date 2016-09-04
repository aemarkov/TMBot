using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMWebSockAPI.Models;
using TMBot.Data;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;
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

        public async Task HandleEvent(string data)
        {
            Log.d("ItemNewGo");

            var itemnew_go = JsonConvert.DeserializeObject<ItemNewGoResponse>(data);
            ItemRequestHelper.MakeSellItemRequest(api, itemnew_go.ui_id);

        }

        
    }
}