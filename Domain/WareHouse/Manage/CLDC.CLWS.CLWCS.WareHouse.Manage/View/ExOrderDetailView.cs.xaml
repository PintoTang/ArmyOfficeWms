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
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.View
{
    /// <summary>
    /// OrderDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class ExOrderDetailView : UserControl
    {
        public ExOrderDetailView(ExOrder dataModel)
        {
            InitializeComponent();
            ExOrderDetailViewModel viewModel = new ExOrderDetailViewModel(dataModel);
            DataContext = viewModel;
            InitilizeSize();
        }

        private void InitilizeSize()
        {
            this.Width = SystemParameters.WorkArea.Width * 0.7;
            this.Height = SystemParameters.WorkArea.Height * 0.7;
        }
    }
}
