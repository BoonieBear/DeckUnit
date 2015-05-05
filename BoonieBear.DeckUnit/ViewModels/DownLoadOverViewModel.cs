using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BoonieBear.TinyMetro.WPF.ViewModel;
namespace BoonieBear.DeckUnit.Views
{
    public class DownLoadOverViewModel:ViewModelBase
    {
        #region Overrides of ViewModelBase



        public override void Initialize()
        {
            Title = Application.Current.Properties["message"].ToString();
            AddPropertyChangedNotification(() => Title);
        }


        public override void InitializePage(object extraData)
        {
        }

        #endregion
        #region 绑定属性
        public string Title
        {
            get { return GetPropertyValue(() => Title); }
            set { SetPropertyValue(() => Title, value); }
        }

        #endregion
    }
}
