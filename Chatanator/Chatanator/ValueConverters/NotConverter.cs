using System;
using System.Globalization;
using Xamarin.Forms;

namespace Chatanator.UI.ValueConverters
{
    public class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return value;
            var on = (bool)value;
            return !on;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
