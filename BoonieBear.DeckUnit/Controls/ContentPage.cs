using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BoonieBear.DeckUnit.ViewModels;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;
using BoonieBear.TinyMetro.WPF.Events;

namespace BoonieBear.DeckUnit.Controls
{
    /// <summary>
    /// 负责内容页面的一些动画效果函数，统一写在这里
    /// 只要和页面内容没关系的都加在这里
    /// </summary>
    public abstract  class ContentPage:Page
    {
        private bool IsPressed = false;
        private bool IsMoved = false;
        private Point sourcePoint;
        public ContentPage()
        {
            
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            Debug.WriteLine("Ispressed={0}", IsPressed);
            if ((IsPressed) && (e.LeftButton == MouseButtonState.Pressed))
            {
                Point p = e.GetPosition(this);

                if ((p.X - sourcePoint.X)>100)
                {
                    var eventAggregator = Kernel.Instance.EventAggregator;
                    //eventAggregator.PublishMessage(new GoBackNavigationRequest());
                }
                else
                {
                    //ChangeBottomBarVisibility(Visibility.Visible);
                }
            }
            else
            {
                IsPressed = false;

            }
            base.OnPreviewMouseMove(e);
        }



        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsPressed = false;
            Debug.WriteLine("OnPreviewMouseLeftButtonUp");
            base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            IsPressed = true;
            sourcePoint = e.GetPosition(this);
            base.OnPreviewMouseDown(e);
        }


        protected void ChangeBottomBarVisibility(Visibility v)
        {
            MainFrameViewModel.pMainFrame.IsShowBottomBar = v;
        }

        protected void ChangeTopBarVisibility(Visibility v)
        {
            MainFrameViewModel.pMainFrame.IsShowTopBar = v;
        }

       
    }
}
