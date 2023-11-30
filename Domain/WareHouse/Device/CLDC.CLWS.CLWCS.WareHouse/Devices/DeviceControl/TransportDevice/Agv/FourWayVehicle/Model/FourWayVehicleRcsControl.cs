using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class FourWayVehicleRcsControl : OrderDeviceControlAbstract
    {
        public string Http { get; set; }

        public IWebNetInvoke WebNetInvoke;
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
                        if (communicationProperty==null)
                        {
                            return  OperateResult.CreateFailedResult(string.Format("获取：{0} 通讯初始化参数失败",this.Name));
                        }
                        WebNetInvoke =(IWebNetInvoke)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                        if (WebNetInvoke == null)
                        {
                            return OperateResult.CreateFailedResult(string.Format("初始化：{0} 通讯接口：{1} 失败，命名空间：{2} 类名：{3}", this.Name, "IWebNetInvoke", communicationProperty.NameSpace, communicationProperty.ClassName));
                        }

                        WebNetInvoke.LogDisplayName = communicationProperty.Name;
                        WebNetInvoke.CommunicationMode = communicationProperty.CommunicationMode;
                        WebNetInvoke.TimeOut = communicationProperty.Config.TimeOut;
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
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetPropertyView()
        {
            WebApiView controlView = new WebApiView { Height = 250 };
            controlView.HttpUrl = this.Http;
            return controlView;
        }

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

        public override OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum, int startValue, int endValue,
            MonitorSpecifiedOpcValueCallback callbackAction)
        {
            throw new NotImplementedException();
        }

        public override OperateResult DoJob(TransportMessage transMsg)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                SendTaskCmd cmd = new SendTaskCmd()
                {
         
                    END_ADDR = transMsg.DestAddr.FullName,
                    PACKAGE_BARCODE = transMsg.PileNo,
                    PRI = transMsg.TransportOrder.OrderPriority,
                    START_ADDR = transMsg.StartAddr.FullName,
                    TASK_NO = transMsg.TransportOrder.OrderId.ToString(),
                    TASK_TYPE = (int)transMsg.TransportOrder.SourceTaskType
                };
                WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.SendTask.ToString(),
               cmd.ToJson());
                return SendCmd(apiCmd);
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

        public OperateResult SendCmd(WebApiInvokeCmd webApiCmd)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<string> invokeResult = WebNetInvoke.ServiceRequest<SyncResReMsg>(Http, webApiCmd.MethodName, webApiCmd.InvokeCmd);
                if (!invokeResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = invokeResult.Message;
                    return result;
                }
                SyncResReMsg responseResult = (SyncResReMsg)invokeResult.Content;
                if (!responseResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = invokeResult.Content;
                    return result;
                }
                result.IsSuccess = true;
                result.Message = invokeResult.Content;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public OperateResult RequestRCSPermit(RequestRCSPermitCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.RequestRCSPermit.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }

        public OperateResult ReportRCSPermitFinish(ReportRCSPermitFinishCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.ReportRCSPermitFinish.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        public OperateResult SendDeviceAction(SendDeviceActionCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, FourWayVehicleRcsApiEnum.SendDeviceAction.ToString(), cmd.ToJson());
            return SendCmd(apiCmd);
        }


    }
}
