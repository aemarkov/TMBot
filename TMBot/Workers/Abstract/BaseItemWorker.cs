using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AutoMapper;
using TMBot.API.Exceptions;
using TMBot.API.Factory;
using TMBot.API.SteamAPI;
using TMBot.API.TMAPI;
using TMBot.Data;
using TMBot.Database;
using TMBot.Models;
using TMBot.Utilities;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Workers
{
    /// <summary>
    /// Базовый класс потока, осуществляющего покупку\продажу.
    /// Циклически обрабатывает список предметов, меняя им цену
    /// 
    /// Алгоритм покупки\продажи:
    /// При запуске потока загружается список  ордеров\трейдов с 
    /// сайта. Каждый переводится в TradeItemViewModel и сохраяется
    /// в базе данных (если не был сохранен ранее).
    /// 
    /// Затем циклически обновляется цена:
    /// 1. Находится минимальная\максимальная цена на торговой
    ///    площадке, наша выставляется +- 1 коп. Если цена не
    ///    найдена - наша не трогается.
    /// 
    /// 2. Так же существует такое ограничение: если наша цена 
    ///    самая маленькая\большая, то вычислияется разница между
    ///    нашим и следующим предметом, и если разница меньше
    ///    порога, то мы не меняем цену (держми мин\макс)
    /// 
    /// 3. После каждой итерации по всем предметам, список
    ///    предметов загружается заново, для обновления списка
    ///    (см. реализацию в наследниках)
    /// 
    /// 4. В процессе обновления учитывается статус предмета. Нормально
    ///    обрабатываются только продаваемые\покупаемые предметы.
    ///    (те, которые еще не купили\продали)
    ///    Если предмет находится в одном из состояний покупки\продажи,
    ///    то либо выполняется ItemRequest, либо предмет игнорируется
    ///    (ItemRequest уже выполнен) 
    /// 
    /// предметов
    /// </summary>
    /// <typeparam name="TTMAPI">Тип АПИ для ТМ</typeparam>
    /// <typeparam name="TSteamAPI">Тип АПИ для стима</typeparam>
    /// <typeparam name="TItem">Тип модели АПИ (ордеры, трейды итп)</typeparam>
    /// <typeparam name="TViewModel">Тип модели вида для элемента</typeparam>
    public abstract class BaseItemWorker<TTMAPI, TSteamAPI, TItem, TViewModel> : BaseWorker where TTMAPI : ITMAPI where TSteamAPI : ISteamAPI where TViewModel : ItemViewModel
    {
        //Апи для выполнения запросов
        protected ITMAPI tmApi;
        protected ISteamAPI steamApi;

        //Список предметов
        #region Items
        private object _itemsLock = new object();
        private SynchronizedObservableCollection<ItemViewModel> _items;

        public SynchronizedObservableCollection<ItemViewModel> Items
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
        /// Текущий выделенный элемент списка
        /// </summary>
        #region LastUpdateItemIndex
        public ItemViewModel LastUpdateItem
        {
            get { return _lastUpdatedItem; }
            set { _lastUpdatedItem = value; NotifyPropertyChanged(); }
        }
        private ItemViewModel _lastUpdatedItem;
        #endregion

        /// Максимальная разница между нашей и следующей ценой, при
        /// который наша цена не меняется
        #region PriceThreshold
        public float PriceThreshold
        {

            get { return _priceThreshold; }
            set
            {
                _priceThreshold = value;
                NotifyPropertyChanged();
            }
        }

        private float _priceThreshold=0;

        #endregion

        protected BaseItemWorker(SynchronizedObservableCollection<ItemViewModel> items )
        {
            //_repository = new ItemsRepository();
            tmApi = TMFactory.GetInstance<TMFactory>().GetAPI<TTMAPI>();
            steamApi = SteamFactory.GetInstance<SteamFactory>().GetAPI<TSteamAPI>();

            Items = items;
        }

        /// <summary>
        /// Запуск обновления цен.
        /// 
        /// Загружается список всех предметов с сайта
        /// и переводится в список TradeItemViewModel
        /// </summary>
        public override void Start()
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

                GetItems(repository);

                RunThread();

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
        /// Получает предметы с сайта
        /// </summary>
        /// <param name="repository"></param>
        protected virtual void GetItems(ItemsRepository repository)
        {
            var api_items = GetTMItems();
            foreach (var api_item in api_items)
            {
                Items.Add(CreateTradeItem(api_item, repository));
            }
        }

       
        /// <summary>
        /// Создает модель вида для предмета на основе
        /// модели ААИ
        /// </summary>
        /// <param name="apiItem">Модель АПИ</param>
        /// <param name="repository">Репозиторий</param>
        /// <returns></returns>
        protected ItemViewModel CreateTradeItem(TItem apiItem, ItemsRepository repository)
        {
            //Заполняем поля
            var item = Mapper.Map<TItem, TViewModel>(apiItem);

            item.MyPrice = GetItemMyPrice(apiItem);

            //Добавляем данные из базы данных
            MapDbItem(apiItem, repository, item);

            item.ImageUrl = steamApi.GetImageUrl(item.ClassId);

            return item;
        }

        //Загружает из БД или создает новую запись в БД
        private void MapDbItem(TItem apiItem, ItemsRepository repository, TViewModel item)
        {
            Item db_item = GetDbItem(repository, apiItem);
            if (db_item != null)
            {
                Mapper.Map<Item, TViewModel>(db_item, item);
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
        }


        //Функция потока обновления цены
        protected override void worker_tread()
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
                //НЕ МЕНЯТЬ НА FOREACH,БЛЕАТЬ
                for(int i = 0; i<Items.Count; i++)
                {
                    update_price(Items[i]);

                    if (!IsRunning)
                        return;

                }
                
                //Обновляем список и статусы предметов
                UpdateItems();
            }
        }


        private void update_price(ItemViewModel item)
        {
            try
            {
                //Необходимо проверить статус предмета
                //и выполинить, при необходимости, действия
                if (!CheckStatusAndMakeRequest(item)) return;

                //Находим цену этого предмета на площадке
                int? _tm_price = GetItemTMPrice(item);
                int tm_price;

                if (_tm_price == null)
                {
                    //Товар не найден на площадке, значит единственный  - наш товар
                    //Значит мы ничего не делаем
                    return;
                }

                tm_price = (int)_tm_price;
                item.TMPrice = tm_price;


                //Изменяем цену
                int my_new_price=item.MyPrice;
                if(!GetItemNewPrice(item, tm_price, ref my_new_price))
                    return;

                //Обновляем цену предмета
                UpdatePrice(item, (int)my_new_price);

                //Обновляем модель
                item.MyPrice = my_new_price;
            }
            catch (Exception exp)
            {

                Log.e("Произошла ошибка при обновлении цены предмета {0}_{1}: {2}", item.ClassId, item.IntanceId, exp.Message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        /// Абстрактные методы
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
        protected abstract bool GetItemNewPrice(ItemViewModel item, int tm_price, ref int myNewPrice);

        /// <summary>
        /// Находит экстремеальную стоимость предмета на площадке
        /// </summary>
        /// <param name="item">Предмет</param>
        /// <returns>Стоимость</returns>
        protected abstract int? GetItemTMPrice(ItemViewModel item);


        /// <summary>
        /// Обновление списка предметов
        /// </summary>
        protected abstract void UpdateItems();

        /// <summary>
        /// Проверяем статус предмета. В зависимости от статуса:
        ///  - нормально меняем цену
        ///  - делаем ItemRequest
        ///  - игнорируем
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Продолжать выполнение</returns>
        protected abstract bool CheckStatusAndMakeRequest(ItemViewModel item);

        /// <summary>
        /// Обновляет цену
        /// </summary>
        /// <param name="itemid">ID предмета</param>
        /// <param name="price">новая цена</param>
        protected abstract void UpdatePrice(ItemViewModel item, int price);
    }
}