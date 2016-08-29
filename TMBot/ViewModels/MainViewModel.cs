using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;
using TMBot.Workers.WebSocket;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида главного окна приложения
	/// </summary>
	public class MainViewModel : IDisposable
	{
	    public ICommand WindowClosingCommand
	    {
	        get { return new RelayCommands(closing);}
	    }

        //Вкладки
		public HomeViewModel HomePage { get; set; }
		public MakeTradesViewModel MakeTradesPage { get; set; }
		public TradesViewModel TradesPage { get; set; }
		public OrdersViewModel OrdersPage { get; set; }


        //Слушание сокетов
	    public WebSocketWorker WebSocketWorker;

        //Лог
        private ObservableCollection<LogItem> _logList;
	    private readonly object _logListLock = new object();
	    public ObservableCollection<LogItem> LogList
	    {
            get { return _logList; }
	        set
	        {
	            _logList = value;
	            BindingOperations.EnableCollectionSynchronization(_logList,_logListLock);
	        }
	    }

		public MainViewModel()
		{
			LogList = new ObservableCollection<LogItem>();
			Log.NewLogMessage += Log_NewLogMessage;

            //Mapper
            MapperHelpers.InitializeMapper();

            //ViewModels
			HomePage = new HomeViewModel();
			MakeTradesPage = new MakeTradesViewModel();
			TradesPage = new TradesViewModel();
			OrdersPage = new OrdersViewModel();

            //Потоки
            WebSocketWorker = new WebSocketWorker("wss://wsn.dota2.net/wsn/");
            WebSocketWorker.Begin();
		}

		//Получение сообщения лога
		private void Log_NewLogMessage(string text, Log.Level level)
		{
            

             LogList.Add(new LogItem() { Text = text, Level = level });
		}

        //Закрытие окна
	    void closing(object param)
	    {
	        Dispose();
	    }

	    public void Dispose()
	    {
	        WebSocketWorker.End();

	    }
	}
}
