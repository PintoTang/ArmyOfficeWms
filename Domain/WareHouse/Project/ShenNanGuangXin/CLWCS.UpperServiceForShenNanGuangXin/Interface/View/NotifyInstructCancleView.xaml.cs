using System.Windows.Controls;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;

namespace CLWCS.UpperServiceForHeFei.Interface.View
{
    /// <summary>
    /// NotifyInstructCancleView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyInstructCancleView : UserControl
    {
        public NotifyInstructCancleView(NotifyInstructCancleCmdMode cmd)
        {
            InitializeComponent();
            this.DataContext = new InterfaceViewModel<NotifyInstructCancleCmdMode>(cmd);
        }
    }
}
