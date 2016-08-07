using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.SteamAPI;

namespace TMBot.API.Factory
{
	public class SteamFactory : AbstactAPIFactory<ISteamAPI>
	{
		/// <summary>
		/// Создает реализацию АПИ
		/// </summary>
		/// <typeparam name="TAPI">Тип апи</typeparam>
		/// <param name="userid">ID профиля пользователя стима</param>
		/// <param name="apikey">API-key для доступа</param>
		public void CreateAPI<TAPI>(string userid, string apikey) where TAPI : ISteamAPI
		{
			Type t = typeof(TAPI);
			if (apis.ContainsKey(t))
				throw new Exception("API already created");

			ISteamAPI api;

			//ГОВНОКОООООООООООООООООД
			//Потому что интерфейс конструктора не существует
			if (t == typeof(SteamAPI.CSSteamAPI))
				api = new SteamAPI.CSSteamAPI(userid, apikey);
			else
				throw new Exception("Can't create instance of this class. Maybe you are govnocoder");

			apis.Add(t, api);
		}
	}
}
