﻿using System.Windows;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Workers;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницый трейдов (продажа)
	/// </summary>
	public class TradesViewModel:BaseWorkerViewModel<Trade>
	{
       
        public TradesViewModel():base()
        {
            Worker = new SellWorker<CSTMAPI,CSSteamAPI>();
        }


	    public override bool HasCountLimit => false;
	    public override string PriceLimitName => "Минимальная цена";
        public Visibility Vis => Visibility.Collapsed;
	}
}
