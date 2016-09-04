using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMBot.Annotations;
using TMBot.Database;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels.ViewModels
{
    /// <summary>
    /// Удобное представление объекта инвентаря стим
    /// Комбинация важных параметров RgItem и RgDescription
    /// </summary>
    public abstract class AbstractItemViewModel : PropertyChangedBase
    {
        private string _name;
        private string _imageUrl;
        private ItemStatus _itemStatus;
        private string _classId;
        private string _instanceId;

        private object statusLock = new object();

        /// <summary>
        /// Название предмета
        /// </summary>
		public string Name { get { return _name;  } set { _name = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Ссылка на изображение предмета
        /// </summary>
		public string ImageUrl { get { return _imageUrl; } set { _imageUrl = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Статус предмета. Так же используется
        /// для обеспечения безопасного межпоточного взаимодействия
        /// с предметов
        /// </summary>
        public ItemStatus Status
        {

            get
            {
                lock (statusLock)
                {
                    return _itemStatus;
                }
            }

            set
            {
                lock (statusLock)
                {
                    _itemStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }



        public string ClassId { get { return _classId; } set { _classId = value; NotifyPropertyChanged(); } }
        public string IntanceId { get { return _instanceId; } set { _instanceId = value; NotifyPropertyChanged(); } }

    }

    public class MakeTradesItemViewModel : AbstractItemViewModel
    {
        private bool _shouldSell;
        private int _sellPrice;

        /// <summary>
        /// Нужно ли выставлять предмет
        /// </summary>
        public bool ShouldSell
        {
            get { return _shouldSell;}
            set { _shouldSell = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Цена, по которой надо будет выставлять
        /// </summary>
        public int SellPrice
        {
            get { return _sellPrice; }
            set { _sellPrice = value; NotifyPropertyChanged(); }
        }
    }


    /// <summary>
    /// Представление предмета для покупки/продажия
    /// </summary>
    public class TradeItemViewModel : AbstractItemViewModel
    {
        private string _itemId;
        private int _tmPrice;
        private int _myPrice;
        private int _priceLimit;
        private int? _countLimit;

        public string ItemId { get { return _itemId; } set { _itemId = value; NotifyPropertyChanged(); } }
        public int TMPrice { get { return _tmPrice; } set { _tmPrice = value; NotifyPropertyChanged(); } }
        public int MyPrice { get { return _myPrice; } set { _myPrice = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Ограничение по цене
        /// </summary>
        public int PriceLimit
        {
            get { return _priceLimit; }
            set
            {
                if (PriceLimit == value) return;

                _priceLimit = value;
                NotifyPropertyChanged();

                _save();
            }
        }

        /// <summary>
        /// Ограничение по количеству (для покупки)
        /// </summary>
        public int? CountLimit
        {
            get { return _countLimit; }
            set
            {
                if(CountLimit == value) return;

                _countLimit = value;
                NotifyPropertyChanged();

                _save();
            }
        }

        //Сохраняет изменения этой модели в репозитории
        /* Не знаю, насколько это вообще кошерно - обновлять
         * модель из модели вида, потому что неизвестно, в общем случае,
         * из чего сделана эта модель вида, может она не имеет отношения
         * к Item */
        private void _save()
        {
            var repository = new ItemsRepository();
            var dto_model = repository.GetById(ClassId, IntanceId);

            if(dto_model==null)
                return;

            dto_model.PriceLimit = PriceLimit;
            dto_model.CountLimit = CountLimit;
            repository.Update(dto_model);
        }
    }

    /// <summary>
    /// Статусы предмета
    /// Вот немного костыльно, потому что они учитывают
    /// логику работы приложения
    /// </summary>
    public enum ItemStatus
    {
        NOT_TRADING,            //Не выставляется
        TRADING,                //Выставляется
        SOLD,                   //Вещь продана
        //SOLD_START_REQUEST,     //Вещь продана, начало выполнения ItemRequest
        SOLD_REQUEST,           //Выполнен ItemRequest после продажи
        GIVEN,                  //Вещь передана боту
        
        ORDERING,               //Ордер на вещь
        BOUGHT,                 //Мы купили вещь
        //BOUGHT_START_REQUEST,   //Мы купили вещь, начало выполнение ItemRequest
        BOUGHT_REQUEST,         //Выполнен ItemRequest после покупки
        TAKEN,                  //Вещь получена от бота

        UNKNOWN
    }
}
