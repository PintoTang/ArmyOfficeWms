using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.StackRfid;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 科陆拆码垛射频门业务类
    /// </summary>
    public sealed class ClStackRfidDoorBusiness : StackRfidBusinessAbstract
    {
        public  override OperateResult ParticularInitlize()
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
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                string controlHandle = xmlNode.OuterXml;
                PalletizerBusinessHandleProperty businessHandleProperty = null;
                using (StringReader sr = new StringReader(controlHandle))
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

                this.Capacity = businessHandleProperty.Config.Capacity;
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

        public override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
