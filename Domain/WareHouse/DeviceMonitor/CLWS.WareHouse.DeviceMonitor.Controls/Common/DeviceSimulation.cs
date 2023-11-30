using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Device;

namespace WHSE.Monitor.Framework.UserControls
{
   public class DeviceSimulation:UserControl
   {
       public int DeviceId { get; set; }

        public static readonly DependencyProperty DeviceNameProperty = DependencyProperty.Register(
            "DeviceName", typeof(string), typeof(DeviceSimulation), new PropertyMetadata(default(string)));

        public string DeviceName
        {
            get { return (string)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }


       
    }
}
