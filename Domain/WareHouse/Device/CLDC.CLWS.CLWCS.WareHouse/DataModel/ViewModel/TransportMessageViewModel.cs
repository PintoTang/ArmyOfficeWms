using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel
{
    /// <summary>
    /// 搬运信息的ViewModel
    /// </summary>
    public class TransportMessageViewModel : ViewModelBase
    {

        public Func<TransportMessage, FinishType, OperateResult> FinishTransport;

        public Func<TransportMessage, OperateResult> DoTransportJob;

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


        private double _transportViewHeight;

        public double TransportViewHeight
        {
            get { return _transportViewHeight; }
            set
            {
                _transportViewHeight = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TransportMessage> UnfinishedTransportList { get; set; }

        private MyCommand _openTransportDeviceCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand OpenTransportDeviceCommand
        {
            get
            {
                if (_openTransportDeviceCommand == null)
                    _openTransportDeviceCommand = new MyCommand(OpenTransportDevice);
                return _openTransportDeviceCommand;
            }
        }



        private void OpenTransportDevice(object arg)
        {
            var curSelectValue = (((System.Windows.Controls.DataGridCellInfo) (arg)).Column);

            if (curSelectValue!=null && curSelectValue.Header != null && !string.IsNullOrEmpty(curSelectValue.Header.ToString()) &&
                ((System.Windows.Controls.DataGridCellInfo) (arg)).Item is TransportMessage)
            {
                string strHeader = curSelectValue.Header.ToString();
                TransportMessage selectTransportMessage =
                    ((System.Windows.Controls.DataGridCellInfo) (arg)).Item as TransportMessage;
                if (selectTransportMessage == null) return;
                if (strHeader.Contains("搬运设备"))
                {
                    if (selectTransportMessage.TransportDevice != null)
                    {
                        DeviceDetailView detailView = new DeviceDetailView(selectTransportMessage.TransportDevice);
                        detailView.ShowDialog();
                    }
                    else
                    {
                        MessageBoxEx.Show("当前选中的 搬运设备为空");
                    }
                }
                else if (strHeader.Contains("开始设备"))
                {
                    if (selectTransportMessage.StartDevice != null)
                    {
                        DeviceDetailView detailView = new DeviceDetailView(selectTransportMessage.StartDevice);
                        detailView.ShowDialog();
                    }
                    else
                    {
                        MessageBoxEx.Show("当前选中的 开始设备为空");
                    }
                }
                else if (strHeader.Contains("目标设备"))
                {
                    if (selectTransportMessage.DestDevice != null)
                    {
                        DeviceDetailView detailView = new DeviceDetailView(selectTransportMessage.DestDevice);
                        detailView.ShowDialog();
                    }
                    else
                    {
                        MessageBoxEx.Show("当前选中的 目标设备为空");
                    }
                }
                else
                {
                    //选中的非 指定的列 不生效
                    MessageBoxEx.Show("您当前鼠标选中的列【"+strHeader+"】 不是 搬运设备、开始设备、目标设备 列");
                }

            }
        }


        private MyCommand _forceFinishTransportCommand;
        /// <summary>
        /// 强制完成指令
        /// </summary>
        public MyCommand ForceFinishTransportCommand
        {
            get
            {
                if (_forceFinishTransportCommand == null)
                    _forceFinishTransportCommand = new MyCommand(ForceFinishTransport);
                return _forceFinishTransportCommand;
            }
        }


        private void ForceFinishTransport(object arg)
        {
            if (!(arg is TransportMessage))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要强制完成的搬运信息");
                return;
            }
            TransportMessage forceFinishTransport = arg as TransportMessage;
            MessageBoxResult result = MessageBoxEx.Show("确定强制完成该搬运动作？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            if (FinishTransport == null)
            {
                SnackbarQueue.MessageQueue.Enqueue("FinishTransport的方法还没有注册");
                return;
            }
            OperateResult forceFinishResult = FinishTransport(forceFinishTransport, FinishType.ForceFinish);
            if (!forceFinishResult.IsSuccess)
            {
                string recordMsg = string.Format("强制完成搬运信息：{0} 操作失败",forceFinishTransport);
                OperateLogHelper.Record(recordMsg);
                string msg = string.Format("搬运动作强制完成失败，原因：{0}", forceFinishResult.Message);
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                string recordMsg = string.Format("强制完成搬运信息：{0} 操作成功", forceFinishTransport);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue("搬运动作强制完成成功");
            }
        }

        private MyCommand _reDoTransportCommand;

        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand ReDoTransportCommand
        {
            get
            {
                if (_reDoTransportCommand == null)
                    _reDoTransportCommand = new MyCommand(ReDoTransport);
                return _reDoTransportCommand;
            }
        }
        private MyCommand _reBackFinishTransportCommand;

        /// <summary>
        /// 回滚写起始地址OPC指令
        /// </summary>
          public MyCommand ReBackFinishTransportCommand
        {
            get
            {
                if (_reBackFinishTransportCommand == null)
                    _reBackFinishTransportCommand = new MyCommand(ReBackTransport);
                return _reBackFinishTransportCommand;
            }
        }
        


        private void ReDoTransport(object arg)
        {
            if (!(arg is TransportMessage))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要重新下发的搬运信息");
                return;
            }
            TransportMessage reDoTransport = arg as TransportMessage;
            MessageBoxResult result = MessageBoxEx.Show("确定重新下发搬运动作？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            if (DoTransportJob == null)
            {
                SnackbarQueue.MessageQueue.Enqueue("DoTransportJob的方法还没有注册");
                return;
            }
            OperateResult forceFinishResult = DoTransportJob(reDoTransport);
            if (!forceFinishResult.IsSuccess)
            {
                string recordMsg = string.Format("重新下发搬运信息：{0} 操作失败", reDoTransport);
                OperateLogHelper.Record(recordMsg);

                string msg = string.Format("搬运动作重新下发失败，原因：{0}", forceFinishResult.Message);
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                string recordMsg = string.Format("重新下发搬运信息：{0} 操作成功", reDoTransport);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue("搬运动作重新下发成功");
            }

        }

        /// <summary>
        /// 回滚写起始地址对应的OPC值
        /// </summary>
        /// <param name="arg"></param>
        private void ReBackTransport(object arg)
        {
            if (!(arg is TransportMessage))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要重新下发回滚的搬运信息");
                return;
            }
            TransportMessage reDoTransport = arg as TransportMessage;
            MessageBoxResult result = MessageBoxEx.Show("确定重新下发回滚搬运动作？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            if (DoTransportJob == null)
            {
                SnackbarQueue.MessageQueue.Enqueue("ReBackTransport的方法还没有注册");
                return;
            }
            OperateResult delReBackResult = DelReBackTransportBus(reDoTransport);
            if (!delReBackResult.IsSuccess)
            {
                string recordMsg = string.Format("重新下发回滚搬运信息：{0} 操作失败", reDoTransport);
                OperateLogHelper.Record(recordMsg);
            }
            else
            {

                string recordMsg = string.Format("重新下发回滚搬运信息：{0} 操作成功", reDoTransport);
                OperateLogHelper.Record(recordMsg);
                SnackbarQueue.MessageQueue.Enqueue("回滚搬运动作重新下发成功");
            }
        }

        private OperateResult DelReBackTransportBus(TransportMessage reDoTransport)
        {
            //1、找到起始地址 是搬运设备
            //2、写入起始地址搬运的OPC值
            OperateResult opResult = OperateResult.CreateFailedResult("");
            TransportMessage transMsg = reDoTransport;

            List<DeviceBaseAbstract> deviceList =
                DeviceManage.DeviceManage.Instance.FindDevicesByCurAddr(reDoTransport.StartAddr);
            if (opResult == null)
            {
                opResult.Message = "根据起始地址查询不到设备信息:" + reDoTransport.StartAddr.ToString();
                return opResult;
            }
            DeviceBaseAbstract deviceBaseAbstract = deviceList.Find(x => x.IsTransportDevice());
            if (deviceBaseAbstract == null)
            {
                opResult.Message = "起始地址不是搬运设备:" + reDoTransport.StartAddr.ToString();
                return opResult;
            }

            TransportDeviceBaseAbstract transDevcie = deviceBaseAbstract as TransportDeviceBaseAbstract;
            if (transDevcie == null)
            {
                opResult.Message = "转换搬运对象失败:" + deviceBaseAbstract.Id;
                return opResult;
            }
            opResult = transDevcie.DoTransportJob(transMsg);
            return opResult;
        }
    }
}
