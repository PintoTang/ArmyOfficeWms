using System;
using System.Collections.Generic;
using System.Globalization;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.ViewModel
{
    public class ReceiveDataViewModel : ViewModelBase
    {
        private readonly ReceiveDataAbstract _receiveDataHandle;
        private readonly UcSplitPagerViewModel _pageViewModel;
        public ReceiveDataViewModel(string serviceName)
        {
            ServiceName = serviceName;
            _receiveDataHandle = DependencyHelper.GetService<ReceiveDataAbstract>();
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
        //    long totalCount = _receiveDataHandle.GetTotalCount();
        //    PageViewModel.TotalCount = totalCount;
        //}

        public List<ReceiveDataModel> CurReceiveDataList
        {
            get { return _curReceiveDataList; }
            set
            {
                _curReceiveDataList = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _searchDataCommand;
        private string _methodName = string.Empty;
        private string _handleStatus;
        private string _receiveFromTime = string.Empty;
        private string _receiveToTime = string.Empty;
        private string _handleFromTime = string.Empty;
        private string _handleToTime = string.Empty;
        private List<ReceiveDataModel> _curReceiveDataList;

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
            ReceiveDataFilter filterModel = CombineFilter(PageViewModel.PageIndex, PageViewModel.PageSize);
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.MethodName))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

            //InitPage();
            CurReceiveDataList = _receiveDataHandle.SelectData(filterModel,out int totalCount);
            PageViewModel.TotalCount = totalCount;


        }

        private readonly Dictionary<string, string> _dicHandleStatusEnum = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> DicHandleStatusEnum
        {
            get
            {
                if (_dicHandleStatusEnum.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(ReceiveDataHandleStatus)))
                    {
                        ReceiveDataHandleStatus em = (ReceiveDataHandleStatus)Enum.Parse(typeof(ReceiveDataHandleStatus), strName);
                        _dicHandleStatusEnum.Add(((int)em).ToString(), em.GetDescription());
                    }
                }
                return _dicHandleStatusEnum;
            }
        }


        private ReceiveDataFilter CombineFilter(long pagIndex, long pageSize)
        {
            ReceiveDataFilter filterModel = new ReceiveDataFilter();
            filterModel.HandleFromTime = this.HandleFromTime;

            filterModel.HandleStatus = HandleStatus;

            filterModel.HandleToTime = this.HandleToTime;
            filterModel.MethodName = this.MethodName;
            filterModel.ReceiveFromTime = this.ReceiveFromTime;
            filterModel.ReceiveToTime = this.ReceiveToTime;
            filterModel.PageIndex = pagIndex;
            filterModel.PageSize = pageSize;
            return filterModel;
        }

        public string MethodName
        {
            get { return _methodName; }
            set
            {
                _methodName = value;
                RaisePropertyChanged();
            }
        }

        public string HandleStatus
        {
            get { return _handleStatus; }
            set
            {
                _handleStatus = value;
                RaisePropertyChanged();
            }
        }

        public string ReceiveFromTime
        {
            get { return _receiveFromTime; }
            set
            {
                _receiveFromTime = value;
                RaisePropertyChanged();
            }
        }

        public string ReceiveToTime
        {
            get { return _receiveToTime; }
            set
            {
                _receiveToTime = value;
                RaisePropertyChanged();
            }
        }

        public string HandleFromTime
        {
            get { return _handleFromTime; }
            set
            {
                _handleFromTime = value;
                RaisePropertyChanged();
            }
        }

        public string HandleToTime
        {
            get { return _handleToTime; }
            set
            {
                _handleToTime = value;
                RaisePropertyChanged();
            }
        }
    }
}
