using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcInOrderManage.xaml 的交互逻辑
    /// </summary>
    public partial class UcInOrderManage : UserControl
    {
        public InOrderListViewModel ViewModel { get; set; }
        public UcInOrderManage()
        {
            InitializeComponent();
            ViewModel = new InOrderListViewModel();
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
