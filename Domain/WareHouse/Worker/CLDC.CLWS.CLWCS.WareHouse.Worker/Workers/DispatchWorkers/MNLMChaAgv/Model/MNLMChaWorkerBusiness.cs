using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.AgvApi;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{

    public class MNLMNotifyDeviceStatusMode
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DEVICE_NO { get; set; }
        /// <summary>
        /// 在线状态 1在线  0离线
        /// </summary>
        public string ONLINE_STATUS { get; set; }
        /// <summary>
        /// 运行状态 1忙碌  0空闲
        /// </summary>
        public int RUN_STATUS { get; set; }
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

        public static explicit operator MNLMNotifyDeviceStatusMode(string json)
        {
            return JsonConvert.DeserializeObject<MNLMNotifyDeviceStatusMode>(json);
        }

    }


    public class MNLMExeResultMode
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public int STATUS { get; set; }
        /// <summary>
        /// 车编号
        /// </summary>
        public string DEVICE_NO { get; set; }

        public static explicit operator MNLMExeResultMode(string json)
        {
            return JsonConvert.DeserializeObject<MNLMExeResultMode>(json);
        }

    }
    public class MNLMNotifyDeviceInfoMode
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string DEVICE_NO { get; set; }
        /// <summary>
        /// 指派任务的总数量
        /// </summary>
        public string TASK_COUNT { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public string BATTERY { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public string SPEED { get; set; }
        /// <summary>
        /// X坐标值
        /// </summary>
        public string Position_X { get; set; }
        /// <summary>
        /// Y坐标值
        /// </summary>
        public string Position_Y { get; set; }
        /// <summary>
        /// Z坐标值
        /// </summary>
        public string Position_Z { get; set; }
     
        /// <summary>
        /// 车编号
        /// </summary>
        public int carNo { get; set; }

        public static explicit operator MNLMNotifyDeviceInfoMode(string json)
        {
            return JsonConvert.DeserializeObject<MNLMNotifyDeviceInfoMode>(json);
        }

    }

    public class MNLMAgvInfo
    {
        public int WcsId
        {
            get;
            set;
        }
        public string XChaId { get; set; }
    }



    public class MNLMChaWorkerBusiness : OrderWorkerBuinessAbstract, IMNLMChaAgvApi
    {
        
        protected override OperateResult ParticularInitlize()
        {
            DependencyFactory.GetDependency().RegisterInstance<IMNLMChaAgvApi>(this);
            return OperateResult.CreateSuccessResult();
        }


        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            AgvInfoDic.Clear();
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                MNLMChaWorkerBusinessHandleProperty handleProperty = null;
                string businessPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(businessPropertyXml))
                {
                    try
                    {
                        handleProperty = (MNLMChaWorkerBusinessHandleProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(MNLMChaWorkerBusinessHandleProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess || handleProperty==null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：IdentifyWorkerProperty", businessPropertyXml));
                }

                string content = handleProperty.Config.AssistantNoConvert.Trim();
                string[] keyValue = content.Split(';');
                foreach (string key_value in keyValue)
                {
                    if (string.IsNullOrEmpty(key_value))
                    {
                        continue;
                    }
                    string[] wcs_hangCha = key_value.Split('_');
                    if (wcs_hangCha.Length >= 2)
                    {
                        MNLMAgvInfo agvinfo = new MNLMAgvInfo
                        {
                            WcsId = int.Parse(wcs_hangCha[0]),
                            XChaId = wcs_hangCha[1]
                        };
                        AgvInfoDic.Add(agvinfo);
                    }
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

        protected override OperateResult ParticularConfig()
        {
            ///获取Port配置
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
            return  OperateResult.CreateSuccessResult();
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

        public OperateResult NotifyDeviceStatus(MNLMNotifyDeviceStatusMode cmd)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                MNLMNotifyDeviceStatusMode deviceStatus = (MNLMNotifyDeviceStatusMode)cmd;
                //int mnlmChaAgvId = deviceStatus.DEVICE_NO;
                int onlineStatus =int.Parse( deviceStatus.ONLINE_STATUS);
                int expStatus = deviceStatus.EXCEPTION_STATUS;
                int expCode = deviceStatus.EXCEPTION_CODE;
                int wcsAgvId = XChaToWcs(deviceStatus.DEVICE_NO);
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

        public OperateResult NotifyExeResult(MNLMExeResultMode cmd)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                MNLMExeResultMode deviceStatus = (MNLMExeResultMode)cmd;
                int orderStatus = deviceStatus.STATUS;
                string taskNo = deviceStatus.TASK_NO;
                int wcsAgvId = XChaToWcs(deviceStatus.DEVICE_NO);
                if (NotifyExeResultEvent != null)
                {
                    return NotifyExeResultEvent(wcsAgvId, orderStatus, taskNo);
                }
                else
                {
                    return OperateResult.CreateFailedResult("事件:NotifyExeResultEvent 未注册", 1);
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

        protected List<MNLMAgvInfo> AgvInfoDic = new List<MNLMAgvInfo>();

        //protected OperateResult NotifyDeviceStatusHandler(MNLMNotifyDeviceStatusMode cmd)
        //{
        //    OperateResult result = OperateResult.CreateFailedResult();
        //    try
        //    {
        //        NotifyDeviceStatusMode deviceStatus = (NotifyDeviceStatusMode)cmd;
        //        int mnlmChaAgvId = deviceStatus.DEVICE_NO;
        //        int onlineStatus = deviceStatus.DEVICE_STATUS;
        //        int expStatus = deviceStatus.EXCEPTION_STATUS;
        //        int expCode = deviceStatus.EXCEPTION_CODE;
        //        int wcsAgvId = XChaToWcs(mnlmChaAgvId);
        //        if (NotifyDeviceStatusEvent != null)
        //        {
        //            return NotifyDeviceStatusEvent(wcsAgvId, onlineStatus, expStatus, expCode);
        //        }
        //        else
        //        {
        //            return OperateResult.CreateFailedResult("事件:NotifyDeviceStatusEvent 未注册", 1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = true;
        //        result.Message = OperateResult.ConvertException(ex);

        //    }
        //    return result;
        //}

        private int XChaToWcs(string xChaId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return 9999;
            }
            if (AgvInfoDic.Exists(a => a.XChaId.Equals(xChaId)))
            {
                return AgvInfoDic.Find(a => a.XChaId.Equals(xChaId)).WcsId;
            }
            return 9999;
        }


        private string WcsToXCha(int wcsId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return "9999";
            }
            if (AgvInfoDic.Exists(a => a.XChaId.Equals(wcsId)))
            {
                return AgvInfoDic.Find(a => a.XChaId.Equals(wcsId)).XChaId;
            }
            return "9999";
        }

        //protected OperateResult NotifyExeResultEventHandler(string cmd)
        //{

        //    OperateResult result = OperateResult.CreateFailedResult();
        //    try
        //    {
        //        MNLMExeResultMode exeResult = (MNLMExeResultMode)cmd;
        //        int mnlmChaAgvId = exeResult.carNo;
        //        int exeStatus = exeResult.state;
        //        string taskNo = exeResult.taskNo;
        //        int wcsAgvId = XChaToWcs(mnlmChaAgvId);
        //        if (NotifyExeResultEvent != null)
        //        {
        //            return NotifyExeResultEvent(wcsAgvId, exeStatus, taskNo);
        //        }
        //        else
        //        {
        //            return OperateResult.CreateFailedResult("事件:NotifyExeResultEvent 未注册", 1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = true;
        //        result.Message = OperateResult.ConvertException(ex);

        //    }
        //    return result;
        //}

        public OperateResult NotifyDeviceInfo(MNLMNotifyDeviceInfoMode cmd)
        {
          //不做 处理
          return OperateResult.CreateSuccessResult();
        }
    }
}
