using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.View
{
    /// <summary>
    /// SendScanTaskCmdView.xaml 的交互逻辑
    /// </summary>
    public partial class SendScanTaskCmdView : UserControl
    {
        public SendScanTaskCmdView(SendScanTaskCmd dataModel)
        {
            InitializeComponent();
            this.DataContext = new SendScanTaskCmdViewModel(dataModel);
        }
    }
}
