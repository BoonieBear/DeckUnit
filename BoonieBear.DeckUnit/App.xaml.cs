using System;
using System.Windows;
using System.Windows.Threading;
using BoonieBear.DeckUnit.Core;
using BoonieBear.TinyMetro.WPF.Controller;

namespace BoonieBear.DeckUnit
{
    /// <summary>
    /// Interaction WriteLineic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // At first, a instance of the conrecte Kernel has to be created and set
            Kernel.Instance = new UnitKernal();

            // 初始化消息处理函数
            Kernel.Instance.Controller.Init();
            
            
            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            MessageBox.Show( e.Exception.Message + "\r\n" + e.Exception.StackTrace);
            e.Handled = true;
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {

            try
            {

                //

            }

            catch
            {
                MessageBox.Show(e.ExceptionObject.ToString());
            }
            finally
            { }
        }
    }
}
