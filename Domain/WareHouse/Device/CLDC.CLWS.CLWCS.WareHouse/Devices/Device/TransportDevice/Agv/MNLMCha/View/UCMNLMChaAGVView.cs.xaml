using System.Windows;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.MNLMCha.View
{
    /// <summary>
    /// UCXChaAGVView.xaml 的交互逻辑
    /// </summary>
    public partial class UCMNLMChaAGVView : Window
    {
        public UCMNLMChaAGVView()
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
