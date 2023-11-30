using System.Windows.Controls;
using CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel;
using CLDC.Infrastructrue.UserCtrl.Domain;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.View
{
    public partial class HistoryTransportManageView : UserControl, IMainUseCtrl
    {
        public HistoryTransportManageView()
        {
            InitializeComponent();
            DataContext = new HistoryTransportManageViewModel();
        }

        private string _useCtrlId = "搬运信息";

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
