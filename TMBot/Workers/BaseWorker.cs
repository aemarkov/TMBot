using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using AutoMapper;
using TMBot.API.Exceptions;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.API.TMAPI.Models;
using TMBot.Database;
using TMBot.Models;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Workers
{
    /// <summary>
    /// Базовый класс потока, осуществляющего покупку\продажу
    /// предметов
    /// </summary>
    /// <typeparam name="TTMAPI">Тип АПИ для ТМ</typeparam>
    /// <typeparam name="TSteamAPI">Тип АПИ для стима</typeparam>
    /// <typeparam name="TItem">Тип модели АПИ (ордеры, трейды итп)</typeparam>
    public abstract class BaseWorker<TTMAPI, TSteamAPI, TItem> : PropertyChangedBase where TTMAPI : ITMAPI where TSteamAPI : ISteamAPI
    {
        //Апи для выполнения запросов
        protected ITMAPI tmApi;
        protected ISteamAPI steamApi;

        //Список предметов
        #region Items
        private object _itemsLock = new object();
        private ObservableCollection<TradeItemViewModel> _items;

        public ObservableCollection<TradeItemViewModel> Items
        {
            get { return _items; }
            protected set
            {
                _items = value;
                BindingOperations.EnableCollectionSynchronization(_items, _itemsLock);
            }
        }

        #endregion

        /// <summary>
        /// Запущен ли поток обновления
        /// </summary>
        #region IsRunning
        public bool IsRunning
        {
            get { return _isRunning; }
            protected set { _isRunning = value; NotifyPropertyChanged(); }
        }
        private volatile bool _isRunning;
        #endregion


        /// <summary>
        /// Текущий выделенный элемент списка
        /// </summary>
        #region LastUpdateItemIndex
        public TradeItemViewModel LastUpdateItem
        {
            get { return _lastUpdatedItem; }
            set { _lastUpdatedItem = value; NotifyPropertyChanged(); }
        }
        private TradeItemViewModel _lastUpdatedItem;
        #endregion

        // Разница в процентах между нашей ценой и следующей,
        // меньше которой цена не меняется
        #region OffsetPercentage

        public float OffsetPercentage
        {

            get { return _offsetPercentage; }
            set
            {
                _offsetPercentage = value;
                NotifyPropertyChanged();
            }
        }

        private float _offsetPercentage=0;

        #endregion


        //Фоновый поток обновления цен
        protected Thread _workThread;


        protected BaseWorker()
        {
            //_repository = new ItemsRepository();
            tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
            steamApi = SteamFactory.GetInstance<SteamFactory>().GetAPI<TSteamAPI>();
            Items = new ObservableCollection<TradeItemViewModel>();
        }

        /// <summary>
        /// Запуск обновления цен
        /// </summary>
        public void Begin()
        {
            /* Получаем списко трейдов/ордеров и список 
            * всех предметов из БД.
            * Выбираем те предметы, которые сейчас
            * продаются/покупаются, чтобы получить из них 
            * настройки мин, макс цены
            */

            try
            {
                var repository = new ItemsRepository();
                Items.Clear();

                var api_items = GetTMItems();
                foreach (var api_item in api_items)
                {
                    Item db_item = GetDbItem(repository, api_item);

                    //Заполняем поля
                    var item = Mapper.Map<TItem, TradeItemViewModel>(api_item);

                    //TODO: сделать нормально
                    //Цена почему-то не в копейках, а в рублях double
                    item.MyPrice = GetItemMyPrice(api_item);

                    if (db_item != null)
                    {
                        Mapper.Map<Item, TradeItemViewModel>(db_item, item);
                    }
                    else
                    {
                        //Такого предмета нет в БД, создадим новый
                        db_item = new Item()
                        {
                            ClassId = item.ClassId,
                            InstanceId = item.IntanceId
                        };

                        repository.Create(db_item);
                    }

                    item.ImageUrl = steamApi.GetImageUrl(item.ClassId);
                    Items.Add(item);
                }

                IsRunning = true;
                _workThread = new Thread(worker_tread);
                _workThread.Start();

            }
            catch (BadKeyException exp)
            {
                ShowErrorMessage("неверный API-key");
            }
            catch (APIException exp)
            {
                ShowErrorMessage(Environment.NewLine+exp.Message);
            }
            catch (Exception exp)
            {
                ShowErrorMessage("неизвестная ошибка");
            }
        }

        /// <summary>
        /// Остановка обновления цен
        /// </summary>
        public void End()
        {
            IsRunning = false;
        }

        //Функция потока обновления цены
        private async void worker_tread()
        {
            /* Необходимо выполнять не более 3х запросов в секунду.
			 * Запрос - ItemRequest
			 * Запрос - изменение цены
			 * Поэтому ограничим 1 предмет в секунду
			 * 
			 * Т.к. операции выполняются какое-то время, то ожидание в 1с
			 * после выполнения операции приведет к тому, что цикл обработки
			 * будет медленнее, чем 1с.
			 * 
			 * Поэтому замерем время, а потом подаждем оставшееся время
			 */

            while (IsRunning)
            {
                foreach (var item in Items)
                {
                    await FixedTimeCall.Call(() => {
                        update_price(item);
                    });

                    LastUpdateItem = item;

                    if (!IsRunning)
                        return;

                }
            }
        }

        private void update_price(TradeItemViewModel item)
        {
            try
            {
                //Находим цену этого предмета на площадке
                int? _tm_price = GetItemTMPrice(item);
                int tm_price;

                if (_tm_price == null)
                {
                    //Товар не найден на площадке, значит единственный  - наш товар
                    //Значит мы ничего не делаем
                    return;
                }
                else
                    tm_price = (int)_tm_price;


                //Изменяем цену
                int my_new_price=item.MyPrice;
                if(!GetItemNewPrice(item, tm_price, ref my_new_price))
                    return;

                //Обновляем цену предмета
                //TODO: убедиться, что itemid - это точно ui_id
                tmApi.SetPrice(item.ItemId, (int)my_new_price);

                //Обновляем модель
                item.MyPrice = my_new_price;
                item.TMPrice = tm_price;
            }
            catch (Exception exp)
            {

                Log.e("Произошла ошибка при обновлении цены предмета {0}_{1}: {2}", item.ClassId, item.IntanceId, exp.Message);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Показывает сообщение об ошибке
        /// </summary>
        /// <param name="error_reason"></param>
        protected abstract void ShowErrorMessage(string error_reason);

        /// <summary>
        /// Получает список предметов по АПИ
        /// </summary>
        /// <returns></returns>
        protected abstract ICollection<TItem> GetTMItems();

        /// <summary>
        /// Загружает соответствующий предмет из БД
        /// </summary>
        /// <param name="api_item"></param>
        /// <returns></returns>
        protected abstract Item GetDbItem(ItemsRepository repository, TItem api_item);

        /// <summary>
        /// Получает текущую мою стоимость предмета
        /// </summary>
        /// <param name="api_item"></param>
        /// <returns>Моя текущая цена предмета</returns>
        protected abstract int GetItemMyPrice(TItem api_item);

        /// <summary>
        /// Расчитывает новую цену предмета
        /// </summary>
        /// <param name="item">Предмет</param>
        /// <param name="myNewPrice">Полученная цена</param>
        /// <returns>Изменилась ли цена</returns>
        protected abstract bool GetItemNewPrice(TradeItemViewModel item, int tm_price, ref int myNewPrice);

        /// <summary>
        /// Находит экстремеальную стоимость предмета на площадке
        /// </summary>
        /// <param name="item">Предмет</param>
        /// <returns>Стоимость</returns>
        protected abstract int? GetItemTMPrice(TradeItemViewModel item);
    }
}