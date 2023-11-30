using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.Manage.View;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel
{
    /// <summary>
    /// 实时指令数据ViewModel
    /// </summary>
    public class HotOrderViewModel : ViewModelBase
    {
        public OrderManage OrderManage { get; set; }
        OrderAddView _ucOrderAdd;

        public HotOrderViewModel()
        {
            OrderManage = DependencyHelper.GetService<OrderManage>();
            OrderManage.NotifyMsgToUiEvent += ShowLogMessage;

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

        protected void ShowLogMessage(string msg, EnumLogLevel level)
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(new Action<string, EnumLogLevel>((m, l) =>
                {
                    ClearLogMsg();
                    UcStationLogInfoModel logMsg = new UcStationLogInfoModel
                    {
                        DateTime = DateTime.Now.ToString("MM/dd HH:mm:ss"),
                        Content = m,
                        Level = level
                    };
                    LogInfoModelLst.Add(logMsg);
                }), DispatcherPriority.Background, msg, level);
        }
        private ObservableCollection<UcStationLogInfoModel> _logInfoModelLst = new ObservableCollection<UcStationLogInfoModel>();
        public ObservableCollection<UcStationLogInfoModel> LogInfoModelLst
        {
            get { return _logInfoModelLst; }
            set
            {
                if (_logInfoModelLst != value)
                {
                    _logInfoModelLst = value;
                    RaisePropertyChanged();
                }
            }
        }


        private void ClearLogMsg()
        {
            lock (LogInfoModelLst)
            {
                if (LogInfoModelLst.Count >= 30)//100改30 不用太多日志
                {
                    LogInfoModelLst.RemoveAt(0);
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


        private RelayCommand _addOrderDataCommand;
        /// <summary>
        /// 添加数据 
        /// </summary>
        public RelayCommand AddOrderDataCommand
        {
            get
            {
                if (_addOrderDataCommand == null)
                {
                    _addOrderDataCommand = new RelayCommand(SwitchToAddOrderData);
                }
                return _addOrderDataCommand;
            }
        }

        /// <summary>
        /// 添加 指令数据
        /// </summary>
        private void SwitchToAddOrderData()
        {
            _ucOrderAdd = new OrderAddView();
            AddOrderViewModel exorderModel = new AddOrderViewModel(OrderManage);
            exorderModel.ChangeOrderDataEvent += ExorderModel_ChangeOrderDataEvent;//事件注册
            _ucOrderAdd.DataContext = exorderModel;
            _ucOrderAdd.Show();
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

                ExOrderDetailView exOrderDetailView=new ExOrderDetailView(exorder);
                
                await MaterialDesignThemes.Wpf.DialogHost.Show(exOrderDetailView, "DialogHostWait");
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show("打开指令修改界面失败:" + OperateResult.ConvertException(ex));
            }
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void ExorderModel_ChangeOrderDataEvent()
        {
            _ucOrderAdd.Close();
        }



        private RelayCommand _pauseCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                {
                    _pauseCommand = new RelayCommand(PauseAndRun);
                }
                return _pauseCommand;
            }
        }

        private void PauseAndRun()
        {
            if (OrderManage.CurRunState.Equals(RunStateMode.Run))
            {
                OperateLogHelper.Record("暂停处理指令");
                OrderManage.Pause();
            }
            else if (OrderManage.CurRunState.Equals(RunStateMode.Pause))
            {
                OperateLogHelper.Record("启动处理指令");
                OrderManage.Run();
            }
        }


        private RelayCommand _resetCommand;
        /// <summary>
        /// 查询数据 
        /// </summary>
        public RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(Reset);
                }
                return _resetCommand;
            }
        }

        private void Reset()
        {
            OperateResult resetResult = OrderManage.Reset();
            if (resetResult.IsSuccess)
            {
                OperateLogHelper.Record("复位处理指令成功");
                SnackbarQueue.MessageQueue.Enqueue("复位成功");
            }
            else
            {
                OperateLogHelper.Record("复位处理指令失败");
                SnackbarQueue.MessageQueue.Enqueue(string.Format("复位失败:{0}", resetResult.Message));
            }
        }
        private MyCommand _cmdOrderManageOpenLog;
        /// <summary>
        /// 打开当前日志
        /// </summary>
        public MyCommand CmdOrderManageOpenLog
        {
            get
            {
                if (_cmdOrderManageOpenLog == null)
                    _cmdOrderManageOpenLog = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              LogManageHelper.Instance.OpenLogFile(OrderManage.Name);
                          }
                        ));
                return _cmdOrderManageOpenLog;
            }
        }



    }
}
