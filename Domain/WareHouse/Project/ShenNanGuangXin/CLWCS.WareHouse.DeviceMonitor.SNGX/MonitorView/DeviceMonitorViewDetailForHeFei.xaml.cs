using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
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
using System.Windows.Threading;
using WHSE.Monitor.Framework.UserControls;

namespace CLWCS.WareHouse.DeviceMonitor.HeFei.MonitorView
{
    /// <summary>
    /// DeviceMonitorViewDetailForHeFei.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceMonitorViewDetailForHeFei : UserControl
    {
        private string _useCtrlId = "1楼监控";

        public string UseCtrlId
        {
            get { return _useCtrlId; }
            set { _useCtrlId = value; }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                base.OnRender(drawingContext);
            }, DispatcherPriority.Background);
        }

        public DeviceMonitorViewDetailForHeFei()
        {
            InitializeComponent();
            InitializeViewMode();
        }
        private void InitializeViewMode()
        {
            foreach (FrameworkElement control in MonitorMainGrid.Children)
            {
                bool isDeviceView = control is DeviceSimulation;
                if (isDeviceView)
                {
                    DeviceSimulation deviceView = control as DeviceSimulation;
                    int id = deviceView.DeviceId;
                    if (id != default(int))
                    {
                        WareHouseViewModelBase deviceViewModel = ViewModelManage.Instance.FindDeviceViewModel(id);
                        if (deviceViewModel != null)
                        {
                            deviceView.DataContext = deviceViewModel;
                        }
                    }
                }

            }
        }
    }
}
