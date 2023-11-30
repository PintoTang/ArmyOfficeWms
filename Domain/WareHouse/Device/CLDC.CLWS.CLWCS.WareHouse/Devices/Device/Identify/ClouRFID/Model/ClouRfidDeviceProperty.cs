using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Config;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ClouRFID.Model
{

    /// <summary>
    /// 科陆RFID扫描设备的配置属性
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public sealed class ClouRfidDeviceProperty : DevicePropertyAbstract<CommonConfig, IdentifyBusinessHandleProperty, ClouRfidDeviceControlHandleProperty>
    {
    }

    [Serializable]
    [XmlRoot("ControlHandle", Namespace = "", IsNullable = false)]
    public class ClouRfidDeviceControlHandleProperty : ControlHandlePropertyAbstract<ClouRfidDeviceProtocolTranslationProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

    [Serializable]
    public class ClouRfidDeviceProtocolTranslationProperty :
        ProtocolTranslationPropertyAbstract<ClouRfidDeviceProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new UserControl();
        }
    }
    [Serializable]
    public class ClouRfidDeviceProtocolTranslationConfigProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }
    }

}

