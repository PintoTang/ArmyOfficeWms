using System.Windows;
using System.Windows.Input;

namespace CLDC.Infrastructrue.UserCtrl
{
    /// <summary>
    /// ShowMessage.xaml 的交互逻辑
    /// </summary>
    public partial class ShowMessage : Window
    {
        public ShowMessage()
        {
            InitializeComponent();
        }

        public ShowMessage(string messageBoxText)
        {
            InitializeComponent();
            this.MessageText = messageBoxText;
            this.MessageTitle = "提示";
            MessageBoxType = MessageBoxButton.OK;
        }
        public ShowMessage(string messageBoxText, string caption)
        {
            InitializeComponent();
            this.MessageText = messageBoxText;
            this.MessageTitle = caption;
            MessageBoxType = MessageBoxButton.OK;
        }
        public ShowMessage(string messageBoxText, string caption, MessageBoxButton button)
        {
            InitializeComponent();
            this.MessageText = messageBoxText;
            this.MessageTitle = caption;
            MessageBoxType = button;
        }


        /// <summary>
        /// 显示MsgBox的标题信息
        /// </summary>
        public string MessageTitle
        {
            get { return (string)GetValue(MessageTitleProperty); }
            set { SetValue(MessageTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageTitleProperty =
            DependencyProperty.Register("MessageTitle", typeof(string), typeof(ShowMessage), new PropertyMetadata(""));


        /// <summary>
        /// 显示MessageBox的弹出的Message
        /// </summary>
        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(ShowMessage), new PropertyMetadata(""));


        MessageBoxButton _MessageBoxType = MessageBoxButton.OK;
        /// <summary>
        /// 记录按钮的状态
        /// </summary>
        public MessageBoxButton MessageBoxType
        {
            get { return _MessageBoxType; }
            set
            {
                _MessageBoxType = value;
                if (_MessageBoxType== MessageBoxButton.OK)
                {
                    btnOK.Visibility = System.Windows.Visibility.Collapsed;
                    btnCancel.Content = "确定";
                }
            }
        }

        MessageBoxResult _result = MessageBoxResult.Cancel;
        /// <summary>
        /// 结果
        /// </summary>
        public MessageBoxResult Result
        {
            get { return _result; }
            set { _result = value; }
        }



        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBoxType)
            {
                case MessageBoxButton.OK:
                    Result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    Result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNo:
                    Result = MessageBoxResult.Yes;
                    break;
                case MessageBoxButton.YesNoCancel:
                    Result = MessageBoxResult.Yes;
                    break;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

            switch (MessageBoxType)
            {
                case MessageBoxButton.OK:
                    Result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    Result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNo:
                    Result = MessageBoxResult.No;
                    break;
                case MessageBoxButton.YesNoCancel:
                    Result = MessageBoxResult.No;
                    break;
            }
            this.Close();
        }
    }
}
