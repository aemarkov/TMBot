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
using TMBot.Data;
using TMBot.Settings;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;
using TMBot.Windows;
using TMBot.Workers;
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

	    public ICommand SettingsCommand
	    {
            get { return new RelayCommands(showSettings);}
	    }

        //Вкладки
		public HomeViewModel HomePage { get; set; }
		public MakeTradesViewModel MakeTradesPage { get; set; }
		public TradesViewModel TradesPage { get; set; }
		public OrdersViewModel OrdersPage { get; set; }


        //Слушание сокетов
	    public WebSocketWorker WebSocketWorker;

        //Пинг
	    public PingWorker PingWorker;

        //Подтверждение стима
	    public SteamWorker SteamWorker;

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
		    var settings = SettingsManager.LoadSettings();

            TMFactory tm_factory = AbstactAPIFactory<ITMAPI>.GetInstance<TMFactory>();
            tm_factory.CreateAPI<CSTMAPI>(settings.TMApiKey, true);

            SteamFactory s_factory = AbstactAPIFactory<ISteamAPI>.GetInstance<SteamFactory>();
            s_factory.CreateAPI<CSSteamAPI>(settings.SteamProfileId, settings.SteamApiKey);

		    ITMAPI csTmapi = TMFactory.GetInstance<TMFactory>().GetAPI<CSTMAPI>();
		    ISteamAPI csSteamApi = SteamFactory.GetInstance<SteamFactory>().GetAPI<CSSteamAPI>();


            //Контейнер для элементов
            ItemCollectionsContainer.GetInstance().CreateList(TradePlatform.CSGO);
		    var csgoItems = ItemCollectionsContainer.GetInstance().GetList(TradePlatform.CSGO);


            //Лог
            LogList = new ObservableCollection<LogItem>();
			Log.NewLogMessage += Log_NewLogMessage;



            //Mapper
            MapperHelpers.InitializeMapper();



            //ViewModels
			HomePage = new HomeViewModel();
			MakeTradesPage = new MakeTradesViewModel();
			TradesPage = new TradesViewModel(csgoItems.Trades);
			OrdersPage = new OrdersViewModel(csgoItems.Orders);



            //Веб-сокеты
            WebSocketWorker = new WebSocketWorker("wss://wsn.dota2.net/wsn/");
            WebSocketWorker.Start();

            WebSocketWorker.Subscribe("itemout_new_go", new ItemNewGoEvent<CSTMAPI>());
            WebSocketWorker.Subscribe("itemstatus_go", new ItemStatusGoEvent());



            //Пинг
            PingWorker = new PingWorker();
            PingWorker.Start();



            //steam
            SteamWorker = new SteamWorker(csSteamApi);
            SteamWorker.Start();
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
            PingWorker.Stop();
	        SteamWorker.Stop();

            TradesPage.Dispose();
            OrdersPage.Dispose();

	    }

        //Настройки
	    void showSettings(object param)
	    {
	        var settings = new SettingsWindow();
	        settings.ShowDialog();
	    }
	}
}
