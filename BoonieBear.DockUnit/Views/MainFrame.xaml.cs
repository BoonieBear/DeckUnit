using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DockUnit.Views
{

    public partial class MainFrame
    {
        public MainFrame()
        {
            InitializeComponent();
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }
    }
}
