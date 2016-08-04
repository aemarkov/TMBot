using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида главного окна приложения
	/// </summary>
	public class MainViewModel
	{
		public HomeViewModel HomePage { get; set; } = new HomeViewModel();
		public MakeTradesViewModel MakeTradesPage { get; set; } = new MakeTradesViewModel();
		public TradesViewModel TradesPage { get; set; } = new TradesViewModel();
		public OrdersViewModel OrdersPage { get; set; } = new OrdersViewModel();

	}
}
