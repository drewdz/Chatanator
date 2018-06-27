using System;
using System.Globalization;
using Xamarin.Forms;

namespace Chatanator.UI.ValueConverters
{
    public class AvailableColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return value;
            var available = (bool)value;
            return (available) ? Color.Black : Color.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
