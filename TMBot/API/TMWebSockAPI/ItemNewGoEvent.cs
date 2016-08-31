using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Событие получения данных по каналу itemnew_go
    /// веб сокетов. Это событие вызывается
    /// 
    /// КОГДА У НАС КУПИЛИ ПРЕДМЕТ
    /// </summary>
    public class ItemNewGoEvent : ITMWebSocketObserver
    {
        public void HandleEvebt(string data)
        {
            throw new System.NotImplementedException();
        }
    }
}