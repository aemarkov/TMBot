namespace TMBot.Workers.WebSocket
{
    /// <summary>
    /// Интерфейс подписчкиво на события TM, которые приходят по веб-сокетам
    /// </summary>
    public interface ITMWebSocketObserver
    {
        void HandleEvebt(string data);
    }
}