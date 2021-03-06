﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class _3DChartFrontAngleShipConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                float? shipheading = values[0] as float?;
                bool? setShipFront = values[1] as bool?;
                if (setShipFront!=null&&setShipFront == false)
                    return (double)shipheading;
                else
                {
                    return (double)0.0;
                }
            }
            catch (Exception)
            {
                return (double)0.0;
            }
                
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
