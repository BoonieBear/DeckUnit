using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using BoonieBear.DeckUnit.Models;
namespace BoonieBear.DeckUnit.Converter
{

        public class NotifyLevelToBrushConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if ((value as NotifyLevel?)== null)
                {
                    return new SolidColorBrush(Colors.LightSkyBlue);
                }
                var level = (value as NotifyLevel?).Value;
                switch (level)
                {
                    case NotifyLevel.Warning:
                         return new SolidColorBrush(Colors.Coral);
                    case NotifyLevel.Error:
                        return new SolidColorBrush(Colors.Crimson);
                    default:
                        return new SolidColorBrush(Colors.LightSkyBlue);
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}
