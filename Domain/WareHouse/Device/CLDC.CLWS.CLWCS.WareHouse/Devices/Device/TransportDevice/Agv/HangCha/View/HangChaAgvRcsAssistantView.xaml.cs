using System.Windows;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.View
{
    /// <summary>
    /// YingChuangAgvRcsAssistantView.xaml 的交互逻辑
    /// </summary>
    public partial class HangChaAgvRcsAssistantView 
    {
        public HangChaAgvRcsAssistantView()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); }; //拖拽
            CtrlTitle.SetOnCloseRoutedEventHandler(OnClose);
            InitViewSize();
        }
        void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void InitViewSize()
        {
            this.Width = SystemParameters.WorkArea.Width * 0.8;
            this.Height = SystemParameters.WorkArea.Height * 0.8;
        }
    }
}
