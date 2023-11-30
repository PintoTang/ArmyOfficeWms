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
    /// UserInfoModifyView.xaml 的交互逻辑
    /// </summary>
    public partial class UserInfoModifyView : Window
    {
        public UserInfoModifyView()
        {
            InitializeComponent();
            UserInfoModifyViewModel viewModel=new UserInfoModifyViewModel();
            this.DataContext = viewModel;
            CtrlTitle.SetOnCloseRoutedEventHandler(OnClose);
            this.MouseLeftButtonDown += delegate { DragMove(); }; //拖拽
            InitilizeViewSize();
        }
        void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void InitilizeViewSize()
        {
            this.Width = SystemParameters.WorkArea.Width * 0.8;
            this.Height = SystemParameters.WorkArea.Height*0.8;
        }

    }
}
