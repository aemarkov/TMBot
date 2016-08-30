using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMWebSockAPI;
using TMBot.Settings;
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
            //API
            TMFactory tm_factory = AbstactAPIFactory<ITMAPI>.GetInstance<TMFactory>();
            //tm_factory.CreateAPI<CSTMAPI>("69SiW4t4ja7BBdihH2UCjb31x275b14", true);
            tm_factory.CreateAPI<CSTMAPI>("BAsMgHzPpM31Tdkf4KeFiX0Ntjg6E46", true);

            SteamFactory s_factory = AbstactAPIFactory<ISteamAPI>.GetInstance<SteamFactory>();
            //s_factory.CreateAPI<CSSteamAPI>("76561198289262955", "868AC98202BC8C4912E3864E26881E1C");
            s_factory.CreateAPI<CSSteamAPI>("76561198031028693", "1644002B410BCCC13E4B3C11A12F82EF");

            //Лог
            LogList = new ObservableCollection<LogItem>();
			Log.NewLogMessage += Log_NewLogMessage;

            //Mapper
            MapperHelpers.InitializeMapper();

            //ViewModels
			HomePage = new HomeViewModel();
			MakeTradesPage = new MakeTradesViewModel();
			TradesPage = new TradesViewModel();
			OrdersPage = new OrdersViewModel();

            //Веб-сокеты
            WebSocketWorker = new WebSocketWorker("wss://wsn.dota2.net/wsn/");
            WebSocketWorker.Start();

            WebSocketWorker.Subscribe("itemout_new_go", new ItemBoughtEvent());
            //WebSocketWorker.Subscribe("additem_go", new ItemSoldEvent());              //??
            WebSocketWorker.Subscribe("itemstatus_go", new ItemTransferedEvent());
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
	        WebSocketWorker.Stop();

            TradesPage.Dispose();
            OrdersPage.Dispose();

	    }
	}
}
