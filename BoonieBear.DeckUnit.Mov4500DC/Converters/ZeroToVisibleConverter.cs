using System;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class ZeroToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is int))
            {
                return Visibility.Visible;
            }
            int intVal = (int)value;
            if (intVal == 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
