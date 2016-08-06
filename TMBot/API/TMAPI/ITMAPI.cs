using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.Models.TM;

namespace TMBot.API.TMAPI
{
	/// <summary>
	/// Направление ввода\вывода вещи
	/// </summary>
	public enum ItemRequestDirection
	{
		IN,   //Передача проданной вещи боту
		OUT   //Вывод купленной вещи
	}

	/// <summary>
	/// Интерфейс API для TM
	/// </summary>
	public interface ITMAPI
	{

		/// <summary>
		/// Возвращает информацию о всех продажах
		/// и покупках предмета
		/// </summary>
		/// <returns></returns>
		ItemInfo GetItemInfo(string classid_instanceid);

		/// <summary>
		/// Возвращает информацию о всех продажах
		/// и покупках предмета
		/// </summary>
		/// <returns></returns>
		ItemInfo GetItemInfo(string classid, string instanceid);




		/// <summary>
		/// Получить список своих трейдов (продаж)
		/// </summary>
		/// <returns></returns>
		IList<Trade> GetTrades();

		/// <summary>
		/// Выставить на продожу новый предмет
		/// </summary>
		/// <param name="price">цена в КОПЕЙКАХ</param>
		/// <returns></returns>
		SetPriceResponse SetNewItem(string classid_instanceid, int price);

		/// <summary>
		/// Выставить на продожу новый предмет
		/// </summary>
		/// <param name="price">цена в КОПЕЙКАХ</param>
		/// <returns></returns>
		SetPriceResponse SetNewItem(string classid, string instanceid, int price);

		/// <summary>
		/// Изменить/удалить трейд
		/// </summary>
		/// <param name="itemid">Уникальный номер вещи (получатеся после выполнения SetNewItem)</param>
		/// <param name="price">Цена в КОПЕЙКАХ, 0 - чтобы снять</param>
		/// <returns></returns>
		SetPriceResponse SetPrice(string itemid, int price);




		/// <summary>
		/// Получить список своих ордеров (покупок)
		/// </summary>
		/// <returns></returns>
		OrdersList GetOrders();

		/// <summary>
		/// Изменение/удаление заявки на покупку
		/// </summary>
		/// <param name="price">Цена в КОПЕЙКАХ, 0 - снять ордер</param>
		/// <returns></returns>
		UpdateOrderResponse UpdateOrder(string classid, string instanceid, int price);




		/// <summary>
		/// Отправка оффлайн-трейда от бота.
		/// Это заставляет бота создать обмен в стиме для получения или
		/// передачи предмета
		/// </summary>
		/// <param name="in_out">Получение или передача. In - передача, Out - получение/param>
		/// <param name="botid">ID бота, можно получить в результате Trades либо в событиях сокета</param>
		/// <returns></returns>
		ItemRequestResponse ItemRequest(ItemRequestDirection in_out, string botid);

	}
}
