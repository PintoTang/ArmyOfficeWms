using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.Infrastructrue.Security;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Service.Authorize.View
{
    /// <summary>
    /// CreateAccountView.xaml 的交互逻辑
    /// </summary>
    public partial class CreateAccountView
    {
        private readonly RoleLevelEnum _curRoleLevel = CookieService.CurSession.RoleLevel;
        readonly AuthorizeService _authorizeService = DependencyHelper.GetService<AuthorizeService>();
        public CreateAccountView()
        {
            InitializeComponent();
            InitCbLevel();
            InitCbGroup();
        }

        private readonly Dictionary<RoleLevelEnum, string> _roleLevelTypeDic = new Dictionary<RoleLevelEnum, string>();
        public Dictionary<RoleLevelEnum, string> RoleLevelTypeDic
        {
            get
            {
                if (_roleLevelTypeDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(RoleLevelEnum)))
                    {
                        RoleLevelEnum em = (RoleLevelEnum)value;
                        if (em < _curRoleLevel)
                        {
                            _roleLevelTypeDic.Add(em, em.GetDescription());
                        }
                    }
                }
                return _roleLevelTypeDic;

            }
        }

        private readonly Dictionary<int, string> _groupTypeDic = new Dictionary<int, string>();
        public Dictionary<int, string> GroupTypeDic
        {
            get
            {
                if (_groupTypeDic.Count == 0)
                {
                    List<GroupMode> grouplist = _authorizeService.GetAllGroupList();
                    foreach (GroupMode value in grouplist)
                    {
                        _groupTypeDic.Add(value.GroupId, value.GroupName);
                    }
                }
                return _groupTypeDic;

            }
        }

        private void InitCbGroup()
        {
            CbGroup.SelectedValuePath = "Key";
            CbGroup.DisplayMemberPath = "Value";
            CbGroup.ItemsSource = GroupTypeDic;
        }

        private void InitCbLevel()
        {
            CbLevel.SelectedValuePath = "Key";
            CbLevel.DisplayMemberPath = "Value";
            CbLevel.ItemsSource = RoleLevelTypeDic;
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            string accout = TbAccount.Text;
            string password01 = PdPassword01.Password;
            string password02 = PdPassword02.Password;
            if (CbLevel.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择权限等级", "提示", MessageBoxButton.OK);
                return;
            }
            RoleLevelEnum level = (RoleLevelEnum)CbLevel.SelectedValue;
            if (CbGroup.SelectedIndex == -1)
            {
                MessageBoxEx.Show("请选择部门信息", "提示", MessageBoxButton.OK);
                return;
            }
            int groupId = (int)CbGroup.SelectedValue;
            if (string.IsNullOrEmpty(accout))
            {
                MessageBoxEx.Show("账号不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(password01))
            {
                MessageBoxEx.Show("密码不能为空", "提示", MessageBoxButton.OK);
                return;
            }
            if (!password01.Equals(password02))
            {
                MessageBoxEx.Show("密码确认失败，请确保两次密码输入一致", "提示", MessageBoxButton.OK);
                return;
            }
            if (!ValidData.RexCheckUserName(accout))
            {
                MessageBoxEx.Show("输入的用户名不符合要求：限16个字符，支持中英文、数字、减号或下划线，请重新输入!");
                return;
            }

            if (!ValidData.RexCheckPassword(password01) || !ValidData.RexCheckPassword(password02))
            {
                MessageBoxEx.Show("输入的密码不符合要求:8-20 位，字母、数字、字符，请重新输入!");
                return;
            }


            AccountMode newAccount = new AccountMode();
            newAccount.AccCode = accout;
            newAccount.Password = SecurityHelper.Encrypt(password01);
            newAccount.CreateTime = DateTime.Now;
            newAccount.EnableTime = DateTime.Now;
            newAccount.ModifierTime = DateTime.Now;
            newAccount.DisableTime=DateTime.Now;
            newAccount.OnlineStatus = AccountLiveStatus.Online;
            newAccount.CreaterId = CookieService.CurSession.UserInfo.Account.AccId;
            newAccount.GroupId = groupId;
            newAccount.RoleLevel = level;
            newAccount.UseStatus = AccountStatusEnum.启用;
            newAccount.Remark = TbRemark.Text;

            OperateResult createResult = _authorizeService.CreateNewAccount(newAccount);
            if (!createResult.IsSuccess)
            {
                MessageBoxEx.Show("创建用户失败，原因：" + createResult.Message, "错误", MessageBoxButton.OK);
                return;
            }
            SnackbarQueue.MessageQueue.Enqueue("账户创建成功");
            

        }

    }
}
