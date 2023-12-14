using CL.WCS.SystemConfigPckg.View;
using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcSetupView.xaml 的交互逻辑
    /// </summary>
    public partial class UcSetupView : UserControl
    {
        public UcSetupView()
        {
            InitializeComponent();
        }

        private void btnSystemParam_Click(object sender, RoutedEventArgs e)
        {
            SystemSettingView systemSettingView = new SystemSettingView();
            systemSettingView.ShowDialog();
        }
        private void BtnExit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hide();
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
