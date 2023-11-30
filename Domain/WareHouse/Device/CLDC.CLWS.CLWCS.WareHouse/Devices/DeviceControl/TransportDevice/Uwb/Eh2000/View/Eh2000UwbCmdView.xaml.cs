using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.View
{
    /// <summary>
    /// Eh2000UwbCmdView.xaml 的交互逻辑
    /// </summary>
    public partial class Eh2000UwbCmdView : UserControl
    {
        public Eh2000UwbCmdView(string requestValue)
        {
            InitializeComponent();
            DataContext =new Eh2000UwbCmdViewModel(requestValue);
        }
    }
}
