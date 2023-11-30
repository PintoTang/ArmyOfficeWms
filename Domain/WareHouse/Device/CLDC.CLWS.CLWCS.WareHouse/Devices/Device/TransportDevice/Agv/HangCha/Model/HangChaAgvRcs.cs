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
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.ViewModel;

using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class HangChaAgvRcs : AgvDeviceAbstract
    {
        public override OperateResult GetDeviceRealData()
        {
            OperateResult baseResult = base.GetDeviceRealData();
            if (!baseResult.IsSuccess)
            {
                return baseResult;
            }
            RestoreLiveData();
            return OperateResult.CreateSuccessResult();
        }
        private TaskOrderDataAbstract _taskOrderDataHandle;

        private void RestoreLiveData()
        {
            OperateResult<List<LiveData>> getLiveData = LiveDataDbHelper.GetAllLiveData(this.Id);
            if (!getLiveData.IsSuccess)
            {
                return;
            }
            foreach (LiveData liveData in getLiveData.Content)
            {
                AgvDeviceRequest request = liveData.DataValue.ToObject<AgvDeviceRequest>();
                AddAgvDeviceStatus(request.DeviceId, request);
            }
        }

        private void SaveAgvDeviceRequest(AgvDeviceRequest request)
        {
            LiveData liveData = new LiveData();
            liveData.Alias = this.Name;
            liveData.DataValue = request.ToString();
            liveData.DeviceId = this.Id;
            liveData.HandleStatus = HandleStatusEnum.UnFinished;
            liveData.Name = this.Name;
            liveData.Index = ConvertHepler.ConvertToInt(request.DeviceId);
            LiveDataDbHelper.Save(liveData);
        }

        private void RemoveAgvDeviceRequest(AgvDeviceRequest request)
        {
            LiveData liveData = new LiveData();
            liveData.Alias = this.Name;
            liveData.DataValue = request.ToString();
            liveData.DeviceId = this.Id;
            liveData.HandleStatus = HandleStatusEnum.Finished;
            liveData.Name = this.Name;
            liveData.Index = ConvertHepler.ConvertToInt(request.DeviceId);
            LiveDataDbHelper.Save(liveData);
        }

        private HangChaProperty _deviceProperty = new HangChaProperty();

        public HangChaProperty DeviceProperty
        {
            get { return _deviceProperty; }
            set { _deviceProperty = value; }
        }

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
                SaveAgvDeviceRequest(deviceStatus);
            }
        }

        private void RemoveAgvDeviceStatus(string deviceNo)
        {
            lock (_agvDeviceStatusDic)
            {
                if (_agvDeviceStatusDic.ContainsKey(deviceNo))
                {
                    RemoveAgvDeviceRequest(_agvDeviceStatusDic[deviceNo]);
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
                IsWaitNextAddr = true,
                DeviceId = deviceNo
            };
            AddAgvDeviceStatus(deviceNo, agvStatus);
            HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Finish);
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult HandleReportTaskResult(string deviceNo, string taskNo, HangChaAgvMoveStepEnum moveStep)
        {
            OperateResult<TransportMessage> getUnfinishTask = UnFinishedTask.FindData(t => t.TransportOrder.DeviceTaskNo.Equals(taskNo));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateSuccessResult(); //Todo 中山临时加
                //return OperateResult.CreateFailedResult(string.Format("获取不到对应任务号：{0} 的未完成任务", taskNo));
            }
            getUnfinishTask.Content.TransportOrder.HelperRGVNo = int.Parse(deviceNo);
            switch (moveStep)
            {
                case HangChaAgvMoveStepEnum.UnExecute:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Free);
                    break;
                case HangChaAgvMoveStepEnum.Executing:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.MoveStart);
                    break;
                case HangChaAgvMoveStepEnum.PickFinish:
                case HangChaAgvMoveStepEnum.MoveWithLoaded:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.PickFinish);
                    break;
                case HangChaAgvMoveStepEnum.PutFinish:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.PutFinish);
                    break;
                case HangChaAgvMoveStepEnum.Finish:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Finish);
                    break;
                case HangChaAgvMoveStepEnum.Exception:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.EmptyForkFinish);
                    break;
                case HangChaAgvMoveStepEnum.Pause:
                    break;
            }
            return OperateResult.CreateSuccessResult();
        }

        private readonly Dictionary<int, TaskExcuteStatusType> _faultCodeDictionary = new Dictionary<int, TaskExcuteStatusType>();

        private void InitilizeFaultCodeDictionary()
        {
            _faultCodeDictionary.Add(2, TaskExcuteStatusType.OutButEmpty);
            _faultCodeDictionary.Add(1, TaskExcuteStatusType.InButExist);
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

        public OperateResult HandleReportTaskException(string taskNo, int exceptionCode)
        {
            OperateResult<TransportMessage> getUnfinishTask = UnFinishedTask.FindData(t => t.TransportOrder.DeviceTaskNo.Equals(taskNo));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("获取不到对应任务号：{0} 的未完成任务", taskNo));
            }
            ///此处转换成Wcs的异常编号
            TaskExcuteStatusType exceptionType = DeviceFaultCodeChangeExceptionType(exceptionCode);
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


        public override OperateResult HandleMoveStepChange(int orderId, AgvMoveStepEnum moveStep)
        {
            string msg = string.Format("指令：{0} 当前执行步骤：{1}", orderId, moveStep.GetDescription());
            LogMessage(msg, EnumLogLevel.Info, true);
            if (moveStep.Equals(AgvMoveStepEnum.Finish))
            {

                OperateResult<TransportMessage> transportRuslt =
                    UnFinishedTask.FindData(o => o.TransportOrderId.Equals(orderId));
                int ownerId = transportRuslt.Content.OwnerId;
                OrderTypeEnum orderType = transportRuslt.Content.TransportOrder.OrderType;
                OperateResult result = HandleDeviceChange(this.DeviceName, DeviceChangeModeEnum.OrderFinish, orderId);
                if (result.IsSuccess)
                {
                    if (ownerId == 3006  && orderType == OrderTypeEnum.Out)
                    {
                        CLDC.CLWS.CLWCS.Framework.IniHelper.WriteSystem("OrderType","1");
                    }
                }
                else
                {
                    return OperateResult.CreateSuccessResult();
                }
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
                    if (agvDeviceStatus.Value.CurrentAddr==transport.StartAddr.FullName)
                    {
                        if (agvDeviceStatus.Value.IsWaitNextAddr)
                        {
                            HangChaAgvRcsControl hangChaAgv = DeviceControl as HangChaAgvRcsControl;
                            if (hangChaAgv != null)
                            {
                                SendNextAddrPermitCmd cmd = new SendNextAddrPermitCmd()
                                {
                                    AGV_NO = agvDeviceStatus.Key,
                                    NEXT_ADDR = transport.DestAddr.FullName,
                                    TASK_NO = agvDeviceStatus.Value.DeviceTaskCode
                                };
                                OperateResult sendNext = hangChaAgv.SendNextAddrPermit(cmd);
                                if (sendNext.IsSuccess)
                                {
                                    RemoveAgvDeviceStatus(agvDeviceStatus.Key);
                                }
                                #region 添加指令与任务的关系表T_AC_TASKORDER
                                transport.TransportOrder.DeviceTaskNo = agvDeviceStatus.Value.DeviceTaskCode;
                                TaskOrderDataModel taskOrder = new TaskOrderDataModel()
                                {
                                    AddTime = DateTime.Now,
                                    OrderId = transport.TransportOrderId,
                                    TaskCode = transport.TransportOrder.UpperTaskNo,
                                    DeviceTaskCode = transport.TransportOrder.DeviceTaskNo
                                };
                                _taskOrderDataHandle.SaveAsync(taskOrder);
                                #endregion
                                isNextAddr = true;
                                break;
                            }
                        }
                    }
                }
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
            LogMessage(string.Format("接收到下发动作结果：{0}", cmd.ToJson()), EnumLogLevel.Info, true);
            HangChaAgvRcsControl hangChaAgv = DeviceControl as HangChaAgvRcsControl;
            if (hangChaAgv != null)
            {
                OperateResult sendAction = hangChaAgv.SendDeviceAction(cmd);
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
            _taskOrderDataHandle = DependencyHelper.GetService<TaskOrderDataAbstract>();
            return OperateResult.CreateSuccessResult();
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
                        DeviceProperty = (HangChaProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(HangChaProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：RobotTecStackingcraneProperty", devicePropertyXml));
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
            HangChaAgvRcsViewModel viewModel = new HangChaAgvRcsViewModel(this);
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
            HangChaAgvRcsAssistantView assistantView = new HangChaAgvRcsAssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<HangChaAgvRcs, HangChaProperty> viewModel = new DeviceConfigViewModel<HangChaAgvRcs, HangChaProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            HangChaAgvRcsDetailView view = new HangChaAgvRcsDetailView();
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


    }
}
