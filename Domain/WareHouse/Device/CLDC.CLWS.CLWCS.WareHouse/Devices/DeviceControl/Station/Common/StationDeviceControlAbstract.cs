using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;
using CLDC.Infrastructrue.Xml;


namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Station.Common
{
    public abstract class StationDeviceControlAbstract : DeviceControlBaseAbstract, IChangeMode
    {

        public IPlcDeviceCom Communicate { get; set; }
        public DeviceModeEnum CurMode { get; set; }

        public abstract OperateResult IsCanChangeMode(DeviceModeEnum destMode);

        /// <summary>
        /// 注册PLC监控的值变化
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        /// <returns></returns>
        public  OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum,
            CallbackContainOpcValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
        }

        /// <summary>
        /// 注册PLC监控的值变化
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        /// <returns></returns>
        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum,
            CallbackContainOpcBoolValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
        }

        public abstract OperateResult ChangeMode(DeviceModeEnum destMode);


        public abstract  bool CheckMode(DeviceModeEnum destMode);

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
                string stationPropertyXml = xmlNode.OuterXml;
                OpcCommunicationProperty communicateProperty = null;
                using (StringReader sr = new StringReader(stationPropertyXml))
                {
                    try
                    {
                        communicateProperty = (OpcCommunicationProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(OpcCommunicationProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess || communicateProperty == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：OpcCommunicationProperty", stationPropertyXml));
                }

                Communicate = (IPlcDeviceCom)Assembly.Load(communicateProperty.NameSpace)
                                .CreateInstance(communicateProperty.NameSpace + "." + communicateProperty.ClassName);

                if (Communicate == null)
                {
                    return OperateResult.CreateFailedResult();
                }
                OperateResult cmmInitResult = Communicate.Initialize(DeviceId, DeviceName);
                if (!cmmInitResult.IsSuccess)
                {
                    return cmmInitResult;
                }
                Communicate.Name = communicateProperty.Name;
                result = OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public abstract OperateResult<SizeProperties> GetGoodsProperties();

    }
}
