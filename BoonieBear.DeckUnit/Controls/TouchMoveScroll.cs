using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DeckUnit.Controls
{
    public class TouchMoveScroll : ScrollViewer
    {
        private bool _isPressed = false;
        private Point _sourcePoint;

        public TouchMoveScroll()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if ((_isPressed) && (e.LeftButton == MouseButtonState.Pressed))
            {
                var p = e.GetPosition(this);
                if(Math.Abs(p.Y - _sourcePoint.Y)>20)
                    VerticalScrollBarVisibility = ScrollBarVisibility.Visible;


                ScrollToVerticalOffset(p.Y - _sourcePoint.Y);
            }
            else
            {
                _isPressed = false;
            }
            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isPressed = false;

            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            _isPressed = true;
            _sourcePoint = e.GetPosition(this);
            base.OnPreviewMouseDown(e);
        }


    }
}