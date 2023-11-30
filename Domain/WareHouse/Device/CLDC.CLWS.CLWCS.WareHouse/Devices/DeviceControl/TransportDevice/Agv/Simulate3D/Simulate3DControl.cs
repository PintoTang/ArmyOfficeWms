using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class Simulate3DControl : OrderDeviceControlAbstract
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
            return OperateResult.CreateSuccessResult();
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

        /// <summary>
        /// MonitorDeviceStatus  (异常)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult DeviceStautsDataUpload(DeviceStautsDataUploadCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.DeviceStautsDataUpload.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// MonitorTrackInfo  (动作轨迹)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult GoodsTrackDataUpload(GoodsTrackDataUploadCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.GoodsTrackDataUpload.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// SNDL（仿真接口改名）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult RobotTrackDataUpload(GoodsTrackDataUploadCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.RobotTrackDataUpload.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }

        /// <summary>
        /// ClearGoods  (清空货物)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult ClearGoodsDataUpload(ClearGoodsCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.ClearGoodsDataUpload.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }



        /// <summary>
        /// MonitorAGVPara
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult AgvRunParamDataUpload(AgvRunParamDataUploadCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.AgvRunParamDataUpload.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
        /// <summary>
        /// MonitorDeviceAction
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OperateResult DeviceActionDataUpload(DeviceActionDataUploadCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.DeviceActionDataUpload.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }


        public OperateResult ReportDeviceTroubleStatus(DeviceFaultRecCmd cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(Http, Simulate3DIOCEnum.AddDeviceFaultRec.ToString(),
                cmd.ToJson());
            return SendCmd(apiCmd);
        }
    }
}
