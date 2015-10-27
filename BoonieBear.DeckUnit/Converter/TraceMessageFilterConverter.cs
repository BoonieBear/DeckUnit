using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
namespace BoonieBear.DeckUnit.Converter
{
    class TraceMessageFilterConverter : IMultiValueConverter
    {
        private string str;
        private string layer;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            str = values[1] as string;
            var list = values[2] as ObservableCollection<string>;
            var strlist = list.ToList();
            layer = values[0] as string;
            if (list !=null&& layer != null && layer.TrimStart(' ') != "")
                strlist = strlist.FindAll(FindLayer);

            if (list !=null&&str != null && str.TrimStart(' ') != "")
                strlist = strlist.FindAll(FindFilter);
            return strlist;
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
