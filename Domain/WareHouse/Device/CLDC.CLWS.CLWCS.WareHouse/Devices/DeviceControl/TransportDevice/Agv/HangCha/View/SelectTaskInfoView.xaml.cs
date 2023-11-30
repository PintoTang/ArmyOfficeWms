using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.View
{
    /// <summary>
    /// SelectTaskInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class SelectTaskInfoView : UserControl
    {
        public SelectTaskInfoView(SelectTaskInfoCmd dataModel)
        {
            InitializeComponent();
            this.DataContext = new SelectTaskInfoCmdViewModel(dataModel);
        }
    }
}
