using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DeckUnit.Views
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
