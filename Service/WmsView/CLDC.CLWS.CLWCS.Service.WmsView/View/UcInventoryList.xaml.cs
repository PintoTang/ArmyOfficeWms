using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcInventoryList.xaml 的交互逻辑
    /// </summary>
    public partial class UcInventoryList : UserControl
    {
        public InventoryListViewModel ViewModel { get; set; }
        public UcInventoryList()
        {
            InitializeComponent();
            ViewModel = new InventoryListViewModel();
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

        private void InOrderGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
