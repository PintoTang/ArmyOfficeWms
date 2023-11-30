using System.Windows.Controls;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.DbService.View
{
    /// <summary>
    /// UCDataMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class DbServiceMonitorView : UserControl,IMainUseCtrl
    {
        public DbServiceMonitorView()
        {
            InitializeComponent();
        }
        private string _useCtrlId = "数据监控";

        public string UseCtrlId
        {
            get { return _useCtrlId; }
            set { _useCtrlId = value; }

        }
        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
