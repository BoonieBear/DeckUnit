using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class _3DChartFrontAngleMovConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var shipheading = (float)values[0];
                var movheading = (float)values[1];
                var setShipFront = (bool)values[2];
                if (setShipFront == false)
                    return (double)movheading;
                else
                {
                    return (double)(movheading - shipheading);
                }
            }
            catch (Exception)
            {
                return null;
            }
                

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
