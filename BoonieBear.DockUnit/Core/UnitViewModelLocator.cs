using System.Windows.Controls;
using BoonieBear.DockUnit.ViewModels;

namespace BoonieBear.DockUnit.Core
{

    public class UnitViewModelLocator
    {
        private MainFrameViewModel _mainFrameViewModel;
        private MainPageViewModel _mainPageViewModel;
        private Page1ViewModel _page1ViewModel;
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

        public Page1ViewModel Page1ViewModel
        {
            get
            {
                // Creates the Example ViewModel
                if (_page1ViewModel == null)
                {
                    _page1ViewModel = new Page1ViewModel();
                    _page1ViewModel.Initialize();
                }
                return _page1ViewModel;
            }
        }
    }
}
