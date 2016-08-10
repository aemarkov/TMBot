using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.Models.TM;
using TMBot.Utilities;

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
		private IList<Trade> _trades;

		//Для управления потоком
		private volatile bool _shouldRun;

		//Фоновый поток обновления цен
		private Thread _workThread;

		private const int _loopPeriodMs = 1000;

		public SellWorker()
		{
			tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
		}

		/// <summary>
		/// Запуск обновления цен
		/// </summary>
		public void Begin()
		{
			_trades = tmApi.GetTrades();
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
					FixedTimeCall.Call(()=> {

						//Находим минимальную цену этого предмета на площадке
						decimal minPrice = PriceCounter.GetMinSellPrice<TTMAPI>(item.i_classid, item.ui_real_instance);

						//Уменьшаем ее на одну копейку
						minPrice -= 1;

						//TODO: убедиться, что itemid - это точно ui_id
						tmApi.SetPrice(item.ui_id, (int)minPrice);

						if (!_shouldRun)
							return;
					});

				}
			}
		}
	}
}