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

namespace CL.WCS.WPF.UserCtrl.MainForm
{
    /// <summary>
    /// Title.xaml 的交互逻辑
    /// </summary>
    public partial class UserLogo : UserControl
    {
        public UserLogo()
        {
            InitializeComponent();
        }

        public delegate void MinWindowHandler();

        public event MinWindowHandler OnMinClick;

        public event MinWindowHandler OnCloseApp;

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            //((((this.Parent as Grid).Parent as Grid).Parent) as Window).WindowState = WindowState.Minimized;
            if (OnMinClick != null)
            {
                OnMinClick();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (OnCloseApp != null)
            {
                OnCloseApp();
            }
        }
    }
}
