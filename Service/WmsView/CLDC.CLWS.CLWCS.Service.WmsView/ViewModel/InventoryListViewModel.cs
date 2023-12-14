using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
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

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    /// <summary>
    /// 库存管理VM
    /// </summary>
    public class InventoryListViewModel : ViewModelBase
    {
        private string _curMaterial = string.Empty;
        private string _curArea;
        private InvStatusEnum? _curInvStatus;
        private string _curTaskType;
        private WmsDataService _wmsDataService;
        public ObservableCollection<Inventory> InventoryList { get; set; }
        public ObservableCollection<Area> AreaList { get; set; }

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
        /// 当前搜索的库存状态
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
        /// 当前搜索的任务分类
        /// </summary>
        public string CurTaskType
        {
            get { return _curTaskType; }
            set
            {
                _curTaskType = value;
                RaisePropertyChanged();
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

        public InventoryListViewModel()
        {
            InventoryList = new ObservableCollection<Inventory>();
            AreaList = new ObservableCollection<Area>();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            InitCbArea(); 
        }

        private void InitCbArea()
        {
            AreaList.Clear();
            try
            {
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
                whereLambda = whereLambda.AndAlso(t => t.Status == CurInvStatus.Value);
            }
            return whereLambda;
        }

    }
}
