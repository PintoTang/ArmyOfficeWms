using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Infrastructrue.Xml;
using Newtonsoft.Json;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.MNLMCha;
using System.Reflection;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{

    public class MNLMChaResponse
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string MESSAGE { get; set; }
        /// <summary>
        /// 返回结果代码
        /// </summary>
        public int RESULT { get; set; }

        public static explicit operator MNLMChaResponse(string json)
        {
            return JsonConvert.DeserializeObject<MNLMChaResponse>(json);
        }

    }

    public class MNLMChaTaskMode
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TASK_TYPE { get; set; }
        /// <summary>
        /// 调用系统名称
        /// </summary>
        public string SYS_NAME { get; set; }
        /// <summary>
        /// 起始地址
        /// </summary>
        public string START_ADDR { get; set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        public string END_ADDR { get; set; }
        /// <summary>
        /// 任务优先级
        /// </summary>
        public int PRI { get; set; }

        /// <summary>
        /// 拓展字段
        /// </summary>
        public string EXT1 { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
        public string EXT2 { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
        public string EXT3 { get; set; }

    }
    public class MNLMChaAgvControl : OrderDeviceControlAbstract
    {
        public string Http { get; set; }

        internal IWebNetInvoke WebNetInvoke;


        public override bool IsLoad()
        {
            return false;
        }

        public override OperateResult ClearFaultCode()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override Package GetCurrentPackage()
        {
            return null;
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum, int startValue, int endValue, MonitorSpecifiedOpcValueCallback callbackAction)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 调用SendTask接口 下发任务
        /// </summary>
        /// <param name="transMsg"></param>
        /// <returns></returns>
        public override OperateResult DoJob(TransportMessage transMsg)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                MNLMChaTaskMode task = new MNLMChaTaskMode();
                task.TASK_NO = transMsg.TransportOrder.OrderId.ToString();
                task.TASK_TYPE = "1";
                task.SYS_NAME = "AMS";
                task.START_ADDR = MNLMChaAgvCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(transMsg.StartAddr.FullName);
                task.END_ADDR = MNLMChaAgvCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(transMsg.DestAddr.FullName);
                task.EXT1 = string.Empty;
                task.PRI = transMsg.TransportOrder.OrderPriority;
                task.EXT2 = string.Empty;
                task.EXT3 = string.Empty;

                string cmd = JsonConvert.SerializeObject(task);
                OperateResult<string> response = WebNetInvoke.ServiceRequest<SyncResReMsg>(Http, "SendTask", cmd);
                if (!response.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(response.Message, 1);
                }
                string message = response.Content.Trim();
                try
                {
                    MNLMChaResponse xChaRespone = (MNLMChaResponse)message;
                    if (xChaRespone.RESULT.Equals(1))
                    {
                        return OperateResult.CreateSuccessResult();
                    }
                    return OperateResult.CreateFailedResult(string.Format("木牛流马叉车系统返回失败，失败原因：\r\n {0}", xChaRespone.MESSAGE), 1);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("木牛流马叉车返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
                }
                return result;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult<int> GetFinishedOrder()
        {
            int order = 0;
            return OperateResult.CreateFailedResult(order, "无数据");
        }

        /// <summary>
        /// 更具固定的XML配置结构读取参数
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("Http"))
                {
                    Http = node.InnerText.Trim();
                    continue;
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitConfig()
        {
            //获取Http 的值
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                string communicationConfigXml = xmlNode.OuterXml;
                WebClientCommunicationProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (WebClientCommunicationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebClientCommunicationProperty));
                        if (communicationProperty == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("获取：{0} 通讯初始化参数失败", this.Name));
                        }
                        WebNetInvoke = (IWebNetInvoke)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                        if (WebNetInvoke == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("初始化：{0} 通讯接口：{1} 失败，命名空间：{2} 类名：{3}", this.Name, "IWebNetInvoke", communicationProperty.NameSpace, communicationProperty.ClassName));
                        }

                        WebNetInvoke.LogDisplayName = communicationProperty.Name;
                        WebNetInvoke.CommunicationMode = communicationProperty.CommunicationMode;
                        WebNetInvoke.TimeOut = communicationProperty.Config.TimeOut * 1000;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (communicationProperty == null)
                {
                    return result;
                }

                Http = communicationProperty.Config.Http.Trim();

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
                result = OperateResult.CreateFailedResult();
            }
            return result;
        }

        public override OperateResult ParticularInitlize()
        {
            //WebNetInvoke = new WebApiInvoke();

            //WebNetInvoke = new WebServicePostSoapInvoke();
            //WebNetInvoke.LogDisplayName = "木牛流马AGV接口调用";
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetPropertyView()
        {
            WebApiViewVertical controlView = new WebApiViewVertical { Height = 250 };
            controlView.HttpUrl = this.Http;
            return controlView;
        }
    }
}
