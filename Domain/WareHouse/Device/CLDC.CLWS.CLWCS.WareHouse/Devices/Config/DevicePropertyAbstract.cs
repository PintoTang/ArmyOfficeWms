using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Infrastructrue.UserCtrl.Model;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    /// <summary>
    /// 设备的公共属性配置
    /// </summary>
    [XmlRoot("Device", Namespace = "", IsNullable = false)]
    [Serializable]
    public abstract class DevicePropertyAbstract<TConfig, TBusinessHandle, TControlHandle> : DeviceBasicProperty,
        ICreateMenuItem
        where TConfig : IProperty
        where TBusinessHandle : BusinessHandleBasicPropertyAbstract
        where TControlHandle : InstanceProperty, IProperty, ICreateMenuItem
    {
        [XmlElement("Config")]
        public TConfig Config { get; set; }

        [XmlElement("BusinessHandle")]
        public TBusinessHandle BusinessHandle { get; set; }

        [XmlElement("ControlHandle")]
        public TControlHandle ControlHandle { get; set; }

        public ObservableCollection<MenuItem> CreateMenuItem()
        {
            ObservableCollection<MenuItem> menuItemList = new ObservableCollection<MenuItem>();

            MenuItem homeMenu = new MenuItem("设备配置", PackIconKind.Home, CreateView());

            MenuItem deviceBasicMenu = new MenuItem("通用配置", CreateView());

            homeMenu.Add(deviceBasicMenu);

            MenuItem businessMenu = new MenuItem("业务配置", PackIconKind.GamepadCircle);

            MenuItem businessBasicMenu = new MenuItem("通用配置", BusinessHandle.CreateView());

            businessMenu.Add(businessBasicMenu);

            ObservableCollection<MenuItem> controlMenu = ControlHandle.CreateMenuItem();


            homeMenu.Add(businessMenu);
            homeMenu.AddRange(controlMenu);

            menuItemList.Add(homeMenu);

            return menuItemList;
        }
    }
}
