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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.View
{
    /// <summary>
    /// SendNextAddrPermitView.xaml 的交互逻辑
    /// </summary>
    public partial class SendNextAddrPermitView : UserControl
    {
        public SendNextAddrPermitView(SendNextAddrPermitCmd dataModel)
        {
            InitializeComponent();
            this.DataContext = new SendNextAddrPermitCmdViewModel(dataModel);

        }
    }
}
