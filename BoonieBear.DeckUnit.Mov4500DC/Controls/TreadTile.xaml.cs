using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.XtraPrinting.Native;

namespace BoonieBear.DeckUnit.Mov4500UI.Controls
{
    /// <summary>
    /// TreadTile.xaml 的交互逻辑
    /// </summary>
    public partial class TreadTile : UserControl
    {
        public TreadTile()
        {
            InitializeComponent();
            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(TreadTile.ValueProperty, typeof(TreadTile));
            descriptor.AddValueChanged(this, new EventHandler(OnValueChanged));
            LastValue = string.Empty;
            SpreadTextBlock.Text = string.Empty;
        }

        private string LastValue;
        private void OnValueChanged(object sender, EventArgs e)
        {
            float lastvalue = 0;
            float newvalue = 0;
            if (LastValue.IsEmpty())
                lastvalue = 0;
            else
            {
                lastvalue = float.Parse(LastValue);
            }
            if (Value.IsEmpty())
                newvalue = 0;
            else
            {
                newvalue = float.Parse(Value);
            }
            if (newvalue - lastvalue > float.Epsilon)
            {
                SpreadTextBlock.Text = (newvalue - lastvalue).ToString("F03");
                UpMovementIcon.Visibility = Visibility.Visible;
                DownMovementIcon.Visibility = Visibility.Hidden;
            }
            else if (lastvalue - newvalue > float.Epsilon)
            {
                SpreadTextBlock.Text = (newvalue - lastvalue).ToString("F03");
                UpMovementIcon.Visibility = Visibility.Hidden;
                DownMovementIcon.Visibility = Visibility.Visible;
            }
            else
            {
                SpreadTextBlock.Text = "0";
                UpMovementIcon.Visibility = Visibility.Hidden;
                DownMovementIcon.Visibility = Visibility.Hidden;
            }
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TreadTile), new PropertyMetadata(string.Empty));
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(TreadTile), new PropertyMetadata(string.Empty));
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(TreadTile), new PropertyMetadata(string.Empty));
    }
}
