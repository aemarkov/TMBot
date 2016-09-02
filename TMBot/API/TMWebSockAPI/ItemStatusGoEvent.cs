using System.Threading.Tasks;
using TMBot.Utilities;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    /// <summary>
    /// Событие получения данных по каналу itemstatus_go
    /// веб сокетов. Это событие вызывается в
    /// ряде случаев, но интересует
    /// 
    /// КОГДА ПРОДАВЕЦ ПЕРЕДАЛ ВЕЩЬ БОТУ
    /// status == 4
    /// </summary>
    public class ItemStatusGoEvent : ITMWebSocketObserver
    {
        public async Task HandleEvent(string data)
        {
            //throw new System.NotImplementedException();
            Log.d("ItemStatusGo");
        }
    }
}