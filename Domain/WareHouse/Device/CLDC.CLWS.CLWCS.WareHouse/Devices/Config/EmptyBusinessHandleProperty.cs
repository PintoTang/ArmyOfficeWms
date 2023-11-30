using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    [Serializable]
    public class EmptyBusinessHandleProperty : BusinessHandleBasicPropertyAbstract
    {

        [XmlElement("Config")]
        public CommonConfig Config { get; set; }

        public override UserControl CreateView()
        {
            return new EmptyBusinessPropertyView(this);
        }
    }
}
