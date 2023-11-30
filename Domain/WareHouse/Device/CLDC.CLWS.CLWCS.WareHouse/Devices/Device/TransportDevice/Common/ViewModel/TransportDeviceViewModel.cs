using System;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel
{
    public class TransportDeviceViewModel : DeviceViewModelAbstract<TransportDeviceBaseAbstract>
    {
        public TransportDeviceViewModel(TransportDeviceBaseAbstract device)
            : base(device)
        {
            InitTransportMessageViewModel();
        }

        private void InitTransportMessageViewModel()
        {
            TransportMessageViewModel = new TransportMessageViewModel();
            TransportMessageViewModel.FinishTransport = Device.FinishTransport;
            TransportMessageViewModel.DoTransportJob = Device.DoTransportJob;
            TransportMessageViewModel.TransportViewHeight = Device.WorkSize * 45.0;
            TransportMessageViewModel.UnfinishedTransportList = Device.UnFinishedTask.DataPool;
        }



        public TransportMessageViewModel TransportMessageViewModel { get; private set; }


        private bool IsCanExcutePauseOrStart(object para)
        {
            if (Device == null)
            {
                return false;
            }
            if (Device.CurConnectState.Equals(ConnectState.Offline))
            {
                return false;
            }
            if (Device.CurRunState.Equals(RunStateMode.Restore) || Device.CurRunState.Equals(RunStateMode.Reset))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 设备是否可以操作
        /// </summary>
        public bool IsEnabledDeviceOperate
        {
            get
            {
                if (Device != null && Device.CurUseState == UseStateMode.Enable)
                {
                    _isEnabledDeviceOperate = true;
                }
                else
                {
                    _isEnabledDeviceOperate = false;
                }
                return _isEnabledDeviceOperate;
            }
            set
            {
                if (_isEnabledDeviceOperate != value)
                {
                    _isEnabledDeviceOperate = value;
                    RaisePropertyChanged("IsEnabledDeviceOperate");
                }
            }
        }
        private MyCommand _cmdDeviceRunAndPause;
        /// <summary>
        /// 设备开始\暂停
        /// </summary>
        public MyCommand CmdDeviceRunAndPause
        {
            get
            {
                if (_cmdDeviceRunAndPause == null)
                    _cmdDeviceRunAndPause = new MyCommand(o =>
                    {
                        if (Device != null)
                        {
                            if (Device.CurRunState.Equals(RunStateMode.Pause) || Device.CurRunState.Equals(RunStateMode.Stop))
                            {
                                string recordMsg = string.Format("启动：{0} 运行", Device.Name);
                                OperateLogHelper.Record(recordMsg);
                                Device.Run();
                            }
                            else if (Device.CurRunState.Equals(RunStateMode.Run))
                            {
                                string recordMsg = string.Format("暂停：{0} 运行", Device.Name);
                                OperateLogHelper.Record(recordMsg);
                                Device.Pause();
                            }
                        }
                    }, IsCanExcutePauseOrStart);
                return _cmdDeviceRunAndPause;
            }
        }

        private MyCommand _cmdDeviceStop;
        /// <summary>
        /// 设备停止
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
                              if (Device != null)
                              {
                                  MessageBoxResult rt = MessageBoxEx.Show("您确定要停止设备？", "提示", MessageBoxButton.YesNo);
                                  if (rt != MessageBoxResult.OK) return;
                                  OperateResult stop = Device.Stop();
                                  if (!stop.IsSuccess)
                                  {
                                      string recordMsg = string.Format("停止：{0} 运行失败", Device.Name);
                                      OperateLogHelper.Record(recordMsg);
                                     MessageBoxEx.Show(string.Format("设备停止失败，原因：{0}", stop.Message), "错误");
                                  }
                                  else
                                  {
                                      string recordMsg = string.Format("停止：{0} 运行成功", Device.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      SnackbarQueue.MessageQueue.Enqueue("设备停止成功");
                                  }
                              }
                          }
                        ));
                return _cmdDeviceStop;
            }
        }

        private MyCommand _cmdDeviceReset;
        /// <summary>
        /// 设备复位
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
                              if (Device != null)
                              {
                                  MessageBoxResult rt = MessageBoxEx.Show("您确定要复位设备？", "提示", MessageBoxButton.YesNo);
                                  if (rt != MessageBoxResult.OK) return;
                                  OperateResult reset = Device.Reset();
                                  if (!reset.IsSuccess)
                                  {
                                      string recordMsg = string.Format("停止：{0} 运行失败", Device.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      MessageBoxEx.Show(string.Format("设备复位失败，原因：{0}", reset.Message), "错误");
                                  }
                                  else
                                  {
                                      string recordMsg = string.Format("停止：{0} 运行成功", Device.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      SnackbarQueue.MessageQueue.Enqueue("设备复位成功");
                                  }
                              }
                          }
                        ));
                return _cmdDeviceReset;
            }
        }
        private bool _isEnabledDeviceOperate = true;
       
        private MyCommand _cmdDeviceOperateEnbled;
        /// <summary>
        /// 设备 禁用、启用
        /// </summary>
        public MyCommand CmdDeviceOperateEnbled
        {
            get
            {
                if (_cmdDeviceOperateEnbled == null)
                    _cmdDeviceOperateEnbled = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              if (Device != null)
                              {
                                  if (Device.CurUseState == UseStateMode.Disable)
                                  {
                                      MessageBoxResult rt = MessageBoxEx.Show("您确定要启用设备？", "提示", MessageBoxButton.YesNo);
                                      if (!rt.Equals(MessageBoxResult.Yes)) return;
                                      OperateResult opResult = Device.SetAbleState(UseStateMode.Enable);
                                      if (!opResult.IsSuccess)
                                      {
                                          string recordMsg = string.Format("启用：{0} 失败", Device.Name);
                                          OperateLogHelper.Record(recordMsg);
                                          MessageBoxEx.Show("启用设备操作失败:" + opResult.Message, "错误");
                                          return;
                                      }
                                      IsEnabledDeviceOperate = true;
                                  }
                                  else
                                  {
                                      MessageBoxResult rt = MessageBoxEx.Show("您确定要禁用设备？", "警告", MessageBoxButton.YesNo);
                                      if (!rt.Equals(MessageBoxResult.Yes)) return;
                                      OperateResult opResult = Device.SetAbleState(UseStateMode.Disable);
                                      if (!opResult.IsSuccess)
                                      {
                                          string recordMsg = string.Format("禁用：{0} 失败", Device.Name);
                                          OperateLogHelper.Record(recordMsg);
                                          MessageBoxEx.Show("禁用设备操作失败:" + opResult.Message, "错误");
                                          return;
                                      }
                                      IsEnabledDeviceOperate = false;
                                  }
                              }
                          } ));
                return _cmdDeviceOperateEnbled;
            }
        }

        private MyCommand _cmdDeviceOperateDispatch;

        /// <summary>
        /// 设备 调度操作
        /// </summary>
        public MyCommand CmdDeviceOperateDispatch
        {
            get
            {
                if (_cmdDeviceOperateDispatch == null)
                    _cmdDeviceOperateDispatch = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              if (Device == null) return;
                              if (Device.CurDispatchState == DispatchState.On)
                              {
                                  OperateResult opResult = Device.SetDispatchState(DispatchState.Off);
                                  if (!opResult.IsSuccess)
                                  {
                                      string recordMsg = string.Format("关闭调度：{0} 失败", Device.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      MessageBoxEx.Show("关闭调度失败:" + opResult.Message, "错误");
                                      return;
                                  }
                              }
                              else
                              {
                                  OperateResult opResult = Device.SetDispatchState(DispatchState.On);
                                  if (!opResult.IsSuccess)
                                  {
                                      string recordMsg = string.Format("启用调度：{0} 失败", Device.Name);
                                      OperateLogHelper.Record(recordMsg);
                                      MessageBoxEx.Show("启用调度失败:" + opResult.Message, "错误");
                                      return;
                                  }
                              }
                          } ));
                return _cmdDeviceOperateDispatch;
            }
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
         
        }
    }
}
