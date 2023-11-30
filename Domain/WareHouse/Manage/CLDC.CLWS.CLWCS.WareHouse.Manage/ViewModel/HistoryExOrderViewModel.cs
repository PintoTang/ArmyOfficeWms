using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Manage.View;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.ViewModel.Page;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel
{
    public class HistoryExOrderViewModel : ViewModelBase
    {
        private readonly OrderManage _orderManage;
        private UcSplitPagerViewModel _pageViewModel;
        public UcSplitPagerViewModel PageViewModel
        {
            get { return _pageViewModel; }
            set
            {
                _pageViewModel = value;
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


        private MyCommand _openExOrderCommand;
        public MyCommand OpenExOrderCommand
        {
            get
            {
                if (_openExOrderCommand == null)
                    _openExOrderCommand = new MyCommand(OpenExOrderView);
                return _openExOrderCommand;
            }
        }

        /// <summary>
        /// 编辑 
        /// </summary>
        /// <param name="obj">DataGrid选中的行 ExOrder 对象</param>
        private async void OpenExOrderView(object obj)
        {
            try
            {
                //实时数据移除
                var exorder = obj as ExOrder;

                if (exorder == null) return;

                ExOrderDetailView exOrderDetailView = new ExOrderDetailView(exorder);

                await MaterialDesignThemes.Wpf.DialogHost.Show(exOrderDetailView, "DialogHostWait");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("打开指令修改界面失败:" + OperateResult.ConvertException(ex));
            }
        }
        public HistoryExOrderViewModel()
        {
            _orderManage = DependencyHelper.GetService<OrderManage>();

            PageViewModel = new UcSplitPagerViewModel();
            PageViewModel.PageChange += InitOrderList;
            //加载历史数据
        }
        //private List<ExOrder> _orderList = new List<ExOrder>();
        //public List<ExOrder> OrderList
        //{
        //    set
        //    {
        //        _orderList = value;
        //        RaisePropertyChanged("TotalSize");
        //    }
        //    get { return _orderList; }
        //}


        private List<ExOrder> _currentShowlist = new List<ExOrder>();
        public List<ExOrder> CurrentShowList
        {
            set
            {
                _currentShowlist = value;
                RaisePropertyChanged();
            }
            get { return _currentShowlist; }
        }

        /// <summary>
        /// 初始化 计算总页数
        /// </summary>
        private void InitOrderList()
        {
            this.CurrentShowList = _orderManage.GetHisData((int)PageViewModel.PageIndex,(int)PageViewModel.PageSize,S_OrderID, S_PackNum, S_CurAddress, S_NextAddress,
                           S_OrderStatu, S_OrderType, s_OrderAddStartTime, s_OrderAddEndTime,out int totalCount);
            PageViewModel.TotalCount = totalCount;
            //if (OrderList == null || OrderList.Count == 0)
            //{
            //    return;
            //}
            //if (OrderList.Count % PageViewModel.PageSize == 0)
            //{
            //    PageViewModel.TotalPageSize = OrderList.Count / PageViewModel.PageSize;
            //}
            //else
            //{
            //    PageViewModel.TotalPageSize = OrderList.Count / PageViewModel.PageSize + 1;
            //}
            //if (OrderList.Count > PageViewModel.PageSize)
            //{
            //    if (OrderList == null) return;
            //    var query = from order in OrderList select order;
            //    var collection = query.Skip((int)(PageViewModel.PageIndex * PageViewModel.PageSize)).Take((int)PageViewModel.PageSize);
            //    CurrentShowList = collection.ToList();
            //}
            //else
            //{
            //    CurrentShowList = OrderList;
            //}
            //PageViewModel.TotalCount = OrderList.Count;
        }
     
        /// <summary>
        /// 加载历史数据
        /// </summary>
        //private Task  LoadData()
        //{
        //    var result = Task.Run(() =>
        //    {
               
        //        OrderList = _orderManage.GetHisData(S_OrderID, S_PackNum, S_CurAddress, S_NextAddress,
        //               S_OrderStatu, S_OrderType, s_OrderAddStartTime, s_OrderAddEndTime);
        //    });
        //    return result;
        //}
        //public async Task GetLoadValueAsync()
        //{
        //   await Task.Run(() => LoadData());
        //}

        #region property

        //查询条件
        private string s_OrderID;
        /// <summary>
        /// 指令ID
        /// </summary>
        public string S_OrderID
        {
            set
            {
                s_OrderID = value;
                RaisePropertyChanged();
            }
            get { return s_OrderID; }
        }

        /// <summary>
        /// 包号
        /// </summary>
        private string s_PackNum;
        public string S_PackNum
        {
            set
            {
                s_PackNum = value;
                RaisePropertyChanged();
            }
            get { return s_PackNum; }
        }
        private string s_CurAddress;
        public string S_CurAddress
        {
            set
            {
                s_CurAddress = value;
                RaisePropertyChanged();
            }
            get { return s_CurAddress; }
        }

        private string s_NextAddress;
        public string S_NextAddress
        {
            set
            {
                s_NextAddress = value;
                RaisePropertyChanged();
            }
            get { return s_NextAddress; }
        }
        private string s_OrderStatu;
        public string S_OrderStatu
        {
            set
            {
                s_OrderStatu = value;
                RaisePropertyChanged();
            }
            get { return s_OrderStatu; }
        }
        //S_OrderType
        private string s_OrderType;
        public string S_OrderType
        {
            set
            {
                s_OrderType = value;
                RaisePropertyChanged();
            }
            get { return s_OrderType; }
        }

        private string s_OrderAddStartTime;
        public string S_OrderAddStartTime
        {
            set
            {
                s_OrderAddStartTime = value;
                RaisePropertyChanged();
            }
            get { return s_OrderAddStartTime; }
        }

        private string s_OrderAddEndTime;
        public string S_OrderAddEndTime
        {
            set
            {
                s_OrderAddEndTime = value;
                RaisePropertyChanged();
            }
            get { return s_OrderAddEndTime; }
        }


        private List<string> _orderStatuLst = new List<string>();
        /// <summary>
        /// 指令状态列表
        /// </summary>
        public List<string> OrderStatuLst
        {
            get
            {
                if (_orderStatuLst.Count == 0)
                {
                    string[] nameArrList = System.Enum.GetNames(typeof(StatusEnum));
                    foreach (string strName in nameArrList)
                    {
                        Enum em = (StatusEnum)Enum.Parse(typeof(StatusEnum), strName);
                        _orderStatuLst.Add(em.GetDescription());
                    }
                }
                return _orderStatuLst;
            }

        }

        private List<string> _orderTypeLst = new List<string>();
        /// <summary>
        /// 指令类型列表
        /// </summary>
        public List<string> OrderTypeLst
        {
            get
            {
                if (_orderTypeLst.Count == 0)
                {
                    string[] nameArrList = System.Enum.GetNames(typeof(OrderTypeEnum));
                    foreach (string strName in nameArrList)
                    {
                        Enum em = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), strName);
                        _orderTypeLst.Add(em.GetDescription());
                    }
                }
                return _orderTypeLst;
            }
        }

      

     

        private RelayCommand searchOrderDataCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand SearchOrderDataCommand
        {
            get
            {
                if (searchOrderDataCommand == null)
                {
                    searchOrderDataCommand = new RelayCommand(SwitchToSearchData);
                }
                return searchOrderDataCommand;
            }
        }
    
        #endregion property

        /// <summary>
        /// 查询数据
        /// </summary>
        private async void SwitchToSearchData()
        {
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(S_OrderID) ||
                  ValidData.CheckSearchParmsLenAndSpecialCharts(S_PackNum) ||
                  ValidData.CheckSearchParmsLenAndSpecialCharts(S_CurAddress) ||
                  ValidData.CheckSearchParmsLenAndSpecialCharts(S_NextAddress)
                  )
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

           //await LoadData();
            InitOrderList();
        }


    }
}
