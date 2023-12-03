using System;
using System.IO;
using CL.Framework.ConfigFilePckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Framework.Log;
using CLDC.Infrastructrue.Security;
using CLDC.Infrastructrue.Xml;

namespace CL.WCS.SystemConfigPckg.Model
{
    public delegate void ApplicationExitingHandlder();
    /// <summary>
    /// 系统配置类
    /// 用于保存系统基础信息，比如当前运行项目等
    /// </summary>
    public class SystemConfig
    {
        private static SystemConfig systemConfig;
        OpcModeEnum _IsTrueOPC;
      
        bool _isUseCellPile;

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseTypeEnum WcsDataBaseType { get; set; }
        /// <summary>
        /// ATS 数据库类型
        /// </summary>
        public DatabaseTypeEnum AtsDataBaseType { get; set; }

        /// <summary>
        /// WCS本地库的链接串
        /// </summary>
        public string WcsConnectionString { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SysNo { get; set; }

        /// <summary>
        /// OPC　Monitor的间隔
        /// </summary>
        private int _monitorIntervalTime = 200;

        public int MonitorIntervalTime
        {
            get
            {
                return _monitorIntervalTime;
            }
            set { _monitorIntervalTime = value; }
        }

        /// <summary>
        /// 是否采用真实的WebService
        /// </summary>
        public bool IsTrueWebService { get; set; }

        /// <summary>
        /// OPC线程数
        /// </summary>
        private int _threadCount = 10;

        public int ThreadCount
        {
            get { return _threadCount; }
            set { _threadCount = value; }
        }

        public DepartmentEnum Department { get; set; }

        /// <summary>
        /// 是否使用仓垛关系
        /// </summary>
        public bool IsUseCellPile
        {
            get { return _isUseCellPile; }
            set { _isUseCellPile = value; }
        }
        bool _isExitApp = false;

        /// <summary>
        /// 此段的配置为内部测试使用
        /// </summary>
        public OpcModeEnum IsTrueOPC { get { return _IsTrueOPC; } }
      
        /// <summary>
        /// 是否退出整个WCS系统程序,默认为False
        /// </summary>
        public bool isExitApp
        {
            get { return _isExitApp; }
            set { _isExitApp = value; }
        }

        public bool IsLicenseAvailable { get; set; }

        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 软件版权所属
        /// </summary>
        public string CopyRight { get; set; }

        /// <summary>
        /// 当前仓位ID
        /// </summary>
        public string WhCode { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SysName { get; set; }




        public event ApplicationExitingHandlder ApplicationExitingEvent;


        /// <summary>
        /// 单实例系统配置管理类
        /// </summary>
        /// <returns></returns>
        public static SystemConfig Instance
        {
            get
            {
                if (systemConfig == null)
                    systemConfig = new SystemConfig();
                return systemConfig;
            }
        }

        public void AppExit()
        {
            if (ApplicationExitingEvent != null)
            {
                ApplicationExitingEvent();
            }
        }

        public SystemConfigMode CurSystemConfig { get; set; }

        private SystemConfig()
        {
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\SystemConfig.xml";

                CurSystemConfig = (SystemConfigMode)XmlSerializerHelper.LoadFromXml(strFileName, typeof(SystemConfigMode));
                WcsDataBaseType = CurSystemConfig.WcsDataBaseType.Value;
                //AtsDataBaseType = CurSystemConfig.AtsDataBaseType.Value;
                CurSystemConfig.WcsDatabaseConn.Value = GetUnPwdDataConnectString(WcsDataBaseType,CurSystemConfig.WcsDatabaseConn.Value);
                //CurSystemConfig.AtsDatabaseConn.Value = GetUnPwdDataConnectString(AtsDataBaseType, CurSystemConfig.AtsDatabaseConn.Value);

                _IsTrueOPC = CurSystemConfig.OpcMode.Value;

                WcsConnectionString = CurSystemConfig.WcsDatabaseConn.Value;

                //SysNo = CurSystemConfig.SysNo.Value;

              

                IsTrueWebService = CurSystemConfig.IsTrueWebService.Value;

                Department = CurSystemConfig.Department.Value;


                //_isUseCellPile = CurSystemConfig.IsUseCellPile.Value;


                WhCode = CurSystemConfig.WhCode.Value;

                Version = CurSystemConfig.Version.Value;

                CopyRight = CurSystemConfig.CopyRight.Value;

                SysName = CurSystemConfig.SysName.Value;

            }
            catch (Exception ex)
            {
                Log.getDebugFile().Info(ex.Message);
                throw ex;
            }

        }

        private string GetUnPwdDataConnectString(DatabaseTypeEnum wcsDataBaseType ,string configConnectString)
        {
          
            switch (wcsDataBaseType)
            {
                case DatabaseTypeEnum.Oracle:
                   return GetDecipheringDataConnForOracle(configConnectString);
                    break;
                case DatabaseTypeEnum.SqlServer:
                    return GetDecipheringDataConnForSqlServer(configConnectString);
                    break;
                case DatabaseTypeEnum.MySql:
                    return configConnectString;
                    break;
                case DatabaseTypeEnum.Sqlite:
                    return configConnectString;
                    break;
            }
            return configConnectString;
        }

        private string GetDecipheringDataConnForOracle(string configConnectString)
        {
            //connectionString="Data Source=10.97.2.22;User ID=用户名;Password=密码;"
            string Host = "";
            string UserName = "";
            string Password = "";
            if (!string.IsNullOrEmpty(configConnectString))
            {
                string[] dbConfigItem = configConnectString.Split(';');
                foreach (string configItem in dbConfigItem)
                {
                    string[] keyValue = configItem.Split('=');
                    if (keyValue[0].Trim().Contains("Data Source"))
                    {
                        Host = keyValue[1].Trim();
                    }
                   
                    else if (keyValue[0].Trim().Contains("User Id"))
                    {
                        UserName = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("Password"))
                    {
                        Password = AESEncryption.DecryptMD5(keyValue[1].Trim(), AESEncryption.strKey);
                    }
                }
                //解析连接串
            }
            //组装Conn
            string connectString = string.Format("Data Source={0};User Id={1};Password={2}", Host, UserName, Password);
            return connectString;
        }
        
        /// <summary>
        /// SqlServer 加密字符串进行解密处理
        /// </summary>
        /// <param name="configConnectString">数据库连接配置</param>
        /// <returns>SqlServer configConnectString</returns>
        private string GetDecipheringDataConnForSqlServer(string configConnectString)
        {
            //string strData = "Data Source=10.97.2.22;Initial Catalog =WcsFuSha01;User Id=WcsFuSha01;Password=677555731260E20B6ADF38C19C3D6F40;";
            string Host = "";
            string DbName = "";
            string UserName = "";
            string Password = "";
            if (!string.IsNullOrEmpty(configConnectString))
            {
                string[] dbConfigItem = configConnectString.Split(';');
                foreach (string configItem in dbConfigItem)
                {
                    string[] keyValue = configItem.Split('=');
                    if (keyValue[0].Trim().Contains("Data Source"))
                    {
                        Host = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("Initial Catalog"))
                    {
                        DbName = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("User Id"))
                    {
                        UserName = keyValue[1].Trim();
                    }
                    else if (keyValue[0].Trim().Contains("Password"))
                    {
                        //处理数据库密码加密 进行解密
                        //1、加密
                        //string strEnPwd = AESEncryption.EncryptMD5(keyValue[1].Trim(), AESEncryption.strKey);
                        //2、解密
                        string strDePwd = AESEncryption.DecryptMD5(keyValue[1].Trim(), AESEncryption.strKey);
                        //PbContent.Password = keyValue[1].Trim();
                        Password = strDePwd;
                    }
                }
                //解析连接串
            }
            //组装Conn
            string connectString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3}", Host,
            DbName, UserName, Password);
            return connectString;
        }


      

        public OperateResult<string> GetProjectPath()
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

    }
}
