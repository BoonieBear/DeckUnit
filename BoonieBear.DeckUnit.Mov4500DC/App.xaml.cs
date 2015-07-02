using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.ExceptionServices;
using BoonieBear.DeckUnit.Mov4500UI.Core;
using BoonieBear.DeckUnit.Mov4500UI.Helpers;
using TinyMetroWpfLibrary.Controller;
namespace BoonieBear.DeckUnit.Mov4500UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex gMu;
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
                return;
            }
             
            this.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Splasher.Splash = new SplashWindow();

            Splasher.ShowSplash();
            /////
            ///加入比较耗费时间的载入操作
            /// 
            Thread.Sleep(3000);
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
            LogHelper.WriteLog("程序关闭");
            if (UnitCore.GetInstance().IsWorking)
                UnitCore.GetInstance().Stop();
        }
    }
}
