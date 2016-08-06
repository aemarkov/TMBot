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
