using BoonieBear.DeckUnit.Mov4500UI.Helpers;

namespace BoonieBear.DeckUnit.Mov4500UI
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow
    {
        public SplashWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string ProductName
        {
            get
            {
                string str = ResourcesHelper.TryFindResourceString("SPLASH_PRODUCT_NAME");
                return str;
            }
        }

        public string Version
        {
            get
            {
                var versionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string[] ver = versionInfo.Split('.');
                var versionNo = string.Format("{0}.{1}", ver[0], ver[1]);
                return string.Format(ResourcesHelper.TryFindResourceString("About_Version"), versionNo);
            }
        }

        public string CopyRight
        {
            get
            {
                string str = ResourcesHelper.TryFindResourceString("SPLASH_COPY_RIGHT");
                return str;
            }
        }

        public string CompanyName
        {
            get
            {
                string str = ResourcesHelper.TryFindResourceString("SPLASH_COMPANY_NAME");
                return str;
            }
        }

        public string RightReserve
        {
            get
            {
                string str = ResourcesHelper.TryFindResourceString("SPLASH_RIGHT_RESERVER");
                return str;
            }
        }

        public string Brand
        {
            get
            {
                string str = ResourcesHelper.TryFindResourceString("SPLASH_BAND");
                return str;
            }
        }

        public string BrandDescription
        {
            get
            {
                string str = ResourcesHelper.TryFindResourceString("SPLASH_BAND_DESCRIPTION");
                return str;
            }
        }
    }
}
