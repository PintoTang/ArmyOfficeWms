using System.Windows;

namespace CL.WCS.SystemConfigPckg.View
{
    /// <summary>
    /// SystemSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSettingView
    {
        public SystemSettingView()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); }; //拖拽
            CtrlTitle.SetOnCloseRoutedEventHandler(OnClose);
            InitViewSize();
        }
        private void InitViewSize()
        {
            this.Width = SystemParameters.WorkArea.Width * 0.6;
            this.Height = SystemParameters.WorkArea.Height * 0.8;
        }
        void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
