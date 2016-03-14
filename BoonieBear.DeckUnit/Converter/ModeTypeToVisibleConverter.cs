using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Converter
{
    public class ModeTypeToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            var bmode = (bool)value;
            var type = (string) parameter;
            if ((bmode && type=="net")||(!bmode && type =="comm"))
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
            throw new NotSupportedException();
        }

    }
}
