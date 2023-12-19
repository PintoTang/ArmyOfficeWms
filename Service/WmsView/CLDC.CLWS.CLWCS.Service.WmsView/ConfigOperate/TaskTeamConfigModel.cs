using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    /// <summary>
    /// TaskTeamConfig的配置类
    /// </summary>
    [XmlRoot("TaskTeamConfig", Namespace = "", IsNullable = false)]
    public class TaskTeamConfigModel
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("TaskTeam")]
        public List<TaskTeam> TaskTeamList { get; set; }

    }

    [XmlRoot("TaskTeam", Namespace = "", IsNullable = false)]
    [Serializable]
    public class TaskTeam
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

    }
}
