using CL.WCS.SystemConfigPckg;
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

        private void btnSystemParam_Click(object sender, RoutedEventArgs e)
        {
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                ucChildrens.Remove(ucChildrens[i]);
            }
            SystemConfigView systemConfig = new SystemConfigView();
            UserContentControl.Children.Add(systemConfig);
        }

        private void btnAreaList_Click(object sender, RoutedEventArgs e)
        {
            var ucChildrens = UserContentControl.Children;
            for (int i = ucChildrens.Count - 1; i >= 0; i--)
            {
                ucChildrens.Remove(ucChildrens[i]);
            }
            UcAreaList areaList = new UcAreaList();
            UserContentControl.Children.Add(areaList);            
        }



    }
}
