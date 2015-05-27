using System;
using System.Windows;
using System.Windows.Threading;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Helps;
using TinyMetroWpfLibrary.Controller;

namespace BoonieBear.DeckUnit
{
    /// <summary>
    /// Interaction WriteLineic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
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
            LogHelper.WriteLog("系统启动");
            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            e.Handled = true;
            LogHelper.ErrorLog(null, e.Exception);
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var o = e.ExceptionObject as Exception;
            if (o != null)
            {
                LogHelper.ErrorLog(null, o);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            UnitCore.Instance.Dispose();
        }
    }
}
