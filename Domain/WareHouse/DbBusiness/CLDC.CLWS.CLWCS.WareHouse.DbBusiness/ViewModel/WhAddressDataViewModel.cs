using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using MaterialDesignThemes.Wpf;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.ViewModel
{
    public class WhAddressDataViewModel : ViewModelBase
    {
        private readonly WhAddressDataAbstract _whAddressDataHandle;
        private readonly UcSplitPagerViewModel _pageViewModel;
        public WhAddressDataViewModel(string serviceName)
        {
            ServiceName = serviceName;
            _whAddressDataHandle = DependencyHelper.GetService<WhAddressDataAbstract>();
            _pageViewModel = new UcSplitPagerViewModel();
            _pageViewModel.PageChange += UpdateData;
            UpdateData();
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

        private RelayCommand _searchDataCommand;

        private RelayCommand _modifyDataCommand;

        /// <summary>
        /// 更改数据 
        /// </summary>
        public RelayCommand ModifyDataCommand
        {
            get
            {
                if (_modifyDataCommand == null)
                {
                    _modifyDataCommand = new RelayCommand(ModifyData);
                }
                return _modifyDataCommand;
            }
        }


        private void ModifyData()
        {
            if (SelectedData != null)
            {
                if (string.IsNullOrEmpty(SelectedData.WcsAddr))
                {
                    SnackbarQueue.MessageQueue.Enqueue("Wcs地址项不能为空");
                    return;
                }
                OperateResult saveResult = _whAddressDataHandle.Save(SelectedData);
                SnackbarQueue.MessageQueue.Enqueue(string.Format("操作结果：{0}", saveResult.Message));
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要更改的数据");
            }
        }


        private RelayCommand _deleteDataCommand;

        /// <summary>
        /// 删除数据 
        /// </summary>
        public RelayCommand DeleteDataCommand
        {
            get
            {
                if (_deleteDataCommand == null)
                {
                    _deleteDataCommand = new RelayCommand(DeleteData);
                }
                return _deleteDataCommand;
            }
        }



        private void DeleteData()
        {
            MessageBoxResult msgResult = MessageBoxEx.Show("确定删除数据?", "警告", MessageBoxButton.YesNo);
            if (msgResult == MessageBoxResult.No)
            {
                return;
            }
            if (CurWhAddressDataList.Count(d => d.IsSelected) <= 0)
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的数据");
                return;
            }
            CurWhAddressDataList.ForEach(d =>
            {
                if (d.IsSelected)
                {
                     _whAddressDataHandle.Delete(d);
                }
            });
            UpdateData();
            SnackbarQueue.MessageQueue.Enqueue("操作成功");
        }


        private List<WhAddressModel> _curWhAddressDataList = new List<WhAddressModel>();
        private WhAddressModel _selectedData = new WhAddressModel();

        public UcSplitPagerViewModel PageViewModel
        {
            get { return _pageViewModel; }
        }
        //private void InitPage()
        //{
        //    long totalCount = _whAddressDataHandle.GetTotalCount();
        //    PageViewModel.TotalCount = totalCount;
        //}
        private void UpdateData()
        {
            WhAddressDataFilter filterModel = CombineFilter(PageViewModel.PageIndex, PageViewModel.PageSize);
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.LowerAddress) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.ShowName) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.UpperAddress) ||
                ValidData.CheckSearchParmsLenAndSpecialCharts(filterModel.WcsAddress))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

            //InitPage();
            
            CurWhAddressDataList = _whAddressDataHandle.SelectData(filterModel,out int totalCount);
            PageViewModel.TotalCount = totalCount;
        }

        public List<WhAddressModel> CurWhAddressDataList
        {
            get { return _curWhAddressDataList; }
            set
            {
                _curWhAddressDataList = value;
                RaisePropertyChanged();
            }
        }


        public WhAddressModel SelectedData
        {
            get { return _selectedData; }
            set { _selectedData = value; }
        }

        private WhAddressDataFilter CombineFilter(long pagIndex, long pageSize)
        {
            WhAddressDataFilter filterModel = new WhAddressDataFilter();

            filterModel.LowerAddress = this.LowerAddress;
            filterModel.UpperAddress = this.UpperAddress;
            filterModel.ShowName = this.ShowName;
            filterModel.WcsAddress = this.WcsAddress;

            filterModel.PageIndex = pagIndex;
            filterModel.PageSize = pageSize;
            return filterModel;
        }

        public string WcsAddress { get; set; }

        public string ShowName { get; set; }

        public string UpperAddress { get; set; }

        public string LowerAddress { get; set; }


        public string ServiceName { get; set; }
    }
}
