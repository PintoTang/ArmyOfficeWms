using System.Windows;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View
{
    /// <summary>
    /// WorkerDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDetailView
    {
        private DeviceDetailViewModel viewModel;
        public DeviceDetailView(DeviceBaseAbstract device)
        {
            InitializeComponent();
            viewModel=new DeviceDetailViewModel(device);
            this.DataContext = viewModel;
            this.MouseLeftButtonDown += delegate { DragMove(); }; //拖拽
            CtrlTitle.SetOnCloseRoutedEventHandler(OnClose);
            InitViewSize();
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InitViewSize()
        {
            this.Width = SystemParameters.WorkArea.Width * 0.9;
            this.Height = SystemParameters.WorkArea.Height * 0.9;
        }


    }
}
