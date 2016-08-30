using System;
using System.Threading.Tasks;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.Utilities.MVVM.AsyncCommand;
using TMBot.Workers;

namespace TMBot.ViewModels
{
    /// <summary>
    /// Базовая модель вида для продаж и покупок
    /// </summary>
    public abstract class BaseWorkerViewModel<TItem> : IDisposable
    {
        public IAsyncCommand ToggleCommand { get; set; }
        public  BaseItemWorker<CSTMAPI, CSSteamAPI, TItem> Worker { get; set; }

        public abstract bool HasCountLimit { get; }
        public abstract string PriceLimitName { get; }

        protected BaseWorkerViewModel()
        {
            ToggleCommand = AsyncCommand.Create(toggle);
        }

        //Запуск\остановка обновления цены
        private async Task toggle(object param)
        {
            if (!Worker.IsRunning)
                await Task.Run(() => Worker.Start());
            else
                Worker.Stop();
        }

        public void Dispose()
        {
            Worker.Stop();
        }
    }
}