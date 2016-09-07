using System;
using System.Threading.Tasks;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.Data;
using TMBot.Utilities.MVVM.AsyncCommand;
using TMBot.ViewModels.ViewModels;
using TMBot.Workers;

namespace TMBot.ViewModels
{
    /// <summary>
    /// Базовая модель вида для продаж и покупок
    /// </summary>
    public abstract class BaseWorkerViewModel<TItem, TViewModel> : IDisposable where TViewModel : ItemViewModel
    {

        //Команда запуска\остановки воркера
        public IAsyncCommand ToggleCommand { get; set; }

        //Воркер для изменения цен
        public  BaseItemWorker<CSTMAPI, CSSteamAPI, TItem, TViewModel> Worker { get; set; }

        //Продажи или покупки
        public abstract bool IsBuying { get; }

        //Как называется столбец с ограничем по стоимости
        public abstract string PriceLimitName { get; }

        /// <summary>
        /// Создает модель вида 
        /// </summary>
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