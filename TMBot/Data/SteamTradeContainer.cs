using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.Data
{
    /// <summary>
    /// Контейнер для предложений обмена, предоставляющий
    /// доступ к ним из любой точки приложения
    /// </summary>
    public static class SteamTradeContainer
    {
        /// <summary>
        /// Предложение на получение предмета
        /// </summary>
        public static SteamTradeList InTrades { get; }=new SteamTradeList();

        /// <summary>
        /// Предложение на передачу предмета
        /// </summary>
        public static SteamTradeList OutTrades { get; } = new SteamTradeList();
    }
}
