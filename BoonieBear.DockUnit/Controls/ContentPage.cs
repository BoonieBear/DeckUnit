using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BoonieBear.DockUnit.ViewModels;

namespace BoonieBear.DockUnit.Controls
{
    /// <summary>
    /// 负责内容页面的一些动画效果函数，统一写在这里
    /// 只要和页面内容没关系的都加在这里
    /// </summary>
    public abstract  class ContentPage:Page
    {
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
