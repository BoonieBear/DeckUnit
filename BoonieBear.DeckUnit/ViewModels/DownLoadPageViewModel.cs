using System.Windows;
using TinyMetroWpfLibrary.ViewModel;

namespace BoonieBear.DeckUnit.ViewModels
{
    public class DownLoadPageViewModel : ViewModelBase
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
