using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.View
{
    public partial class HistoryOrderManageView : UserControl, IMainUseCtrl
    {
        public HistoryOrderManageView()
        {
            InitializeComponent();
            DataContext=new HistoryExOrderViewModel();
        }

        private string _useCtrlId = "历史指令";

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
