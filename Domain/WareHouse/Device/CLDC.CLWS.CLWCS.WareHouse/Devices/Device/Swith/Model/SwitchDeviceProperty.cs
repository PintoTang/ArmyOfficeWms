using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.Model
{
    /// <summary>
    /// 开关设备属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class SwitchDeviceProperty : DevicePropertyAbstract<CommonConfig, EmptyBusinessHandleProperty, SwitchDeviceControlHandleProperty>
    {

    }
    [Serializable]
    public class SwitchDeviceControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }



}
