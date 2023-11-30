using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.WareHouse.DeviceMonitor.DeviceSimulationCommon
{
    public class DeviceSimulationBase:UserControl
    {

        public int DeviceId
        {
            get { return (int)GetValue(DeviceIdPropertyProperty); }
            set { SetValue(DeviceIdPropertyProperty, value); }
        }

        public static readonly DependencyProperty DeviceIdPropertyProperty = DependencyProperty.Register(
           "DeviceIdProperty", typeof(int), typeof(DeviceSimulationBase), new PropertyMetadata(default(int)));


        public static readonly DependencyProperty DeviceTypeProperty = DependencyProperty.Register(
            "DeviceType", typeof(string), typeof(DeviceSimulationBase), new PropertyMetadata(default(string)));

        public string DeviceType
        {
            get { return (string)GetValue(DeviceTypeProperty); }
            set { SetValue(DeviceTypeProperty, value); }
        }

        public static readonly DependencyProperty DeviceNameProperty = DependencyProperty.Register(
            "DeviceName", typeof(string), typeof(DeviceSimulationBase), new PropertyMetadata(default(string)));

        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }
    }
}
