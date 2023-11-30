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

namespace CLDC.CLWS.CLWCS.Service.Authorize.View
{
    /// <summary>
    /// EditUserInfo.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserInfo : UserControl
    {
        public EditUserInfo()
        {
            InitializeComponent();
            EditUserInfoViewModel viewModel = new EditUserInfoViewModel(CookieService.CurSession.UserInfo);
            this.DataContext = viewModel;
        }
    }
}
