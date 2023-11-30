using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClRfidDoor;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClStackRfid.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClStackRfid.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClouRfidDoor : RfidDoorAbstract, ISwitch, IShowScreen
    {
        public override OperateResult CloseDoor(string cmd)
        {
            OperateResult isAvailable = Availabe();
            if (!isAvailable.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("设备：{0} 此时不可用,不可用原因：{1}", this.Name, isAvailable.Message));
            }
            LogMessage(string.Format("即将下发关门任务：{0}", cmd), EnumLogLevel.Info, true);
            OperateResult result = OperateResult.CreateFailedResult();
            SendSwitchTaskCmd closeCmd = cmd.ToObject<SendSwitchTaskCmd>();
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendSwitchTask.ToString(), cmd);
            OperateResult<string> sendResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            if (!sendResult.IsSuccess)
            {
                LogMessage(string.Format("关门任务下发失败：{0}", sendResult.Message), EnumLogLevel.Error, true);
                result.IsSuccess = false;
                result.Message = sendResult.Message;
                return result;
            }
            try
            {
                LogMessage(string.Format("设备返回结果：{0}", sendResult.Content), EnumLogLevel.Info, true);
                SyncResReMsg responseResult = (SyncResReMsg)sendResult.Content;
                if (!responseResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(sendResult.Content);
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("设备返回信息转换协议失败：{0}", sendResult.Content), EnumLogLevel.Error, true);
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }
            LogMessage(string.Format("任务下发成功：{0}", sendResult.Content), EnumLogLevel.Info, true);
            StringCharTask task = new StringCharTask(closeCmd.TASK_NO);
            task.DeviceId = this.Id;
            task.UpperTaskCode = closeCmd.TASK_NO;
            task.LowerTaskCode = closeCmd.TASK_NO;
            task.ProcessStatus = TaskProcessStatus.Sended;
            task.TaskType = StringCharTaskTypeEnum.CloseDoor;
            task.TaskValue = cmd;
            AddUnfinishedTask(task);
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult OpenDoor(string cmd)
        {
            OperateResult isAvailable = Availabe();
            if (!isAvailable.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("设备：{0} 此时不可用,不可用原因：{1}", this.Name, isAvailable.Message));
            }
            LogMessage(string.Format("即将下发开门任务：{0}", cmd), EnumLogLevel.Info, true);
            OperateResult result = OperateResult.CreateFailedResult();
            SendSwitchTaskCmd openCmd = cmd.ToObject<SendSwitchTaskCmd>();
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendSwitchTask.ToString(), cmd);
            OperateResult<string> sendResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            if (!sendResult.IsSuccess)
            {
                LogMessage(string.Format("开门任务下发失败：{0}", sendResult.Message), EnumLogLevel.Error, true);
                result.IsSuccess = false;
                result.Message = sendResult.Message;
                return result;
            }
            try
            {
                LogMessage(string.Format("设备返回结果：{0}", sendResult.Content), EnumLogLevel.Info, true);
                SyncResReMsg responseResult = (SyncResReMsg)sendResult.Content;
                if (!responseResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(sendResult.Content);
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("设备返回信息转换协议失败：{0}", sendResult.Content), EnumLogLevel.Error, true);
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }
            LogMessage(string.Format("任务下发成功：{0}", sendResult.Content), EnumLogLevel.Info, true);
            StringCharTask task = new StringCharTask(openCmd.TASK_NO);
            task.DeviceId = this.Id;
            task.UpperTaskCode = openCmd.TASK_NO;
            task.LowerTaskCode = openCmd.TASK_NO;
            task.ProcessStatus = TaskProcessStatus.Sended;
            task.TaskType = StringCharTaskTypeEnum.OpenDoor;
            task.TaskValue = cmd;
            AddUnfinishedTask(task);
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Scan(string cmd)
        {
            OperateResult isAvailable = Availabe();
            if (!isAvailable.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("设备：{0} 此时不可用,不可用原因：{1}", this.Name, isAvailable.Message));
            }
            LogMessage(string.Format("即将下发扫描任务：{0}", cmd), EnumLogLevel.Info, true);
            OperateResult result = OperateResult.CreateFailedResult();
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendScanTask.ToString(), cmd);
            OperateResult<string> scanResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            if (!scanResult.IsSuccess)
            {
                LogMessage(string.Format("扫描任务下发失败：{0}", scanResult.Message), EnumLogLevel.Error, true);
                result.IsSuccess = false;
                result.Message = scanResult.Message;
                return result;
            }
            try
            {
                LogMessage(string.Format("设备返回结果：{0}", scanResult.Content), EnumLogLevel.Info, true);
                SyncResReMsg responseResult = (SyncResReMsg)scanResult.Content;
                if (!responseResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = scanResult.Content;
                    return result;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("设备返回信息转换协议失败：{0}", scanResult.Content), EnumLogLevel.Error, true);

                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }
            result.IsSuccess = true;
            result.Message = scanResult.Content;
            return result;
        }

        public override OperateResult ShowMessage(string cmd)
        {
            LogMessage(string.Format("即将下发显示任务：{0}", cmd), EnumLogLevel.Info, true);

            OperateResult result = OperateResult.CreateFailedResult();
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendShow.ToString(), cmd);
            OperateResult<string> scanResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            if (!scanResult.IsSuccess)
            {
                LogMessage(string.Format("显示任务下发失败：{0}", scanResult.Message), EnumLogLevel.Error, true);

                result.IsSuccess = false;
                result.Message = scanResult.Message;
                return result;
            }
            try
            {
                LogMessage(string.Format("设备返回结果：{0}", scanResult.Content), EnumLogLevel.Info, true);

                SyncResReMsg responseResult = (SyncResReMsg)scanResult.Content;
                if (!responseResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = scanResult.Content.ToJson();
                    return result;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("设备返回信息转换协议失败：{0}", scanResult.Content), EnumLogLevel.Error, true);

                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }
            result.IsSuccess = true;
            result.Message = scanResult.Content;
            return result;
        }

        public override OperateResult FinishTask(StringCharTask taskCode)
        {
            OperateResult<StringCharTask> unFinishTask = UnFinishedTask.FindData(t => t.UniqueCode.Equals(taskCode.LowerTaskCode));
            if (!unFinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult<StringCharTask>(null, string.Format("不存在任务号：{0} 的未完成任务", taskCode));
            }
            unFinishTask.Content.ProcessStatus = TaskProcessStatus.Finished;
            RemoveUnfinishedTask(unFinishTask.Content);
            NotifyTaskExcuteStatus(this, unFinishTask.Content, TaskExcuteStepStatusEnum.Finished);
            return OperateResult.CreateSuccessResult(unFinishTask.Content);
        }

        public override OperateResult<StringCharTask> FinishTask(string taskCode)
        {
            OperateResult<StringCharTask> unFinishTask = UnFinishedTask.FindData(t => t.UniqueCode.Equals(taskCode));
            if (!unFinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult<StringCharTask>(null, string.Format("不存在任务号：{0} 的未完成任务", taskCode));
            }
            unFinishTask.Content.ProcessStatus = TaskProcessStatus.Finished;
            RemoveUnfinishedTask(unFinishTask.Content);
            NotifyTaskExcuteStatus(this, unFinishTask.Content, TaskExcuteStepStatusEnum.Finished);
            return OperateResult.CreateSuccessResult(unFinishTask.Content);
        }

        public override OperateResult DoTask(StringCharTask task)
        {
            switch (task.TaskType)
            {
                case StringCharTaskTypeEnum.ShowMessage:
                    return ShowMessage(task.TaskValue);
                case StringCharTaskTypeEnum.CloseDoor:
                    return CloseDoor(task.TaskValue);
                case StringCharTaskTypeEnum.OpenDoor:
                    return OpenDoor(task.TaskValue);
                case StringCharTaskTypeEnum.Scan:
                    return Scan(task.TaskValue);
                case StringCharTaskTypeEnum.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return OperateResult.CreateFailedResult("没有相应方法类型");
        }

        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
        }

        private ClStackRfidDoorDeviceProperty _deviceProperty = new ClStackRfidDoorDeviceProperty();

        public ClStackRfidDoorDeviceProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
        }

        public override OperateResult ParticularConfig()
        {
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

                string stationPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(stationPropertyXml))
                {
                    try
                    {
                        DeviceProperty = (ClStackRfidDoorDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(ClStackRfidDoorDeviceProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：StationProperty", stationPropertyXml));
                }

                this.ExposedUnId = DeviceProperty.Config.ExposedUnId;
                this.RequestAddr = DeviceProperty.BusinessHandle.Config.RequestAddr;
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetDeviceRealData()
        {
            //1.获取未完成得任务列表
            OperateResult<List<StringCharTaskModel>> getUnFinishResult = StringCharTaskDal.GetUnFinishTask(this.Id);
            if (!getUnFinishResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult(getUnFinishResult.Message);
            }
            foreach (StringCharTaskModel taskModel in getUnFinishResult.Content)
            {
                StringCharTask task = new StringCharTask()
                {
                    DeviceId = taskModel.DeviceId.GetValueOrDefault(),
                    LowerTaskCode = taskModel.LowerTaskCode,
                    ProcessStatus = (TaskProcessStatus)taskModel.ProcessStatus.GetValueOrDefault(),
                    TaskSource = (TaskSourceEnum)taskModel.TaskSource.GetValueOrDefault(),
                    TaskType = (StringCharTaskTypeEnum)taskModel.TaskType.GetValueOrDefault(),
                    TaskValue = taskModel.TaskValue,
                    UniqueCode = taskModel.UniqueCode,
                    UpperTaskCode = taskModel.UpperTaskCode
                };
                UnFinishedTask.AddPool(task);
            }
            return OperateResult.CreateSuccessResult();
        }

        public override void AddUnfinishedTask(StringCharTask task)
        {
            OperateResult result = UnFinishedTask.AddPool(task);
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("指令：{0} 添加到任务列表失败", task.UniqueCode), EnumLogLevel.Error, true);
            }
            //此处需要添加保存数据库

            StringCharTaskDal.SaveAsync(task.DatabaseMode);

            LogMessage(string.Format("成功接收任务：{0} 等待设备搬运完成", task.UniqueCode), EnumLogLevel.Info, true);
        }

        public override void RemoveUnfinishedTask(StringCharTask task)
        {
            OperateResult result = UnFinishedTask.RemovePool(o => o.UniqueCode.Equals(task.UniqueCode));
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("任务信息：{0} 从未完成任务列表移除失败", task.UniqueCode), EnumLogLevel.Error, true);
            }
            task.ProcessStatus = TaskProcessStatus.Finished;
            StringCharTaskDal.SaveAsync(task.DatabaseMode);
        }

        public override OperateResult SetAbleState(UseStateMode destState)
        {
            CurUseState = destState;
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult SetControlState(ControlStateMode destState)
        {
            CurControlMode = destState;
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Availabe()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }

        public override bool Accessible(Addr destAddr)
        {
            if (CurAddress.IsContain(destAddr))
            {
                return true;
            }
            return false;
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            ClouRfidDoorViewModel viewModel = new ClouRfidDoorViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }

        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            TaskDeviceView view = new TaskDeviceView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override Window CreateAssistantView()
        {
            ClRfidDoorAssistantView assistantView = new ClRfidDoorAssistantView();
            return assistantView;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<ClouRfidDoor, ClStackRfidDoorDeviceProperty> viewModel = new DeviceConfigViewModel<ClouRfidDoor, ClStackRfidDoorDeviceProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            UserControl view = new ClouStackRfidDoorDetailView();
            return OperateResult.CreateSuccessResult(view);
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = DeviceProperty.Name;
                this.WorkSize = DeviceProperty.WorkSize;
                this.CurAddress = new Addr(DeviceProperty.CurAddress);
                this.DeviceName = new DeviceName(DeviceProperty.DeviceName);
                this.NameSpace = DeviceProperty.NameSpace;
                this.ClassName = DeviceProperty.ClassName;
                this.Id = DeviceProperty.DeviceId;
                this.IsShowUi = DeviceProperty.IsShowUi;

                this.DeviceControl.Name = DeviceProperty.ControlHandle.Name;
                this.DeviceControl.ClassName = DeviceProperty.ControlHandle.ClassName;
                this.DeviceControl.NameSpace = DeviceProperty.ControlHandle.NameSpace;

                this.DeviceBusiness.Name = DeviceProperty.BusinessHandle.Name;
                this.DeviceBusiness.NameSpace = DeviceProperty.BusinessHandle.NameSpace;
                this.DeviceBusiness.ClassName = DeviceProperty.BusinessHandle.ClassName;

                OperateResult initControlConfig = this.DeviceControl.ParticularInitConfig();
                if (!initControlConfig.IsSuccess)
                {
                    return initControlConfig;
                }

                OperateResult initBusinessConfig = this.DeviceBusiness.ParticularConfig();

                if (!initBusinessConfig.IsSuccess)
                {
                    return initBusinessConfig;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
                result.IsSuccess = false;
            }
            return result;
        }

        public string RequestAddr { get; set; }

        public OperateResult Switch(string cmd)
        {
            LogMessage(string.Format("收到按钮切换：{0}", cmd),EnumLogLevel.Info,true);
            if (SwitchChangeEvent!=null)
            {
                SwitchChangeEvent(this, 1, RequestAddr);
            }
            return OperateResult.CreateSuccessResult();
        }

        public NotifySwitchChange SwitchChangeEvent { get; set; }
    }
}
