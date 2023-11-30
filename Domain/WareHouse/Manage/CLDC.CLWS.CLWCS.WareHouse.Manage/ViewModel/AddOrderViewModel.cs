using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage.ViewModel
{
    public class AddOrderViewModel : ViewModelBase
    {
        private readonly OrderManage _orderManage;
        private IWmsWcsArchitecture _architecture;
        public AddOrderViewModel(OrderManage orderManage)
        {
            _orderManage = orderManage;
            _architecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
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
        //定义委托 指令数据发生改变 (添加、修改)窗体关闭 事件订阅
        public delegate void ChangeOrderDataHandler();

        public event ChangeOrderDataHandler ChangeOrderDataEvent;
        private bool isFrmClose = false;

        /// <summary>
        /// 窗体是否关闭
        /// </summary>
        public bool IsFrmClose
        {
            set
            {
                isFrmClose = value;
                if (IsFrmClose)
                {
                    //关闭窗体 
                }

                RaisePropertyChanged();
            }
            get { return isFrmClose; }
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        public EventHandler RequestClose;

        public void CloseView()
        {
            if (RequestClose != null)
            {
                RequestClose(this, null);
            }
        }

        private ExOrder _exOrder = new ExOrder();

        public ExOrder ExOrder
        {
            get { return _exOrder; }
            set
            {
                _exOrder = value;
                RaisePropertyChanged();
            }
        }

        private readonly Dictionary<StatusEnum, string> _dicEmStatus = new Dictionary<StatusEnum, string>();

        public Dictionary<StatusEnum, string> DicEmStatus
        {
            get
            {
                if (_dicEmStatus.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(StatusEnum)))
                    {
                        Enum em = (StatusEnum)Enum.Parse(typeof(StatusEnum), strName);
                        _dicEmStatus.Add((StatusEnum)em, em.GetDescription());
                    }
                }
                return _dicEmStatus;
            }
        }


        public List<string> AddrList
        {
            get { return _architecture.GetAllShowName().Take(150).ToList(); }
        }

        private readonly Dictionary<OrderTypeEnum, string> _dicEmOrderStatu = new Dictionary<OrderTypeEnum, string>();

        public Dictionary<OrderTypeEnum, string> DicEmOrderStatu
        {
            get
            {
                if (_dicEmOrderStatu.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(OrderTypeEnum)))
                    {
                        Enum em = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), strName);
                        _dicEmOrderStatu.Add((OrderTypeEnum)em, em.GetDescription());
                    }
                }
                return _dicEmOrderStatu;
            }
        }

        private RelayCommand _addOrderDataCommand;
        public RelayCommand AddOrderDataCommand
        {
            get
            {
                if (_addOrderDataCommand == null)
                {
                    _addOrderDataCommand = new RelayCommand(AddOrderData);
                }
                return _addOrderDataCommand;
            }
        }


        /// <summary>
        /// 添加数据到
        /// </summary>
        private void AddOrderData()
        {
            try
            {
                if (ExOrder == null) return;
                MessageBoxResult rt = MessageBoxEx.Show("您确认添加？", "警告", MessageBoxButton.YesNo);

                if (rt.Equals(MessageBoxResult.Yes))
                {
                    //添加真实处理命令
                    OperateResult<Order> opResult = _orderManage.GenerateOrder(ExOrder);
                    SnackbarQueue.MessageQueue.Enqueue("指令添加:" + GetShowResultMsgByOperateResult(opResult));

                    string recordMsg = string.Format("指令添加：{0} 结果：成功", opResult.Content);

                    OperateLogHelper.Record(recordMsg);

                    IsFrmClose = true;
                    if (ChangeOrderDataEvent != null)
                    {
                        ChangeOrderDataEvent(); //调用 事件通知 
                    }
                }
            }
            catch (Exception ex)
            {
                OperateLogHelper.Record("指令添加异常:" + ex.ToString());

                MessageBoxEx.Show("指令添加失败:" + ex.ToString());
                IsFrmClose = true;
            }
        }

        /// <summary>
        /// 根据 OperateResult 显示结果信息
        /// </summary>
        /// <param name="opResult"></param>
        /// <returns></returns>
        private string GetShowResultMsgByOperateResult(OperateResult opResult)
        {
            string strShowMsg = "";
            if (opResult.IsSuccess)
            {
                strShowMsg = "成功";
            }
            else
            {
                strShowMsg = "失败";
            }
            if (!string.IsNullOrEmpty(opResult.Message))
            {
                strShowMsg = opResult.Message;
            }
            return strShowMsg;
        }
    }

}
