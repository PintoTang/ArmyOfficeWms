using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.Model;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;
using SyncResReMsg = CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model.SyncResReMsgForHangChaAgv;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public sealed class Eh2000UwbControl : OrderDeviceControlAbstract
    {

        public OperateResult<string> EinkApi(WebApiInvokeCmd cmd)
        {
            OperateResult<string> invokeResult = new OperateResult<string>();
            string http = Http + "/ModularEink/EinkAPI";
            Hashtable hearders = new Hashtable();
            hearders.Add("Content-Type", "application/x-www-form-urlencoded");
            OperateResult<string> reponseResult = WebNetInvoke.ServiceRequest<ResponseResult>(http, cmd.MethodName, cmd.InvokeCmd, hearders);
            if (!reponseResult.IsSuccess)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = reponseResult.Message;
                return invokeResult;
            }
            return reponseResult;
        }

        public string Http { get; set; }

        internal IWebNetInvoke WebNetInvoke;
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

        private IWmsWcsArchitecture _architecture;
        public override OperateResult ParticularInitlize()
        {
            //WebNetInvoke = new WebApiInvoke();
            //WebNetInvoke.LogDisplayName = "恒高Uwb接口调用";
            _architecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
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

        private string AddrChangeMsg(Addr sourceAddr)
        {
            OperateResult<string> getShowNameResult = _architecture.WcsToShowName(sourceAddr.FullName);
            if (!getShowNameResult.IsSuccess)
            {
                return sourceAddr.FullName;
            }
            return getShowNameResult.Content;
        }

        private string ChangeTaskMsg(TransportMessage transMsg)
        {
            string taskMsg;
            if (transMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.In))
            {
                taskMsg = string.Format("入库 \r\n从{0}到{1} 数量1跺 ", AddrChangeMsg(transMsg.StartAddr), AddrChangeMsg(transMsg.DestAddr));
            }
            else if (transMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.Out))
            {
                taskMsg = string.Format("出库 \r\n从{0}到{1} 数量1跺 ", AddrChangeMsg(transMsg.StartAddr), AddrChangeMsg(transMsg.DestAddr));
            }
            else if (transMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.PickOut))
            {
                taskMsg = string.Format("拣选 \r\n从{0}到{1} 数量{2}只 ", AddrChangeMsg(transMsg.StartAddr), AddrChangeMsg(transMsg.DestAddr), transMsg.TransportOrder.Qty);
            }
            else if (transMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.Move))
            {
                taskMsg = string.Format("移库 \r\n从{0}到{1} 数量1跺 ", AddrChangeMsg(transMsg.StartAddr), AddrChangeMsg(transMsg.DestAddr));
            }
            else if (transMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.InventoryOut) || transMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.InventoryIn))
            {
                taskMsg = string.Format("盘点 \r\n从{0}到{1} 数量1跺 ", AddrChangeMsg(transMsg.StartAddr), AddrChangeMsg(transMsg.DestAddr));
            }
            else
            {
                taskMsg = string.Format("未知类型\r\n从货物从{0}搬运到{1}数量1跺 ", AddrChangeMsg(transMsg.StartAddr), AddrChangeMsg(transMsg.DestAddr));
            }
            return taskMsg;
        }

        public override OperateResult DoJob(TransportMessage transMsg)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                string taskMsg = ChangeTaskMsg(transMsg);
                LogMessage(taskMsg, EnumLogLevel.Info, true);
                PublishEinkCmd publishEinkCmd = new PublishEinkCmd(transMsg.TransportDevice.ExposedUnId, taskMsg, EinkNotifyTypeEnum.Notice, EinkVoiceTypeEnum.HasVoice, EinkShakeTypeEnum.HasShake);
                WebApiInvokeCmd invokeCmd = new WebApiInvokeCmd(Http, Eh2000UwbApiEnum.publishEink.ToString(), publishEinkCmd.ToString());
                OperateResult<string> responseResult = EinkApi(invokeCmd);
                if (!responseResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(responseResult.Message, 1);
                }
                try
                {
                    ResponseResult response = responseResult.Content.ToObject<ResponseResult>();
                    if (response.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult();
                    }
                    result.IsSuccess = false;
                    result.Message = response.message;
                    return result;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = true;
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
    }
}
