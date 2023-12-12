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
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Service.License;
using Infrastructrue.Ioc.DependencyFactory;

namespace CL.WCS.WPF.View.MainTitle
{
    /// <summary>
    /// MainTitle.xaml 的交互逻辑
    /// </summary>
    public partial class MainTitle : UserControl
    {
        public MainTitle()
        {
            InitializeComponent();
            DataContext = new MainTitleViewModel();
        }
        public delegate void TitleHandler();
        public event TitleHandler OnMinClick;
        public event TitleHandler DragMove;
        public event TitleHandler OnCloseApp;
        public event TitleHandler OnLogout;
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            if (OnMinClick != null)
            {
                OnMinClick();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (OnCloseApp != null)
            {
                OnCloseApp();
            }
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            if (OnLogout != null)
            {
                OnLogout();
            }
        }

        private void BtnAbout_OnClick(object sender, RoutedEventArgs e)
        {
            ILicenseService licenseService = DependencyHelper.GetService<ILicenseService>();
            if (licenseService == null)
            {
                return;
            }
            licenseService.ShowAndRegisterLicense();
        }

        private void MainTitle_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DragMove != null)
            {
                DragMove();
            }
        }
    }

}
