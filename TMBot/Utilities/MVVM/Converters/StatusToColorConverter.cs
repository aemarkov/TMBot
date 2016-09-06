using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TMBot.ViewModels.ViewModels;

namespace TMBot.Utilities.MVVM.Converters
{
    class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (ItemStatus)value;

            switch (status)
            {
                case ItemStatus.NOT_TRADING:
                    return Brushes.Red;
                case ItemStatus.TRADING:
                    return Brushes.Green;
                case ItemStatus.SOLD:
                    return Brushes.Chocolate;
                case ItemStatus.SOLD_REQUEST:
                    return Brushes.Purple;
                case ItemStatus.GIVEN:
                    return Brushes.Blue;
                case ItemStatus.ORDERING:
                    return Brushes.Green;
                case ItemStatus.BOUGHT:
                    return Brushes.Aqua;
                case ItemStatus.BOUGHT_TAKE:
                    return Brushes.Chocolate;
                case ItemStatus.BOUGHT_REQUEST:
                    return Brushes.Purple;
                case ItemStatus.TAKEN:
                    return Brushes.Blue;
                case ItemStatus.UNKNOWN:
                    return Brushes.DarkGray;
                default:
                    return Brushes.DarkGray;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
