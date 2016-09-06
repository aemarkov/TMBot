using System.Threading.Tasks;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.Workers.WebSocket;

namespace TMBot.API.TMWebSockAPI
{
    public abstract class BaseEvent<TTMAPI> : ITMWebSocketObserver where TTMAPI : ITMAPI
    {
        protected ITMAPI api;

        public BaseEvent()
        {
            api = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
        } 

        public abstract Task HandleEvent(string data);
    }
}