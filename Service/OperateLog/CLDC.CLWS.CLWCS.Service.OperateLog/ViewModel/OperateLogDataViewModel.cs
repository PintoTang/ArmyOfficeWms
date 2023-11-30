using System;
using System.Collections.Generic;
using System.Globalization;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.Service.OperateLog.ViewModel
{
    public class OperateLogDataViewModel : ViewModelBase
    {
        private readonly OperateLogDataAbstract _operateLogDataHandle;
        private readonly UcSplitPagerViewModel _pageViewModel;
        public OperateLogDataViewModel(string serviceName)
        {
            ServiceName = serviceName;
            _operateLogDataHandle = DependencyHelper.GetService<OperateLogDataAbstract>();
            _pageViewModel = new UcSplitPagerViewModel();
            _pageViewModel.PageChange += UpdateData;
            UpdateData();
        }
        public string ServiceName { get; set; }
        public UcSplitPagerViewModel PageViewModel
        {
            get { return _pageViewModel; }
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
        //private void InitPage()
        //{
        //    long totalCount = _operateLogDataHandle.GetTotalCount();
        //    PageViewModel.TotalCount = totalCount;
        //}

        public List<OperateLogModel> CurOperateLogDataList
        {
            get { return _curOperateLogDataList; }
            set
            {
                _curOperateLogDataList = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _searchDataCommand;
        private string _logContent = string.Empty;
        private string _logUserAccount;
        private string _recordFromTime = string.Empty;
        private string _recordToTime = string.Empty;
        private List<OperateLogModel> _curOperateLogDataList;
        private string _logUserName=string.Empty;

        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand SearchDataCommand
        {
            get
            {
                if (_searchDataCommand == null)
                {
                    _searchDataCommand = new RelayCommand(UpdateData);
                }
                return _searchDataCommand;
            }
        }

        private void UpdateData()
        {
            OperateLogFilter filterModel = CombineFilter(PageViewModel.PageIndex, PageViewModel.PageSize);
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.LogUserName) ||
                       ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.LogUserAccount) ||
                       ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.LogContent))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

            //InitPage();
            CurOperateLogDataList = _operateLogDataHandle.SelectData(filterModel,out int totalCount);
            PageViewModel.TotalCount = totalCount;
        }




        private OperateLogFilter CombineFilter(long pagIndex, long pageSize)
        {
            OperateLogFilter filterModel = new OperateLogFilter();
            filterModel.LogUserAccount = this.LogUserAccount;

            filterModel.LogUserName = this.LogUserName;

            filterModel.LogContent = this.LogContent;
            filterModel.RecordFromTime = this.RecordFromTime;
            filterModel.RecordToTime = this.RecordToTime;
            filterModel.PageIndex = pagIndex;
            filterModel.PageSize = pageSize;
            return filterModel;
        }

        public string LogUserName
        {
            get { return _logUserName; }
            set { _logUserName = value; }
        }

        public string LogContent
        {
            get { return _logContent; }
            set
            {
                _logContent = value;
                RaisePropertyChanged();
            }
        }

        public string LogUserAccount
        {
            get { return _logUserAccount; }
            set
            {
                _logUserAccount = value;
                RaisePropertyChanged();
            }
        }

        public string RecordFromTime
        {
            get { return _recordFromTime; }
            set
            {
                _recordFromTime = value;
                RaisePropertyChanged();
            }
        }

        public string RecordToTime
        {
            get { return _recordToTime; }
            set
            {
                _recordToTime = value;
                RaisePropertyChanged();
            }
        }
    }
}
