using System.Windows.Controls;
using CL.WCS.SystemConfigPckg.Model;
using CL.WCS.SystemConfigPckg.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CL.WCS.SystemConfigPckg
{
    /// <summary>
    /// SystemConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemConfigView : UserControl,IMainUseCtrl
    {
        public SystemConfigView()
        {
            InitializeComponent();
            this.DataContext = new SystemConfigViewMode(SystemConfig.Instance.CurSystemConfig);
        }
        private string _useCtrlId = "系统设置";

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
