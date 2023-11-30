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

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.View
{
    /// <summary>
    /// WebApiView.xaml 的交互逻辑
    /// </summary>
    public partial class WebApiViewVertical : UserControl
    {
        public WebApiViewVertical()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
           "TitleContent", typeof(object), typeof(WebApiViewVertical), new PropertyMetadata(default(object)));

        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        public static readonly DependencyProperty HttpUrlProperty = DependencyProperty.Register(
           "HttpUrl", typeof(object), typeof(WebApiViewVertical), new PropertyMetadata(default(object)));

        public object HttpUrl
        {
            get { return (object)GetValue(HttpUrlProperty); }
            set { SetValue(HttpUrlProperty, value); }
        }


    }
}
