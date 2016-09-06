using System.Windows;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Data;
using TMBot.ViewModels.ViewModels;
using TMBot.Workers;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницый трейдов (продажа)
	/// </summary>
	public class TradesViewModel:BaseWorkerViewModel<Trade>
	{
       
        public TradesViewModel(SynchronizedObservableCollection<TradeItemViewModel> items):base()
        {
            Worker = new SellWorker<CSTMAPI,CSSteamAPI>(items);
        }


	    public override bool HasCountLimit => false;
	    public override string PriceLimitName => "Минимальная цена";
        public Visibility Vis => Visibility.Collapsed;
	}
}
