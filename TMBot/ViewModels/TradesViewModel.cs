﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.SteamAPI;
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


	    public SellWorker<CSTMAPI,CSSteamAPI> SellWorker { get; set; } 

	    public bool WTF => true;

        public TradesViewModel()
        {

            ToggleCommand = AsyncCommand.Create(toggle);
            SellWorker = new SellWorker<CSTMAPI,CSSteamAPI>();
        }

        //Запуск\остановка обновления цены
        private async Task toggle(object param)
        {
            if(!SellWorker.IsRunning)
                await Task.Run(()=>SellWorker.Begin());
            else
                SellWorker.End();
        }
    }
}
