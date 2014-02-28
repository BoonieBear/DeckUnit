using System.Windows;
using BoonieBear.DeckUnit.Core;
using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DeckUnit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // At first, a instance of the conrecte Kernel has to be created and set
            Kernel.Instance = new UnitKernal();

            // 初始化消息处理函数
            Kernel.Instance.Controller.Init();

            base.OnStartup(e);
        }
    }
}
