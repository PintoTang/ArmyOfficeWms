using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.YingChuang.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.YingChuang.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.YingChuang.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class YingChuangAgvRcs : AgvDeviceAbstract
    {
        private Dictionary<string, AgvDeviceRequest> _agvDeviceStatusDic = new Dictionary<string, AgvDeviceRequest>();

        public Dictionary<string, AgvDeviceRequest> AgvDeviceStatusDic
        {
            get { return _agvDeviceStatusDic; }
            set
            {
                lock (_agvDeviceStatusDic)
                {
                    _agvDeviceStatusDic = value;
                }
            }
        }
        private void AddAgvDeviceStatus(string deviceNo, AgvDeviceRequest deviceStatus)
        {
            lock (_agvDeviceStatusDic)
            {
                if (_agvDeviceStatusDic.ContainsKey(deviceNo))
                {
                    _agvDeviceStatusDic[deviceNo] = deviceStatus;
                }
                else
                {
                    _agvDeviceStatusDic.Add(deviceNo, deviceStatus);
                }
            }
        }

        private void RemoveAgvDeviceStatus(string deviceNo)
        {
            lock (_agvDeviceStatusDic)
            {
                if (_agvDeviceStatusDic.ContainsKey(deviceNo))
                {
                    _agvDeviceStatusDic.Remove(deviceNo);
                }
            }
        }

        public OperateResult RequestAddr(string deviceNo, string taskNo, string currentAddr)
        {
            OperateResult<TransportMessage> getUnfinishTask = UnFinishedTask.FindData(t => t.TransportOrder.DeviceTaskNo.Equals(taskNo));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("获取不到对应任务号：{0} 的未完成任务", taskNo));
            }
            AgvDeviceRequest agvStatus = new AgvDeviceRequest
            {
                CurrentAddr = currentAddr,
                DeviceTaskCode = taskNo,
                IsWaitNextAddr = true
            };
            AddAgvDeviceStatus(deviceNo, agvStatus);
            HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Finish);
            return OperateResult.CreateSuccessResult();
        }



        public OperateResult HandleReportTaskResult(string deviceNo, string taskNo, YChuangAgvMoveStepEnum moveStep)
        {
            OperateResult<TransportMessage> getUnfinishTask = UnFinishedTask.FindData(t => t.TransportOrder.DeviceTaskNo.Equals(taskNo));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("获取不到对应任务号：{0} 的未完成任务", taskNo));
            }
            getUnfinishTask.Content.TransportOrder.HelperRGVNo = int.Parse(deviceNo);
            switch (moveStep)
            {
                case YChuangAgvMoveStepEnum.UnExecute:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Free);
                    break;
                case YChuangAgvMoveStepEnum.Executing:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.MoveStart);
                    break;
                case YChuangAgvMoveStepEnum.PickFinish:
                case YChuangAgvMoveStepEnum.MoveWithLoaded:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.PickFinish);
                    break;
                case YChuangAgvMoveStepEnum.PutFinish:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.PutFinish);
                    break;
                case YChuangAgvMoveStepEnum.Finish:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Finish);
                    break;
                case YChuangAgvMoveStepEnum.Exception:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.EmptyForkFinish);
                    break;
                case YChuangAgvMoveStepEnum.Pause:
                    break;
            }
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult HandleMoveStepChange(int orderId, AgvMoveStepEnum moveStep)
        {
            string msg = string.Format("指令：{0} 当前执行步骤：{1}", orderId, moveStep.GetDescription());
            LogMessage(msg, EnumLogLevel.Info, true);
            if (moveStep.Equals(AgvMoveStepEnum.Finish))
            {
                HandleDeviceChange(this.DeviceName, DeviceChangeModeEnum.OrderFinish, orderId);
            }
            CurMoveStep = moveStep;
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult DoTransportJob(TransportMessage transport)
        {
            OperateResult businessResult = DeviceBusiness.DoJob(transport);
            if (!businessResult.IsSuccess)
            {
                string msg = string.Format("指令：{0} 业务处理发生错误：{1}", transport.TransportOrderId, businessResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            bool isNextAddr = false;
            if (AgvDeviceStatusDic.Count > 0)
            {
                foreach (KeyValuePair<string, AgvDeviceRequest> agvDeviceStatus in AgvDeviceStatusDic)
                {
                    if (agvDeviceStatus.Value.CurrentAddr.Equals(transport.StartAddr.FullName))
                    {
                        if (agvDeviceStatus.Value.IsWaitNextAddr)
                        {
                            transport.TransportOrder.DeviceTaskNo = agvDeviceStatus.Value.DeviceTaskCode;
                            YingChuangAgvRcsControl yingChuangAgv = DeviceControl as YingChuangAgvRcsControl;
                            if (yingChuangAgv != null)
                            {
                                SendNextAddrPermitCmd cmd = new SendNextAddrPermitCmd()
                                {
                                    AGV_NO = agvDeviceStatus.Key,
                                    NEXT_ADDR = transport.DestAddr.FullName,
                                    TASK_NO = agvDeviceStatus.Value.DeviceTaskCode
                                };
                                OperateResult sendNext = yingChuangAgv.SendNextAddrPermit(cmd);
                                if (sendNext.IsSuccess)
                                {
                                    RemoveAgvDeviceStatus(agvDeviceStatus.Key);
                                }
                                return sendNext;
                            }
                        }
                    }
                }
                isNextAddr = true;
            }
            if (!isNextAddr)
            {
                OperateResult controlResult = DeviceControl.DoJob(transport);
                if (!controlResult.IsSuccess)
                {
                    string msg = string.Format("指令：{0} 业务控制发生错误：{1}", transport.TransportOrderId, controlResult.Message);
                    LogMessage(msg, EnumLogLevel.Error, true);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
            }
            AddUnfinishedTask(transport);
            LogMessage(string.Format("开始创建搬运信息：{0} 判断超时", transport.TransportOrderId), EnumLogLevel.Debug, false);
            transport.CheckTimeOut.Check(TransportTimeOutAction);
            LogMessage(string.Format("创建搬运信息：{0} 判断超时完成", transport.TransportOrderId), EnumLogLevel.Debug, false);
            transport.UpdateDateTime = DateTime.Now;
            transport.TransportStatus = TransportResultEnum.UnFinish;
            CurWorkState = WorkStateMode.Working;

            LogMessage(string.Format("指令：{0} 条码：{1} 已成功放行到下一个设备：{2} ", transport.TransportOrderId, transport.PileNo, transport.DestDevice.Name), EnumLogLevel.Info, true);

            LogMessage(string.Format("开始更新，搬运信息，对应的指令编号：{0}", transport.TransportOrderId), EnumLogLevel.Debug, false);
            TransportManageHandler.UpdateTransportAsync(transport);
            LogMessage(string.Format("结束更新，搬运信息，对应的指令编号：{0}", transport.TransportOrderId), EnumLogLevel.Debug, false);

            return OperateResult.CreateSuccessResult();
        }

        public OperateResult SendDeviceAction(SendDeviceActionCmd cmd)
        {
            YingChuangAgvRcsControl yingChuangAgv = DeviceControl as YingChuangAgvRcsControl;
            if (yingChuangAgv != null)
            {
                OperateResult sendAction = yingChuangAgv.SendDeviceAction(cmd);
                return sendAction;
            }
            return OperateResult.CreateFailedResult("没有找到SendDeviceAction方法");
        }

        public override OperateResult RegisterOrderFinishFlag(TransportMessage transMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override void HandleOrderValueChange(int value)
        {
            return;
        }

        public override OperateResult IsCanChangeControlState(ControlStateMode destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
        }
        private YChuangAgvRcsProperty _deviceProperty = new YChuangAgvRcsProperty();

        public YChuangAgvRcsProperty DeviceProperty
        {
            get { return _deviceProperty; }
            set { _deviceProperty = value; }
        }
        public override OperateResult ParticularConfig()
        {
            InitilizeFaultCodeDictionary();

            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                XmlNode xmlNode = doc.GetXmlNode("Device", "DeviceId", Id.ToString());


                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string devicePropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(devicePropertyXml))
                {
                    try
                    {
                        DeviceProperty = (YChuangAgvRcsProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(YChuangAgvRcsProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：YChuangAgvRcsProperty", devicePropertyXml));
                }

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override bool Accessible(Addr destAddr)
        {
            return false;
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            YingChuangAgvRcsViewModel viewModel = new YingChuangAgvRcsViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }

        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            DeviceShowCard view = new DeviceShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override Window CreateAssistantView()
        {
            YingChuangAgvRcsAssistantView assistantView = new YingChuangAgvRcsAssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<YingChuangAgvRcs, YChuangAgvRcsProperty> viewModel = new DeviceConfigViewModel<YingChuangAgvRcs, YChuangAgvRcsProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            YingChuangAgvRcsDetailView view = new YingChuangAgvRcsDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult UpdateProperty()
        {
            return OperateResult.CreateSuccessResult();
        }

        private TaskExcuteStatusType DeviceFaultCodeChangeExceptionType(int exceptionCode)
        {
            if (_faultCodeDictionary.ContainsKey(exceptionCode))
            {
                return _faultCodeDictionary[exceptionCode];
            }
            else
            {
                return TaskExcuteStatusType.UnknowException;
            }
        }
        private readonly Dictionary<int, TaskExcuteStatusType> _faultCodeDictionary = new Dictionary<int, TaskExcuteStatusType>();

        private void InitilizeFaultCodeDictionary()
        {
            _faultCodeDictionary.Add(2, TaskExcuteStatusType.OutButEmpty);
            _faultCodeDictionary.Add(1, TaskExcuteStatusType.InButExist);
        }
        public OperateResult HandleReportTaskException(string taskNo, int taskexceptionCode)
        {
            OperateResult<TransportMessage> getUnfinishTask = UnFinishedTask.FindData(t => t.TransportOrder.DeviceTaskNo.Equals(taskNo));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("获取不到对应任务号：{0} 的未完成任务", taskNo));
            }
            ///此处转换成Wcs的异常编号
            TaskExcuteStatusType exceptionType = DeviceFaultCodeChangeExceptionType(taskexceptionCode);
            TaskExcuteMessage<TransportMessage> exceptionMsg = new TaskExcuteMessage<TransportMessage>(this, getUnfinishTask.Content, exceptionType, TaskExcuteStepStatusEnum.Finished);
            OperateResult finishResult = FinishTransport(getUnfinishTask.Content, FinishType.ExpFinish);
            if (!finishResult.IsSuccess)
            {
                string msg = string.Format("异常导致指令完成失败，原因：{0}", finishResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(finishResult.Message);
            }
            NotifyDeviceExceptionEvent(exceptionMsg);
            return OperateResult.CreateFailedResult();
        }
    }
}
