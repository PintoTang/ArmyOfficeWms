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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View
{
    /// <summary>
    /// TransportBusinessPropertyView.xaml 的交互逻辑
    /// </summary>
    public partial class TransportBusinessPropertyView : UserControl
    {
        public TransportBusinessPropertyView(TransportBusinessHandleProperty model)
        {
            InitializeComponent();
            TransportBusinessPropertyViewModel viewModel= new TransportBusinessPropertyViewModel(model);
            this.DataContext = viewModel;
        }
    }
}
