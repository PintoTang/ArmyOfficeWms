using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.Model
{
    /// <summary>
    /// 四向车的属性配置
    /// </summary>
    [Serializable]
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    public class FourWayVehicleProperty : DevicePropertyAbstract<CommonConfig, TransportBusinessHandleProperty, FourWayVehicleControlHandleProperty>
    {

    }



    [Serializable]
    public class FourWayVehicleControlHandleProperty : ControlHandlePropertyAbstract<EmptyProtocolTranslationProperty, WebClientCommunicationProperty>
    {
        public override UserControl CreateView()
        {
            return new UserControl();
        }
    }

}
