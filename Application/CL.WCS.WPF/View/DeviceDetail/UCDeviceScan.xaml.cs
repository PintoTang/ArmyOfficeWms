using System.Windows;
using System.Windows.Input;


namespace CL.WCS.WPF
{
    /// <summary>
    /// UCScanDevice.xaml 的交互逻辑
    /// </summary>
    public partial class UCDeviceScan : Window
    {
        public UCDeviceScan()
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
