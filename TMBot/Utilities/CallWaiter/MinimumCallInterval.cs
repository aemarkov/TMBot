using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TMBot.Utilities.CallWaiter
{
    /// <summary>
    /// Гарантирует, что код методов будет вызываться не чаще, 
    /// чем в заданное время.
    /// 
    /// Если метод будет вызван раньше, задача будет задержана на оставшееся
    /// время
    /// 
    /// ВНИМАНИЕ: ТРЕБУЕТСЯ AWAIT
    /// </summary>
    public class MinimumCallInterval
    {
        private readonly Stopwatch _stopwatch;
        private long _waitTime;
        private readonly Semaphore _semaphore;

        /// <summary>
        /// Создает новый объект MinimumCallInterval с заданным временем интервала
        /// </summary>
        /// <param name="waitTime">Минимальное время между вызовами методов</param>
        public MinimumCallInterval(long waitTime)
        {
            _waitTime = waitTime;
            _stopwatch = new Stopwatch();
            _semaphore = new Semaphore(1,1);
        }

        /// <summary>
        /// Начало метода, блокирует выполнение и запускает таймер
        /// </summary>
        /// <returns></returns>
        public async Task Wait()
        {
            _semaphore.WaitOne();

            _stopwatch.Stop();
            if (_stopwatch.Elapsed.Ticks != 0 && _stopwatch.ElapsedMilliseconds < _waitTime)
            {
                long delay = _waitTime - _stopwatch.ElapsedMilliseconds;
                //Thread.Sleep((int)delay);
                await Task.Delay((int)delay);
            }
        }

        /// <summary>
        /// Конец метода, снимает блокировку и останавливает таймер
        /// </summary>
        public void Finish()
        {
            _stopwatch.Restart();
            _semaphore.Release();
        }
    }
}