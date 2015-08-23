using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Converter
{
    public class MultiDebugStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var serialtring = values[2] as string;//serial
            var tcpstring = values[1] as string;//tcp
            bool type = (bool)values[0];//type:true->tcp; false->serial
            if (serialtring != null || tcpstring != null)
                return type ? tcpstring : serialtring;
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
