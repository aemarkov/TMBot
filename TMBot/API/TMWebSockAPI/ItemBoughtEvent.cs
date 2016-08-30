using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMBot.API.TMWebSockAPI.Models;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Обработка события покупки у нас предмета
    /// 
    /// Процесс продажи предмета
    ///  - Выставление (MakeTradesViewModel)
    ///  - Мониторинг цены (SellWorker + TradesViewModel)
    ///    --------------------------
    ///  - Событие продажи          |
    ///  - Передача предмета боту   |
    ///    --------------------------
    ///  - Подтверждение обмена в стим
    /// 
    /// </summary> 
    public class ItemBoughtEvent : ITMWebSocketObserver
    {
        public void HandleEvebt(string data)
        {
            var itemNew = JsonConvert.DeserializeObject<ItemNew>(data);
        }
    }
}
