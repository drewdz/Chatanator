using System;
using System.Globalization;
using Xamarin.Forms;

namespace Chatanator.UI.ValueConverters
{
    public class MessageMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return value;
            var sent = (bool)value;
            return (sent) ? new Thickness(64, 2, 2, 2) : new Thickness(2, 2, 64, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
