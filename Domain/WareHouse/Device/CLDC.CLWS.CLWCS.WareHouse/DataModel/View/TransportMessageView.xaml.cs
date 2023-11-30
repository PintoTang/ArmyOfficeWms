using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.View
{
    /// <summary>
    /// TransportMessageView.xaml 的交互逻辑
    /// </summary>
    public partial class TransportMessageView : UserControl
    {
        public TransportMessageView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
            "TitleContent", typeof(object), typeof(TransportMessageView), new PropertyMetadata(default(object)));

        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }


        public static readonly DependencyProperty ForceFinishVisibilityProperty = DependencyProperty.Register(
         "ForceFinishVisibility", typeof(Visibility), typeof(TransportMessageView), new PropertyMetadata(default(Visibility)));

        public Visibility ForceFinishVisibility
        {
            get { return (Visibility)GetValue(ForceFinishVisibilityProperty); }
            set { SetValue(ForceFinishVisibilityProperty, value); }
        }


        public static readonly DependencyProperty ReDoVisibilityProperty = DependencyProperty.Register(
       "ReDoVisibility", typeof(Visibility), typeof(TransportMessageView), new PropertyMetadata(default(Visibility)));

        public Visibility ReDoVisibility
        {
            get { return (Visibility)GetValue(ReDoVisibilityProperty); }
            set { SetValue(ReDoVisibilityProperty, value); }
        }

    }
}
