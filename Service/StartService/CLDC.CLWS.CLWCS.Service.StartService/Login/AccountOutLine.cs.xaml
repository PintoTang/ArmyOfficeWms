using System.Windows;

namespace CLDC.CLWS.CLWCS.Service.StartService.Login
{
    /// <summary>
    /// AccountOutLine.xaml 的交互逻辑
    /// </summary>
    public partial class AccountOutLine : Window
    {
        public AccountOutLine()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            TxtBoxInputPwd.Focus();//输入框设置焦点
        }
        public bool IsCheckSuccessed = false;
        //校验退出密码
        private void BtnCheckPwd_Click(object sender, RoutedEventArgs e)
        {
            if(TxtBoxInputPwd.Password.Trim()!="123456")
            {
                MessageBox.Show("输入的密码不正确!");
                TxtBoxInputPwd.Password = "";
                return;
            }
            IsCheckSuccessed = true;
            this.Close();
        }
    }
}
