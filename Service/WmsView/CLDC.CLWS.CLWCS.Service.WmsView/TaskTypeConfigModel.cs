using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    /// <summary>
    /// TaskType的配置类
    /// </summary>
    [XmlRoot("TaskTypeConfig", Namespace = "", IsNullable = false)]
    public class TaskTypeConfigModel
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("TaskButton")]
        public List<TaskButton> TaskButtonList { get; set; }

    }

    [XmlRoot("TaskButton", Namespace = "", IsNullable = false)]
    [Serializable]
    public class TaskButton
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Row")]
        public int Row { get; set; }

        [XmlAttribute("Column")]
        public int Column { get; set; }

        [XmlAttribute("Show")]
        public bool Show { get; set; }

    }
}
