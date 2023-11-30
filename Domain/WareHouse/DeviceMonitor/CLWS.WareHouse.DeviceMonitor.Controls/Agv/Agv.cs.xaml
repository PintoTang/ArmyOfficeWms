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

namespace WHSE.Monitor.Framework.UserControls
{
    /// <summary>
    /// Agv.xaml 的交互逻辑
    /// </summary>
    public partial class Agv : TransportDeviceBase
    {

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(Agv), new PropertyMetadata(default(double)));

        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
       "Y", typeof(double), typeof(Agv), new PropertyMetadata(default(double)));

        /// <summary>
        /// X坐标
        /// </summary>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }



        public static readonly DependencyProperty StatusColorProperty = DependencyProperty.Register(
            "StatusColor", typeof(string), typeof(Agv), new PropertyMetadata(default(string)));

        /// <summary>
        /// 状态颜色
        /// </summary>
        public string StatusColor
        {
            get { return (string)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        public Agv()
        {
            InitializeComponent();
        }
    }
}
