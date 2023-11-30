using System.Windows;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.View
{
    /// <summary>
    /// UCHangChaAGVView2.xaml 的交互逻辑
    /// </summary>
    public partial class UCHangChaAGVView : Window
    {
        public UCHangChaAGVView()
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
