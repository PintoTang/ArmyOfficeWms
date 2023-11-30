using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.TransportDevice.Common
{
    /// <summary>
    /// 具有状态切换的设备的业务处理虚类
    /// </summary>
    public abstract class OrderDeviceBusinessAbstract : DeviceBusinessBaseAbstract
    {

        public abstract bool Accessible(Addr startAddr, Addr destAddr);

        /// <summary>
        /// 开始地址的列表
        /// </summary>
        public List<Addr> StartAddressLst = new List<Addr>();

        /// <summary>
        /// 目的地址的列表
        /// </summary>
        public List<Addr> DestAddressLst = new List<Addr>();
        public abstract OperateResult IsCanChangeAbleState(UseStateMode destState);

        public abstract OperateResult SetAbleState(UseStateMode destState);

        public abstract OperateResult SetControlState(ControlStateMode destState);

        public  override OperateResult ParticularConfig()
        {
            StartAddressLst.Clear();
            DestAddressLst.Clear();

            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "BusinessHandle";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);

                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", DeviceId);
                    result.IsSuccess = false;
                    return result;
                }

                string businessConfigXml = xmlNode.OuterXml;

                TransportBusinessHandleProperty businessHandleProperty = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        businessHandleProperty = (TransportBusinessHandleProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(TransportBusinessHandleProperty));
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (businessHandleProperty == null)
                {
                    return result;
                }

                string startAddress = string.IsNullOrEmpty(businessHandleProperty.Config.StartAddress)?"":businessHandleProperty.Config.StartAddress.Trim();
                string destAddress = string.IsNullOrEmpty(businessHandleProperty.Config.DestAddress) ? "" : businessHandleProperty.Config.DestAddress.Trim();

                string[] startAddressNames = startAddress.Split('|');
                foreach (string addressName in startAddressNames)
                {
                    if (string.IsNullOrEmpty(addressName.Trim()))
                    {
                        continue;
                    }
                    StartAddressLst.Add(new Addr(addressName.Trim()));
                }

                string[] destAddressNames = destAddress.Split('|');
                foreach (string addressName in destAddressNames)
                {
                    if (string.IsNullOrEmpty(addressName.Trim()))
                    {
                        continue;
                    }
                    DestAddressLst.Add(new Addr(addressName.Trim()));
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transMsg"></param>
        /// <returns></returns>
        public abstract OperateResult DoJob(TransportMessage transMsg);

        public abstract bool IsNeedHanldeOrderValue(int value);


    }
}
