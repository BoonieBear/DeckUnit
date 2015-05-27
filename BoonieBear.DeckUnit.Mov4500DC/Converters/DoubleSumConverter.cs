using System;
using System.Globalization;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
	public class DoubleSumConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double? doubleValue = value as double?;
			double? doubleParameter = parameter as double?;
            if (doubleParameter == null)
            {
                string paramStr = parameter as string;
                if (!string.IsNullOrWhiteSpace(paramStr))
                {
                    double dp;
                    if (double.TryParse(paramStr, out dp))
                    {
                        doubleParameter = dp;
                    }
                }
            }
			if (doubleValue == null || doubleParameter == null)
			{
				return doubleValue;
			}
            double ret = doubleValue.Value + doubleParameter.Value;
            if (ret <= 10)
            {
                ret = 10;
            }
			return ret;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
