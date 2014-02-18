using System.Windows;
using System.Windows.Input;
using BoonieBear.TinyMetro.WPF.Events;
using BoonieBear.TinyMetro.WPF.ViewModel;

namespace BoonieBear.DockUnit.ViewModels
{

    public class HistoryDataPageViewModel : ViewModelBase
    {
        #region Overrides of ViewModelBase

        public string title
        {
            get { return GetPropertyValue(() => title); }
            set { SetPropertyValue(() => title, value); }
        }

        public override void Initialize()
        {
            title = Application.Current.Properties["message"].ToString();
            AddPropertyChangedNotification(()=>title);
        }


        public override void InitializePage(object extraData)
        {
        }

        #endregion

      

      
    }
}