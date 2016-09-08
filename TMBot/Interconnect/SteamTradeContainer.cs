using TMBot.Data;

namespace TMBot.Interconnect
{
    /// <summary>
    /// Контейнер для предложений обмена, предоставляющий
    /// доступ к ним из любой точки приложения
    /// </summary>
    public static class SteamTradeContainer
    {
        /// <summary>
        /// Созданные ботом ТМ предложения обмена
        /// </summary>
        public static SteamTradeList Trades { get; }=new SteamTradeList();

        /// <summary>
        /// Предложение на передачу предмета
        /// </summary>
        //public static SteamTradeList OutTrades { get; } = new SteamTradeList();
    }
}
