namespace BoonieBear.DeckUnit.ViewModels
{

    public class UnitViewModelLocator
    {
        private MainFrameViewModel _mainFrameViewModel;
        private MainPageViewModel _mainPageViewModel;
        private WaterTelViewModel _waterTelViewModel;
        private HistoryDataPageViewModel _historyDataPageViewModel;
        private SystemResourceViewModel _systemResourceViewModel;
        /// <summary>
        /// Gets the MainFrame ViewModel
        /// </summary>
        public MainFrameViewModel MainFrameViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_mainFrameViewModel == null)
                {
                    _mainFrameViewModel = new MainFrameViewModel();
                    _mainFrameViewModel.Initialize();
                }
                return _mainFrameViewModel;
            }
        }

        /// <summary>
        /// Gets the Example ViewModel
        /// </summary>
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_mainPageViewModel == null)
                {
                    _mainPageViewModel = new MainPageViewModel();
                    _mainPageViewModel.Initialize();
                }
                return _mainPageViewModel;
            }
        }

        public WaterTelViewModel WaterTelViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_waterTelViewModel == null)
                {
                    _waterTelViewModel = new WaterTelViewModel();
                    _waterTelViewModel.Initialize();
                }
                return _waterTelViewModel;
            }
        }
        public HistoryDataPageViewModel HistoryDataPageViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_historyDataPageViewModel == null)
                {
                    _historyDataPageViewModel = new HistoryDataPageViewModel();
                    _historyDataPageViewModel.Initialize();
                }
                return _historyDataPageViewModel;
            }
        }

        public SystemResourceViewModel SystemResourceViewModel
        {
            get
            {
                if (_systemResourceViewModel == null)
                {
                    _systemResourceViewModel = new SystemResourceViewModel();
                    _systemResourceViewModel.Initialize();
                }
                return _systemResourceViewModel;
            }
        }
    }
}
