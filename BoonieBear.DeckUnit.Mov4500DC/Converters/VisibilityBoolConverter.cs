using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class VisibilityBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? boolValue = value as bool?;
            if (boolValue == null || boolValue == false)
            {
                string isCollapsed = parameter as string;
                if (isCollapsed != null && isCollapsed == "Collapsed")
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            else
            {
                return Visibility.Visible;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
