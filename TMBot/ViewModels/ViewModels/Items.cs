using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMBot.Annotations;
using TMBot.Utilities;
using TMBot.Utilities.MVVM;
using TMBot.ViewModels.ViewModels;

namespace TMBot.ViewModels.ViewModels
{
    /// <summary>
    /// Удобное представление объекта инвентаря стим
    /// Комбинация важных параметров RgItem и RgDescription
    /// </summary>
    public class InventoryItemViewModel : PropertyChangedBase
    {
        private string _name;
        private string _imageUrl;
        private bool _isSelling;
        private string _classId;
        private string _instanceId;

		public string Name { get { return _name;  } set { _name = value; NotifyPropertyChanged(); } }
		public string ImageUrl { get { return _imageUrl; } set { _imageUrl = value; NotifyPropertyChanged(); } }
        public bool IsSelling { get { return _isSelling; } set { _isSelling = value; NotifyPropertyChanged(); } }
        public string ClassId { get { return _classId; } set { _classId = value; NotifyPropertyChanged(); } }
        public string IntanceId { get { return _instanceId; } set { _instanceId = value; NotifyPropertyChanged(); } }

    }

    /// <summary>
    /// Представление предмета для покупки/продажия
    /// </summary>
    public class TradeItemViewModel : InventoryItemViewModel
    {
        private string _itemId;
        private int _tmPrice;
        private int _myPrice;
        private int _priceLimit;
        private int? _countLimit;

        public string ItemId { get { return _itemId; } set { _itemId = value; NotifyPropertyChanged(); } }
        public int TMPrice { get { return _tmPrice; } set { _tmPrice = value; NotifyPropertyChanged(); } }
        public int MyPrice { get { return _myPrice; } set { _myPrice = value; NotifyPropertyChanged(); } }
        public int PriceLimit { get { return _priceLimit; } set { _priceLimit = value; NotifyPropertyChanged(); } }
        public int? CountLimint { get { return _countLimit; } set { _countLimit = value; NotifyPropertyChanged(); } }
    }
}
