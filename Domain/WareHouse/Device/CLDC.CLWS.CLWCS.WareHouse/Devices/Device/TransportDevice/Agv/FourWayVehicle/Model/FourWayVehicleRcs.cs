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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.ViewModel;

using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class FourWayVehicleRcs : AgvDeviceAbstract
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

        private FourWayVehicleRcsProperty _deviceProperty = new FourWayVehicleRcsProperty();

        public FourWayVehicleRcsProperty DeviceProperty
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

        public OperateResult HandleReportTaskResult(string deviceNo, string taskNo, FourWayVehicleMoveStepEnum moveStep)
        {
            OperateResult<TransportMessage> getUnfinishTask = UnFinishedTask.FindData(t => t.TransportOrder.OrderId.ToString().Equals(taskNo));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateSuccessResult();
            }
            getUnfinishTask.Content.TransportOrder.HelperRGVNo = int.Parse(deviceNo);
            switch (moveStep)
            {
                case FourWayVehicleMoveStepEnum.UnExecute:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Free);
                case FourWayVehicleMoveStepEnum.Executing:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.MoveStart);
                case FourWayVehicleMoveStepEnum.PickFinish:
                case FourWayVehicleMoveStepEnum.MoveWithLoaded:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.PickFinish);
                case FourWayVehicleMoveStepEnum.PutFinish:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.PutFinish);
                case FourWayVehicleMoveStepEnum.Finish:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.Finish);
                case FourWayVehicleMoveStepEnum.Exception:
                    return HandleMoveStepChange(getUnfinishTask.Content.TransportOrderId, AgvMoveStepEnum.EmptyForkFinish);
                case FourWayVehicleMoveStepEnum.Pause:
                    break;
            }
            return OperateResult.CreateSuccessResult();
        }

        private readonly Dictionary<int, TaskExcuteStatusType> _faultCodeDictionary = new Dictionary<int, TaskExcuteStatusType>();

        private void InitilizeFaultCodeDictionary()
        {
            _faultCodeDictionary.Add(1, TaskExcuteStatusType.InButExist);
            _faultCodeDictionary.Add(2, TaskExcuteStatusType.OutButEmpty);
            _faultCodeDictionary.Add(3, TaskExcuteStatusType.OutDepthButShallowExist);
            _faultCodeDictionary.Add(4, TaskExcuteStatusType.InDepthButShallowExist);
           
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
            OperateResult<TransportMessage> getUnfinishTask =
                UnFinishedTask.FindData(t => t.TransportOrder.OrderId.Equals(int.Parse(taskNo)));
            if (!getUnfinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("获取不到对应任务号：{0} 的未完成任务", taskNo));
            }
            //此处转换成Wcs的异常编号
            TaskExcuteStatusType exceptionType = DeviceFaultCodeChangeExceptionType(exceptionCode);
            TaskExcuteMessage<TransportMessage> exceptionMsg = new TaskExcuteMessage<TransportMessage>(this,
                getUnfinishTask.Content, exceptionType, TaskExcuteStepStatusEnum.Finished);
            OperateResult finishResult = FinishTransport(getUnfinishTask.Content, FinishType.ExpFinish);
            if (!finishResult.IsSuccess)
            {
                string msg = string.Format("异常导致指令完成失败，原因：{0}", finishResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(finishResult.Message);
            }
            OperateResult opResult = NotifyDeviceExceptionEvent(exceptionMsg);
            return opResult;
        }


        public override OperateResult HandleMoveStepChange(int orderId, AgvMoveStepEnum moveStep)
        {
            string msg = string.Format("指令：{0} 当前执行步骤：{1}", orderId, moveStep.GetDescription());
            LogMessage(msg, EnumLogLevel.Info, true);
            if (moveStep.Equals(AgvMoveStepEnum.Finish))
            {
                if (UnFinishedTask.DataPool.Count == 0)
                {
                    return OperateResult.CreateFailedResult("当前不存在未完成的指令 ");
                }
                OperateResult<TransportMessage> transportRuslt =
                    UnFinishedTask.FindData(o => o.TransportOrderId.Equals(orderId));
                if (transportRuslt.Content == null)
                {
                    //根据指令ID 未找到未完成的任务
                    string strMsg = string.Format("根据AGV传入的指令ID:{0} 在WCS未找到对应的任务信息", orderId);
                    LogMessage(strMsg, EnumLogLevel.Error, true);
                    return OperateResult.CreateFailedResult(strMsg);
                }
                OperateResult result = HandleDeviceChange(this.DeviceName, DeviceChangeModeEnum.OrderFinish, orderId);
                if (!result.IsSuccess)
                {
                    return result;
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
            #region 代码已屏蔽 待移除代码
            //bool isNextAddr = false;
            //if (AgvDeviceStatusDic.Count > 0)
            //{
            //    foreach (KeyValuePair<string, AgvDeviceRequest> agvDeviceStatus in AgvDeviceStatusDic)
            //    {
            //        if (agvDeviceStatus.Value.CurrentAddr==transport.StartAddr.FullName)
            //        {
            //            if (agvDeviceStatus.Value.IsWaitNextAddr)
            //            {
            //                FourWayVehicleRcsControl FourWayVehicle = DeviceControl as FourWayVehicleRcsControl;
            //                if (FourWayVehicle != null)
            //                {
            //                    ReportRCSPermitFinishCmd cmd = new ReportRCSPermitFinishCmd()
            //                    {
            //                        PER_SITE = transport.DestAddr.FullName,
            //                        TASK_NO = agvDeviceStatus.Value.DeviceTaskCode
            //                    };
            //                    OperateResult sendNext = FourWayVehicle.ReportRCSPermitFinish(cmd);
            //                    if (sendNext.IsSuccess)
            //                    {
            //                        RemoveAgvDeviceStatus(agvDeviceStatus.Key);
            //                    }
            //                    #region 添加指令与任务的关系表T_AC_TASKORDER
            //                    transport.TransportOrder.DeviceTaskNo = agvDeviceStatus.Value.DeviceTaskCode;
            //                    TaskOrderDataModel taskOrder = new TaskOrderDataModel()
            //                    {
            //                        AddTime = DateTime.Now,
            //                        OrderId = transport.TransportOrderId,
            //                        TaskCode = transport.TransportOrder.UpperTaskNo,
            //                        DeviceTaskCode = transport.TransportOrder.DeviceTaskNo
            //                    };
            //                    _taskOrderDataHandle.SaveAsync(taskOrder);
            //                    #endregion
            //                    isNextAddr = true;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
            //if (!isNextAddr)
            //{
            //    OperateResult controlResult = DeviceControl.DoJob(transport);
            //    if (!controlResult.IsSuccess)
            //    {
            //        string msg = string.Format("指令：{0} 业务控制发生错误：{1}", transport.TransportOrderId, controlResult.Message);
            //        LogMessage(msg, EnumLogLevel.Error, true);
            //        return OperateResult.CreateFailedResult(msg, 1);
            //    }
            //}
            #endregion

            OperateResult controlResult = DeviceControl.DoJob(transport);
            if (!controlResult.IsSuccess)
            {
                string msg = string.Format("指令：{0} 业务控制发生错误：{1}", transport.TransportOrderId, controlResult.Message);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
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
                        DeviceProperty = (FourWayVehicleRcsProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(FourWayVehicleRcsProperty));
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
            FourWayVehicleRcsViewModel viewModel = new FourWayVehicleRcsViewModel(this);
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
            FourWayVehicleRcsAssistantView assistantView = new FourWayVehicleRcsAssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<FourWayVehicleRcs, FourWayVehicleRcsProperty> viewModel = new DeviceConfigViewModel<FourWayVehicleRcs, FourWayVehicleRcsProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            FourWayVehicleRcsDetailView view = new FourWayVehicleRcsDetailView();
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
