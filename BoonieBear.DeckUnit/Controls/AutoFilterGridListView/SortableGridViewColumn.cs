﻿using System.Windows;
using System.Windows.Controls;

// SortableListView, from the following blog post:
//
// http://blogs.interknowlogy.com/joelrumerman/archive/2007/04/03/12497.aspx

namespace BoonieBear.DeckUnit.Controls.AutoFilterGridListView
{
    public class SortableGridViewColumn : GridViewColumn
    {

        public string SortPropertyName
        {
            get { return (string)GetValue(SortPropertyNameProperty); }
            set { SetValue(SortPropertyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortPropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.Register("SortPropertyName", typeof(string), typeof(SortableGridViewColumn), new UIPropertyMetadata(""));
        
        public bool IsDefaultSortColumn
        {
            get { return (bool)GetValue(IsDefaultSortColumnProperty); }
            set { SetValue(IsDefaultSortColumnProperty, value); }
        }

        public static readonly DependencyProperty IsDefaultSortColumnProperty =
            DependencyProperty.Register("IsDefaultSortColumn", typeof(bool), typeof(SortableGridViewColumn), new UIPropertyMetadata(false));

    }
}
