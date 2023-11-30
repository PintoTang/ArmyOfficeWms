using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.YChuangRfidDoor.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{

    public sealed class YChuangRfidDoorControl : StackRfidControlAbstract
    {
        public override OperateResult ParticularInitConfig()
        {
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

                string communicationHandle = xmlNode.OuterXml;
                WebClientCommunicationProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationHandle))
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
        internal IWebNetInvoke WebNetInvoke;
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
            return new UserControl();
        }


        public override  OperateResult<string> SendCmd<TResponse>(WebApiInvokeCmd webApiCmd)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                OperateResult<string> invokeResult = WebNetInvoke.ServiceRequest<TResponse>(Http, webApiCmd.MethodName, webApiCmd.InvokeCmd);
                return invokeResult;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("下发命令：{0} 发生异常：{1}", webApiCmd.MethodName, OperateResult.ConvertException(ex)),EnumLogLevel.Error, false);
                result.Message = OperateResult.ConvertExMessage(ex);
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
