using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.WCS.SystemConfigPckg.Model
{
    [XmlRoot("SystemConfig", Namespace = "", IsNullable = false)]
    public class SystemConfigMode
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlElement("SysName")]
        public ConfigItem<string> SysName { get; set; }

        [XmlElement("SysNo")]
        public ConfigItem<string> SysNo { get; set; }

        [XmlElement("WhCode")]
        public ConfigItem<string> WhCode { get; set; }
        [XmlElement("IsTrueWebService")]
        public ConfigItem<bool> IsTrueWebService { get; set; }
        [XmlElement("IsUseCellPile")]
        public ConfigItem<bool> IsUseCellPile { get; set; }
        
        [XmlElement("OpcMode")]
        public ConfigItem<OpcModeEnum> OpcMode { get; set; }
        [XmlElement("AtsDatabaseConn")]
        public ConfigItem<string> AtsDatabaseConn { get; set; }
        [XmlElement("AtsDataBaseType")]
        public ConfigItem<DatabaseTypeEnum> AtsDataBaseType { get; set; }

        [XmlElement("Department")]
        public ConfigItem<DepartmentEnum> Department { get; set; }

        [XmlElement("WcsDataBaseType")]
        public ConfigItem<DatabaseTypeEnum> WcsDataBaseType { get; set; }
        [XmlElement("WcsDatabaseConn")]
        public ConfigItem<string> WcsDatabaseConn { get; set; }
        [XmlElement("MaxByteLength")]
        public ConfigItem<int> MaxByteLength { get; set; }
        [XmlElement("LogRecordLevel")]
        public ConfigItem<EnumLogLevel> LogRecordLevel { get; set; }

        [XmlElement("MaxLogCount")]
        public ConfigItem<int> MaxLogCount { get; set; }
        [XmlElement("LogSavedDays")]
        public ConfigItem<int> LogSavedDays { get; set; }

        [XmlElement("IsRecordEventLog")]
        public ConfigItem<bool> IsRecordEventLog { get; set; }

        [XmlElement("CopyRight")]
        public ConfigItem<string> CopyRight { get; set; }

        [XmlElement("Version")]
        public ConfigItem<string> Version { get; set; }

        [XmlElement("RfidPower")]
        public ConfigItem<string> RfidPower { get; set; }
        [XmlElement("Interval")]
        public ConfigItem<string> Interval { get; set; }
        [XmlElement("ModbudHub")]
        public ConfigItem<string> RemoteIp { get; set; }

    }
}
