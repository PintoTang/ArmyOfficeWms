using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Roller;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 承载设备业务逻辑虚类
    /// </summary>
    public sealed class RollerDeviceControl : OrderDeviceControlAbstract
    {


        private RollerProtocolAnalysisAbstract _protocolAnalysis;


        public IPlcDeviceCom Communicate { get; set; }

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
            return null;
        }

        public override bool IsLoad()
        {
            OperateResult<int> readOrderResult = Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            if (readOrderResult.IsSuccess)
            {
                int order = readOrderResult.Content;
                if (order > 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private OperateResult InitProtocolAnalysis()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/ProtocolTranslation";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    result.IsSuccess = false;
                    return result;
                }

                string communicationConfigXml = xmlNode.OuterXml;
                EmptyProtocolTranslationProperty protocolTranslationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        protocolTranslationProperty =
                            (EmptyProtocolTranslationProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(EmptyProtocolTranslationProperty));
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (protocolTranslationProperty == null)
                {
                    return result;
                }

                _protocolAnalysis = (RollerProtocolAnalysisAbstract)Assembly.Load(protocolTranslationProperty.NameSpace)
                    .CreateInstance(protocolTranslationProperty.NameSpace + "." + protocolTranslationProperty.ClassName);

                if (_protocolAnalysis == null)
                {
                    return
                        OperateResult.CreateFailedResult(
                            string.Format("协议分析类反射失败,类名：{0} 命名空间：{1}", protocolTranslationProperty.ClassName, protocolTranslationProperty.NameSpace), 1);
                }

                OperateResult protocolResult = _protocolAnalysis.Initialize(DeviceId, DeviceName);
                if (!protocolResult.IsSuccess)
                {
                    return protocolResult;
                }
                result= OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }


        private OperateResult InitCommunicate()
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
                OpcCommunicationProperty communicationProperty=null;
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

                if (communicationProperty==null)
                {
                    return result;
                }

                Communicate = (IPlcDeviceCom)Assembly.Load(communicationProperty.NameSpace).CreateInstance(communicationProperty.NameSpace + "." + communicationProperty.ClassName);

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

        public override OperateResult ParticularInitConfig()
        {

           OperateResult initProtocol= InitProtocolAnalysis();
            if (!initProtocol.IsSuccess)
            {
                return initProtocol;
            }

            OperateResult initCommunicate= InitCommunicate();
            if (!initCommunicate.IsSuccess)
            {
                return initCommunicate;
            }
            return OperateResult.CreateSuccessResult();
        }

        public override Package GetCurrentPackage()
        {
            Package currentPackage = new Package();
            OperateResult<int> readOrderResult = Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            if (readOrderResult.IsSuccess)
            {
                currentPackage.PackageId = readOrderResult.Content;
            }
            OperateResult<string> readBarcodeResult = Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

            if (readBarcodeResult.IsSuccess)
            {
                currentPackage.Barcode = readBarcodeResult.Content;
            }
            return currentPackage;
        }

        /// <summary>
        /// 注册PLC监控的值变化
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        /// <returns></returns>
        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
        }

        public override OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            return Communicate.RegisterValueChange(dbBlockEnum, monitervaluechange);
        }


        public override OperateResult DoJob(TransportMessage transportMsg)
        {
            OperateResult notifyResult = new OperateResult();
            try
            {
                OperateResult<Dictionary<DataBlockNameEnum, object>> composeResult = _protocolAnalysis.ComposeCmd(transportMsg);
                if (!composeResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(composeResult.Message, 1);
                }
                OperateResult writeResult = Communicate.Write(composeResult.Content);
                if (!writeResult.IsSuccess)
                {
                    return writeResult;
                }
                notifyResult.IsSuccess = true;
                return notifyResult;
            }
            catch (Exception ex)
            {
                notifyResult.IsSuccess = false;
                notifyResult.Message = OperateResult.ConvertException(ex);
            }
            return notifyResult;
        }


        public override OperateResult SetAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeAbleState(UseStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            throw new NotImplementedException();
        }

        public override OperateResult<int> GetFinishedOrder()
        {
            throw new NotImplementedException();
        }

        public override OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum dbBlockEnum, int startValue, int endValue, MonitorSpecifiedOpcValueCallback callbackAction)
        {
            return Communicate.RegisterFromStartToEndStatus(dbBlockEnum, startValue, endValue, callbackAction);
        }

        public override OperateResult ClearFaultCode()
        {
            throw new NotImplementedException();
        }
    }

}
