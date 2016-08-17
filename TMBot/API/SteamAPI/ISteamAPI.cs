using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.SteamAPI.Models;
using TMBot.Models.Steam;

namespace TMBot.API.SteamAPI
{
	/// <summary>
	/// Интерфейс API для Steam
	/// </summary>
	public interface ISteamAPI : IAbstractAPI
	{
		/// <summary>
		/// Возвращает инвентарь
		/// </summary>
		/// <returns></returns>
		Task<SteamInventory> GetSteamInventoryAsync();

		/// <summary>
		/// Возвращает трейды (обмены)
		/// </summary>
		/// <returns></returns>
		Task<SteamTrades> GetSteamTradesAsync();

        /// <summary>
        /// Возвращает ссылку на изображение предмета
        /// </summary>
        /// <param name="classid">Тип предмета</param>
        /// <returns></returns>
	    string GetImageUrl(string classid);
	}
}
