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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Config;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View
{
    /// <summary>
    /// EmptyBusinessPropertyView.xaml 的交互逻辑
    /// </summary>
    public partial class EmptyBusinessPropertyView : UserControl
    {
        public EmptyBusinessPropertyView(BusinessHandleBasicPropertyAbstract viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
