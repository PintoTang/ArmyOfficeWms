using System;
using System.IO;
using System.Reflection;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Swith.Common
{
    public abstract class SwitchDeviceControlAbstract : DeviceControlBaseAbstract
    {
        public IPlcDeviceCom Communicate { get; set; }
        /// <summary>
        /// 注册PLC监控的值变化
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        /// <returns></returns>
        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum,
            CallbackContainOpcValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
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
                Communicate = (IPlcDeviceCom)Assembly.Load(communicationProperty.NameSpace)
                                .CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

                if (Communicate == null)
                {
                    return OperateResult.CreateFailedResult();
                }
                OperateResult cmmInitResult = Communicate.Initialize(DeviceId, DeviceName);
                if (!cmmInitResult.IsSuccess)
                {
                    return cmmInitResult;
                }
                Communicate.Name = communicationProperty.Name;
                result = OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
