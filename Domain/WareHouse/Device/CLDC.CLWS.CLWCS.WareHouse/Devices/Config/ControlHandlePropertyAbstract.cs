using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml.Serialization;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using MaterialDesignThemes.Wpf;
using MenuItem = CLDC.Infrastructrue.UserCtrl.Model.MenuItem;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config
{
    public abstract class ControlHandlePropertyAbstract<TProtocolTranslation, TCommunication> : ControlHandleBasicProperty, IProperty, ICreateMenuItem
        where TProtocolTranslation : IProperty
        where TCommunication : IProperty
    {
        [XmlElement("ProtocolTranslation")]
        public TProtocolTranslation ProtocolTranslation { get; set; }

        [XmlElement("Communication")]

        public TCommunication Communication { get; set; }

        public abstract UserControl CreateView();

        public ObservableCollection<MenuItem> CreateMenuItem()
        {
            ObservableCollection<MenuItem> menuItemList = new ObservableCollection<MenuItem>();

            MenuItem controlMenu = new MenuItem("控制配置", PackIconKind.AlphaEBox);

            MenuItem controlBasicMenu = new MenuItem("通用配置", CreateView());

            MenuItem controlPocotolMenu = new MenuItem("协议配置", ProtocolTranslation.CreateView());

            MenuItem controlCommMen = new MenuItem("通讯配置", Communication.CreateView());

            controlMenu.Add(controlBasicMenu);
            controlMenu.Add(controlPocotolMenu);
            controlMenu.Add(controlCommMen);

            menuItemList.Add(controlMenu);

            return menuItemList;
        }
    }
}
