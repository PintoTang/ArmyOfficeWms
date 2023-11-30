using System;
using System.IO;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Infrastructrue.Xml;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{

    public class FourWayVehicleResponse
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 返回结果代码
        /// </summary>
        public int code { get; set; }

        public static explicit operator FourWayVehicleResponse(string json)
        {
            return JsonConvert.DeserializeObject<FourWayVehicleResponse>(json);
        }

    }

    public class FourWayVehicleControl : OrderDeviceControlAbstract
    {
        public string Http { get; set; }

        public IWebNetInvoke WebNetInvoke;
      
     
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





        public override OperateResult DoJob(TransportMessage transMsg)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                SendAddTaskCmd task = new SendAddTaskCmd();
                task.businessType = "01";
                task.deviceName = transMsg.TransportDevice.Name;
                task.ext1 = string.Empty;
                task.ext2 = string.Empty;
                task.ext3 = string.Empty;
                task.extParam = string.Empty;
                task.locationFrom =FourWayVehicleCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(transMsg.StartAddr.FullName);
                task.locationTo = FourWayVehicleCarWalkingPathManage.Instance.GetPostionNumAddrByConvertAddrData(transMsg.DestAddr.FullName);
                task.priority = transMsg.TransportOrder.OrderPriority.ToString();
                task.sysName = "WCS控制系统";
                task.taskNo = transMsg.TransportOrder.OrderId.ToString();
                string cmd = JsonConvert.SerializeObject(task);
                OperateResult<string> response = WebNetInvoke.ServiceRequest<SyncResReMsgForFourWayVehicle>(Http, "addTask", cmd);
                if (!response.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(response.Message, 1);
                }
                string message = response.Content.Trim();
                try
                {
                    FourWayVehicleResponse FourWayVehicleRespone = (FourWayVehicleResponse)message;
                    if (FourWayVehicleRespone.code.Equals(0))
                    {
                        return OperateResult.CreateSuccessResult();
                    }
                    return OperateResult.CreateFailedResult(string.Format("杭叉系统返回失败，失败原因：\r\n {0}", FourWayVehicleRespone.msg), 1);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("杭叉返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
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
            WebNetInvoke = new WebApiInvoke();
            WebNetInvoke.LogDisplayName = "杭叉AGV接口调用";
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
    }
}
