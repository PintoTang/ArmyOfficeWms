using System;
using System.IO;
using System.Reflection;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Identify
{
    /// <summary>
    /// 信息识别控制类
    /// </summary>
    public abstract class IdentifyDeviceControlAbstract<BarcodeT> : DeviceControlBaseAbstract
    {
        public abstract OperateResult<BarcodeT> GetIdentifyMessage(params object[] para);

        public abstract void GetIdentityMessageAsync(params object[] para);

        public BarcodeCallback<BarcodeT> OnReceiveBarcodeEvent { get; set; }


        public IIdentifyDeviceCom<BarcodeT> communicate;
        public IIdentifyDeviceCom<BarcodeT> Communicate
        {
            get { return communicate; }
        }
        protected virtual void Communicate_OnReceiveBarcode(DeviceName deviceName, BarcodeT barcode, params object[] para)
        {
            if (OnReceiveBarcodeEvent != null)
            {
                OnReceiveBarcodeEvent(deviceName, barcode, para);
            }
        }

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
                    result.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    result.IsSuccess = false;
                    return result;
                }

                string communicationConfigXml = xmlNode.OuterXml;
                OpcCommunicationProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (OpcCommunicationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(OpcCommunicationProperty));
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

                communicate = (IIdentifyDeviceCom<BarcodeT>)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                if (communicate == null)
                {
                    return OperateResult.CreateFailedResult();
                }
                OperateResult cmmInitResult = communicate.Initialize(DeviceId, DeviceName);
                if (!cmmInitResult.IsSuccess)
                {
                    return cmmInitResult;
                }
                communicate.Name = communicationProperty.Name;
                communicate.OnReceiveBarcode += Communicate_OnReceiveBarcode;
                communicate.MessageReportEvent += MessageReportEvent;
                result = OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        private void MessageReportEvent(string message, EnumLogLevel messageLevel)
        {
            LogMessage(message, messageLevel, true);
        }
    }
}
