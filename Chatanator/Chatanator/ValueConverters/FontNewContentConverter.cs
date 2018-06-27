using System;
using System.Globalization;
using Xamarin.Forms;

namespace Chatanator.UI.ValueConverters
{
    public class FontNewContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return value;
            var newContent = (bool)value;
            return (newContent) ? FontAttributes.Bold : FontAttributes.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
