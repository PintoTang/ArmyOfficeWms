using System.Windows;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.View
{
    /// <summary>
    /// StringCharTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class StringCharTaskView : UserControl
    {
        public StringCharTaskView()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
           "TitleContent", typeof(object), typeof(StringCharTaskView), new PropertyMetadata(default(object)));

        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }


        public static readonly DependencyProperty ForceFinishVisibilityProperty = DependencyProperty.Register(
         "ForceFinishVisibility", typeof(Visibility), typeof(StringCharTaskView), new PropertyMetadata(default(Visibility)));

        public Visibility ForceFinishVisibility
        {
            get { return (Visibility)GetValue(ForceFinishVisibilityProperty); }
            set { SetValue(ForceFinishVisibilityProperty, value); }
        }


        public static readonly DependencyProperty ReDoVisibilityProperty = DependencyProperty.Register(
       "ReDoVisibility", typeof(Visibility), typeof(StringCharTaskView), new PropertyMetadata(default(Visibility)));

        public Visibility ReDoVisibility
        {
            get { return (Visibility)GetValue(ReDoVisibilityProperty); }
            set { SetValue(ReDoVisibilityProperty, value); }
        }
    }
}
