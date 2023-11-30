using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Interface;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    [XmlRoot("Communication", Namespace = "", IsNullable = false)]
    [Serializable]
    public class OpcCommunicationProperty : InstanceProperty, IProperty
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }


        [XmlElement("Config")]
        public OpcCommunicationConfig Config { get; set; }

        public UserControl CreateView()
        {
            return new OpcCommunicationView(this);
        }
    }
    public class OpcCommunicationConfig
    {
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("Calculate")]
        public OpcCalculateProperty Calculate { get; set; }

        [XmlElement("Connection")]
        public string Connection { get; set; }

        [XmlElement("DataBlockItems")]
        public OpcDatablockProperty DataBlockItems { get; set; }
    }

    public class OpcCalculateProperty : InstanceProperty
    {

    }

    public class OpcDatablockProperty
    {
        [XmlAttribute("Template")]

        public int Template { get; set; }

        [XmlElement("Item")]
        public List<DatablockItem> Item { get; set; }

    }

    public class DatablockItem
    {

        [XmlAttribute("OpcId")]
        public int OpcId { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("DataBlockName")]
        public string DataBlockName { get; set; }
        [XmlAttribute("realDataBlockAddr")]
        public string RealDataBlockAddr { get; set; }
    }
}
