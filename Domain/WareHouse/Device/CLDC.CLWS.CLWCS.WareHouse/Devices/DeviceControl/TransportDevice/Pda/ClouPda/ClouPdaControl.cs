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
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.View;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Pda.CmdModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClouPdaControl : OrderDeviceControlAbstract
    {
        public string Http { get; set; }

        internal IWebNetInvoke WebNetInvoke;
        public  override OperateResult ParticularInitConfig()
        {
            //获取Http 的值
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication/Config";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                string communicationConfigXml = xmlNode.OuterXml;
                WebClientCommunicationConfig communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (WebClientCommunicationConfig)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebClientCommunicationConfig));
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

                Http = communicationProperty.Http.Trim();
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
           WebNetInvoke.LogDisplayName = "科陆Pda接口调用";
          
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
               SendInstructCmd task = new SendInstructCmd();
               task.BARCODE = transMsg.PileNo;
               task.DST_ADDR = transMsg.DestAddr.FullName;
               task.INSTRUCTION_CODE = transMsg.TransportOrderId;
               task.PRI = transMsg.TransportOrder.OrderPriority;
               task.SRC_ADDR = transMsg.StartAddr.FullName;
               string cmd = task.ToString();
               OperateResult<string> response = WebNetInvoke.ServiceRequest<SyncResReErr>(Http, "SendInstruct", cmd);
               if (!response.IsSuccess)
               {
                   return OperateResult.CreateFailedResult(response.Message, 1);
               }
               string message = response.Content.Trim();
               try
               {
                   SyncResReErr clouPdaRespone = (SyncResReErr)message;
                   if (clouPdaRespone.IsSuccess)
                   {
                       return OperateResult.CreateSuccessResult();
                   }
                   return OperateResult.CreateFailedResult(string.Format("科陆Pda系统返回失败，失败原因：\r\n {0}", clouPdaRespone.ERR_MSG), 1);
               }
               catch (Exception ex)
               {
                   result.IsSuccess = false;
                   result.Message = string.Format("科陆Pda返回的信息：{0} 格式转换出错，信息：{1} ", message, ex.Message);
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
