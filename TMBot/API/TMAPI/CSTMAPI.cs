using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.Exceptions;
using TMBot.Models.TM;

namespace TMBot.API.TMAPI
{
	/// <summary>
	/// Класс для выполнения запросов к 
	/// API market.csgo.tm
	/// </summary>
	public class CSTMAPI : ITMAPI
	{

		RestClient rest_client;

		/// <summary>
		/// Создает новый объект для запросов к АПИ
		/// </summary>
		/// <param name="key">API-key для доступа</param>
		public CSTMAPI(String key)
		{
			rest_client = new RestClient();
			rest_client.BaseUrl = new Uri("https://csgo.tm/api");
			rest_client.Authenticator = new KeyAuthenticator("key", "Yg0skGdNIVST7811G6zGF8XDY29165T");

		}

		/// <summary>
		/// Возвращает информацию о всех продажах
		/// и покупках предмета
		/// </summary>
		/// <returns></returns>
		public ItemInfo GetItemInfo(string classid_instanceid)
		{
			var request = new RestRequest("ItemInfo/{classid_instanceid}/{ru_or_en}", Method.GET);
			request.AddParameter("classid_instanceid", classid_instanceid, ParameterType.UrlSegment);
			request.AddParameter("ru_or_en", "ru", ParameterType.UrlSegment);
			var response = rest_client.Execute<ItemInfo>(request);

			//Если что-то пойдет не так, тут почему-то будут null в полях
			//TODO: нормальная обработка ошибок запросов
			if (response.Data == null || response.Data.name	 == null)
				throw new APIException();

			return response.Data;
		}

		/// <summary>
		/// Возвращает информацию о всех продажах
		/// и покупках предмета
		/// </summary>
		/// <returns></returns>
		public ItemInfo GetItemInfo(string classid, string instanceid)
		{
			return GetItemInfo(classid + "_" + instanceid);
		}

		/// <summary>
		/// Получить список своих трейдов (продаж)
		/// </summary>
		/// <returns></returns>
		public OrdersList GetOrders()
		{
			var request = new RestRequest("GetOrders", Method.GET);
			var response = rest_client.Execute<OrdersList>(request);

			if (response.Data == null)
				throw new APIException();

			return response.Data;
		}

		/// <summary>
		/// Выставить на продожу новый предмет
		/// </summary>
		/// <param name="price">цена в КОП
		public IList<Trade> GetTrades()
		{
			var request = new RestRequest("Trades", Method.GET);
			var response = rest_client.Execute<List<Trade>>(request);

			if (response.Data == null)
				throw new APIException();

			return response.Data;
		}

		/// <summary>
		/// Выставить на продожу новый предмет
		/// </summary>
		/// <param name="price">цена в КОПЕЙКАХ</param>
		/// <returns></returns>
		public ItemRequestResponse ItemRequest(ItemRequestDirection in_out, string botid)
		{
			var request = new RestRequest("Trades/{in_out}/{botid}", Method.GET);

			string direction;
			if (in_out == ItemRequestDirection.IN)
				direction = "in";
			else
				direction = "out";

			request.AddParameter("in_out", direction, ParameterType.UrlSegment);
			request.AddParameter("botid", botid, ParameterType.UrlSegment);

			var response = rest_client.Execute<ItemRequestResponse>(request);

			//TODO: нормальная обработка ошибок запросов
			if (response.Data == null)
				throw new APIException();

			return response.Data;
		}

		/// <summary>
		/// Изменить/удалить трейд
		/// </summary>
		/// <param name="itemid">Уникальный номер вещи (получатеся после выполнения SetNewItem)</param>
		/// <param name="price">Цена в КОПЕЙКАХ, 0 - чтобы снять</param>
		/// <returns></returns>
		public SetPriceResponse SetNewItem(string classid_instanceid, int price)
		{
			var request = new RestRequest("SetPrice/new_{classid_instanceid}/{price}", Method.GET);
			request.AddParameter("classid_instanceid", classid_instanceid, ParameterType.UrlSegment);
			request.AddParameter("price", price, ParameterType.UrlSegment);

			var response = rest_client.Execute<SetPriceResponse>(request);

			//TODO: нормальная обработка ошибок запросов
			if (response.Data == null)
				throw new APIException();

			return response.Data;
		}

		/// <summary>
		/// Получить список своих ордеров (покупок)
		/// </summary>
		/// <returns></returns>
		public SetPriceResponse SetNewItem(string classid, string instanceid, int price)
		{
			return SetNewItem(classid + "_" + instanceid, price);
		}

		/// <summary>
		/// Изменение/удаление заявки на покупку
		/// </summary>
		/// <param name="price">Цена в КОПЕЙКАХ, 0 - снять ордер</param>
		/// <returns></returns>
		public SetPriceResponse SetPrice(string itemid, int price)
		{
			var request = new RestRequest("SetPrice/{itemid}/{price}", Method.GET);
			request.AddParameter("itemid", itemid, ParameterType.UrlSegment);
			request.AddParameter("price", price, ParameterType.UrlSegment);

			var response = rest_client.Execute<SetPriceResponse>(request);

			//TODO: нормальная обработка ошибок запросов
			if (response.Data == null)
				throw new APIException();

			return response.Data;
		}

		/// <summary>
		/// Отправка оффлайн-трейда от бота.
		/// Это заставляет бота создать обмен в стиме для получения или
		/// передачи предмета
		/// </summary>
		/// <param name="in_out">Получение или передача. In - передача, Out - получение/param>
		/// <param name="botid">ID бота, можно получить в результате Trades либо в событиях сокета</param>
		/// <returns></returns>
		public UpdateOrderResponse UpdateOrder(string classid, string instanceid, int price)
		{
			var request = new RestRequest("SetPrice/{classid}/{instanceid}/{price}", Method.GET);
			request.AddParameter("classid", classid, ParameterType.UrlSegment);
			request.AddParameter("instanceid", instanceid, ParameterType.UrlSegment);
			request.AddParameter("price", price, ParameterType.UrlSegment);

			var response = rest_client.Execute<UpdateOrderResponse>(request);

			//TODO: нормальная обработка ошибок запросов
			if (response.Data == null)
				throw new APIException();

			return response.Data;
		}
	}
}
