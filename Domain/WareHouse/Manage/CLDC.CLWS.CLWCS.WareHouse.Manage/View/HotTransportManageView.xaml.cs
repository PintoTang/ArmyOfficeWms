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
using CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.View
{
    /// <summary>
    /// HotTransportView.xaml 的交互逻辑
    /// </summary>
    public partial class HotTransportManageView : UserControl, IMainUseCtrl
    {
        public HotTransportManageView()
        {
            InitializeComponent();
            DataContext =new HotTransportManageViewModel();
        }
        private string _useCtrlId = "实时搬运信息";

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
