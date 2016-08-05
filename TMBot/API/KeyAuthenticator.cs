using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace TMBot.API
{
	/// <summary>
	/// Идентификация по ключу для  RestSharp
	/// </summary>
	public class KeyAuthenticator : IAuthenticator
	{
		private readonly string  key_param, key;

		public KeyAuthenticator(string key_param, string key)
		{
			this.key_param = key_param;
			this.key = key;
		}

		public void Authenticate(IRestClient client, IRestRequest request)
		{
			request.AddQueryParameter(key_param, key);
		}
	}
}
