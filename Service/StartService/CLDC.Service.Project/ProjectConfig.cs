using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLDC.Service.Project
{
    [XmlRoot("ProjectConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class ProjectConfig
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlElement("ProjectMain")]
        public ProjectMain Project { get; set; }

    }

    [XmlRoot("ProjectConfig", Namespace = "", IsNullable = false)]
    [Serializable]
    public class ProjectMain
    {
        /// <summary>
        /// 命名控件
        /// </summary>
        [XmlAttribute("NameSpace")]
        public string NameSpace { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        [XmlAttribute("ClassName")]
        public string ClassName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }
    }

}
