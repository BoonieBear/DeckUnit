using System;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Mov4500UI.Converters
{
    public class VisibilityBoolToggleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? boolValue = value as bool?;
            //IsReversed=True;IsCollapsed=True
            bool isReversed = false;
            bool isCollapsed = false;
            string paramStr = parameter as string;
            ParseParameter(ref isReversed, ref isCollapsed, paramStr);
            if (isReversed)
            {
                if (boolValue == null || boolValue == false)
                {
                    boolValue = true;
                }
                else
                {
                    boolValue = false;
                }
            }
            if (boolValue == null || boolValue == false)
            {
                if (isCollapsed)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            else
            {
                return Visibility.Visible;
            }
        }

        private static void ParseParameter(ref bool isReversed, ref bool isCollapsed, string paramStr)
        {
            if (paramStr != null)
            {
                string[] parameters = paramStr.Split(new char[] { ';' });
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        string[] kvpair = item.Split(new char[] { '=' });
                        if (kvpair == null || kvpair.Length != 2)
                        {
                            continue;
                        }
                        bool res;
                        if (bool.TryParse(kvpair[1], out res))
                        {
                            if (kvpair[0].ToLower().Equals("isreversed"))
                            {
                                isReversed = res;
                            }
                            else if (kvpair[0].ToLower().Equals("iscollapsed"))
                            {
                                isCollapsed = res;
                            }
                        }
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
