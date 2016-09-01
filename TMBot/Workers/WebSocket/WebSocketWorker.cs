using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMBot.API.Exceptions;
using TMBot.API.Factory;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Utilities;

namespace TMBot.Workers.WebSocket
{
    /// <summary>
    /// Реализует работу с веб-сокетами.
    /// Слушает веб-сокеты и обрабатывает оповещения 
    /// от сайта TM. Как-то не очень хорошо, API
    /// у меня в другом месте обычно
    /// </summary>
    public class WebSocketWorker : BaseWorker
    {
        private readonly string _url;
        private readonly ClientWebSocket _webSocket;

        //Список наблюдателей за событиями
        private readonly IDictionary<string, List<ITMWebSocketObserver>> _observers;

        private CancellationTokenSource _receiveToken;

        /// <summary>
        /// Создает новый объект WebSocketWorker с заданным адресом подключения
        /// </summary>
        /// <param name="url">Адрес подключения</param>
        public WebSocketWorker(string url)
        {
            _url = url;
            _webSocket = new ClientWebSocket();
            _observers = new Dictionary<string, List<ITMWebSocketObserver>>();
        }

        /// <summary>
        /// Запуск потока
        /// </summary>
        public async override void Start()
        {
            var connectToken = new CancellationTokenSource();
            connectToken.CancelAfter(TimeSpan.FromMilliseconds(1000));

            try
            {
                await _webSocket.ConnectAsync(new Uri(_url), connectToken.Token);

                //Теперь надо получить ключ подписки на веб-сокеты и отправить его
                var tmapi = TMFactory.GetInstance<TMFactory>().GetAPI<CSTMAPI>();
                WebSocketAuth auth = tmapi.GetWSAuth();
                await SendText(auth.wsAuth);

                RunThread();
            }
            catch (TaskCanceledException)
            {
                showErrorMessage("время ожидания истекло");
            }
            catch (WebSocketException exp)
            {
                showErrorMessage(exp.Message);
            }
            catch (APIException exp)
            {
                showErrorMessage("не удалось получить ключ подписки на веб сокеты: "+exp.Message);
            }
        }

        //Показывает сообщение об ошибке
        private void showErrorMessage(string reason)
        {
            if (MessageBox.Show(
                $"Не удалось подключиться к вебсокетам: {reason}. Вебсокеты обеспечивают лучшую работу программы. Продолжить работу программы без вебсокетов?",
                "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Добавляет наблюдателя для конкретного события
        /// </summary>
        /// <param name="eventName">Название события, на которое подписывается наблюдатель</param>
        /// <param name="observer">Наблюдатель</param>
        public void Subscribe(String eventName, ITMWebSocketObserver observer)
        {
            if (!_observers.ContainsKey(eventName))
            {
                _observers.Add(eventName, new List<ITMWebSocketObserver>());

                //Подписка на канал
                SendText(eventName);
            }


            _observers[eventName].Add(observer);
        }

        /// <summary>
        /// Добавляет наблюдателя для событий без имени
        /// </summary>
        /// <param name="observer"></param>
        public void Subscribe(ITMWebSocketObserver observer)
        {
            Subscribe("", observer);
        }

        /// <summary>
        /// Удаляет наблюдателя
        /// </summary>
        /// <param name="observer"></param>
        public void Unsubscribe(ITMWebSocketObserver observer)
        {
            IList<ITMWebSocketObserver> listWithObserver = null;
            string eventName = null;

            foreach (var obsList in _observers)
            {
                obsList.Value.Remove(observer);
                listWithObserver = obsList.Value;
                eventName = obsList.Key;
                break;
            }

            if(listWithObserver==null || eventName==null)
                return;

            if (listWithObserver.Count == 0)
            {
                _observers.Remove(eventName);
                //Отписаться от канала????
            }
        }

        /// <summary>
        /// Отправляет текст серверу
        /// </summary>
        /// <param name="text"></param>
        public async Task<CancellationTokenSource> SendText(string text)
        {
            var token = new CancellationTokenSource();
            var array = new ArraySegment<Byte>(Encoding.ASCII.GetBytes(text));
            await _webSocket.SendAsync(array, WebSocketMessageType.Text, true, token.Token);

            return token;
        }

        public override void Stop()
        {
            IsRunning = false;
            _receiveToken?.Cancel();

        }

        /// <summary>
        /// Поток
        /// </summary>
        protected async override void worker_tread()
        {
            while (IsRunning)
            {
                if (_webSocket.State == WebSocketState.Aborted)
                {
                    IsRunning = false;
                    return;
                }

                String text = "";

                try
                {

                    //Получение данных
                    byte[] arr = new byte[4096];
                    ArraySegment<Byte> buffer = new ArraySegment<byte>(arr);

                    _receiveToken = new CancellationTokenSource();
                    await _webSocket.ReceiveAsync(buffer, _receiveToken.Token);
                    text = Encoding.ASCII.GetString(buffer.Array);
                    Debug.WriteLine(text);

                    //Обработка разных событий и рассылка уведомлений

                    JObject response = JObject.Parse(text);
                    string type = (string) response["type"];
                    string data = (string) response["data"];

                    data = Regex.Unescape(data);

                    notifyOservers(type, data);
                }
                catch (JsonException exp)
                {
                    notifyOservers("", text);
                }
                catch (TaskCanceledException exp)
                {
                    return;
                }
                catch (Exception exp)
                {
                    Log.e(exp.Message);   
                }
            }
        }

        //Уведомляет наблюдателей о событии
        private void notifyOservers(string eventName, string data)
        {
            if (_observers.ContainsKey(eventName))
                foreach (var observer in _observers[eventName])
                    observer.HandleEvebt(data);
        }


    }
}