using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    public class ControlHandleBasicProperty : InstanceProperty
    {

        [XmlAttribute("Type")]
        public string Type { get; set; }
    }
}
