using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.Model
{
    /// <summary>
    /// 杭叉AGV的属性配置
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class HangChaProperty : DevicePropertyAbstract<CommonConfig, TransportBusinessHandleProperty, HangChaControlHandleProperty>
    {

    }



    [Serializable]
    public class HangChaControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

}
