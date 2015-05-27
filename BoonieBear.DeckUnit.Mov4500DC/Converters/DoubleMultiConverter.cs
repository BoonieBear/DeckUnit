using System;
using System.Globalization;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class DoubleMultiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? doubleValue = value as double?;
            double? doubleParameter = parameter as double?;
            if (doubleParameter == null)
            {
                string parameterStr = parameter as string;
                double dp;
                if (double.TryParse(parameterStr, NumberStyles.Number, new CultureInfo("en-US", false).NumberFormat, out dp))
                {
                    doubleParameter = dp;
                }
            }
            if (doubleValue == null || doubleParameter == null)
            {
                return doubleValue;
            }
            double ret = doubleValue.Value * doubleParameter.Value;
            return ret;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
