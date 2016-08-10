using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида главного окна приложения
	/// </summary>
	public class MainViewModel
	{
		public HomeViewModel HomePage { get; set; }
		public MakeTradesViewModel MakeTradesPage { get; set; }
		public TradesViewModel TradesPage { get; set; }
		public OrdersViewModel OrdersPage { get; set; }

		public ObservableCollection<LogItem> LogList { get; set; }

		public MainViewModel()
		{
			LogList = new ObservableCollection<LogItem>();
			Log.NewLogMessage += Log_NewLogMessage;

			HomePage = new HomeViewModel();
			MakeTradesPage = new MakeTradesViewModel();
			TradesPage = new TradesViewModel();
			OrdersPage = new OrdersViewModel();
		}

		//Получение сообщения лога
		private void Log_NewLogMessage(string text, Log.Level level)
		{
			LogList.Add(new LogItem() { Text = text, Level = level });
		}
	}
}
