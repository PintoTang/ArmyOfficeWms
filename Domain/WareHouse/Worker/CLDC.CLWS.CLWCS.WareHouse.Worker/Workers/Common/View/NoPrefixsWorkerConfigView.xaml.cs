using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View
{
    /// <summary>
    /// IdentifyWorkerConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class NoPrefixsWorkerConfigView : UserControl
    {

        public NoPrefixsWorkerConfigView(NoPrefixsWorkerConfigProperty model)
        {
            InitializeComponent();
            NoPrefixsWorkerConfigViewModel viewModel=new NoPrefixsWorkerConfigViewModel(model);
            this.DataContext = viewModel;
        }
    }
}
