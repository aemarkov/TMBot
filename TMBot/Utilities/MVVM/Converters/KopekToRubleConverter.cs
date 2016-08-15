using System;
using System.Globalization;
using System.Windows.Data;

namespace TMBot.Utilities.MVVM.Converters
{
    /// <summary>
    /// Преобразует копейки в рубли
    /// </summary>
    public class KopekToRubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float rubles = ((int) value)/100.0f;
            return  rubles.ToString("C2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int kop =  (int)(decimal.Parse((string) value, NumberStyles.Currency)*100);
            return kop;
        }
    }
}