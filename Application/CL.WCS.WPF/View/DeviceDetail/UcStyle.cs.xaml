using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CL.WCS.WPF
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
