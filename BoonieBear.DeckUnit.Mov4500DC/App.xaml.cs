using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.ExceptionServices;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using TinyMetroWpfLibrary.Controller;
using BoonieBear.DeckUnit.Mov4500UI.Events;
namespace BoonieBear.DeckUnit.Mov4500UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex gMu;
        public static bool IsExit = false;
        public App()
        {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //检查实例是否重复
            bool createdNew = false;
            gMu = new Mutex(true, "Mov4500", out createdNew);
            if (!createdNew)
            {
                String strTitle = ResourcesHelper.TryFindResourceString("Mov4500_ApplicationName");
                String strErrMsg = ResourcesHelper.TryFindResourceString("Mov4500_RUNNING");
                MessageBox.Show(strErrMsg, strTitle);
                App.Current.Shutdown();
                return;
            }
             
            Splasher.Splash = new SplashWindow();

            Splasher.ShowSplash();
            //  初始化框架
            //派生接口单实例
            UnitKernal.Instance = new UnitKernal();
            //基类单实例，给basecontroller赋值
            //basecontroller用的是基类接口
            Kernel.Instance = UnitKernal.Instance;
            // 初始化消息处理函数
            UnitKernal.Instance.Controller.Init();//导航消息响应
            UnitKernal.Instance.MessageController.Init();//系统消息响应
            
            LogHelper.WriteLog("程序启动");
            
  
            base.OnStartup(e);
        }
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            if (App.IsExit)
                return;
            e.Handled = true;
            if (Application.Current.MainWindow.IsActive)
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(e.Exception, LogType.Both));
            else
            {
                LogHelper.ErrorLog(e.Exception.Message, e.Exception);
            }
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (App.IsExit)
                return;
            var o = e.ExceptionObject as Exception;
            if (o != null)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (App.Current.MainWindow.IsActive)
                        UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(o, LogType.Both));
                    else
                    {
                        LogHelper.ErrorLog(o.Message, o);
                    }
                }));
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IsExit = true;
            UnitCore.GetInstance().Stop();
            if(UnitCore.Instance.Wave!=null)
                UnitCore.Instance.Wave.Dispose();
        }
    }
}
