using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Validation;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class AbsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            sbyte a = (sbyte)value;
            return Math.Abs((int)a);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
