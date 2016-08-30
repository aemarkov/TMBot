using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TMBot.ViewModels;
using RestSharp;
using RestSharp.Authenticators;
using TMBot.API;
using System.Net.Http;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using TMBot.Models.Steam;
using TMBot.API.SteamAPI;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.Database;

namespace TMBot
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainViewModel ViewModel { get; set; }

		public MainWindow()
		{

            ViewModel = new MainViewModel();
			InitializeComponent();
			DataContext = ViewModel;

		}
	}
}
