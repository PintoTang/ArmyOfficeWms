using System;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    /// <summary>
    /// 设备的公共属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public class DeviceBasicProperty : InstanceProperty
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        [XmlAttribute("DeviceId")]
        public int DeviceId { get; set; }

        /// <summary>
        /// 对外编号
        /// </summary>
        [XmlAttribute("ExposedUnId")]
        public string ExposedUnId { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [XmlAttribute("IsShowUi")]
        public bool IsShowUi { get; set; }

        /// <summary>
        /// 设备内部标识
        /// </summary>
        [XmlAttribute("DeviceName")]
        public string DeviceName { get; set; }
        /// <summary>
        /// 设备当前地址
        /// </summary>
        [XmlAttribute("CurAddress")]
        public string CurAddress { get; set; }

        /// <summary>
        /// 工作容量大小
        /// </summary>
        [XmlAttribute("WorkSize")]
        public int WorkSize { get; set; }


        public UserControl CreateView()
        {
            UserControl view = new DeviceBasicPropertyView();
            view.DataContext = new DeviceBasicPropertyViewModel(this);
            return view;
        }


    }

}
