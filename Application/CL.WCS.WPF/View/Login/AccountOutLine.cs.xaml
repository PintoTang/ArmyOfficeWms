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
using System.Windows.Shapes;

namespace CL.WCS.WPF.UserCtrl.Login
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
