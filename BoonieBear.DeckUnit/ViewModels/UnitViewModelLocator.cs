using BoonieBear.DeckUnit.ViewModels.CommandViewModel;
namespace BoonieBear.DeckUnit.ViewModels
{

    public class UnitViewModelLocator
    {
        private MainFrameViewModel _mainFrameViewModel;
        private MainPageViewModel _mainPageViewModel;
        private AcousticViewModel _acousticViewModel;
        private HistoryDataPageViewModel _historyDataPageViewModel;
        private SystemResourceViewModel _systemResourceViewModel;
        private DownLoadPageViewModel _downLoadPageViewModel;
        private DownLoadOverViewModel _downLoadOverViewModel;
        private MonitorViewModel _monitorViewModel;
        private ADViewModel _adViewModel;
        private SimpleViewModel _simpleViewModel;
        private DeviceBackSetViewModel _deviceBackSetViewModel;
        private DeviceParaSetViewModel _deviceParaSetViewModel;
        private NodeRecvEmitSetViewModel _nodeRecvEmitSetViewModel;
        private NodeComSchemaViewModel _nodeComSchemaViewModel;
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

        public AcousticViewModel AcousticViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_acousticViewModel == null)
                {
                    _acousticViewModel = new AcousticViewModel();
                    _acousticViewModel.Initialize();
                }
                return _acousticViewModel;
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
        public DownLoadPageViewModel DownLoadPageViewModel
        {
            get
            {
                if (_downLoadPageViewModel == null)
                {
                    _downLoadPageViewModel = new DownLoadPageViewModel();
                    _downLoadPageViewModel.Initialize();
                }
                return _downLoadPageViewModel;
            }
        }
        public DownLoadOverViewModel DownLoadOverViewModel
        {
            get
            {
                if (_downLoadOverViewModel == null)
                {
                    _downLoadOverViewModel = new DownLoadOverViewModel();
                    _downLoadOverViewModel.Initialize();
                }
                return _downLoadOverViewModel;
            }
        }
        public MonitorViewModel MonitorViewModel
        {
            get
            {
                if (_monitorViewModel == null)
                {
                    _monitorViewModel = new MonitorViewModel();
                    _monitorViewModel.Initialize();
                }
                return _monitorViewModel;
            }
        }
        public ADViewModel ADViewModel
        {
            get
            {
                if (_adViewModel == null)
                {
                    _adViewModel = new ADViewModel();
                    _adViewModel.Initialize();
                }
                return _adViewModel;
            }
        }
        public SimpleViewModel SimpleViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_simpleViewModel == null)
                {
                    _simpleViewModel = new SimpleViewModel();
                    _simpleViewModel.Initialize();
                }
                return _simpleViewModel;
            }
        }
        public DeviceBackSetViewModel DeviceBackSetViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_deviceBackSetViewModel == null)
                {
                    _deviceBackSetViewModel = new DeviceBackSetViewModel();
                    _deviceBackSetViewModel.Initialize();
                }
                return _deviceBackSetViewModel;
            }
        }
        public DeviceParaSetViewModel DeviceParaSetViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_deviceParaSetViewModel == null)
                {
                    _deviceParaSetViewModel = new DeviceParaSetViewModel();
                    _deviceParaSetViewModel.Initialize();
                }
                return _deviceParaSetViewModel;
            }
        }
        public NodeRecvEmitSetViewModel NodeRecvEmitSetViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_nodeRecvEmitSetViewModel == null)
                {
                    _nodeRecvEmitSetViewModel = new NodeRecvEmitSetViewModel();
                    _nodeRecvEmitSetViewModel.Initialize();
                }
                return _nodeRecvEmitSetViewModel;
            }
        }
        public NodeComSchemaViewModel NodeComSchemaViewModel
        {
            get
            {
                // Creates the MainFrame ViewModel
                if (_nodeComSchemaViewModel == null)
                {
                    _nodeComSchemaViewModel = new NodeComSchemaViewModel();
                    _nodeComSchemaViewModel.Initialize();
                }
                return _nodeComSchemaViewModel;
            }
        }
    }
}
