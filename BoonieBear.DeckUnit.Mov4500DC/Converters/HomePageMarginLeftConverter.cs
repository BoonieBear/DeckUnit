using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class HomePageMarginLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as double?) == null)
            {
                throw new ArgumentException("HomePageMarginLeftConverter: Window width is null");
            }
            double winWidth = (value as double?).Value;
            if ((parameter as double?) == null)
            {
                throw new ArgumentException("HomePageMarginLeftConverter: parameter is null");
            }
            double marginStar = (parameter as double?).Value;
            double main1Width = 632;
            double main2Width = 316;
            //double delta = 70;
            if (winWidth <= main1Width + main2Width + 40)
            {
                return new Thickness(20 * marginStar, 0, 0, 0);
            }
            Thickness t = new Thickness();
            t.Left = (winWidth - main1Width - main2Width) * marginStar / 4.2;
            return t;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
