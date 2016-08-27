using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TMBot.Utilities.CallWaiter
{
	/// <summary>
	/// Этот класс используется, чтобы метод вызывался не быстрее определенного времени
	/// Если метод вызывается быстрее - будет ожидание
	/// </summary>
	public class FixedTimeCall
	{
		//Требуемое время в миллисекундах
		private const int _callTimeMillis = 1000;

		/// <summary>
		/// Вызов метода за определенное время
		/// </summary>
		/// <param name="action">Метод</param>
		public static async Task  Call(Action action)
		{
			Stopwatch sw = new Stopwatch();

			sw.Start();
			await Task.Run(action);
			sw.Stop();

			long waitMs = _callTimeMillis - sw.ElapsedMilliseconds;
		    if (waitMs > 0)
		        //Thread.Sleep((int)waitMs);
		        await Task.Delay((int) waitMs);
		}
	}
}
