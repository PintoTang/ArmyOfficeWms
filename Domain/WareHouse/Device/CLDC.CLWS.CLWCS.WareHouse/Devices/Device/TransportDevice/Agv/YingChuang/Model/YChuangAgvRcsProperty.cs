using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.YingChuang.Model
{
    /// <summary>
    /// 杭叉AGV的属性配置
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class YChuangAgvRcsProperty : DevicePropertyAbstract<CommonConfig, TransportBusinessHandleProperty, YChuangAgvRcsControlHandleProperty>
    {

    }



    [Serializable]
    public class YChuangAgvRcsControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

}
