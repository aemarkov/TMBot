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
	}
}
