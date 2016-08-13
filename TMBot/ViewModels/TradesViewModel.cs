using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.TMAPI;
using TMBot.Utilities.MVVM;
using TMBot.Workers;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницый трейдов (продажа)
	/// </summary>
	public class TradesViewModel
	{
        public IAsyncCommand ToggleCommand { get; set; }


	    public SellWorker<CSTMAPI> SellWorker { get; set; } 
        public ObservableCollection<string> WTF { get; set; } 

        public TradesViewModel()
        {
            WTF = new ObservableCollection<string>();
            WTF.Add("FFFF");
            WTF.Add("FF4444");

            ToggleCommand = AsyncCommand.Create(toggle);
            SellWorker = new SellWorker<CSTMAPI>();
        }

        //Запуск\остановка обновления цены
        private async Task toggle(object param)
        {
            if(!SellWorker.IsRunning)
                SellWorker.Begin();
            else
                SellWorker.End();
        }
    }
}
