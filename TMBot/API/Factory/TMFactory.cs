using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.TMAPI;

namespace TMBot.API.Factory
{
	public class TMFactory: AbstactAPIFactory<ITMAPI>
	{
		/// <summary>
		/// Создает реализацию АПИ
		/// </summary>
		/// <typeparam name="TAPI">Тип апи</typeparam>
		/// <param name="apikey">api-key для доступа</param>
		public void CreateAPI<TAPI>(string apikey) where TAPI : ITMAPI
		{
			Type t = typeof(TAPI);
			if (apis.ContainsKey(t))
				throw new Exception("API already created");

			ITMAPI api;

			//ГОВНОКОООООООООООООООООД
			//Потому что интерфейс конструктора не существует
			if (t == typeof(CSTMAPI))
				api = new CSTMAPI(apikey);
			else
				throw new Exception("Can't create instance of this class. Maybe you are govnocoder");

			apis.Add(t, api);
		}

		public void CreateAPI<TAPI>(string apikey, bool is_debug) where TAPI : ITMAPI
		{
			CreateAPI<TAPI>(apikey);
			var api = GetAPI<TAPI>();
			api.IsDebug = is_debug;
		}
	}
}
