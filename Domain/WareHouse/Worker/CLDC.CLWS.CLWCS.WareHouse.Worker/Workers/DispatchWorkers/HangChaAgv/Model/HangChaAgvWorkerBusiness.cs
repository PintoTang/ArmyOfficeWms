using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.AgvApi;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.HangChaAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{

    public class NotifyDeviceStatusMode
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public int DEVICE_NO { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DEVICE_TYPE { get; set; }
        /// <summary>
        /// 设备状态 1在线0离线
        /// </summary>
        public int DEVICE_STATUS { get; set; }
        /// <summary>
        /// 设备异常状态 1异常0无异常
        /// </summary>
        public int EXCEPTION_STATUS { get; set; }
        /// <summary>
        /// 设备异常代码
        /// </summary>
        public int EXCEPTION_CODE { get; set; }
        /// <summary>
        /// 异常说明
        /// </summary>
        public int EXCEPTION_MSG { get; set; }
        /// <summary>
        /// 异常时间
        /// </summary>
        public DateTime EXCEPTION_DATE { get; set; }

        public static explicit operator NotifyDeviceStatusMode(string json)
        {
            return JsonConvert.DeserializeObject<NotifyDeviceStatusMode>(json);
        }

    }


    public class ExeResultMode
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string taskNo { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int state { get; set; }
        /// <summary>
        /// 车编号
        /// </summary>
        public int carNo { get; set; }

        public static explicit operator ExeResultMode(string json)
        {
            return JsonConvert.DeserializeObject<ExeResultMode>(json);
        }

    }

    public class AgvInfo
    {
        public int WcsId
        {
            get;
            set;
        }
        public int HangChaId { get; set; }
    }



    public class HangChaWorkerBusiness : OrderWorkerBuinessAbstract, IHangChaAgvApi
    {
        protected override OperateResult ParticularInitlize()
        {
            DependencyFactory.GetDependency().RegisterInstance<IHangChaAgvApi>(this);
            return OperateResult.CreateSuccessResult();
        }


        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            AgvInfoDic.Clear();
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                HangChaWorkerBusinessHandleProperty handleProperty = null;
                string businessPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(businessPropertyXml))
                {
                    try
                    {
                        handleProperty = (HangChaWorkerBusinessHandleProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(HangChaWorkerBusinessHandleProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess || handleProperty == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：IdentifyWorkerProperty", businessPropertyXml));
                }

                string content = handleProperty.Config.DeviceNoConvert.Trim();
                InitilizeAgvInfo(content);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }

            return result;
        }

        private void InitilizeAgvInfo(string deviceConvert)
        {
            string[] keyValue = deviceConvert.Split(';');
            foreach (string key_value in keyValue)
            {
                if (string.IsNullOrEmpty(key_value))
                {
                    continue;
                }
                string[] wcs_hangCha = key_value.Split('_');
                if (wcs_hangCha.Length >= 2)
                {
                    AgvInfo agvinfo = new AgvInfo
                    {
                        WcsId = int.Parse(wcs_hangCha[0]),
                        HangChaId = int.Parse(wcs_hangCha[1])
                    };
                    AgvInfoDic.Add(agvinfo);
                }
            }
        }

        protected override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig;
                string path = "BusinessHandle";
                XmlElement xmlElement = doc.GetXmlElement("Coordination", "Id", WorkerId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", WorkerId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                return InitalizeConfig(xmlNode);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult BeforeTransportBusinessHandle(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }



        public override OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult CancleOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();

        }

        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();

        }

        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            return OperateResult.CreateSuccessResult();

        }

        public OperateResult NotifyDeviceStatus(string cmd)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                NotifyDeviceStatusMode deviceStatus = (NotifyDeviceStatusMode)cmd;
                int hangChaAgvId = deviceStatus.DEVICE_NO;
                int onlineStatus = deviceStatus.DEVICE_STATUS;
                int expStatus = deviceStatus.EXCEPTION_STATUS;
                int expCode = deviceStatus.EXCEPTION_CODE;
                int wcsAgvId = HangChaToWcs(hangChaAgvId);
                if (NotifyDeviceStatusEvent != null)
                {
                    return NotifyDeviceStatusEvent(wcsAgvId, onlineStatus, expStatus, expCode);
                }
                else
                {
                    return OperateResult.CreateFailedResult("事件:NotifyDeviceStatusEvent 未注册", 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        public OperateResult NotifyExeResult(string cmd)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                ExeResultMode deviceStatus = (ExeResultMode)cmd;
                int hangChaAgvId = deviceStatus.carNo;
                int orderStatus = deviceStatus.state;
                string taskNo = deviceStatus.taskNo;
                int wcsAgvId = HangChaToWcs(hangChaAgvId);
                if (NotifyDeviceStatusEvent != null)
                {
                    return NotifyExeResultEvent(wcsAgvId, orderStatus, taskNo);
                }
                else
                {
                    return OperateResult.CreateFailedResult("事件:NotifyDeviceStatusEvent 未注册", 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        public Func<int, int, int, int, OperateResult> NotifyDeviceStatusEvent;
        public Func<int, int, string, OperateResult> NotifyExeResultEvent;

        protected List<AgvInfo> AgvInfoDic = new List<AgvInfo>();

        protected OperateResult NotifyDeviceStatusHandler(string cmd)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                NotifyDeviceStatusMode deviceStatus = (NotifyDeviceStatusMode)cmd;
                int hangChaAgvId = deviceStatus.DEVICE_NO;
                int onlineStatus = deviceStatus.DEVICE_STATUS;
                int expStatus = deviceStatus.EXCEPTION_STATUS;
                int expCode = deviceStatus.EXCEPTION_CODE;
                int wcsAgvId = HangChaToWcs(hangChaAgvId);
                if (NotifyDeviceStatusEvent != null)
                {
                    return NotifyDeviceStatusEvent(wcsAgvId, onlineStatus, expStatus, expCode);
                }
                else
                {
                    return OperateResult.CreateFailedResult("事件:NotifyDeviceStatusEvent 未注册", 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = true;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        private int HangChaToWcs(int hangChaId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return hangChaId;
            }
            if (AgvInfoDic.Exists(a => a.HangChaId.Equals(hangChaId)))
            {
                return AgvInfoDic.Find(a => a.HangChaId.Equals(hangChaId)).WcsId;
            }
            return hangChaId;
        }


        private int WcsToHangCha(int wcsId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return wcsId;
            }
            if (AgvInfoDic.Exists(a => a.HangChaId.Equals(wcsId)))
            {
                return AgvInfoDic.Find(a => a.HangChaId.Equals(wcsId)).HangChaId;
            }
            return wcsId;
        }

        protected OperateResult NotifyExeResultEventHandler(string cmd)
        {

            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                ExeResultMode exeResult = (ExeResultMode)cmd;
                int hangChaAgvId = exeResult.carNo;
                int exeStatus = exeResult.state;
                string taskNo = exeResult.taskNo;
                int wcsAgvId = HangChaToWcs(hangChaAgvId);
                if (NotifyExeResultEvent != null)
                {
                    return NotifyExeResultEvent(wcsAgvId, exeStatus, taskNo);
                }
                else
                {
                    return OperateResult.CreateFailedResult("事件:NotifyExeResultEvent 未注册", 1);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = true;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }
    }
}
