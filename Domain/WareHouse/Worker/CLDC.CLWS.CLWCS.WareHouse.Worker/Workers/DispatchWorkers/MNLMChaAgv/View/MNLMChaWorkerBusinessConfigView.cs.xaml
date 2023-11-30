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
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.MNLMChaAgv.View
{
    /// <summary>
    /// HangChaWorkerBusinessConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class MNLMChaWorkerBusinessConfigView : UserControl
    {
        public MNLMChaWorkerBusinessConfigView(MNLMChaWorkerBusinessHandleProperty model)
        {
            InitializeComponent();
            MNLMChaWorkerBusinessHandleViewModel viewModel = new MNLMChaWorkerBusinessHandleViewModel(model);
            this.DataContext = viewModel;
        }
    }
}
