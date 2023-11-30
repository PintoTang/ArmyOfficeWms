using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.ViewModel;


namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.View
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
