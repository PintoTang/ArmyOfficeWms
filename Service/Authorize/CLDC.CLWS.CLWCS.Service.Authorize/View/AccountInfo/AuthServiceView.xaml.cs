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
using CLDC.CLWS.CLWCS.Service.Authorize.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    /// <summary>
    /// AuthServiceView.xaml 的交互逻辑
    /// </summary>
    public partial class AuthServiceView : UserControl,IMainUseCtrl
    {
        public AuthServiceView()
        {
            InitializeComponent();
            AuthServiceViewModel viewModel=new AuthServiceViewModel();
            this.DataContext = viewModel;
        }

        private string _useCtrlId = "账号管理";

        public string UseCtrlId
        {
            get
            {
                return _useCtrlId;
            }
            set { _useCtrlId = value; }
        }

        public void Show()
        {
            Visibility= Visibility.Visible;
        }

        public void Hide()
        {
            Visibility= Visibility.Hidden;
        }

    }
}
