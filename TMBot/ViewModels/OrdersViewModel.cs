using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Data;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;
using TMBot.Workers;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида для страницы ордеров (покупка)
	/// </summary>
	public class OrdersViewModel : BaseWorkerViewModel<Order, OrderItemViewModel>
	{
	    public OrdersViewModel(SynchronizedObservableCollection<ItemViewModel>  items)
	    {
	        Worker = new OrderWorker<CSTMAPI, CSSteamAPI>(items);
	    }


	    public override bool IsBuying => true;

	    public override string PriceLimitName => "Максимальная цена";
	}
}
