using System.Windows.Controls;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.UpperServiceForHeFei.Interface.ViewModel;

namespace CLWCS.UpperServiceForHeFei.Interface.View
{
    /// <summary>
    /// NotifyInstructFinishView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyInstructFinishView : UserControl
    {
        public NotifyInstructFinishView(NotifyInstructFinishCmdMode cmd)
        {
            InitializeComponent();
            this.DataContext = new InterfaceViewModel<NotifyInstructFinishCmdMode>(cmd);
        }

    }
}
