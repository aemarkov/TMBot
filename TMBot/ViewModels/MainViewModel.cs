﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels
{
	/// <summary>
	/// Модель вида главного окна приложения
	/// </summary>
	public class MainViewModel
	{
        //Вкладки
		public HomeViewModel HomePage { get; set; }
		public MakeTradesViewModel MakeTradesPage { get; set; }
		public TradesViewModel TradesPage { get; set; }
		public OrdersViewModel OrdersPage { get; set; }


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
