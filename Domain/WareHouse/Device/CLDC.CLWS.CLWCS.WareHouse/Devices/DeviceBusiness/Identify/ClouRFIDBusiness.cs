using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System.Xml;
using System.IO;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Config;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// clouRFID 扫描条码设备业务
    /// </summary>
    public class ClouRFIDBusiness : IdentifyDeviceBusiness
    {

    
        public override OperateResult ParticularInitlize()
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
                IdentifyBusinessHandleProperty businessHandleProperty = null;
                using (StringReader sr = new StringReader(controlHandle))
                {
                    try
                    {
                        businessHandleProperty =
                            (IdentifyBusinessHandleProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr,
                                    typeof(IdentifyBusinessHandleProperty));
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

                //this.Capacity = businessHandleProperty.Config.Capacity; TO DO
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

        public  override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
