using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool)value;
            string para = parameter as string;

            if (para == "TrueToVisible")
            {
                return isTrue ? Visibility.Visible : Visibility.Collapsed;   
            }
            else
            {
                return isTrue ? Visibility.Collapsed : Visibility.Visible;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
