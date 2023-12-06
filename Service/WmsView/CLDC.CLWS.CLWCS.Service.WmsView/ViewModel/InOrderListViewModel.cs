using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.Authorize.View;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.UserCtrl;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CLDC.CLWS.CLWCS.Service.WmsView.View;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class InOrderListViewModel : ViewModelBase
    {
        private string _curSearchAccCode = string.Empty;
        private RoleLevelEnum? _curSearchRoleLevel;
        private AccountStatusEnum? _curAccountUseStatus;
        private int? _curSearchGroupId;
        private AuthorizeService _authorizeService;

        public InOrderListViewModel()
        {
            AccountList = new ObservableCollection<AccountMode>();
            _authorizeService = DependencyHelper.GetService<AuthorizeService>();
        }
        public ObservableCollection<AccountMode> AccountList { get; set; }

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
                        _roleLevelTypeDic.Add(em, em.GetDescription());
                    }
                }
                return _roleLevelTypeDic;

            }
        }

        private readonly Dictionary<AccountStatusEnum, string> _useStatusTypeDic = new Dictionary<AccountStatusEnum, string>();
        public Dictionary<AccountStatusEnum, string> UseStatusTypeDic
        {
            get
            {
                if (_useStatusTypeDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(AccountStatusEnum)))
                    {
                        AccountStatusEnum em = (AccountStatusEnum)value;
                        _useStatusTypeDic.Add(em, em.GetDescription());
                    }
                }
                return _useStatusTypeDic;

            }
        }

        /// <summary>
        /// 当前搜索的账号
        /// </summary>
        public string CurSearchAccCode
        {
            get { return _curSearchAccCode; }
            set
            {
                _curSearchAccCode = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的等级
        /// </summary>
        public RoleLevelEnum? CurSearchRoleLevel
        {
            get { return _curSearchRoleLevel; }
            set
            {
                _curSearchRoleLevel = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的使用状态
        /// </summary>
        public AccountStatusEnum? CurAccountUseStatus
        {
            get { return _curAccountUseStatus; }
            set
            {
                _curAccountUseStatus = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的组编号
        /// </summary>
        public int? CurSearchGroupId
        {
            get { return _curSearchGroupId; }
            set
            {
                _curSearchGroupId = value;
                RaisePropertyChanged();
            }
        }

        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }


        private RelayCommand _searchCommand;

        public RelayCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(Search);
                }
                return _searchCommand;
            }
        }

        private RelayCommand _deleteAccountCommand;

        public RelayCommand DeleteAccountCommand
        {
            get
            {
                if (_deleteAccountCommand == null)
                {
                    _deleteAccountCommand = new RelayCommand(DeleteAccount);
                }
                return _deleteAccountCommand;
            }
        }

        private RelayCommand _editAcountCommand;

        public RelayCommand EditAccountCommand
        {
            get
            {
                if (_editAcountCommand == null)
                {
                    _editAcountCommand = new RelayCommand(EditAccount);
                }
                return _editAcountCommand;
            }
        }

        private void EditAccount()
        {
            if (SelectedValue == null)
            {
                SnackbarQueue.Enqueue("请选择需要编辑的用户");
                return;
            }
            MessageBoxResult msgResult = MessageBoxEx.Show(string.Format("确定编辑用户：{0}", SelectedValue.AccCode), "警告", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            OperateResult editResult = _authorizeService.EditAccount(SelectedValue);
            SnackbarQueue.Enqueue(string.Format("操作结果：{0}", editResult.Message));
            Search();
        }

        private void DeleteAccount()
        {
            if (SelectedValue == null)
            {
                SnackbarQueue.Enqueue("请选择需要删除的用户");
                return;
            }
            MessageBoxResult msgResult = MessageBoxEx.Show(string.Format("确定删除用户：{0}", SelectedValue.AccCode), "警告", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            OperateResult deleteResult = _authorizeService.DeleteAccount(SelectedValue);
            SnackbarQueue.Enqueue(string.Format("操作结果：{0}", deleteResult.Message));
            Search();
        }

        /// <summary>
        /// 当前选中的用户信息
        /// </summary>
        public AccountMode SelectedValue { get; set; }


        private void Search()
        {
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(CurSearchAccCode))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

            AccountList.Clear();
            try
            {
                var where = CombineSearchSql();
                OperateResult<List<AccountMode>> accountListResult = _authorizeService.GetAccountList(where);
                if (!accountListResult.IsSuccess)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                    return;
                }
                if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                {
                    accountListResult.Content.ForEach(ite => AccountList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private Expression<Func<AccountMode, bool>> CombineSearchSql()
        {
            int curRoleLevel = (int)CookieService.CurSession.RoleLevel;
            Expression<Func<AccountMode, bool>> whereLambda = t => (int)t.RoleLevel < curRoleLevel;
            //string searchSql = "SELECT *FROM T_ST_ACCOUNT WHERE 1=1 AND ROLE_ID <" + "'" + curRoleLevel + "'";
            if (!string.IsNullOrEmpty(CurSearchAccCode))
            {
                whereLambda = whereLambda.AndAlso(t => t.AccCode == CurSearchAccCode);
            }
            if (CurSearchRoleLevel.HasValue)
            {
                whereLambda = whereLambda.AndAlso(t => t.RoleLevel == CurSearchRoleLevel.Value);
            }
            if (CurAccountUseStatus.HasValue)
            {
                whereLambda = whereLambda.AndAlso(t => t.UseStatus == CurAccountUseStatus.Value);
            }
            if (CurSearchGroupId.HasValue)
            {
                whereLambda = whereLambda.AndAlso(t => t.GroupId == CurSearchGroupId.Value);
            }
            return whereLambda;
        }


        private RelayCommand _createInOrderCommand;

        public RelayCommand CreateInOrderCommand
        {
            get
            {
                if (_createInOrderCommand == null)
                {
                    _createInOrderCommand = new RelayCommand(CreateNewInOrder);
                }
                return _createInOrderCommand;
            }
        }

        private async void CreateNewInOrder()
        {
            CreateInOrderView createAccountView = new CreateInOrderView();
            await MaterialDesignThemes.Wpf.DialogHost.Show(createAccountView, "DialogHostWait");
        }



    }
}
