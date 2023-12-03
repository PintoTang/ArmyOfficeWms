using System;
using System.Collections.Generic;
using System.Windows;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.Xml;
using CLDC.Infrastructrue.Security;

namespace CL.WCS.SystemConfigPckg.ViewModel
{
    public class SystemConfigViewMode : NotifyObject
    {
        public SystemConfigMode SysConfig { get; set; }
        public SystemConfigViewMode(SystemConfigMode configMode)
        {
            SysConfig = configMode;
        }


        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }


        private Dictionary<DepartmentEnum, string> departmentDic = new Dictionary<DepartmentEnum, string>();
        public Dictionary<DepartmentEnum, string> DepartmentDic
        {
            get
            {
                if (departmentDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(DepartmentEnum)))
                    {
                        DepartmentEnum em = (DepartmentEnum)value;
                        departmentDic.Add(em, em.GetDescription());
                    }
                }
                return departmentDic;

            }
        }     

        private Dictionary<OpcModeEnum, string> opcModeDic = new Dictionary<OpcModeEnum, string>();
        public Dictionary<OpcModeEnum, string> OpcModeDic
        {
            get
            {
                if (opcModeDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(OpcModeEnum)))
                    {
                        OpcModeEnum em = (OpcModeEnum)value;
                        opcModeDic.Add(em, em.GetDescription());
                    }
                }
                return opcModeDic;

            }
        }

        private Dictionary<DatabaseTypeEnum, string> dataBaseTypeDic = new Dictionary<DatabaseTypeEnum, string>();
        public Dictionary<DatabaseTypeEnum, string> DataBaseTypeDic
        {
            get
            {
                if (dataBaseTypeDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(DatabaseTypeEnum)))
                    {
                        DatabaseTypeEnum em = (DatabaseTypeEnum)value;
                        dataBaseTypeDic.Add(em, em.GetDescription());
                    }
                }
                return dataBaseTypeDic;

            }
        }

        private Dictionary<int, string> logLenghtDic = new Dictionary<int, string>();
        public Dictionary<int, string> LogLenghtDic
        {
            get
            {
                if (logLenghtDic.Count == 0)
                {
                    logLenghtDic.Add(10, "10M");
                    logLenghtDic.Add(20, "20M");
                    logLenghtDic.Add(30, "30M");
                    logLenghtDic.Add(40, "40M");
                    logLenghtDic.Add(50, "50M");
                    logLenghtDic.Add(60, "60M");
                }
                return logLenghtDic;

            }
        }

        private Dictionary<int, string> logNumMaxDic = new Dictionary<int, string>();
        public Dictionary<int, string> LogNumMaxDic
        {
            get
            {
                if (logNumMaxDic.Count == 0)
                {
                    logNumMaxDic.Add(10, "10个");
                    logNumMaxDic.Add(20, "20个");
                    logNumMaxDic.Add(30, "30个");
                    logNumMaxDic.Add(40, "40个");
                    logNumMaxDic.Add(50, "50个");
                    logNumMaxDic.Add(60, "60个");
                }
                return logNumMaxDic;

            }
        }

        private Dictionary<int, string> logSaveDaysDic = new Dictionary<int, string>();

        public Dictionary<int, string> LogSaveDaysDic
        {
            get
            {
                if (logSaveDaysDic.Count == 0)
                {
                    logSaveDaysDic.Add(10, "10天");
                    logSaveDaysDic.Add(15, "15天");
                    logSaveDaysDic.Add(20, "20天");
                    logSaveDaysDic.Add(30, "30天");
                    logSaveDaysDic.Add(40, "40天");
                    logSaveDaysDic.Add(50, "50天");
                    logSaveDaysDic.Add(60, "60天");
                    logSaveDaysDic.Add(180, "180天");
                    logSaveDaysDic.Add(360, "360天");
                }
                return logSaveDaysDic;

            }

        }


      


        private Dictionary<EnumLogLevel, string> logLevelDic = new Dictionary<EnumLogLevel, string>();
        public Dictionary<EnumLogLevel, string> LogLevelDic
        {
            get
            {
                if (logLevelDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(EnumLogLevel)))
                    {
                        EnumLogLevel em = (EnumLogLevel)value;
                        logLevelDic.Add(em, em.GetDescription());
                    }
                }
                return logLevelDic;

            }
        }


        private RelayCommand saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(SaveConfig);
                }
                return saveCommand;
            }
        }

       

        /// <summary>
        /// 添加数据到
        /// </summary>
        private void SaveConfig()
        {
            try
            {          
                MessageBoxResult rt = MessageBoxEx.Show("您确认保存？", "警告", MessageBoxButton.YesNo);
                if (rt.Equals(MessageBoxResult.Yes))
                {
                    OperateResult<string> projectPathResult = SystemConfig.Instance.GetProjectPath();
                    if (!projectPathResult.IsSuccess)
                    {
                        string msg = string.Format("保存失败，原因：{0}", projectPathResult.Message);
                        MessageBoxEx.Show(msg, "错误");
                        return;
                    }
                    string strFileName = Environment.CurrentDirectory + "\\Config\\SystemConfig.xml";
                    string projectPath = projectPathResult.Content + "\\SystemConfig.xml";

                    SysConfig.WcsDatabaseConn.Value = GetEnPwdDataConnectString(SysConfig.WcsDataBaseType.Value,SysConfig.WcsDatabaseConn.Value);
                    //SysConfig.AtsDatabaseConn.Value = GetEnPwdDataConnectString(SysConfig.AtsDataBaseType.Value,SysConfig.AtsDatabaseConn.Value);

                    XmlSerializerHelper.SaveToXml(strFileName, SysConfig,typeof(SystemConfigMode), "SystemConfig");
                    XmlSerializerHelper.SaveToXml(projectPath, SysConfig, typeof(SystemConfigMode), "SystemConfig");
                    MessageBoxEx.Show("保存成功，重新启动系统后生效");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("配置保存失败:" +OperateResult.ConvertException(ex));
            }
        }

        private string GetEnPwdDataConnectString(DatabaseTypeEnum dataTypeEnum, string configConnectString)
        {
            string Host = "";
            string DbName = "";
            string UserName = "";
            string Password = "";
            string rtConnString = "";
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
                        Password = AESEncryption.EncryptMD5(keyValue[1].Trim(), AESEncryption.strKey);
                    }
                }
                //解析连接串
            }

            switch (dataTypeEnum)
            {
                case DatabaseTypeEnum.Oracle:
                    rtConnString = string.Format("Data Source={0};User Id={1};Password={2}", Host,
                        DbName, UserName, Password);
                    break;
                case DatabaseTypeEnum.SqlServer:
                    rtConnString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3}", Host,
                        DbName, UserName, Password);
                    break;
                case DatabaseTypeEnum.Sqlite:
                    rtConnString = configConnectString; //TO DO 暂时不考虑
                    break;
                case DatabaseTypeEnum.MySql:
                    rtConnString = configConnectString; //TO DO 暂时不考虑
                    break;
            }
            //组装Conn
            return rtConnString;
        }



    }

}
