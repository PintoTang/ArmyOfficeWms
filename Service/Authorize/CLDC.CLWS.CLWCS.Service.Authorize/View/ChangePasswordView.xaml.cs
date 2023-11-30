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
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.Security;
using CLDC.CLWS.CLWCS.Service.Authorize.ViewModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Service.Authorize.View
{
    /// <summary>
    /// ChangePasswordView.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordView : UserControl
    {

        public ChangePasswordView()
        {
            InitializeComponent();
        }
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            string oldPassword = PdbOldPassword.Password;
            string newPassword01 = PdbNewPassword01.Password;
            string newPassword02 = PdbNewPassword02.Password;
            try
            {
                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword01) || string.IsNullOrEmpty(newPassword02))
                {
                    MessageBoxEx.Show("密码值或更改密码值输入不能为空", "提示", MessageBoxButton.OK);
                    return;
                }
                MessageBoxResult rt = MessageBoxEx.Show("您确认更改密码？", "警告", MessageBoxButton.YesNo);
                if (rt.Equals(MessageBoxResult.Yes))
                {
                    if (!newPassword01.Equals(newPassword02))
                    {
                        MessageBoxEx.Show("密码确认失败，确保两次密码输入是否一致");
                        return;
                    }
                    string encryptNewPd = SecurityHelper.Encrypt(newPassword01);
                    string encryptOldPd = SecurityHelper.Encrypt(oldPassword);
                    AuthorizeService authorizeService = DependencyHelper.GetService<AuthorizeService>();
                    OperateResult updateResult = authorizeService.ChangePassword(CookieService.CurSession.UserInfo.Account.AccCode, encryptOldPd, encryptNewPd);
                    if (!updateResult.IsSuccess)
                    {
                        MessageBoxEx.Show("用户信息保存失败：" + updateResult.Message);
                        return;
                    }
                    SnackbarQueue.MessageQueue.Enqueue("密码修改成功");
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("用户信息保存失败:" + OperateResult.ConvertException(ex));
            }
        }
    }
}
