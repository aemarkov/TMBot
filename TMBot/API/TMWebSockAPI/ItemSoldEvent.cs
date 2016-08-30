using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Обработка события продажи нам предмета
    /// 
    /// Процесс покупки предмета
    /// - выставление трейдов (через сайт)
    /// - мониторинг цены (OrdersWorker + OrdersViewModel)
    ///   -----------------------
    /// - Событие продажи       | а что делать-то?
    ///   -----------------------
    /// - Событие передачи боту
    /// - Запрос боту
    /// - Подтверждение в стим
    /// </summary> 
    public class ItemSoldEvent : ITMWebSocketObserver
    {
        public void HandleEvebt(string data)
        {
            throw new System.NotImplementedException();
        }
    }
}