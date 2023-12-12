using CLDC.CLWS.CLWCS.Service.WmsView.ViewModel;
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

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcOutOrderList.xaml 的交互逻辑
    /// </summary>
    public partial class UcOutOrderList : UserControl
    {
        public OrderListViewModel ViewModel { get; set; }

        public UcOutOrderList()
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
