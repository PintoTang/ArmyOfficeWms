using System.Windows;
using System.Windows.Controls;

namespace CLDC.Infrastructrue.UserCtrl
{
    /// <summary>
    /// UcStyle.xaml 的交互逻辑
    /// </summary>
    public partial class UcStyle : UserControl
    {
       
        public UcStyle()
        {
            InitializeComponent();
        }
        public void SetOnCloseRoutedEventHandler(RoutedEventHandler RouteOnClose)
        {
            btnClose.Click += RouteOnClose;
        }

        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
            "TitleContent", typeof(object), typeof(UcStyle), new PropertyMetadata(default(object)));

        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

    }
}
