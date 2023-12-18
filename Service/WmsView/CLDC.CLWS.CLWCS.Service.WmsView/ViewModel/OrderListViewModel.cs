using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    public class OrderListViewModel : ViewModelBase
    {
        private WmsDataService _wmsDataService;
        private string _curMaterial = string.Empty;
        private string _curArea;
        private string _curTeam;
        private string _curTaskType;
        public ObservableCollection<Inventory> InventoryList { get; set; }
        public ObservableCollection<Order> OrderDetailList { get; set; }
        public ObservableCollection<Area> AreaList { get; set; }
        public ObservableCollection<Reason> ReasonList { get; set; }
        public ObservableCollection<AreaTeam> TeamList { get; set; }
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
        /// 当前搜索的分队
        /// </summary>
        public string CurTeam
        {
            get { return _curTeam; }
            set
            {
                _curTeam = value;
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
        public ObservableCollection<Order> OrderList { get; set; }

        public OrderListViewModel()
        {
            OrderDetailList = new ObservableCollection<Order>();
            OrderList = new ObservableCollection<Order>();
            ReasonList = new ObservableCollection<Reason>();
            AreaList = new ObservableCollection<Area>();
            TeamList = new ObservableCollection<AreaTeam>();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            InitReasonList();
            InitCbArea();
            InitTeam();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    QureyOrderDetail();
                    Thread.Sleep(5000);//30秒刷新一下
                }
            });
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

        private void InitTeam()
        {
            TeamList.Clear();
            {
                for (int i = 1; i < 4; i++)
                {
                    AreaTeam team = new AreaTeam();
                    team.Id = i; team.Name = i + "排"; team.Remark = string.Empty;
                    TeamList.Add(team);
                }
                TeamList.Add(new AreaTeam { Id = 4, Name = "首长机关" });
                TeamList.Add(new AreaTeam { Id = 5, Name = "民兵" });
            }
        }

        private void InitReasonList()
        {
            ReasonList.Clear();
            try
            {
                if (ReasonConfig.Instance.ReasonList.Count > 0)
                {
                    ReasonConfig.Instance.ReasonList.ForEach(ite => ReasonList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("事由配置文件异常：" + ex.Message);
            }
        }

        private GalaSoft.MvvmLight.Command.RelayCommand<string> _searchCommand;

        public GalaSoft.MvvmLight.Command.RelayCommand<string> SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(Search);
                }
                return _searchCommand;
            }
        }

        private void Search(string inOrOut)
        {
            int inOrOutType=int.Parse(inOrOut);
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(CurMaterial))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }

            OrderList.Clear();
            try
            {
                var where = CombineSearchSql(inOrOutType);
                OperateResult<List<Order>> accountListResult = _wmsDataService.GetInOrderPageList(where);
                if (!accountListResult.IsSuccess)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                    return;
                }
                if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                {
                    accountListResult.Content.ForEach(ite =>
                    {
                        var material = _wmsDataService.GetOrderDetail(ite.OrderSN);
                        var area = _wmsDataService.GetAreaList(ite.AreaCode);
                        ite.MaterialDesc = material?.MaterialDesc;
                        OrderList.Add(ite);
                    });
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }
        private Expression<Func<Order, bool>> CombineSearchSql(int inOrOut)
        {
            Expression<Func<Order, bool>> whereLambda = t => t.IsDeleted == false;
            whereLambda = whereLambda.AndAlso(t => t.InOutType == (InOrOutEnum)inOrOut);
            if (!string.IsNullOrEmpty(CurArea))
            {
                whereLambda = whereLambda.AndAlso(t => t.AreaCode == CurArea);
            }
            if (!string.IsNullOrEmpty(CurTeam))
            {
                whereLambda = whereLambda.AndAlso(t => t.AreaTeam == CurTeam);
            }
            return whereLambda;
        }

        private void QureyOrderDetail()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                OrderDetailList.Clear();
                try
                {
                    OperateResult<List<Order>> accountListResult = _wmsDataService.GetOrderAndMaterList();
                    if (!accountListResult.IsSuccess)
                    {
                        SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                        return;
                    }
                    if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                    {
                        accountListResult.Content.ForEach(ite => OrderDetailList.Add(ite));
                    }
                }
                catch (Exception ex)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
                }
            }));
        }


        private GalaSoft.MvvmLight.Command.RelayCommand<string> _createInOrderCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand<string> CreateInOrderCommand
        {
            get
            {
                if (_createInOrderCommand == null)
                {
                    _createInOrderCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(CreateInOrder);
                }
                return _createInOrderCommand;
            }
        }
        private async void CreateInOrder(string reason)
        {
            if (reason == "1")
            {
                CreateReturnOrderView createReturnOrder = new CreateReturnOrderView();
                await MaterialDesignThemes.Wpf.DialogHost.Show(createReturnOrder, "DialogHostWait");
            }
            else
            {
                CreateNewOrderView createNewOrder = new CreateNewOrderView(reason);
                await MaterialDesignThemes.Wpf.DialogHost.Show(createNewOrder, "DialogHostWait");
            }
        }


        private GalaSoft.MvvmLight.Command.RelayCommand<string> _createOutOrderCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand<string> CreateOutOrderCommand
        {
            get
            {
                if (_createOutOrderCommand == null)
                {
                    _createOutOrderCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(CreateOutOrder);
                }
                return _createOutOrderCommand;
            }
        }
        private async void CreateOutOrder(string reason)
        {
            CreateOutOrderView createOutOrder = new CreateOutOrderView(reason);
            await MaterialDesignThemes.Wpf.DialogHost.Show(createOutOrder, "DialogHostWait");
        }

        
    }
}
