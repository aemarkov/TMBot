using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.SteamAPI.Models;
using TMBot.Models.Steam;

namespace TMBot.API.SteamAPI
{
	public class CSSteamAPI : ISteamAPI
	{
		string userid;
		string api_key;
		HttpClient http_client;

		/// <summary>
		/// Создает новый объект для запросов к апи стима
		/// </summary>
		/// <param name="userid">ID пользователя (http://steamcommunity.com/profiles/xxxxx/)</param>
		/// <param name="key">API-key стима (надо получить)</param>
		public CSSteamAPI(string userid, string key)
		{
			this.userid = userid;
			http_client = new HttpClient();
			api_key = key;
		}

		/// <summary>
		/// Возвращает инвентарь
		/// </summary>
		/// <returns></returns>
		public async Task<SteamInventory> GetSteamInventoryAsync()
		{
			//TOOD: не только CS:GO
			//TODO: нормальная обработка ошибок запросов
			var response =  await http_client.GetAsync("http://steamcommunity.com/profiles/"+userid+"/inventory/json/730/2").ConfigureAwait(false);
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			return JsonConvert.DeserializeObject<SteamInventory>(content);
		}

		/// <summary>
		/// Возвращает трейды (обмены)
		/// </summary>
		/// <returns></returns>
		public async Task<SteamTrades> GetSteamTradesAsync()
		{
			//TODO: можно будет настраивать параметры
			//TODO: нормальная обработка ошибок запросов
			var response = await http_client.GetAsync("https://api.steampowered.com/IEconService/GetTradeOffers/v1/?key="+api_key+"&format=json&get_sent_offers=1&get_received_offers=1&get_descriptions=0&active_only=1").ConfigureAwait(false);
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			return JsonConvert.DeserializeObject<SteamTrades>(content);

		}
	}
}
