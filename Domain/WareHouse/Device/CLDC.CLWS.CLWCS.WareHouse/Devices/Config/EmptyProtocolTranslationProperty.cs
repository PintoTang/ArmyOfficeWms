using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    [XmlRoot("ProtocolTranslation", Namespace = "", IsNullable = false)]
    [Serializable]
    public class EmptyProtocolTranslationProperty : ProtocolTranslationPropertyAbstract<EmptyProtocolTranslationConfigProperty>, IProperty
    {
        public UserControl CreateView()
        {
            return new EmptyProtocolTranslationView(this);
        }
    }
}
