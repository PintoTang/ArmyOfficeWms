using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Palletizer.Common;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public sealed class PalletizerWithTask : DeviceBaseForTask<StringCharTask>, IPalletize
    {
        private StringCharTaskAbstract _stringCharTaskDal;


        public PalletizerBusinessAbstract DeviceBusiness;

        public PalletierControlAbstract DeviceControl;

        /// <summary>
        /// 注册OPC监控的值变化处理
        /// </summary>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            OperateResult registerResutl = DeviceControl.RegisterValueChange(dbBlockEnum, monitervaluechange);
            LogMessage(registerResutl.Message, EnumLogLevel.Info, false);
            return registerResutl;
        }

        private void MonitorPalletizerFinish(int finishValue)
        {
            LogMessage(string.Format("监控到拆码盘机的就绪信号变化{0}", finishValue), EnumLogLevel.Info, false);
            if (finishValue.Equals(2))
            {
                LogMessage("通知拆盘完成", EnumLogLevel.Info, true);
                DePalletizeFinish();
            }
            else if (finishValue.Equals(3))
            {
                LogMessage("通知码盘完成", EnumLogLevel.Info, true);
                PalletizeFinish();
            }
            else
            {
                LogMessage("不处理任何业务", EnumLogLevel.Info, false);
            }
        }


        private PalletizerDeviceProperty _deviceProperty = new PalletizerDeviceProperty();

        public PalletizerDeviceProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
        }
        public override OperateResult ParticularStart()
        {
            OperateResult registerFinishResult = RegisterValueChange(DataBlockNameEnum.PickingReadyDataBlock,
                MonitorPalletizerFinish);
            if (!registerFinishResult.IsSuccess)
            {
                return registerFinishResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.DeviceBusiness = business as PalletizerBusinessAbstract;
            this.DeviceControl = control as PalletierControlAbstract;
            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "PalletizerBusinessAbstract", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "PalletierControlAbstract", control.GetType().FullName);

                return OperateResult.CreateFailedResult(msg, 1);
            }
            _stringCharTaskDal = DependencyHelper.GetService<StringCharTaskAbstract>();

            return OperateResult.CreateSuccessResult();
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
                        DeviceProperty = (PalletizerDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(PalletizerDeviceProperty));
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
            OperateResult<List<StringCharTaskModel>> getUnFinishResult = _stringCharTaskDal.GetUnFinishTask(this.Id);
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
            PalletizerWithTaskViewModel viewModel = new PalletizerWithTaskViewModel(this);
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
            return new Window();
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<PalletizerWithTask, PalletizerDeviceProperty> viewModel = new DeviceConfigViewModel<PalletizerWithTask, PalletizerDeviceProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            UserControl view = new PalletizerWithTaskDetailView();
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

        public override void AddUnfinishedTask(StringCharTask task)
        {
            OperateResult result = UnFinishedTask.AddPool(task);
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("指令：{0} 添加到任务列表失败", task.UniqueCode), EnumLogLevel.Error, true);
            }
            //此处需要添加保存数据库

            _stringCharTaskDal.SaveAsync(task.DatabaseMode);
            RefreshDeviceState();
            LogMessage(string.Format("成功接收任务：{0} 等待设备处理完成", task.UniqueCode), EnumLogLevel.Info, true);
        }

        public override void RemoveUnfinishedTask(StringCharTask task)
        {
            OperateResult result = UnFinishedTask.RemovePool(o => o.UniqueCode.Equals(task.UniqueCode));
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("任务信息：{0} 从未完成任务列表移除失败", task.UniqueCode), EnumLogLevel.Error, true);
            }
            task.ProcessStatus = TaskProcessStatus.Finished;
            RefreshDeviceState();
            _stringCharTaskDal.SaveAsync(task.DatabaseMode);
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
            CurDispatchState = destState;
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Palletize(string cmd)
        {
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult DePalletize(string cmd)
        {
            LogMessage(string.Format("接收到拆垛任务：{0}", cmd), EnumLogLevel.Info, true);
            if (IsFull)
            {
                string msg = "设备正在拆垛中，无法接收新任务";
                LogMessage(msg, EnumLogLevel.Warning, true);
                return OperateResult.CreateFailedResult(msg);
            }
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                DePalletizeCmd dePalletizeCmd = cmd.ToObject<DePalletizeCmd>();
                OperateResult sendCmdResult = DeviceControl.SendDePalletizeCmd(dePalletizeCmd);
                if (!sendCmdResult.IsSuccess)
                {
                    LogMessage(string.Format("下发拆垛任务失败：{0}", sendCmdResult.Message), EnumLogLevel.Error, true);
                    return sendCmdResult;
                }
                string depalletizeCode = dePalletizeCmd.DeviceTaskCode;
                StringCharTask task = new StringCharTask(depalletizeCode);
                task.DeviceId = this.Id;
                task.UpperTaskCode = depalletizeCode;
                task.LowerTaskCode = depalletizeCode;
                task.ProcessStatus = TaskProcessStatus.Sended;
                task.TaskType = StringCharTaskTypeEnum.UnStack;
                task.TaskValue = cmd;
                task.TaskSource = TaskSourceEnum.UPPER;
                AddUnfinishedTask(task);
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public PalletizerTaskFinish PalletizerFinishEvent { get; set; }

        public OperateResult PalletizeFinish()
        { 
             StringCharTask task=new StringCharTask(string.Empty);
            task.ProcessStatus= TaskProcessStatus.Finished;
            task.TaskType= StringCharTaskTypeEnum.Stack;
            task.TaskSource= TaskSourceEnum.UPPER;
            if (PalletizerFinishEvent!=null)
            {
                PalletizerFinishEvent(this, task);
            }
            LogMessage(string.Format("通知码盘任务：{0} 完成", task), EnumLogLevel.Info, true);
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult DePalletizeFinish()
        {
            OperateResult<StringCharTask> curPalletizeTask =
                UnFinishedTask.FindData(t => t.TaskType.Equals(StringCharTaskTypeEnum.UnStack));
            if (!curPalletizeTask.IsSuccess)
            {
                LogMessage("查找不到尚未完成的拆盘任务", EnumLogLevel.Error, true);
              return  OperateResult.CreateFailedResult("查找不到尚未完成的拆盘任务");
            }
            if (PalletizerFinishEvent != null)
            {
                PalletizerFinishEvent(this, curPalletizeTask.Content);
            }
            curPalletizeTask.Content.ProcessStatus= TaskProcessStatus.Finished;
            RemoveUnfinishedTask(curPalletizeTask.Content);
            LogMessage(string.Format("通知拆盘任务：{0} 完成", curPalletizeTask.Content), EnumLogLevel.Info, true);
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult FinishTask(StringCharTask task)
        {
            OperateResult<StringCharTask> unFinishTask = UnFinishedTask.FindData(t => t.UniqueCode.Equals(task.UniqueCode));
            if (!unFinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult<StringCharTask>(null, string.Format("不存在任务号：{0} 的未完成任务", task.UniqueCode));
            }
            NotifyTaskExcuteStatus(this, unFinishTask.Content, TaskExcuteStepStatusEnum.Finished);
            unFinishTask.Content.ProcessStatus = TaskProcessStatus.Finished;
            RemoveUnfinishedTask(unFinishTask.Content);
            return OperateResult.CreateSuccessResult();
        }

        public  OperateResult DoTask(StringCharTask task)
        {
            switch (task.TaskType)
            {
                case StringCharTaskTypeEnum.UnStack:
                    return DePalletize(task.TaskValue);
                case StringCharTaskTypeEnum.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return OperateResult.CreateFailedResult("没有相应方法类型");
        }

    }
}
