using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CLDC.CLAT.License;
using CLDC.CLWS.CLWCS.Service.License.ViewModel;

namespace CLDC.CLWS.CLWCS.Service.License.View
{
    /// <summary>
    /// RegisterLicenseView.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterLicenseView
    {
        public RegisterLicenseView()
        {
            InitializeComponent();
            TbDeviceCode.Text = ComputerAttributes.GetIdentifyCode();
        }


        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            RegisterLicenseViewModel viewModel = DataContext as RegisterLicenseViewModel;
            if (viewModel == null)
            {
                DialogResult = false;
                this.Close();
                return;
            }
            DialogResult = viewModel.IsRegisterSuccess;
            this.Close();
        }

        private void RegisterLicenseView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
