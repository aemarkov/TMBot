using System;
using System.Globalization;
using System.Windows.Data;

namespace TMBot.Utilities.MVVM.Converters
{
    public class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Неограниченно";


            int count = (int) value;
            return count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = (string) value;
            if (String.IsNullOrEmpty(val) || String.IsNullOrWhiteSpace(val))
                return null;

            return int.Parse(val);
        }
    }
}