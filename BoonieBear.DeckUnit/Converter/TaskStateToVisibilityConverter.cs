using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BoonieBear.DeckUnit.Converter
{
    class TaskStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = value as string;
            var content = parameter as string;
            switch(state)
            {
                case "UNSTART":
                    if (content == "开始")
                        return Visibility.Visible;
                    if (content == "停止")
                        return Visibility.Collapsed;
                    if (content == "删除")
                        return Visibility.Visible;
                    if (content == "数据")
                        return Visibility.Collapsed;
                    if (content == "重试")
                        return Visibility.Collapsed;
                    break;
                case "STOP":
                    if (content == "开始")
                        return Visibility.Visible;
                    if (content == "停止")
                        return Visibility.Collapsed;
                    if (content == "删除")
                        return Visibility.Visible;
                    if (content == "数据")
                        return Visibility.Collapsed;
                    if (content == "重试")
                        return Visibility.Collapsed;
                    break;
                case "WORKING":
                    if (content == "开始")
                        return Visibility.Collapsed;
                    if (content == "停止")
                        return Visibility.Visible;
                    if (content == "删除")
                        return Visibility.Collapsed;
                    if (content == "数据")
                        return Visibility.Collapsed;
                    if (content == "重试")
                        return Visibility.Collapsed;
                    break;
                case "COMPLETED":
                    if (content == "开始")
                        return Visibility.Collapsed;
                    if (content == "停止")
                        return Visibility.Collapsed;
                    if (content == "删除")
                        return Visibility.Visible;
                    if (content == "数据")
                        return Visibility.Visible;
                    if (content == "重试")
                        return Visibility.Collapsed;
                    break;
                case "WAITING":
                    if (content == "开始")
                        return Visibility.Collapsed;
                    if (content == "停止")
                        return Visibility.Collapsed;
                    if (content == "删除")
                        return Visibility.Visible;
                    if (content == "数据")
                        return Visibility.Collapsed;
                    if (content == "重试")
                        return Visibility.Visible;
                    break;
                default:
                    if (content == "开始")
                        return Visibility.Collapsed;
                    if (content == "停止")
                        return Visibility.Visible;
                    if (content == "删除")
                        return Visibility.Collapsed;
                    if (content == "数据")
                        return Visibility.Collapsed;
                    if (content == "重试")
                        return Visibility.Collapsed;
                    break;
            } return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
