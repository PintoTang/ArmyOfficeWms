using System;
using System.IO;
using System.Threading.Tasks;
using CL.Framework.ConfigFilePckg;
using System.Threading;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Framework.Log;

namespace CLDC.CLWS.CLWCS.Service.StartService
{
    /// <summary>
    /// 系统配置文件 处理帮助类
    /// </summary>
    public class SystemRunConfigHelper
    {

        public static OperateResult RunConfig()
        {
            bool isCopy = CopyAllConfigFiles();
            if (!isCopy)
            {
                return OperateResult.CreateFailedResult("复制配置文件失败！", 1);
            }

            string strMaxLength = SystemConfig.Instance.CurSystemConfig.MaxByteLength.Value.ToString();
            int strMaxLogCount = SystemConfig.Instance.CurSystemConfig.MaxLogCount.Value;
            int saveDays = SystemConfig.Instance.CurSystemConfig.LogSavedDays.Value;

            Task.Run(() =>
            {
                DeleteOldLogFile(saveDays);
            });

            //这个必须放在最前！否则后面的日志接口调用都会出问题！
            SetLogObject(strMaxLength, strMaxLogCount);
            return OperateResult.CreateSuccessResult();
        }
        private static void SetLogObject(string maxByteLength, int maxLogCount)
        {
            if (!string.IsNullOrEmpty(maxByteLength))
            {
                CustomFileLoggerImp.MaxFileSize = maxByteLength;
            }

            if (maxLogCount != 0)
            {
                CustomFileLoggerImp.MaxSizeRollBackUp = maxLogCount;
            }
        }


        private static void DeleteOldLogFile(int saveDays)
        {
            if (saveDays == 0)
            {
                saveDays = 3;
            }
            string strAppPath = Directory.GetCurrentDirectory() + "\\Log";
            if (!Directory.Exists(strAppPath))
            {
                return;
            }
            string[] dirInfos = Directory.GetDirectories(strAppPath);
            foreach (string dir in dirInfos)
            {
                string name = dir.Right(dir.Length - dir.LastIndexOf("\\") - 1);
                DateTime dirTimeName = DateTime.MinValue;
                if (DateTime.TryParse(name, out dirTimeName))
                {
                    if (dirTimeName.AddDays(saveDays) <= DateTime.Now.Date)
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
        }
        /// <summary>
        /// 将各个项目对应的配置从其文件夹中复制到Config文件夹中，使得系统启动可用这个项目的配置
        /// </summary>
        private static bool CopyAllConfigFiles()
        {
            try
            {
                string strAppPath = Directory.GetCurrentDirectory();
                string strConfigFilePath = System.IO.Path.Combine(strAppPath, @"Config\ConfigFilePath.xml");
                if (!System.IO.File.Exists(strConfigFilePath))
                {
                    //Log.getExceptionFile().Info("启动失败，缺少配置文件ConfigFilePath.xml");
                    return false;
                }
                ConfigFile configFile = new ConfigFile(strConfigFilePath);
                string strDirName = configFile.AppSettings["Path"];
                if (string.IsNullOrEmpty(strDirName))
                {

                    // Log.getExceptionFile().Info("启动失败，请在配置文件ConfigFilePath.xml里配置Value的值");
                    return false;
                }
                string strSourceDir = System.IO.Path.Combine(strAppPath, strDirName);
                if (!System.IO.Directory.Exists(strSourceDir))
                {
                    //Log.getExceptionFile().Info("复制配置文件的源路径不存在，请检查配置文件ConfigFilePath.xml里配置Value的值是否配置正确");
                    return true;
                }
                string strDestDir = System.IO.Path.Combine(strAppPath, "Config");//复制的目的地址
                if (!System.IO.Directory.Exists(strDestDir))
                {
                    System.IO.Directory.CreateDirectory(strDestDir);
                }

                string[] strFiles = System.IO.Directory.GetFiles(strSourceDir);
                for (int i = 0; i < strFiles.Length; i++)
                {
                    string strFileName = System.IO.Path.GetFileName(strFiles[i]);
                    string strDestFilePath = System.IO.Path.Combine(strDestDir, strFileName);
                    System.IO.File.Copy(strFiles[i], strDestFilePath, true);
                }

                if (System.IO.File.Exists(strAppPath + "\\Resources\\x64\\SQLite.Interop.dll") && System.IO.File.Exists(strAppPath + "\\Resources\\x86\\SQLite.Interop.dll"))
                {
                    if (!System.IO.Directory.Exists(strAppPath + "\\x64"))
                    {
                        System.IO.Directory.CreateDirectory(strAppPath + "\\x64");
                    }
                    if (!System.IO.Directory.Exists(strAppPath + "\\x86"))
                    {
                        System.IO.Directory.CreateDirectory(strAppPath + "\\x86");
                    }
                    System.IO.File.Copy(strAppPath + "\\Resources\\x64\\SQLite.Interop.dll", strAppPath + "\\x64\\SQLite.Interop.dll", true);
                    System.IO.File.Copy(strAppPath + "\\Resources\\x86\\SQLite.Interop.dll", strAppPath + "\\x86\\SQLite.Interop.dll", true);
                }
                return true;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                //Log.getExceptionFile().Info("自动复制配置文件出现异常，异常信息为：\n" + ex.Message);
                return false;
            }
        }
        private static Mutex mutex;
        private static bool IsRunning(string projectName)
        {
            bool result;

            mutex = new Mutex(true, projectName, out result);

            if (result)
            {
                mutex.ReleaseMutex();
            }

            return !result;
        }
    }
}
