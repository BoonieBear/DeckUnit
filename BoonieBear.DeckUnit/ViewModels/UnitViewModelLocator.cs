using BoonieBear.DeckUnit.ViewModels.CommandViewModel;
using BoonieBear.DeckUnit.ViewModels.SetViewModel;

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
        private ConfigViewModel _configViewModel;
        private ConnectConfigViewModel _connectConfigViewModel;
        private DebugViewModel _debugViewModel;
        private GetNodeStatusViewModel _getNodeStatusViewModel;
        private PingTestViewModel _pingTestViewModel;
        private RefreshNodeConfigViewModel _refreshNodeConfigViewModel;
        private SetEnergyInfoViewModel _setEnergyInfoViewModel;
        private SetDateTimeViewModel _setDateTimeViewModel;
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
                if (_nodeComSchemaViewModel == null)
                {
                    _nodeComSchemaViewModel = new NodeComSchemaViewModel();
                    _nodeComSchemaViewModel.Initialize();
                }
                return _nodeComSchemaViewModel;
            }
        }
        public ConfigViewModel ConfigViewModel
        {
            get
            {
                if (_configViewModel == null)
                {
                    _configViewModel = new ConfigViewModel();
                    _configViewModel.Initialize();
                }
                return _configViewModel;
            }
        }
        public ConnectConfigViewModel ConnectConfigViewModel
        {
            get
            {
                if (_connectConfigViewModel == null)
                {
                    _connectConfigViewModel = new ConnectConfigViewModel();
                    _connectConfigViewModel.Initialize();
                }
                return _connectConfigViewModel;
            }
        }
        public DebugViewModel DebugViewModel
        {
            get
            {
                if (_debugViewModel == null)
                {
                    _debugViewModel = new DebugViewModel();
                    _debugViewModel.Initialize();
                }
                return _debugViewModel;
            }
        }
        public GetNodeStatusViewModel GetNodeStatusViewModel
        {
            get
            {
                if (_getNodeStatusViewModel == null)
                {
                    _getNodeStatusViewModel = new GetNodeStatusViewModel();
                    _getNodeStatusViewModel.Initialize();
                }
                return _getNodeStatusViewModel;
            }
        }

        public PingTestViewModel PingTestViewModel
        {
            get
            {
                if (_pingTestViewModel == null)
                {
                    _pingTestViewModel = new PingTestViewModel();
                    _pingTestViewModel.Initialize();
                }
                return _pingTestViewModel;
            }
        }
        public RefreshNodeConfigViewModel RefreshNodeConfigViewModel
        {
            get
            {
                if (_refreshNodeConfigViewModel == null)
                {
                    _refreshNodeConfigViewModel = new RefreshNodeConfigViewModel();
                    _refreshNodeConfigViewModel.Initialize();
                }
                return _refreshNodeConfigViewModel;
            }
        }
        public SetEnergyInfoViewModel SetEnergyInfoViewModel
        {
            get
            {
                if (_setEnergyInfoViewModel == null)
                {
                    _setEnergyInfoViewModel = new SetEnergyInfoViewModel();
                    _setEnergyInfoViewModel.Initialize();
                }
                return _setEnergyInfoViewModel;
            }
        }
        public SetDateTimeViewModel SetDateTimeViewModel
        {
            get
            {
                if (_setDateTimeViewModel == null)
                {
                    _setDateTimeViewModel = new SetDateTimeViewModel();
                    _setDateTimeViewModel.Initialize();
                }
                return _setDateTimeViewModel;
            }
        }
    }
}
