using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Xml;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.Model
{
    /// <summary>
    /// 接收到WebService过来的数据处理
    /// </summary>
    public abstract class DbServiceAbstract : ServiceAbstract, IDbServiceHandle, IStateControl
    {
        public delegate void BusinessMethod(string xmlPara);
        protected int MaxHandleCmdSeq = 0;
        protected bool PowerOnFlag = true;
        protected DateTime BeginTime = DateTime.MinValue;


        protected ReceiveDataAbstract ReceiveData;
        public string ReceiveName = "WMS的接收业务";

        protected override OperateResult ParticularInitlize()
        {
            this.ReceiveData = DependencyHelper.GetService<ReceiveDataAbstract>();
            if (ReceiveData == null)
            {
                return OperateResult.CreateFailedResult("获取对象：ReceiveDataAbstract 失败", 1);
            }
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 上层接口数据处理器
        /// </summary>
        public DbServiceMonitor DataMonitorHandle { get; set; }

        public virtual OperateResult Start()
        {
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 处理数据库的数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="isAsync"></param>
        public abstract void BusinessProcessData(DataRow row, bool isAsync);


        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            LogHelper.WriteLog(this.ReceiveName, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }

        private RunStateMode _curRunState = RunStateMode.Pause;

        public RunStateMode CurRunState
        {
            get { return _curRunState; }
            set
            {
                _curRunState = value;
            }
        }

        private ControlStateMode _curControlState = ControlStateMode.Auto;
        public ControlStateMode CurControlMode
        {
            get { return _curControlState; }
            set
            {
                _curControlState = value;
            }
        }

        public OperateResult Run()
        {
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Pause()
        {
            if (CurRunState.Equals(RunStateMode.Pause))
            {
                CurRunState = RunStateMode.Run;
            }
            else if (CurRunState.Equals(RunStateMode.Run))
            {
                CurRunState = RunStateMode.Pause;
            }
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Reset()
        {
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Stop()
        {
            CurRunState = RunStateMode.Stop;
            return OperateResult.CreateSuccessResult();
        }



        protected override OperateResult InitilizeConfig()
        {
            OperateResult initilizeConfigResult = OperateResult.CreateFailedResult();
            try
            {
                string fileName = "Config/WcsServiceConfig.xml";
                string path = "Config";
                var config = new XmlOperator(fileName);
                XmlElement xmlElement = config.GetXmlElement("WcsService", "Id", WebserviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);

                if (xmlNode == null)
                {
                    initilizeConfigResult.Message = string.Format("通过设备：{0} 获取配置失败", WebserviceId);
                    initilizeConfigResult.IsSuccess = false;
                    return initilizeConfigResult;
                }

                string businessConfigXml = xmlNode.OuterXml;

                DbServiceConfigProperty webserviceConfig = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        webserviceConfig =
                            (DbServiceConfigProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(DbServiceConfigProperty));
                    }
                    catch (Exception ex)
                    {
                        initilizeConfigResult.IsSuccess = false;
                        initilizeConfigResult.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (webserviceConfig == null)
                {
                    return initilizeConfigResult;
                }
                string selectSql = webserviceConfig.SelectSql;
                string updateSql = webserviceConfig.UpdateSql;
                int monitorInterval = webserviceConfig.MonitorInterval;
                DataMonitorHandle = new DbServiceMonitor(selectSql,updateSql,monitorInterval,this);
                initilizeConfigResult.IsSuccess = true;
                return initilizeConfigResult;

            }
            catch (Exception ex)
            {
                initilizeConfigResult.IsSuccess = false;
                initilizeConfigResult.Message = OperateResult.ConvertException(ex);
            }
            return initilizeConfigResult;
        }

        protected override OperateResult StartService()
        {
            OperateResult monitorStart = DataMonitorHandle.Start();
            if (!monitorStart.IsSuccess)
            {
                return monitorStart;
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult StopService()
        {
            OperateResult monitorStart = DataMonitorHandle.Stop();
            if (!monitorStart.IsSuccess)
            {
                return monitorStart;
            }
            return OperateResult.CreateSuccessResult();
        }


    }
}
