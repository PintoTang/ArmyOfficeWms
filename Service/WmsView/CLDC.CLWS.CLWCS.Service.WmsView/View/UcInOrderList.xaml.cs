using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcInOrderManage.xaml 的交互逻辑
    /// </summary>
    public partial class UcInOrderList : UserControl
    {
        public OrderListViewModel ViewModel { get; set; }
        public UcInOrderList()
        {
            InitializeComponent();
            ViewModel = new OrderListViewModel();
            DataContext = ViewModel;
        }        

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void BtnExit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Hide();
        }
    }
}
