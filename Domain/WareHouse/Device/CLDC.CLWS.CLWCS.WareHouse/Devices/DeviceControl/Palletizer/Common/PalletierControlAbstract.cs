using System;
using System.IO;
using System.Reflection;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Palletizer.Common
{
    public abstract class PalletierControlAbstract : DeviceControlBaseAbstract
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
                    result.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    result.IsSuccess = false;
                    return result;
                }
                string stationPropertyXml = xmlNode.OuterXml;
                OpcCommunicationProperty communicateProperty=null;
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

                if (!result.IsSuccess || communicateProperty==null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：OpcCommunicationProperty", stationPropertyXml));
                }

                Communicate = (IPlcDeviceCom)Assembly.Load(communicateProperty.NameSpace)
                                .CreateInstance(communicateProperty.NameSpace+ "." + communicateProperty.ClassName);

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

        public OperateResult<int> GetPalletizerCount()
        {
            OperateResult<int> readResult = Communicate.ReadInt(DataBlockNameEnum.CountDataBlock);
            return readResult;
        }

        /// <summary>
        /// 获取当前完成碟盘机信息
        /// </summary>
        /// <returns></returns>
        public OperateResult<PalletizeContent> GetCurFinishContent()
        {
            OperateResult<string> readBarcode = Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            if (!readBarcode.IsSuccess)
            {
                return OperateResult.CreateFailedResult<PalletizeContent>("获取当前碟盘机的条码失败");
            }
            OperateResult<int> readCount = GetPalletizerCount();
            if (!readCount.IsSuccess)
            {
                return OperateResult.CreateFailedResult<PalletizeContent>("获取当前碟盘机的数量失败");
            }
            PalletizeContent curContent=new PalletizeContent(readCount.Content,readBarcode.Content);
            return OperateResult.CreateSuccessResult(curContent);
        }


        public OperateResult SendDePalletizeCmd(DePalletizeCmd cmd)
        {
            //fusha
            Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, cmd.ContainerType);
            Communicate.Write(DataBlockNameEnum.DePalletizeCountDataBlock, cmd.DePalletizeCount);
            return OperateResult.CreateSuccessResult();
        }


    }
}
