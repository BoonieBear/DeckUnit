using System.Windows;
using System.Windows.Input;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{

    public class HistoryDataPageViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase

        

        public override void Initialize()
        {
            Title = Application.Current.Properties["message"].ToString();
            AddPropertyChangedNotification(()=>Title);
        }


        public override void InitializePage(object extraData)
        {
        }
        #region 绑定属性
        public string Title
        {
            get { return GetPropertyValue(() => Title); }
            set { SetPropertyValue(() => Title, value); }
        }

        #endregion


        #endregion




    }
}