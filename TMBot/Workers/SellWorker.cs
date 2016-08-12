using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AutoMapper;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Database;
using TMBot.Models;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Workers
{
	/// <summary>
	/// Выполняет мониторинг и изменнеие цены продажи
	/// предметов в фоне
	/// </summary>
	/// <typeparam name="TTMAPI">Класс АПИ площадки</typeparam>
	public class SellWorker<TTMAPI> where TTMAPI: ITMAPI
	{
		//Апи для выполнения запросов
		private readonly ITMAPI tmApi;

		//Список продаваемых предметов
		private IList<TradeItemViewModel> _trades;

		//Для управления потоком
		private volatile bool _shouldRun;

		//Фоновый поток обновления цен
		private Thread _workThread;

	    private ItemsRepository _repository;

		public SellWorker()
		{
            _repository = new ItemsRepository();
			tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
		}

		/// <summary>
		/// Запуск обновления цен
		/// </summary>
		public void Begin()
		{
            /* Получаем списко трейдов и список 
             * всех предметов из БД.
             * Выбираем те предметы, которые сейчас
             * продаются, чтобы получить из них 
             * настройки мин, макс цены
             */

			var trades = tmApi.GetTrades();
            _trades = new List<TradeItemViewModel>();

            foreach(var trade_item in trades)
            {
                Item db_item = _repository.GetById(trade_item.i_classid, trade_item.ui_real_instance);
               
                //Заполняем поля
                var mapper = MapperHelpers.MapTradeToTradeItem();
                var item = mapper.Map<Trade, TradeItemViewModel>(trade_item);

                if (db_item != null)
                {
                    mapper.Map<Item, TradeItemViewModel>(db_item, item);
                }
                else
                {
                    //Такого предмета нет в БД, создадим новый
                    db_item = new Item()
                    {
                        ClassId = item.ClassId,
                        InstanceId = item.IntanceId
                    };

                    _repository.Create(db_item);
                }

                _trades.Add(item);
            }


			_shouldRun = true;
			_workThread = new Thread(worker_thread);
			_workThread.Start();
			
		}

		/// <summary>
		/// Остановка обновления цен
		/// </summary>
		public void End()
		{
			_shouldRun = false;
		}

		//Функция потока
		private void worker_thread()
		{
			/* Необходимо выполнять не более 3х запросов в секунду.
			 * Запрос - ItemRequest
			 * Запрос - изменение цены
			 * Поэтому ограничим 1 предмет в секунду
			 * 
			 * Т.к. операции выполняются какое-то время, то ожидание в 1с
			 * после выполнения операции приведет к тому, что цикл обработки
			 * будет медленнее, чем 1с.
			 * 
			 * Поэтому замерем время, а потом подаждем оставшееся время
			 */

			while (_shouldRun)
			{
				foreach (var item in _trades)
				{
					FixedTimeCall.Call(()=>{
                        update_price(item);
						if (!_shouldRun)
							return;
					});

				}
			}
		}

	    private void update_price(TradeItemViewModel item)
	    {
            //Находим минимальную цену этого предмета на площадке
            int minPrice = PriceCounter.GetMinSellPrice<TTMAPI>(item.ClassId, item.IntanceId);

            //Уменьшаем ее на одну копейку
            item.TMPrice = minPrice;
            int new_my_price;

            /* Если минимальная цена меньше текущей - делаем нашу меньше минимальной на 
             * 1 коп.
             * 
             * Если минимальная - наша, то увеличиваем цену (до минимальной - 1коп) только
             * если разница больше 10%
             */
            if (minPrice < item.MyPrice || ((minPrice - item.MyPrice) / (float)item.MyPrice > 0.1f))
            {
                new_my_price = minPrice - 1;
            }
            else
                new_my_price = item.MyPrice;

            if (new_my_price < item.PriceLimit)
                new_my_price = item.PriceLimit;

            //Обновляем цену предмета
            //TODO: убедиться, что itemid - это точно ui_id
            tmApi.SetPrice(item.ItemId, (int)new_my_price);

            //Обновляем модель
	        item.MyPrice = new_my_price;
	        item.TMPrice = minPrice;

	    }
    }
}