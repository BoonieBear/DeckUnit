using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BoonieBear.DeckUnit.Mov4500UI.AttachedProperty
{
    public class ScreenModelPropertys
    {
        public static bool GetIsLandScape(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLandScapeProperty);
        }

        public static void SetIsLandScape(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLandScapeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLandScape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLandScapeProperty =
            DependencyProperty.RegisterAttached("IsLandScape", typeof(bool), typeof(ScreenModelPropertys), new PropertyMetadata(true));  
    }
}
