using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
namespace BoonieBear.DeckUnit.Converter
{
    class TraceMessageFilterConverter : IMultiValueConverter
    {
        private string str;
        private string layer;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            str = values[1] as string;
            var list = values[2] as List<string>;
            layer = values[0] as string;
            if (layer != null && layer.TrimStart(' ') != "")
                list = list.FindAll(FindLayer);

            if (str != null && str.TrimStart(' ') != "")
                list = list.FindAll(FindFilter);
            return list;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        private  bool FindLayer(string s)
        {

            if (s.IndexOf(layer.TrimStart(' ')) >= 0)
            {
                return true;
            }
            {
                return false;
            }

        }
        private bool FindFilter(string s)
        {

            if (s.IndexOf(str.TrimStart(' ')) >= 0)
            {
                return true;
            }
            {
                return false;
            }

        }

    }
}
