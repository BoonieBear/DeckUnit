using System;
using System.Windows;
using System.Windows.Threading;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Core.Controllers;
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
            //  初始化框架
            //派生接口单实例
            UnitKernal.Instance = new UnitKernal();
            //基类单实例，给basecontroller赋值
            //basecontroller用的是基类接口
            Kernel.Instance = UnitKernal.Instance;
            // 初始化消息处理函数
            UnitKernal.Instance.Controller.Init();//导航消息响应
            UnitKernal.Instance.MessageController.Init();//系统消息响应
            
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
