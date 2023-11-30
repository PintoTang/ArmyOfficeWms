using System.Windows;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.View
{
    /// <summary>
    /// ExOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class ExOrderListView
    {
        
        public ExOrderListView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
            "TitleContent", typeof(object), typeof(ExOrderListView), new PropertyMetadata(default(object)));

        /// <summary>
        /// 
        /// </summary>
        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        public static readonly DependencyProperty ForceFinishVisibilityProperty = DependencyProperty.Register(
           "ForceFinishVisibility", typeof(Visibility), typeof(ExOrderListView), new PropertyMetadata(default(Visibility)));

        public Visibility ForceFinishVisibility
        {
            get { return (Visibility)GetValue(ForceFinishVisibilityProperty); }
            set { SetValue(ForceFinishVisibilityProperty, value); }
        }


        public static readonly DependencyProperty CancelVisibilityProperty = DependencyProperty.Register(
          "CancelVisibility", typeof(Visibility), typeof(ExOrderListView), new PropertyMetadata(default(Visibility)));

        public Visibility CancelVisibility
        {
            get { return (Visibility)GetValue(CancelVisibilityProperty); }
            set { SetValue(CancelVisibilityProperty, value); }
        }



    }
}
