

using CL.Framework.ConfigFilePckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Framework.Log;
using System;
using System.IO;
using System.Threading;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    /// <summary>
    /// 系统配置文件 处理帮助类
    /// </summary>
    public  class SystemRunConfigHelper
    {
       
        public static OperateResult RunConfig()
        {
            bool isCopy = CopyAllConfigFiles();
            if (!isCopy)
            {
                return OperateResult.CreateFailedResult("复制配置文件失败！",1);
            }

            ConfigFile configFile = new ConfigFile("Config/App.config");
            if (IsRunning(configFile.AppSettings["CurrentProject"]))
            {
                Log.getExceptionFile().Info("存在多个运行实例！");
				return OperateResult.CreateFailedResult("存在多个运行实例",1);
            }

            DeleteOldLogFile(configFile);
            string strMaxLength = configFile.AppSettings["MaxByteLength"];
            string strMaxLogCount = configFile.AppSettings["MaxLogCount"];
            //这个必须放在最前！否则后面的日志接口调用都会出问题！
            SetLogObject(strMaxLength, strMaxLogCount);
			return OperateResult.CreateSuccessResult();
        }
        private static void SetLogObject(string maxByteLength, string maxLogCount)
        {
            if (!string.IsNullOrEmpty(maxByteLength))
            {
                CustomFileLoggerImp.MaxFileSize = maxByteLength;
            }

            int count = Convert.ToInt32(maxLogCount);
            if (count != 0)
            {
                CustomFileLoggerImp.MaxSizeRollBackUp = count;
            }
        }
     

        private static void DeleteOldLogFile(ConfigFile configFile)
        {
            int savedDays = 3;
            string logSavedDays = configFile.AppSettings["LogSavedDays"];
            if (!string.IsNullOrEmpty(logSavedDays))
            {
                savedDays = Convert.ToInt32(logSavedDays);
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
                    if (dirTimeName.AddDays(savedDays) <= DateTime.Now.Date)
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
        }

        public  static OperateResult<string> GetProjectPath()
        {
            OperateResult<string> result = new OperateResult<string>();
            string strAppPath = Directory.GetCurrentDirectory();
            string strConfigFilePath = System.IO.Path.Combine(strAppPath, @"Config\ConfigFilePath.xml");
            if (!System.IO.File.Exists(strConfigFilePath))
            {
                result.IsSuccess = false;
                result.Message = string.Format("不存在文件：{0}", strConfigFilePath);
                return result;
            }
            ConfigFile configFile = new ConfigFile(strConfigFilePath);
            string strDirName = configFile.AppSettings["Path"];
            if (string.IsNullOrEmpty(strDirName))
            {
                result.IsSuccess = false;
                result.Message = "配置文件ConfigFilePath.xml里配置Value的值为空，请手动配置";
                return result;
            }
            string projectPath = System.IO.Path.Combine(strAppPath, strDirName);
            if (!System.IO.Directory.Exists(projectPath))
            {
                result.IsSuccess = false;
                result.Message = string.Format("不存在文件：{0}", projectPath);
                return result;
            }
            result.Content = projectPath;
            result.IsSuccess = true;
            return result;
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
            catch (Exception ex)
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
