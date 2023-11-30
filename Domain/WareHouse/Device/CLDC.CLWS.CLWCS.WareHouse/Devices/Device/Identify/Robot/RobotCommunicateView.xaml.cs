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
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Robot
{
    /// <summary>
    /// RobotCommunicateView.xaml 的交互逻辑
    /// </summary>
    public partial class RobotCommunicateView : UserControl
    {
        public RobotCommunicateView(RobotCommunicateProperty viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            SocketClientView.DataContext = new SocketClientPropertyViewModel(viewModel.Config);
        }
    }
}
