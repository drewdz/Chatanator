using Chatanator.Core.Services;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Chatanator.UI.ValueConverters
{
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ChatState)) return value;
            var state = (ChatState)value;
            return (state == ChatState.None) ? string.Empty : state.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
