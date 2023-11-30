using System;
using System.Collections.ObjectModel;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel
{
    public sealed class StringCharTaskViewModel
    {
        public ObservableCollection<StringCharTask> UnFinishTaskList { get; set; }

        public Func<StringCharTask,OperateResult> FinishTask;

        public Func<StringCharTask, OperateResult> DoTask;

        private MyCommand _forceFinishTaskCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand ForceFinishTaskCommand
        {
            get
            {
                if (_forceFinishTaskCommand == null)
                    _forceFinishTaskCommand = new MyCommand(ForceFinishTask);
                return _forceFinishTaskCommand;
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


        private void ForceFinishTask(object arg)
        {
            if (!(arg is StringCharTask))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要取消的任务信息");
                return;
            }
            StringCharTask forceFinishTransport = arg as StringCharTask;
            MessageBoxResult result = MessageBoxEx.Show("确定强制完成该搬运动作？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            if (FinishTask == null)
            {
                SnackbarQueue.MessageQueue.Enqueue("FinishTask的方法还没有注册");
                return;
            }
            OperateResult forceFinishResult = FinishTask(forceFinishTransport);
            if (!forceFinishResult.IsSuccess)
            {
                string msg = string.Format("任务强制完成失败，原因：{0}", forceFinishResult.Message);
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("任务强制完成成功");
            }
        }

        private MyCommand _reDoTaskCommand;

        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand ReDoTaskCommand
        {
            get
            {
                if (_reDoTaskCommand == null)
                    _reDoTaskCommand = new MyCommand(ReDoTask);
                return _reDoTaskCommand;
            }
        }

        private void ReDoTask(object arg)
        {
            if (!(arg is StringCharTask))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要重新下发的任务信息");
                return;
            }
            StringCharTask reDoTask = arg as StringCharTask;
            MessageBoxResult result = MessageBoxEx.Show("确定重新下发任务信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            if (DoTask == null)
            {
                SnackbarQueue.MessageQueue.Enqueue("DoTask的方法还没有注册");
                return;
            }
            OperateResult reDoResult = DoTask(reDoTask);
            if (!reDoResult.IsSuccess)
            {
                string msg = string.Format("任务信息重新下发失败，原因：{0}", reDoResult.Message);
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("任务信息重新下发成功");
            }

        }
    }
}
