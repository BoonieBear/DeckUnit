using System;
using System.Windows;
using System.Windows.Threading;
using BoonieBear.DeckUnit.Core;
using BoonieBear.DeckUnit.Events;
using BoonieBear.DeckUnit.Helps;
using TinyMetroWpfLibrary.Controller;

namespace BoonieBear.DeckUnit
{
    /// <summary>
    /// Interaction WriteLineic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsExit = false;
        public App()
        {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        }
        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);
            if (exitOperation.Status !=DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
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
            UnitCore Instance = UnitCore.GetInstance();
            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (App.IsExit)
            {
                App.Current.Shutdown();
            }
            e.Handled = true;
            if(App.Current.MainWindow.IsActive)
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(e.Exception, LogType.Both));
            else
            {
                LogHelper.ErrorLog(e.Exception.Message,e.Exception);
            }

        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (App.IsExit)
                App.Current.Shutdown();
            var o = e.ExceptionObject as Exception;
            if (o != null)
            {
                if (App.Current.MainWindow.IsActive)
                    UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(o, LogType.Both));
                else
                {
                    LogHelper.ErrorLog(o.Message, o);
                }
            }
        }
        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as
            DispatcherFrame;
            frame.Continue = false;
            return null;
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            IsExit = true;
            UnitCore.Instance.Stop();
        }
    }
}
