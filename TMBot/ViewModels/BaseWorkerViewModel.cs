using System.Threading.Tasks;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.Utilities.MVVM;
using TMBot.Workers;

namespace TMBot.ViewModels
{
    /// <summary>
    /// Базовая модель вида для продаж и покупок
    /// </summary>
    public abstract class BaseWorkerViewModel<TItem>
    {
        public IAsyncCommand ToggleCommand { get; set; }
        public  BaseWorker<CSTMAPI, CSSteamAPI, TItem> Worker { get; set; }

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
                await Task.Run(() => Worker.Begin());
            else
                Worker.End();
        }
    }
}