using System;
using System.Diagnostics;
using System.Timers;
using GalaSoft.MvvmLight;
using BoonieBear.DockUnit.Model;
using GalaSoft.MvvmLight.Command;

namespace BoonieBear.DockUnit.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private bool _cando = false;
        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        public bool CanDo
        {
            get
            {
                return _cando;
            }
            set
            {
                if (_cando == value)
                    return;
                else
                {
                    _cando = value;
                    NewCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _welcomeTitle = string.Empty;
        public RelayCommand NewCommand
        {
            get;
            private set;
        }

        public void ChangeTitle()
        {
                WelcomeTitle = "new title!"; 
        }

        public bool Iscan()
        {
            return CanDo;
        }

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });
            NewCommand = new RelayCommand(ChangeTitle, Iscan);
            Timer t =new Timer(3000);
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            WelcomeTitle = DateTime.Now.ToString();
            if ( DateTime.Now.Second%2 == 0)
            {
                CanDo = false;
                
                Debug.WriteLine("cando change to false");
            }
            else
            {
                CanDo = true;
                Debug.WriteLine("cando change to true");
            }
        }



        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}