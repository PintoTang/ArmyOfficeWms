using System;
using System.Threading.Tasks;
using System.Windows;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.Service.StartService;
using CLDC.CLWS.CLWCS.Service.StartService.Login;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.UserCtrl;
using ThemeHelper = CL.WCS.WPF.Theme.ThemeHelper;

namespace CL.WCS.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 指示了应用程序退出时的代码 0刚登陆 1 注销 2退出
        /// </summary>
        public static SystemMode SystemCurMode { get; set; }

        readonly ThemeHelper _themeHelper = new ThemeHelper();
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogHelper.WriteLog("未处理异常", e.Exception+e.Exception.StackTrace);
            SystemCurMode = SystemMode.Reboot;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.WriteLog("未处理异常", e.ExceptionObject.ToString());
            SystemCurMode = SystemMode.Reboot;
        }


        private async void AppInitilize(AppStart app)
        {
            try
            {
                await Task.Run(() => app.Initilize());
                Window main = new MainWindow();
                MainWindow = main;
                SystemCurMode = SystemMode.Running;
                main.ShowDialog();
                if (SystemCurMode.Equals(SystemMode.LogOut))
                {
                    //继续显示登录窗口
                    OperateLogHelper.Record("注销系统");
                    SystemCurMode = SystemMode.LogOut;
                    LogOut(app);
                }
                else
                {
                    app.AppExit();
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message + "\r\n", "错误", MessageBoxButton.OK);
            }
        }


        private void LogOut(AppStart appStart)
        {
            Login lw = new Login();
            bool? result = lw.ShowDialog();
            if (result != null && result.Value)
            {
                Window main = new MainWindow();
                MainWindow = main;
                SystemCurMode = SystemMode.Running;
                main.ShowDialog();
                if (SystemCurMode.Equals(SystemMode.LogOut))
                {
                    //继续显示登录窗口
                    OperateLogHelper.Record("注销系统");
                    SystemCurMode = SystemMode.LogOut;
                    LogOut(appStart);
                }
                else
                {
                    OperateLogHelper.Record("退出系统");
                    appStart.AppExit();
                }
            }
            else
            {
                SystemCurMode = SystemMode.Exiting;
                appStart.AppExit();
            }
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            SystemCurMode = SystemMode.Starting;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppStart appStart = new AppStart();
            OperateResult checkResult= appStart.AppStartCheck();
            
            if (!checkResult.IsSuccess)
            {
                if (checkResult.ErrorCode==101)
                {
                    MessageBoxEx.Show(checkResult.Message);
                    Current.Shutdown();
                }
                else
                {
                    MessageBoxEx.Show(checkResult.Message);
                    Environment.Exit(0);
                }
            }

            _themeHelper.SetGlobalSystemStyle(SystemConfig.Instance.Department);

            try
            {
                Login lw = new Login();
                bool? result = lw.ShowDialog();
                if (result != null && result.Value)
                {
                    AppInitilize(appStart);
                }
                else
                {
                    appStart.AppExit();
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message + "\r\n", "错误", MessageBoxButton.OK);
            }
        }

    }
}
