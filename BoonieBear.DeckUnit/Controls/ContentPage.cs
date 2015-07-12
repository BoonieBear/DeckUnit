using System.Windows;
using System.Windows.Controls;
using BoonieBear.DeckUnit.ViewModels;
using BoonieBear.DeckUnit.Views;

namespace BoonieBear.DeckUnit.Controls
{
    /// <summary>
    /// 负责内容页面的一些动画效果函数，统一写在这里
    /// 只要和页面内容没关系的都加在这里
    /// </summary>
    public  class ContentPage:Page
    {
        public ContentPage()
        {
            Loaded+=ContentPage_Loaded;
        }
        private void ContentPage_Loaded(object sender, RoutedEventArgs e)
        {

            

        }

        

       
    }
}
