using System;
using System.IO;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.Common
{
    public abstract class PalletizerBusinessAbstract : DeviceBusinessBaseAbstract
    {

        /// <summary>
        /// 是否需要处理每一个叠盘完成
        /// </summary>
        public bool IsNeedHandleEachFinish { get; set; }
        /// <summary>
        /// 是否需要Wcs系统自行判断是否满跺
        /// </summary>
        public bool IsNeedVerifyCapacity { get; set; }

        /// <summary>
        /// 碟盘机的容器
        /// </summary>
        public int Capacity { get; set; }

        public virtual bool GetIsNeedHandleEachFinish()
        {
            return IsNeedHandleEachFinish;
        }

        public virtual bool GetIsNeedVerifyCapacity()
        {
            return IsNeedVerifyCapacity;
        }

        internal abstract bool IsNeedHandleNeedSignValue(bool newValue);

        public override OperateResult ParticularConfig()
        {
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

                PalletizerBusinessHandleProperty businessHandleProperty = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        businessHandleProperty =
                            (PalletizerBusinessHandleProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr,
                                    typeof(PalletizerBusinessHandleProperty));
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

                Capacity = businessHandleProperty.Config.Capacity;
                IsNeedVerifyCapacity = businessHandleProperty.Config.IsNeedVerifyCapacity;
                IsNeedHandleEachFinish = businessHandleProperty.Config.IsNeedHandleEachFinish;

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }
    }
}
