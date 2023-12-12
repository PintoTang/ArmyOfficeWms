using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcDefaultView.xaml 的交互逻辑
    /// </summary>
    public partial class UcDefaultView : UserControl
    {
        public UcDefaultView()
        {
            InitializeComponent();
            DataContext = new OrderListViewModel();
        }

        private void OrderDetailGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

    }
}
