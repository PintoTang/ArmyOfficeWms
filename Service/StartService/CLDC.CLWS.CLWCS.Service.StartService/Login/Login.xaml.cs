using System;
using System.Linq;
using System.Windows;
using CL.WCS.SystemConfigPckg.View;
using CL.WCS.WPF.UserCtrl.Login;
using CL.WCS.WPF.View.Login;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.Infrastructrue.Security;
using CLDC.Infrastructrue.UserCtrl;
using Infrastructrue.Ioc.DependencyFactory;
using System.Windows.Forms;

namespace CLDC.CLWS.CLWCS.Service.StartService.Login
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        /// <summary>
        /// 用户信息 文件路径
        /// </summary>
        string txtUserInfoFullPath = "";

        private string txtLoginDataFullPath = "";
        //勾子管理类
        //private KeyboardHookLib _keyboardHook = null;
        /// <summary>
        /// 窗体 初始化
        /// </summary>
        public Login()
        {
            txtLoginDataFullPath = AppDomain.CurrentDomain.BaseDirectory + "LoginData.txt";
            txtUserInfoFullPath = AppDomain.CurrentDomain.BaseDirectory + "UserData.txt";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            DataContext = new LoginViewModel();
            loadUserInfo();
            this.MouseLeftButtonDown += delegate { this.DragMove(); };

            ////安装勾子
            //if (_keyboardHook == null)
            //{
            //    _keyboardHook = new KeyboardHookLib();
            //}
            //_keyboardHook.InstallHook(this.OnKeyPress);
        }

        public bool IsLoginSuccessExit = false;

        #region 基本参数
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string userPwd;

        public string UserPwd
        {
            get { return userPwd; }
            set { userPwd = value; }
        }
        private bool isRmdPwd;

        public bool IsRmdPwd
        {
            get { return isRmdPwd; }
            set { isRmdPwd = value; }
        }
        #endregion

        private string strPrivatekey = "SHENZHEN.CLOU.ZH";//AES 16位 私钥
        string[] userArr = null;
        string[] LoginArr = null;
        /// <summary>
        /// 加载用户信息 绑定到窗体控件
        /// </summary>
        private void loadUserInfo()
        {
            userArr = UserDataForTxt.GetRmdUserInfo(txtUserInfoFullPath);
            LoginArr = LoginDataForTxt.GetLgoinInfo(txtLoginDataFullPath);
            if (userArr == null) return;

            if (!string.IsNullOrEmpty(userArr[0]))
            {
                TxtBoxUserName.Text = AESEncryption.Decrypt(userArr[0], strPrivatekey);
            }
            if (!string.IsNullOrEmpty(userArr[1]))
            {
                TxtBoxPwd.Password = AESEncryption.Decrypt(userArr[1], strPrivatekey);
            }
            if (!string.IsNullOrEmpty(userArr[2]))
            {
                TogBtnIsRmdPwd.IsChecked = (bool)(userArr[2].Trim() == "1");
            }
        }
        /// <summary>
        /// 点击登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            LoginArr = LoginDataForTxt.GetLgoinInfo(txtLoginDataFullPath);
            int curLoginErrTimes = int.Parse(LoginArr[0]);//当前次数
            int sleepTimes = int.Parse(LoginArr[1]);//超时 分
            string strWriteDateTime = LoginArr[2];//记录错误的最后时间
            string dateNow = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
            if (curLoginErrTimes >= 5)
            {
                if (!string.IsNullOrEmpty(strWriteDateTime))
                {
                    DateTime writeTime = DateTime.ParseExact(strWriteDateTime, "yyyy-MM-dd hh-mm-ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime nowTime = DateTime.ParseExact(dateNow, "yyyy-MM-dd hh-mm-ss", System.Globalization.CultureInfo.CurrentCulture);
                    TimeSpan timeSpan = nowTime - writeTime;
                    if (timeSpan.Minutes >= sleepTimes)
                    {
                        //大于 则表示已经等待完成  需要解锁
                        LoginDataForTxt.WriteLoginErrInfo(txtLoginDataFullPath, 0, 5, dateNow);
                        LoginArr = LoginDataForTxt.GetLgoinInfo(txtLoginDataFullPath);
                        curLoginErrTimes = int.Parse(LoginArr[0]);//当前次数
                        sleepTimes = int.Parse(LoginArr[1]);//超时 分
                        strWriteDateTime = LoginArr[2];//记录错误的最后时间
                        dateNow = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
                    }
                    else
                    {
                        int sleepMini = sleepTimes - timeSpan.Minutes;
                        MessageBoxEx.Show("当前输入错误次数过多，请联系管理员! 或等待解锁时间:"+sleepMini+"分钟", "提示", MessageBoxButton.OK);
                        return;
                    }
                }
              
            }

            username = TxtBoxUserName.Text.Trim();
            userPwd = TxtBoxPwd.Password.Trim();
            isRmdPwd = (bool)TogBtnIsRmdPwd.IsChecked;
            if (username == "" || userPwd == "")
            {
                MessageBoxEx.Show("用户名或密码不能为空，请输入用户名或密码!","提示", MessageBoxButton.OK);
                return;
            }
            if (!ValidData.RexCheckUserName(username))
            {
                MessageBoxEx.Show("输入的用户名不合法!", "提示", MessageBoxButton.OK);
                return;
            }
            if (!ValidData.RexCheckPassword(userPwd))
            {
                MessageBoxEx.Show("输入的密码不合法!", "提示", MessageBoxButton.OK);
                return;
            }
            if (userArr != null && userArr.Count() > 0)
            {
                //登录成功
                UserDataForTxt.WriteUserInfo(txtUserInfoFullPath, AESEncryption.Encrypt(username, strPrivatekey), AESEncryption.Encrypt(userPwd, strPrivatekey), isRmdPwd);
                //UserDataForTxt.WriteUserInfo(txtUserInfoFullPath, username, userPwd, isRmdPwd);
                AuthorizeService authorizeService = DependencyHelper.GetService<AuthorizeService>();
                OperateResult<Session> loginResult = authorizeService.Login(username, userPwd);

             
                if (!loginResult.IsSuccess)
                {
                    //登录失败 记录错误次数
                    LoginDataForTxt.WriteLoginErrInfo(txtLoginDataFullPath, curLoginErrTimes + 1, 5, dateNow);
                    MessageBoxEx.Show(loginResult.Message, "提示", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    //登录成功 错误次数清0
                    LoginDataForTxt.WriteLoginErrInfo(txtLoginDataFullPath, 0, 5, dateNow);
                    CookieService.CurSession = loginResult.Content;
                }
                OperateLogHelper.Record("登陆系统");
                IsLoginSuccessExit = true;
                DialogResult = true;
                ////取消勾子
                //if (_keyboardHook != null)
                //{
                //    _keyboardHook.UninstallHook();
                //}
                this.Close();
            }
            else
            {
                //如果第一次txt没有值，则默认写入新的用户名和密码 当做后面的新用户
                UserDataForTxt.WriteUserInfo(txtUserInfoFullPath, AESEncryption.Encrypt(username, strPrivatekey), AESEncryption.Encrypt(userPwd, strPrivatekey), isRmdPwd);
                userArr = UserDataForTxt.GetRmdUserInfo(txtUserInfoFullPath);
                IsLoginSuccessExit = true;
            }

        }

        /// <summary>
        /// 取消登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelLogin_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgResult = MessageBoxEx.Show("确定退出系统?", "警告", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            //取消勾子
            //if (_keyboardHook != null)
            //{
            //    _keyboardHook.UninstallHook();
            //}
            this.Close();
        }

        private void SysSet_OnClick(object sender, RoutedEventArgs e)
        {
            SystemSettingView systemSettingView = new SystemSettingView();
            systemSettingView.ShowDialog();
        }

        /// <summary>
        /// 客户端键盘捕捉事件.  不用待移除 by zhangxing
        /// </summary>
        /// <param name="hookStruct">由Hook程序发送的按键信息</param>
        /// <param name="handle">是否拦截</param>
        public void OnKeyPress(KeyboardHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false; //预设不拦截任何键 （true 则拦截一切即键盘按什么都不启用）
            if (hookStruct.vkCode == 91) // 截获左win(开始菜单键)
            {
                handle = true;
            }

            if (hookStruct.vkCode == 92)// 截获右win
            {
                handle = true;
            }

            //截获Ctrl+Esc
            if (hookStruct.vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control)
            {
                handle = true;
            }

            //截获alt+f4
            if (hookStruct.vkCode == (int)Keys.F4 && (int)Control.ModifierKeys == (int)Keys.Alt)
            {
                handle = true;
            }

            //截获alt+tab
            if (hookStruct.vkCode == (int)Keys.Tab && (int)Control.ModifierKeys == (int)Keys.Alt)
            {
                handle = true;
            }

            //截获F1
            if (hookStruct.vkCode == (int)Keys.F1)
            {
                handle = true;
            }

            //截获Ctrl+Alt+Delete
            if ((int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt + (int)Keys.Delete)
            {
                handle = true;
            }

            //如果键A~Z
            if (hookStruct.vkCode >= (int)Keys.A && hookStruct.vkCode <= (int)Keys.Z)
            {
                //挡掉B键
                if (hookStruct.vkCode == (int)Keys.B)
                    hookStruct.vkCode = (int)Keys.None; //设键为0

                handle = true;
            }
            //Keys key = (Keys)hookStruct.vkCode;
        }
     
    }
}
