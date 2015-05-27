using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace BoonieBear.DeckUnit.Mov4500UI.AttachedProperty
{
    public class AppBarButtonPropertys
    {

        #region NormalBackground
        public static Brush GetNormalBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(NormalBackgroundProperty);
        }

        public static void SetNormalBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(NormalBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for NormalBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalBackgroundProperty =
            DependencyProperty.RegisterAttached("NormalBackground", typeof(Brush), typeof(AppBarButtonPropertys), new PropertyMetadata(null));
        #endregion

        #region OverBackground
        public static Brush GetOverBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(OverBackgroundProperty);
        }

        public static void SetOverBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(OverBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for OverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverBackgroundProperty =
            DependencyProperty.RegisterAttached("OverBackground", typeof(Brush), typeof(AppBarButtonPropertys), new PropertyMetadata(null));
        #endregion

        #region PressedBackground
        public static Brush GetPressedBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressedBackgroundProperty);
        }

        public static void SetPressedBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressedBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for PressedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.RegisterAttached("PressedBackground", typeof(Brush), typeof(AppBarButtonPropertys), new PropertyMetadata(null));

        #endregion   

        #region  disableBackground


        public static Brush GetDisableBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(DisableBackgroundProperty);
        }

        public static void SetDisableBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(DisableBackgroundProperty, value);
        }

        // Using a DependencyProperty as the backing store for DisableBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisableBackgroundProperty =
            DependencyProperty.RegisterAttached("DisableBackground", typeof(Brush), typeof(AppBarButtonPropertys), new PropertyMetadata(null));


        #endregion

    }
}
