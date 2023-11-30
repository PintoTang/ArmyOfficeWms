using System.Windows.Controls;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Common;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;

namespace CLWCS.UpperServiceForHeFei.Interface.View
{
    /// <summary>
    /// NotifyInstructExceptionView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyInstructExceptionView : UserControl
    {
        public NotifyInstructExceptionView(NotifyInstructExceptionMode cmd)
        {
            InitializeComponent();
            this.DataContext = new InterfaceViewModel<NotifyInstructExceptionMode>(cmd);
        }
    }
}
