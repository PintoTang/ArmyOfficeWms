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

namespace CL.WCS.WPF
{
    /// <summary>
    /// UCLoadDeviceSetConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UCLoadDeviceSetConfig : Window
    {
        public UCLoadDeviceSetConfig()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); }; //拖拽
            UcStyleWindows.SetOnCloseRoutedEventHandler(OnClose_Click);
        }

        void OnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
