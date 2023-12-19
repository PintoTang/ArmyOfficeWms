using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Service.WmsView.View
{
    /// <summary>
    /// UcTopTitle.xaml 的交互逻辑
    /// </summary>
    public partial class UcTopTitle : UserControl
    {
        public UcTopTitle()
        {
            InitializeComponent();
        }

        public void SetOnCloseRoutedEventHandler(RoutedEventHandler RouteOnClose)
        {
            btnClose.Click += RouteOnClose;
        }

        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
            "TitleContent", typeof(object), typeof(UcTopTitle), new PropertyMetadata(default(object)));

        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

    }
}
