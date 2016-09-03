using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Utilities.MVVM.Converters
{
    /// <summary>
    /// Преобразует ItemStatus в его текстовое описание
    /// </summary>
    class StatusToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (ItemStatus) value;

            switch (status)
            {
                case ItemStatus.NOT_TRADING:
                    return "Не выставляется";
                case ItemStatus.TRADING:
                    return "Выставляется";
                case ItemStatus.SOLD:
                    return "Продано, ожидание ItemRequest";
                case ItemStatus.SOLD_REQUEST:
                    return "Продано, ожидание передачи боту";
                case ItemStatus.GIVEN:
                    return "Передано боту";
                case ItemStatus.ORDERING:
                    return "Выставлен ордер";
                case ItemStatus.BOUGHT:
                    return "Куплено, ожидание ItemRequest";
                case ItemStatus.BOUGHT_REQUEST:
                    return "Продано, ожидание получение от бота";
                case ItemStatus.TAKEN:
                    return "Получено от бота";
                case ItemStatus.UNKNOWN:
                    return "Неизвестный статус";
                default:
                    return "Совсем неизвестный статус";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
