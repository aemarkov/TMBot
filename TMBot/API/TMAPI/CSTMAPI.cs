using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMBot.API.Exceptions;
using TMBot.API.TMAPI.Models;
using TMBot.Utilities;
using TMBot.Utilities.CallWaiter;

namespace TMBot.API.TMAPI
{
	/// <summary>
	/// Класс для выполнения запросов к 
	/// API market.csgo.tm
	/// 
	/// Существует ограничение - не более 3х запросов в секунду,
	/// поэтому если следующий запрос придет быстрее, чем через 1/3 секунды,
	/// он будет ожидать
	/// </summary>
	public class CSTMAPI : ITMAPI
	{

        private RestClient rest_client;
	    private MinimumCallInterval callInterval;

		//Не выполнять по-настоящему 
		public bool IsDebug { get; set; }

		/// <summary>
		/// Создает новый объект для запросов к АПИ
		/// </summary>
		/// <param name="key">API-key для доступа</param>
		public CSTMAPI(String key)
		{
			rest_client = new RestClient();
			rest_client.BaseUrl = new Uri("https://csgo.tm/api");
			rest_client.Authenticator = new KeyAuthenticator("key", key);

			IsDebug = false;


            callInterval = new MinimumCallInterval(333);
		}

        //Проверяет ошибки, возвращаемые АПИ
	    private void check_errors(string content)
        { 
	        JToken token;
	        try
	        {
	            token = JToken.Parse(content);
	        }
	        catch (JsonException exp)
	        {
	            Log.e($"API not in JSON: {content}");
                throw new APIException(content);
	        }


	        if(token.Type==JTokenType.Array)
                return;

	        var error = token["error"];
	        if (error != null)
	        {
	            if ((string) error == "Bad KEY")
	            {
	                Log.e("API error: bad key");
	                throw new BadKeyException();
	            }
                else if ((string) error == "bad method")
                {
                    Log.e("API error: bad method");
                    throw  new BadMethodException();
                }
	            else
	            {
                    Log.e("Unknown API error");
	                throw new APIException((string)error);
	            }
	        }
	    }

		/// <summary>
		/// Возвращает информацию о всех продажах
		/// и покупках предмета
		/// </summary>
		/// <returns></returns>
		public  ItemInfo GetItemInfo(string classid_instanceid)
		{
		    using (new CallHelper(callInterval))
		    {

		        var request = new RestRequest("ItemInfo/{classid_instanceid}/{ru_or_en}", Method.GET);
		        request.AddParameter("classid_instanceid", classid_instanceid, ParameterType.UrlSegment);
		        request.AddParameter("ru_or_en", "ru", ParameterType.UrlSegment);
		        var response = rest_client.Execute<ItemInfo>(request);

		        check_errors(response.Content);

		        //Если что-то пойдет не так, тут почему-то будут null в полях
		        //TODO: нормальная обработка ошибок запросов
		        if (response.Data?.name == null)
		            return null;

                return response.Data;
            }

		}

		/// <summary>
		/// Возвращает информацию о всех продажах
		/// и покупках предмета
		/// </summary>
		/// <returns></returns>
		public  ItemInfo GetItemInfo(string classid, string instanceid)
		{
               return GetItemInfo(classid + "_" + instanceid);
		}

		/// <summary>
		/// Получить список своих трейдов (продаж)
		/// </summary>
		/// <returns></returns>
		public  OrdersList GetOrders()
		{
		    using (new CallHelper(callInterval))
		    {

		        var request = new RestRequest("GetOrders", Method.GET);
		        var response = rest_client.Execute<OrdersList>(request);

		        check_errors(response.Content);

		        return response.Data;
		    }
		}

		/// <summary>
		/// Выставить на продожу новый предмет
		/// </summary>
		/// <param name="price">цена в КОП
		public  IList<Trade> GetTrades()
		{
		    using (new CallHelper(callInterval))
		    {
		        var request = new RestRequest("Trades", Method.GET);
		        var response = rest_client.Execute<List<Trade>>(request);

		        check_errors(response.Content);

		        return response.Data;
		    }
		}

		/// <summary>
		/// Выставить на продожу новый предмет
		/// </summary>
		/// <param name="price">цена в КОПЕЙКАХ</param>
		/// <returns></returns>
		public  ItemRequestResponse ItemRequest(ItemRequestDirection in_out, string botid)
		{
		    using (new CallHelper(callInterval))
		    {

		        if (IsDebug)
		        {
		            //Log.w("ItemRequest({0}, {1})",in_out, botid);
		            return null;
		        }

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
		        check_errors(response.Content);

		        return response.Data;
		    }
		}

		/// <summary>
		/// Изменить/удалить трейд
		/// </summary>
		/// <param name="itemid">Уникальный номер вещи (получатеся после выполнения SetNewItem)</param>
		/// <param name="price">Цена в КОПЕЙКАХ, 0 - чтобы снять</param>
		/// <returns></returns>
		public  SetPriceResponse SetNewItem(string classid_instanceid, int price)
		{
		    using (new CallHelper(callInterval))
		    {

		        if (IsDebug)
		        {
		            //Log.w("SetNewItem({0}, {1})", classid_instanceid, price);
		            return null;
		        }

		        var request = new RestRequest("SetPrice/new_{classid_instanceid}/{price}", Method.GET);
		        request.AddParameter("classid_instanceid", classid_instanceid, ParameterType.UrlSegment);
		        request.AddParameter("price", price, ParameterType.UrlSegment);

		        var response = rest_client.Execute<SetPriceResponse>(request);

		        check_errors(response.Content);
		        //TODO: нормальная обработка ошибок запросов

		        return response.Data;
		    }
		}

		/// <summary>
		/// Получить список своих ордеров (покупок)
		/// </summary>
		/// <returns></returns>
		public  SetPriceResponse SetNewItem(string classid, string instanceid, int price)
		{
            return SetNewItem(classid + "_" + instanceid, price);
		}

		/// <summary>
		/// Изменение/удаление заявки на покупку
		/// </summary>
		/// <param name="price">Цена в КОПЕЙКАХ, 0 - снять ордер</param>
		/// <returns></returns>
		public  SetPriceResponse SetPrice(string itemid, int price)
		{
		    using (new CallHelper(callInterval))
		    {

		        if (IsDebug)
		        {
		            //Log.w("SetPrice({0}, {1})", itemid, price);
		            return null;
		        }

		        var request = new RestRequest("SetPrice/{itemid}/{price}", Method.GET);
		        request.AddParameter("itemid", itemid, ParameterType.UrlSegment);
		        request.AddParameter("price", price, ParameterType.UrlSegment);

		        var response = rest_client.Execute<SetPriceResponse>(request);

		        check_errors(response.Content);
		        //TODO: нормальная обработка ошибок запросов

		        return response.Data;
		    }
		}

		/// <summary>
		/// Отправка оффлайн-трейда от бота.
		/// Это заставляет бота создать обмен в стиме для получения или
		/// передачи предмета
		/// </summary>
		/// <param name="in_out">Получение или передача. In - передача, Out - получение/param>
		/// <param name="botid">ID бота, можно получить в результате Trades либо в событиях сокета</param>
		/// <returns></returns>
		public  UpdateOrderResponse UpdateOrder(string classid, string instanceid, int price)
		{
		    using (new CallHelper(callInterval))
		    {

		        if (IsDebug)
		        {
		            //Log.w("UpdateOrder({0}, {1}, {2})", classid, instanceid, price);
		            return null;
		        }

		        var request = new RestRequest("SetPrice/{classid}/{instanceid}/{price}", Method.GET);
		        request.AddParameter("classid", classid, ParameterType.UrlSegment);
		        request.AddParameter("instanceid", instanceid, ParameterType.UrlSegment);
		        request.AddParameter("price", price, ParameterType.UrlSegment);

		        var response = rest_client.Execute<UpdateOrderResponse>(request);

		        check_errors(response.Content);
		        //TODO: нормальная обработка ошибок запросов

		        return response.Data;
		    }
		}
	}
}
