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

using TMBot.Models.TM;

namespace TMBot
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainViewModel ViewModel { get; set; }

		public MainWindow()
		{
			ViewModel = new MainViewModel();
			InitializeComponent();
			DataContext = ViewModel;

			var client = new RestClient();
			client.BaseUrl = new Uri("https://csgo.tm/api");
			client.Authenticator = new KeyAuthenticator("key", "Yg0skGdNIVST7811G6zGF8XDY29165T");

			//var request = new RestRequest("GetWSAuth", Method.GET);
			//var websock = client.Execute<WebSocketAuth>(request);

			//var request = new RestRequest("Trades", Method.GET);
			//var response = client.Execute<List<Trade>>(request);

			//var request = new RestRequest("GetOrders", Method.GET);
			//var response = client.Execute<OrdersList>(request);

			var request = new RestRequest("ItemInfo/{classid_instanceid}/{ru_or_en}", Method.GET);
			request.AddParameter("classid_instanceid", "310815809_188530139", ParameterType.UrlSegment);
			//request.AddParameter("ru_or_en", "ru", ParameterType.UrlSegment);
			var response = client.Execute<ItemInfo>(request);


		}
	}
}
