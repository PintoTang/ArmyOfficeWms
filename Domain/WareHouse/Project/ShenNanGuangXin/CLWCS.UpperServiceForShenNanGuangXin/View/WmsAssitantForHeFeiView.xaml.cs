using System.Windows;

namespace CLWCS.UpperServiceForHeFei.View
{
    /// <summary>
    /// WmsAssitantForSNDLView.xaml 的交互逻辑
    /// </summary>
    public partial class WmsAssitantForHeFeiView
    {
        public WmsAssitantForHeFeiView()
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
