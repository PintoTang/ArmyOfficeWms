using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.License;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.Service.ThreadHandle;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Service.Project;
using Infrastructrue.Ioc.DependencyFactory;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CLDC.CLWS.CLWCS.Service.StartService
{
    /// <summary>
    /// 应用程序启动
    /// </summary>
    public class AppStart
    {

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        public OperateResult AppStartCheck()
        {
            OperateResult checkResult = OperateResult.CreateFailedResult();
            try
            {
                Process process = Process.GetCurrentProcess();
                //遍历应用程序的同名进程组
                foreach (Process p in Process.GetProcessesByName(process.ProcessName))
                {
                    if (process.ProcessName.Contains("CL.WCS.WPF.vshost")) continue;

                    //不是同一进程则关闭刚刚创建的进程
                    if (p.Id != process.Id)
                    {
                        //此处显示原先的窗口需要一定的时间，不然无法显示
                        ShowWindowAsync(p.MainWindowHandle, 9);
                        SetForegroundWindow(p.MainWindowHandle);
                        Thread.Sleep(10);
                        checkResult.IsSuccess = false;
                        checkResult.ErrorCode = 101;
                        checkResult.Message = "Wcs系统已启动";
                        CurSysErrCode = 101;
                        return checkResult;
                    }
                }
                CurSysErrCode = 0;
                SystemCurMode = SystemMode.Starting;


                OperateResult copyConfig = CopyConfig();
                if (!copyConfig.IsSuccess)
                {
                    MessageBoxEx.Show(copyConfig.Message, "错误", MessageBoxButton.OK);
                    return copyConfig;
                }

                OperateResult registerResult = RegisterSysBaseService();
                if (!registerResult.IsSuccess)
                {
                    MessageBoxEx.Show(registerResult.Message, "错误", MessageBoxButton.OK);
                    return registerResult;
                }
                

                ILicenseService licenseService = DependencyHelper.GetService<ILicenseService>();
                if (licenseService == null)
                {
                    MessageBoxEx.Show("启动注册服务失败，请联系管理员", "错误", MessageBoxButton.OK);
                    return OperateResult.CreateFailedResult("启动系统注册失败");
                }

                OperateResult isLiceseAvailable = licenseService.IsLicenseAvailable(true);

                if (!isLiceseAvailable.IsSuccess)
                {
                    return isLiceseAvailable;
                }
                licenseService.StartCheckLicense();
                checkResult.IsSuccess = true;

                ////增加OPC License 自动关闭进程
                //Thread th = new Thread(DelKillProcess);
                //th.IsBackground = true;
                //th.Start();
                checkResult.Message = "系统检查成功";
            }
            catch (Exception ex)
            {
                checkResult.IsSuccess = false;
                checkResult.Message = OperateResult.ConvertExceptionMsg(ex);
                MessageBoxEx.Show("系统启动发生异常：" + ex.Message, "错误", MessageBoxButton.OK);
            }
            return checkResult;
        }
        private void DelKillProcess()
        {

            while (!SystemConfig.Instance.isExitApp)
            {
                Thread.Sleep(1000);

                KillProcess("rundll32");
            }
        }


        private  void KillProcess(string processName)
        {
            //获得进程对象，以用来操作  
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();
            //得到所有打开的进程   
            try
            {
                //获得需要杀死的进程名  
                foreach (Process thisproc in Process.GetProcessesByName(processName))
                {
                    //立即杀死进程  
                    thisproc.Kill();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
        }


        private int CurSysErrCode { get; set; }
        public SystemMode SystemCurMode { get; set; }


        public const string LogName = "系统启动";

        public OperateResult CopyConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult runConfigResult = SystemRunConfigHelper.RunConfig();
                if (!runConfigResult.IsSuccess)
                {
                    throw new Exception("拷贝配置文件出错");
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        public OperateResult RegisterSysBaseService()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                //注册依赖
                LogHelper.WriteLog(LogName, string.Format("{0} 开始注册依赖", DateTime.Now), EnumLogLevel.Debug);

                DependencyRegister iocRegister = new DependencyRegister();
                OperateResult iocInitResult = iocRegister.InitializeSysBaseService();
                if (!iocInitResult.IsSuccess)
                {
                    string msg = string.Format("依赖注入注册失败，原因：\r\n{0}", iocInitResult.Message);
                    ExitAppWithMessageBox(msg);
                }
                LogHelper.WriteLog(LogName, string.Format("{0} 结束注册依赖", DateTime.Now), EnumLogLevel.Debug);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public void Initilize()
        {
            try
            {

                ProjectStart projectStart = new ProjectStart();
                OperateResult startProjectResult = projectStart.StartProject();

                if (!startProjectResult.IsSuccess)
                {
                    string msg = string.Format("系统初始化失败，失败原因：\r\n {0} ", startProjectResult.Message);
                    LogHelper.WriteLog(LogName, msg, EnumLogLevel.Error);
                    ExitAppWithMessageBox(msg);
                }
                LogHelper.WriteLog(LogName, string.Format("{0} 结束初始化立库系统", DateTime.Now), EnumLogLevel.Debug);

                LogHelper.WriteLog(LogName, "系统启动成功", EnumLogLevel.Info);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogName, OperateResult.ConvertException(ex), EnumLogLevel.Error);
                ExitAppWithMessageBox(OperateResult.ConvertException(ex));
            }

        }

        private async void ExitAppWithMessageBox(string msg)
        {
            DialogResult result = MessageBox.Show(msg, "系统启动", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.OK)
            {
                SystemConfig.Instance.isExitApp = true;
                await WaitThreadFinish();
                //Thread.Sleep(100);
                //Environment.Exit(0);
                Environment.Exit(System.Environment.ExitCode);
            }
        }

        public async void AppExit()
        {
            if (CurSysErrCode != 101)
            {
                OperateLogHelper.Record("退出系统");
            }
            SystemConfig.Instance.isExitApp = true;
            await WaitThreadFinish();
            //Thread.Sleep(500);
            Environment.Exit(System.Environment.ExitCode);
        }

        private Task WaitThreadFinish()
        {
            var task = Task.Run(() =>
            {
                ThreadHandleManage.StopAllThreadHandle();
            });
            return task;
        }


    }

    public enum SystemMode
    {
        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Running,
        /// <summary>
        /// 正在启动
        /// </summary>
        [Description("启动中")]
        Starting,
        /// <summary>
        /// 正常退出
        /// </summary>
        [Description("退出中")]
        Exiting,
        /// <summary>
        /// 注销 返回登陆界面
        /// </summary>
        [Description("注销中")]
        LogOut,
        /// <summary>
        /// 异常退出 重新返回登陆界面
        /// </summary>
        [Description("重启界面")]
        Reboot
    }
}
