using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMBot.API.TMAPI;
using TMBot.API.TMWebSockAPI.Models;
using TMBot.Utilities;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Событие получения данных по каналу itemstatus_go
    /// веб сокетов. Это событие вызывается 
    /// когда изменился статус предмета на странице
    /// sell. Нас интересует:
    /// 
    /// КОГДА ПРОДАВЕЦ ПЕРЕДАЛ ВЕЩЬ БОТУ
    /// status == 4
    /// </summary>
    public class ItemStatusGoEvent<TTMAPI> : BaseEvent<TTMAPI> where TTMAPI :ITMAPI
    {
        public override async Task HandleEvent(string data)
        {
            //throw new System.NotImplementedException();
            Log.d("ItemStatusGo");

            try
            {

                var itemstatus_go = JsonConvert.DeserializeObject<ItemStatusGoResponse>(data);

                if (itemstatus_go.status == 4)
                {
                    ItemRequestHelper.MakeBuyItemRequest(api, itemstatus_go.bid, itemstatus_go.id);
                }
            }
            catch (Exception exp)
            {
                Log.e($"Не удалось обработать событие ItemStatus_Go: {exp.Message}");
            }
        }
    }
}