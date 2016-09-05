using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.API.TMWebSockAPI.Models;
using TMBot.Data;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Событие получения данных по каналу itemoutnew_go - 
    /// исчезание предмета на странице sell
    /// веб сокетов. Это событие вызывается
    /// 
    /// КОГДА У НАС КУПИЛИ ПРЕДМЕТ
    /// </summary>
    public class ItemOutNewGoEvent<TTMAPI> : BaseEvent<TTMAPI> where TTMAPI: ITMAPI
    {
        public override async Task HandleEvent(string data)
        {
            Log.d("ItemOutNewGo");

            try
            {
                var itemnew_go = JsonConvert.DeserializeObject<Trade>(data);
                ItemRequestHelper.MakeSellItemRequest(api, itemnew_go.ui_id);
            }
            catch (Exception exp)
            {
                Log.e($"Не удалось обработать событие ItemOutNew_Go: {exp.Message}");
            }
            
        }

        
    }
}