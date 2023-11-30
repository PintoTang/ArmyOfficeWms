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
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.RcsApi;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class ClouRcsAgvWorkerBusiness : OrderWorkerBuinessAbstract
    {
        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }
        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            AgvInfoDic.Clear();
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                ClouRcsAgvWorkerBusinessHandleProperty handleProperty = null;
                string businessPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(businessPropertyXml))
                {
                    try
                    {
                        handleProperty = (ClouRcsAgvWorkerBusinessHandleProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(ClouRcsAgvWorkerBusinessHandleProperty));
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
        /// <summary>
        /// RcsAgv上报任务信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult ReportTaskResult(ReportTaskResultMode cmd)
        {
            OperateResult result = OperateResult.CreateFailedResult();
       
            try
            {
                ReportTaskResultMode taskMode = (ReportTaskResultMode)cmd;
                string strAgvNo = taskMode.AGV_NO;
                int wcsAgvId;
                int.TryParse(strAgvNo, out wcsAgvId);
                if (wcsAgvId != 0)
                {
                    result.IsSuccess = false;
                    result.Message = "AGV编号参数有误,不等于0";
                    return result;
                }
                wcsAgvId = XChaToWcs(wcsAgvId);//转换成wcs真实的设置ID

                string strTaskNo = taskMode.TASK_NO;
                string strTaskStatus = taskMode.TASK_STATUS;

                AgvMoveStepEnum agvMoveStep = AgvMoveStepEnum.Free;
                if (strTaskStatus == "5")
                {
                    agvMoveStep = AgvMoveStepEnum.Finish;
                }

                if (NotifyExeResultEvent != null)
                {

                    return NotifyExeResultEvent(wcsAgvId, (int)agvMoveStep, strTaskNo);
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

        protected List<AgvInfo> AgvInfoDic = new List<AgvInfo>();


        /// <summary>
        /// 上报的任务异常信息处理
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult ReportTaskException(ReportTaskExceptionMode cmd)
        {
            OperateResult result = OperateResult.CreateSuccessResult();
            //记录上报的异常任务信息 To Do

            return result;
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
        private int XChaToWcs(int xChaId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return 9999;
            }
            if (AgvInfoDic.Exists(a => a.HangChaId.Equals(xChaId)))
            {
                return AgvInfoDic.Find(a => a.HangChaId.Equals(xChaId)).WcsId;
            }
            return 9999;
        }


        private string WcsToXCha(int wcsId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return "9999";
            }
            if (AgvInfoDic.Exists(a => a.HangChaId.Equals(wcsId)))
            {
                return AgvInfoDic.Find(a => a.HangChaId.Equals(wcsId)).HangChaId.ToString();
            }
            return "9999";
        }
      
       
    }
}
