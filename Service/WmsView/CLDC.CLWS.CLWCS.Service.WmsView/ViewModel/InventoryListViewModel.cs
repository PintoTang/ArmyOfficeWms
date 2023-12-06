using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.CLWS.CLWCS.Service.WmsView.View;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class InventoryListViewModel : ViewModelBase
    {
        private string _curMaterial = string.Empty;
        private string _curArea;
        private InvStatusEnum? _curInvStatus;
        private int? _curSearchGroupId;
        private AuthorizeService _authorizeService;
        private WmsDataService _wmsDataService;
        public ObservableCollection<Inventory> InventoryList { get; set; }
        public ObservableCollection<Area> AreaList { get; set; }


        public InventoryListViewModel()
        {
            InventoryList = new ObservableCollection<Inventory>();
            AreaList = new ObservableCollection<Area>();
            _authorizeService = DependencyHelper.GetService<AuthorizeService>();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            InitCbArea();
        }

        private void InitCbArea()
        {
            AreaList.Clear();
            try
            {
                var where = CombineSearchSql();
                List<Area> accountListResult = _wmsDataService.GetAreaList(string.Empty);
                if (accountListResult.Count > 0)
                {
                    accountListResult.ForEach(ite => AreaList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
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

        private readonly Dictionary<InvStatusEnum, string> _invStatusDict = new Dictionary<InvStatusEnum, string>();
        public Dictionary<InvStatusEnum, string> InvStatusDict
        {
            get
            {
                if (_invStatusDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(InvStatusEnum)))
                    {
                        InvStatusEnum em = (InvStatusEnum)value;
                        _invStatusDict.Add(em, em.GetDescription());
                    }
                }
                return _invStatusDict;

            }
        }

        /// <summary>
        /// 当前搜索的装备
        /// </summary>
        public string CurMaterial
        {
            get { return _curMaterial; }
            set
            {
                _curMaterial = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的区域
        /// </summary>
        public string CurArea
        {
            get { return _curArea; }
            set
            {
                _curArea = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的使用状态
        /// </summary>
        public InvStatusEnum? CurInvStatus
        {
            get { return _curInvStatus; }
            set
            {
                _curInvStatus = value;
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
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(CurMaterial))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

            InventoryList.Clear();
            try
            {
                var where = CombineSearchSql();
                OperateResult<List<Inventory>> accountListResult = _wmsDataService.GetInventoryPageList(where);
                if (!accountListResult.IsSuccess)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                    return;
                }
                if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                {
                    accountListResult.Content.ForEach(ite => InventoryList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private Expression<Func<Inventory, bool>> CombineSearchSql()
        {
            int curRoleLevel = (int)CookieService.CurSession.RoleLevel;
            Expression<Func<Inventory, bool>> whereLambda = t => t.IsDeleted == false;
            if (!string.IsNullOrEmpty(CurMaterial))
            {
                whereLambda = whereLambda.AndAlso(t => t.MaterialDesc.Contains(CurMaterial));
            }
            if (!string.IsNullOrEmpty(CurArea))
            {
                whereLambda = whereLambda.AndAlso(t => t.AreaCode == CurArea);
            }
            if (CurInvStatus.HasValue)
            {
                whereLambda = whereLambda.AndAlso(t => t.Status == (int?)CurInvStatus.Value);
            }
            //if (CurSearchGroupId.HasValue)
            //{
            //    whereLambda = whereLambda.AndAlso(t => t.GroupId == CurSearchGroupId.Value);
            //}
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
