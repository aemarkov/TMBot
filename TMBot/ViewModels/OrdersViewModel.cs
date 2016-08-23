using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Utilities.MVVM;
using TMBot.Workers;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницы ордеров (покупка)
	/// </summary>
	public class OrdersViewModel : BaseWorkerViewModel<Order>
	{
	    public OrdersViewModel()
	    {
	        Worker = new OrderWorker<CSTMAPI, CSSteamAPI>();
	    }


	    public override bool HasCountLimit => true;
	    public override string PriceLimitName => "Максимальная цена";
	}
}
