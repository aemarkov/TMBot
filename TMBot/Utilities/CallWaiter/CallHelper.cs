using System;
using System.Threading.Tasks;

namespace TMBot.Utilities.CallWaiter
{
    /// <summary>
    /// Объект для более удобного использования MinimumCallInterval.
    /// Используйте using, чтобы гарантировать выполнение метода Finish
    /// при выходе из scope
    /// </summary>
    public class CallHelper : IDisposable
    {
        private MinimumCallInterval _mci;
        public CallHelper(MinimumCallInterval mci)
        {
            _mci = mci;
            Task.Run(async () => await mci.Wait()).Wait();
        }

        private async Task wrapper()
        {
            await _mci.Wait();
        }

        public void Dispose()
        {
            _mci.Finish();
        }
    }
}