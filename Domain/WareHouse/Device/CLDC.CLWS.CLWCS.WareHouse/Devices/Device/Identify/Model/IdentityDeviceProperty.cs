using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model
{
    /// <summary>
    /// 条码枪的配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class IdentityDeviceProperty : DevicePropertyAbstract<CommonConfig, EmptyBusinessHandleProperty, IdentityDeviceControlHandleProperty>
    {
    }
    [Serializable]
    public class IdentityDeviceControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, OpcCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new EmptyControlPropertyView(this);
        }
    }
}
