using Newtonsoft.Json;
using TMBot.API.TMWebSockAPI.Models;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Обработка события передачи продавцом предмета боту
    /// 
    /// Процесс покупки предмета
    /// - выставление трейдов (через сайт)
    /// - мониторинг цены (OrdersWorker + OrdersViewModel)
    /// - Событие продажи
    ///   ----------------------------
    /// - Событие передачи боту      |
    /// - Запрос боту                |
    ///   ----------------------------
    /// - Подтверждение в стим
    /// </summary> 
    public class ItemTransferedEvent : ITMWebSocketObserver
    {
        public void HandleEvebt(string data)
        {
            ItemStatus status = JsonConvert.DeserializeObject<ItemStatus>(data);
        }
    }
}