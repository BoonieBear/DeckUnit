using System.ComponentModel;
using System.Windows.Data;
using BoonieBear.DeckUnit.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System;

namespace BoonieBear.DeckUnit.Views
{
    public partial class HistoryDataPage
    {
        public HistoryDataPage()
        {
            InitializeComponent();
        }

        private void FilterableListView_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
    }
}