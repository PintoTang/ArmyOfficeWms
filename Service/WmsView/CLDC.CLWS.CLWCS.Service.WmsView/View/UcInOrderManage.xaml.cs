using CLDC.CLWS.CLWCS.Service.Authorize.ViewModel;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcInOrderManage.xaml 的交互逻辑
    /// </summary>
    public partial class UcInOrderManage : UserControl
    {
        public UcInOrderManage()
        {
            InitializeComponent();
            DataContext = new AccountManageViewModel();
        }
    }
}
