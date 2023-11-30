using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Infrastructrue.UserCtrl.Model;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config
{
    [XmlRoot("Configuration", Namespace = "", IsNullable = false)]
    [Serializable]
    public abstract class WorkerPropertyAbstract<TConfig, TBusinessHandle> : WorkerBasicProperty, ICreateMenuItem
        where TConfig : IProperty
        where TBusinessHandle : BusinessHandleBasicPropertyAbstract
    {
        [XmlElement("Config")]
        public TConfig Config { get; set; }

        [XmlElement("BusinessHandle")]
        public TBusinessHandle BusinessHandle { get; set; }

        public ObservableCollection<MenuItem> CreateMenuItem()
        {
            ObservableCollection<MenuItem> menuItemList = new ObservableCollection<MenuItem>();

            MenuItem homeMenu = new MenuItem("组件配置", PackIconKind.Home,CreateView());

            MenuItem workerBasicMenu = new MenuItem("通用配置", CreateView());

            MenuItem workerConfigMenu = new MenuItem("特别配置", Config.CreateView());

            homeMenu.Add(workerBasicMenu);
            homeMenu.Add(workerConfigMenu);

            MenuItem businessMenu = new MenuItem("业务配置", PackIconKind.GamepadCircle);

            MenuItem businessBasicMenu = new MenuItem("通用配置", BusinessHandle.CreateView());

            businessMenu.Add(businessBasicMenu);

            homeMenu.Add(businessMenu);

            menuItemList.Add(homeMenu);

            return menuItemList;
        }
    }
}
