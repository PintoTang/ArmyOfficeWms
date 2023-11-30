using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.View
{
    /// <summary>
    /// SendStackTaskCmdView.xaml 的交互逻辑
    /// </summary>
    public partial class SendStackDispatchCmdView : UserControl
    {
        public SendStackDispatchCmdView(SendStackDispatchCmd dataModel)
        {
            InitializeComponent();
            this.DataContext = new SendStackDispatchCmdViewModel(dataModel);
        }
    }
}
