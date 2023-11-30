using System;
using System.Windows;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.ViewModel
{
    public class OrderWorkerViewModel<TOrderWorkerBusiness> : WorkerViewModelAbstract<OrderWorkerAbstract<TOrderWorkerBusiness>> where TOrderWorkerBusiness : OrderWorkerBuinessAbstract
    {
        public OrderWorkerViewModel(OrderWorkerAbstract<TOrderWorkerBusiness> worker)
            : base(worker)
        {

        }


        private MyCommand _cancelUnhandledExOrderCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand CancelUnhandledExOrderCommand
        {
            get
            {
                if (_cancelUnhandledExOrderCommand == null)
                    _cancelUnhandledExOrderCommand = new MyCommand(CancelUnhandledExOrder);
                return _cancelUnhandledExOrderCommand;
            }
        }

        private void CancelUnhandledExOrder(object arg)
        {
            if (!(arg is ExOrder))
            {
                MessageBoxEx.Show("请选择需要取消的指令信息", "提示");
                return;
            }
            ExOrder cancleExOrder = arg as ExOrder;
            MessageBoxResult result = MessageBoxEx.Show("确定取消执行该指令？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            OperateResult cancleResult = Worker.CancelOrder(cancleExOrder);
            if (!cancleResult.IsSuccess)
            {
                string recordMsg = string.Format("取消指令：{0} 失败", cancleExOrder);
                OperateLogHelper.Record(recordMsg);
                string msg = string.Format("指令取消失败，原因：{0}", cancleResult.Message);
                MessageBoxEx.Show(msg, "提示");
            }
            else
            {
                string recordMsg = string.Format("取消指令：{0} 成功", cancleExOrder);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue("指令取消成功");
            }

        }


        private MyCommand _cmdDeviceRunAndPause;
        /// <summary>
        /// 组件开始\暂停
        /// </summary>
        public MyCommand CmdDeviceRunAndPause
        {
            get
            {
                if (_cmdDeviceRunAndPause == null)
                    _cmdDeviceRunAndPause = new MyCommand(o =>
                    {
                        if (Worker != null)
                        {
                            Worker.Pause();
                            string recordMsg = string.Format("暂停：{0} 成功", Worker.Name);
                            OperateLogHelper.Record(recordMsg);
                            SnackbarQueue.MessageQueue.Enqueue("操作成功");
                        }
                    });
                return _cmdDeviceRunAndPause;
            }
        }

        //
        private MyCommand _cmdDeviceReset;
        /// <summary>
        /// 组件复位
        /// </summary>
        public MyCommand CmdDeviceReset
        {
            get
            {
                if (_cmdDeviceReset == null)
                    _cmdDeviceReset = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              if (Worker != null)
                              {
                                  MessageBoxResult rt = MessageBoxEx.Show("您确定要复位组件？", "提示", MessageBoxButton.YesNo);
                                  if (rt != MessageBoxResult.Yes) return;
                                  OperateResult resetResult = Worker.Reset();
                                  if (!resetResult.IsSuccess)
                                  {
                                      string recordMsg = string.Format("复位：{0} 失败", Worker.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      MessageBoxEx.Show(string.Format("组件复位失败，原因：{0}", resetResult.Message), "错误");
                                  }
                                  else
                                  {
                                      string recordMsg = string.Format("复位：{0} 成功", Worker.Name);
                                      OperateLogHelper.Record(recordMsg);

                                      SnackbarQueue.MessageQueue.Enqueue("组件复位成功");
                                  }
                              }
                          }
                        ));
                return _cmdDeviceReset;
            }
        }


        private MyCommand _cmdDeviceStop;
        /// <summary>
        /// 组件停止
        /// </summary>
        public MyCommand CmdDeviceStop
        {
            get
            {
                if (_cmdDeviceStop == null)
                    _cmdDeviceStop = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              if (Worker != null)
                              {
                                  MessageBoxResult rt = MessageBoxEx.Show("您确定要停止组件？", "提示", MessageBoxButton.YesNo);
                                  if (rt != MessageBoxResult.Yes) return;
                                  OperateResult stop = Worker.Stop();
                                  if (!stop.IsSuccess)
                                  {
                                      string recordMsg = string.Format("停止：{0} 失败", Worker.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      MessageBoxEx.Show(string.Format("组件停止失败，原因：{0}", stop.Message), "错误");
                                  }
                                  else
                                  {
                                      string recordMsg = string.Format("停止：{0} 失败", Worker.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      SnackbarQueue.MessageQueue.Enqueue("组件停止成功");
                                  }
                              }
                          }
                        ));
                return _cmdDeviceStop;
            }
        }


        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            if (attributeName.Equals("LogInfoModelLst"))
            {

            }
        }
    }
}
