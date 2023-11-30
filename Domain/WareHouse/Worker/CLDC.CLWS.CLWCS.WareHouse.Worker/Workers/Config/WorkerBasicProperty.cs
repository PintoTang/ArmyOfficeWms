using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config
{
    /// <summary>
    /// 协助者的属性配置
    /// </summary>
    [XmlRoot("Coordination", Namespace = "", IsNullable = false)]
    [Serializable]
    public class WorkerBasicProperty : InstanceProperty
    {
        /// <summary>
        /// 类型
        /// </summary>
        [XmlAttribute("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [XmlAttribute("Id")]
        public int WorkerId { get; set; }

        /// <summary>
        /// 标识名称
        /// </summary>
        [XmlAttribute("WorkerName")]
        public string WorkerName { get; set; }

        /// <summary>
        /// 工作容量
        /// </summary>
        [XmlAttribute("WorkSize")]
        public int WorkSize { get; set; }


        public UserControl CreateView()
        {
            UserControl view = new WorkerBasicPropertyView();
            view.DataContext = this;
            return view;
        }


    }
}
