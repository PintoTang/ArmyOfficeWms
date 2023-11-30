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
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClStackRfid.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClStackRfid.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.ClStackRfid.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 拆码垛射频门
    /// </summary>
    public sealed class ClStackRfidDoorDevice : StackRfidDeviceAbstract, ISwitch, IShowScreen
    {

        public bool Accessible()
        {
            return true;
        }

        /// <summary>
        /// 调度栈板车
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult StackDispatch(string cmd)
        {
            OperateResult isAvailable = Availabe();
            if (!isAvailable.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("设备：{0} 此时不可用,不可用原因：{1}", this.Name, isAvailable.Message));
            }
            OperateResult result = OperateResult.CreateFailedResult();
            LogMessage(string.Format("即将下发空栈板车调度任务：{0}", cmd), EnumLogLevel.Info, true);
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendStackDispatch.ToString(), cmd);
            OperateResult<string> sendResult = SendCmd(apiCmd);
            if (!sendResult.IsSuccess)
            {
                LogMessage(string.Format("空栈板车调度任务下发失败：{0}", sendResult.Message), EnumLogLevel.Error, true);
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
            LogMessage(string.Format("空栈板车调度任务下发成功：{0}", cmd), EnumLogLevel.Info, true);
            SendStackDispatchCmd dispatchCmd = cmd.ToObject<SendStackDispatchCmd>();

            StringCharTask task = new StringCharTask(dispatchCmd.TASK_NO);
            task.DeviceId = this.Id;
            task.UpperTaskCode = dispatchCmd.TASK_NO;
            task.LowerTaskCode = dispatchCmd.TASK_NO;
            task.ProcessStatus = TaskProcessStatus.Sended;
            task.TaskType = StringCharTaskTypeEnum.SendStackDispatch;
            task.TaskValue = cmd;
            AddUnfinishedTask(task);
            return OperateResult.CreateSuccessResult();
        }

        


        private ClStackRfidDoorDeviceProperty _deviceProperty = new ClStackRfidDoorDeviceProperty();

        /// <summary>
        /// ClStackRfidDoorDeviceProperty
        /// </summary>
        public ClStackRfidDoorDeviceProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <returns>OperateResult</returns>
        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 配置加载
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestAddr { get; set; }
        /// <summary>
        /// 获取设备真实的状态
        /// </summary>
        /// <returns>OperateResult</returns>
        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 获得设备真实数据
        /// </summary>
        /// <returns>OperateResult</returns>
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
        /// <summary>
        /// 添加未完成任务到未完成任务池
        /// </summary>
        /// <param name="task">StringCharTask</param>
        public override void AddUnfinishedTask(StringCharTask task)
        {

            OperateResult result = UnFinishedTask.AddPool(task);
            if (!result.IsSuccess)
            {
                LogMessage(string.Format("指令：{0} 添加到任务列表失败", task.UniqueCode), EnumLogLevel.Error, true);
            }
            //此处需要添加保存数据库

            StringCharTaskDal.SaveAsync(task.DatabaseMode);

            LogMessage(string.Format("成功接收任务：{0} 等待设备处理完成", task.UniqueCode), EnumLogLevel.Info, true);
        }
        /// <summary>
        /// 移除未完成任务
        /// </summary>
        /// <param name="task">StringCharTask</param>
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
        /// <summary>
        /// 设置当前使用状态
        /// </summary>
        /// <param name="destState"></param>
        /// <returns></returns>
        public override OperateResult SetAbleState(UseStateMode destState)
        {
            CurUseState = destState;
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 设置当前控制模式
        /// </summary>
        /// <param name="destState"></param>
        public override OperateResult SetControlState(ControlStateMode destState)
        {
            CurControlMode = destState;
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 是否可以调度
        /// </summary>
        /// <param name="destState">调度状态</param>
        /// <returns>OperateResult</returns>
        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 完成后下一步地址列表
        /// </summary>
        /// <param name="destAddr"></param>
        /// <returns></returns>
        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }
        /// <summary>
        /// 是否是最后的设备
        /// </summary>
        /// <returns></returns>
        public override bool IsEndDevice()
        {
            return true;
        }
        /// <summary>
        /// 是否接受 传入的地址
        /// </summary>
        /// <param name="destAddr"></param>
        /// <returns></returns>
        public override bool Accessible(Addr destAddr)
        {
            if (CurAddress.IsContain(destAddr))
            {
                return true;
            }
            if (destAddr.FullName.Equals("StackBufferStation:1_1_1"))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 清理搬运任务消息
        /// </summary>
        /// <param name="transport">搬运消息</param>
        /// <returns></returns>
        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 创建ViewModel
        /// </summary>
        /// <returns></returns>
        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            ClRfidDoorViewModel viewModel = new ClRfidDoorViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }
        /// <summary>
        /// View
        /// </summary>
        /// <returns>OperateResult<ViewAbstract></returns>
        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            TaskDeviceView view = new TaskDeviceView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }
        /// <summary>
        /// AssistantView
        /// </summary>
        /// <returns></returns>
        protected override Window CreateAssistantView()
        {
            ClRfidDoorAssistantView assistantView = new ClRfidDoorAssistantView();
            return assistantView;
        }
        /// <summary>
        /// ConfigView
        /// </summary>
        /// <returns>Window</returns>
        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<ClStackRfidDoorDevice, ClStackRfidDoorDeviceProperty> viewModel = new DeviceConfigViewModel<ClStackRfidDoorDevice, ClStackRfidDoorDeviceProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;

        }
        /// <summary>
        /// DetailView
        /// </summary>
        /// <returns>OperateResult<UserControl></returns>
        protected override OperateResult<UserControl> CreateDetailView()
        {
            UserControl view = new ClStackRfidDoorDetailView();
            return OperateResult.CreateSuccessResult(view);
        }
        /// <summary>
        /// MonitorView
        /// </summary>
        /// <returns>OperateResult<UserControl></returns>
        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }
        /// <summary>
        /// Property
        /// </summary>
        /// <returns>OperateResult</returns>
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
        /// <summary>
        /// 关门
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
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
                    return OperateResult.CreateFailedResult(sendResult.Content.ToJson());
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
        /// <summary>
        /// 开门
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
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
                    return OperateResult.CreateFailedResult(sendResult.Content.ToJson());
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
        /// <summary>
        /// 查询栈板车缓存状态
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult<string></returns>
        public OperateResult<string> AskStackLoadedStatusCmd(string cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.AskStackLoadedStatus.ToString(), cmd);
            OperateResult<string> requestResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            return requestResult;
        }
        /// <summary>
        /// 申请栈板车专机取/放
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult<string></returns>
        public OperateResult<string> AskStackAction(string cmd)
        {
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.AskStackAction.ToString(), cmd);
            OperateResult<string> requestResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            return requestResult;
        }
        /// <summary>
        /// 扫描
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
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
                    result.Message = scanResult.Content.ToString();
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
            result.Message = scanResult.Content.ToJson();
            return result;
        }
        /// <summary>
        /// LED消息展示
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
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
                    result.Message = scanResult.Content;
                    return result;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("设备返回信息转换协议失败：{0}", scanResult.Content), EnumLogLevel.Error, true);

                result.IsSuccess = false;
                result.Message = OperateResult.ConvertExMessage(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }
            result.IsSuccess = true;
            result.Message = scanResult.Content;
            return result;
        }
        /// <summary>
        /// 叠垛
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="stackCode">栈板车编号</param>
        /// <param name="pileNo">垛号</param>
        /// <param name="stackNo">栈板数</param>
        /// <param name="boxType">箱类型</param>
        /// <returns>OperateResult</returns>
        public override OperateResult Stack(string taskNo, string stackCode, int pileNo, int stackNo, int boxType)
        {

            OperateResult result = OperateResult.CreateFailedResult();
            OperateResult isAvailable = Availabe();
            if (!isAvailable.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("设备：{0} 此时不可用,不可用原因：{1}", this.Name, isAvailable.Message));
            }
            
            SendStackTaskCmd stackTask = new SendStackTaskCmd();
            stackTask.BOX_COUNT = pileNo;
            
            stackTask.DEVICE_NO = this.ExposedUnId;
            stackTask.SEP_COUNT = stackNo;
            stackTask.TASK_TYPE = StackActionTypeEnum.Stack;
            stackTask.TASK_NO = stackCode;
            if (boxType.Equals(1) || boxType.Equals(2) || boxType.Equals(3) || boxType.Equals(6))
            {
                stackTask.BOX_TYPE = BoxTypeEnum.SingleMeter;
            }
            else
            {
                stackTask.BOX_TYPE = BoxTypeEnum.ThreeMeter;
            }
            stackTask.ToJson();
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendStackTask.ToString(), stackTask.ToJson());
            LogMessage(string.Format("即将下发码跺任务：{0}", stackTask.ToJson()), EnumLogLevel.Info, true);
            OperateResult<string> sendToDeviceResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            if (!sendToDeviceResult.IsSuccess)
            {
                LogMessage(string.Format("码跺任务下发失败：{0}", sendToDeviceResult.Message), EnumLogLevel.Error, true);
                result.IsSuccess = false;
                result.Message = sendToDeviceResult.Message;
                return result;
            }
            try
            {
                LogMessage(string.Format("设备返回结果：{0}", sendToDeviceResult.Content), EnumLogLevel.Info, true);
                SyncResReMsg responseResult = (SyncResReMsg)sendToDeviceResult.Content;
                if (!responseResult.IsSuccess)
                {
                    string msg = string.Format("拆码垛设备处理码叠跺任务失败：{0}", responseResult.MESSAGE);
                    LogMessage(msg, EnumLogLevel.Info, true);
                    return OperateResult.CreateFailedResult(msg);
                }
                LogMessage(string.Format("拆跺设备处理码跺任务成功：{0}", responseResult.MESSAGE), EnumLogLevel.Info, true);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }
            StringCharTask task = new StringCharTask(stackCode);
            task.DeviceId = this.Id;
            task.UpperTaskCode = taskNo;
            task.LowerTaskCode = stackCode;
            task.ProcessStatus = TaskProcessStatus.Sended;
            task.TaskType = StringCharTaskTypeEnum.Stack;
            task.TaskValue = stackTask.ToJson();
            if (stackNo.Equals(0))
            {
                task.TaskSource = TaskSourceEnum.SELF;
            }
            AddUnfinishedTask(task);
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 拆垛
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="stackCode">垛编号</param>
        /// <param name="pileNo">垛数</param>
        /// <param name="unStackNo">拆垛任务数</param>
        /// <param name="boxType">箱类型</param>
        /// <returns>OperateResult</returns>
        public override OperateResult UnStack(string taskNo, string stackCode, int pileNo, int unStackNo, int boxType)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            OperateResult isAvailable = Availabe();
            if (!isAvailable.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("设备：{0} 此时不可用,不可用原因：{1}", this.Name, isAvailable.Message));
            }
            SendStackTaskCmd stackTask = new SendStackTaskCmd();
            stackTask.BOX_COUNT = pileNo;
            stackTask.DEVICE_NO = this.ExposedUnId;
            stackTask.SEP_COUNT = unStackNo;
            stackTask.TASK_TYPE = StackActionTypeEnum.UnStack;
            stackTask.TASK_NO = stackCode;
            if (boxType.Equals(1) || boxType.Equals(2) || boxType.Equals(3) || boxType.Equals(6))
            {
                stackTask.BOX_TYPE = BoxTypeEnum.SingleMeter;
            }
            else
            {
                stackTask.BOX_TYPE = BoxTypeEnum.ThreeMeter;
            }
            stackTask.ToJson();
            WebApiInvokeCmd apiCmd = new WebApiInvokeCmd(string.Empty, ClStackRfidDoorApiEnum.SendStackTask.ToString(), stackTask.ToJson());
            LogMessage(string.Format("即将下发拆跺任务：{0}", stackTask.ToJson()), EnumLogLevel.Info, true);
            OperateResult<string> sendToDeviceResult = DeviceControl.SendCmd<SyncResReMsg>(apiCmd);
            if (!sendToDeviceResult.IsSuccess)
            {
                LogMessage(string.Format("下发拆跺任务失败：{0}", sendToDeviceResult.Content), EnumLogLevel.Info, true);
                result.IsSuccess = false;
                result.Message = sendToDeviceResult.Message;
                return result;
            }
            try
            {
                SyncResReMsg responseResult = (SyncResReMsg)sendToDeviceResult.Content;
                if (!responseResult.IsSuccess)
                {
                    string msg = string.Format("拆码垛设备处理拆垛任务失败：{0}", responseResult.MESSAGE);
                    LogMessage(msg, EnumLogLevel.Info, true);
                    return OperateResult.CreateFailedResult(msg);
                }
                LogMessage(string.Format("拆跺设备处理拆垛任务成功：{0}", responseResult.MESSAGE), EnumLogLevel.Info, true);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            if (!result.IsSuccess)
            {
                return result;
            }

            StringCharTask task = new StringCharTask(stackCode);
            task.DeviceId = this.Id;
            task.UpperTaskCode = taskNo;
            task.TaskType = StringCharTaskTypeEnum.UnStack;
            task.ProcessStatus = TaskProcessStatus.Sended;
            task.TaskValue = stackTask.ToJson();
            AddUnfinishedTask(task);
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskCode">任务号</param>
        /// <returns> OperateResult<StringCharTask></returns>
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
            LogMessage(string.Format("任务：{0} 处理完成",taskCode),EnumLogLevel.Info, true);
            return OperateResult.CreateSuccessResult(unFinishTask.Content);
        }
        /// <summary>
        /// 完成任务 业务处理
        /// </summary>
        /// <param name="task">StringCharTask</param>
        /// <returns>OperateResult</returns>
        public override OperateResult FinishTask(StringCharTask task)
        {
            OperateResult<StringCharTask> unFinishTask = UnFinishedTask.FindData(t => t.UniqueCode.Equals(task.UniqueCode));
            if (!unFinishTask.IsSuccess)
            {
                return OperateResult.CreateFailedResult<StringCharTask>(null, string.Format("不存在任务号：{0} 的未完成任务", task.UniqueCode));
            }
           
            unFinishTask.Content.ProcessStatus = TaskProcessStatus.Finished;
            RemoveUnfinishedTask(unFinishTask.Content);

            NotifyTaskExcuteStatus(this, unFinishTask.Content, TaskExcuteStepStatusEnum.Finished);
            LogMessage(string.Format("任务：{0} 处理完成", task.UniqueCode), EnumLogLevel.Info, true);
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 任务执行
        /// </summary>
        /// <param name="task">StringCharTask</param>
        /// <returns>OperateResult</returns>
        public override OperateResult DoTask(StringCharTask task)
        {
            switch (task.TaskType)
            {
                case StringCharTaskTypeEnum.ShowMessage:
                    return ShowMessage(task.TaskValue);
                case StringCharTaskTypeEnum.Stack:
                    SendStackTaskCmd stackCmd = task.TaskValue.ToObject<SendStackTaskCmd>();
                    return Stack(task.UpperTaskCode, task.UniqueCode, stackCmd.BOX_COUNT, stackCmd.SEP_COUNT,
                         (int)stackCmd.BOX_TYPE);
                case StringCharTaskTypeEnum.UnStack:
                    SendStackTaskCmd unStackCmd = task.TaskValue.ToObject<SendStackTaskCmd>();
                    return UnStack(task.UpperTaskCode, task.UniqueCode, unStackCmd.BOX_COUNT, unStackCmd.SEP_COUNT,
                        (int)unStackCmd.BOX_TYPE);
                case StringCharTaskTypeEnum.CloseDoor:
                    return CloseDoor(task.TaskValue);
                case StringCharTaskTypeEnum.OpenDoor:
                    return OpenDoor(task.TaskValue);
                case StringCharTaskTypeEnum.Scan:
                    return Scan(task.TaskValue);
                case StringCharTaskTypeEnum.AskStackAction:
                    return AskStackAction(task.TaskValue);
                case StringCharTaskTypeEnum.AskStackLoadedStatus:
                    return AskStackLoadedStatusCmd(task.TaskValue);
                    case StringCharTaskTypeEnum.SendStackDispatch:
                    return StackDispatch(task.TaskValue);
                case StringCharTaskTypeEnum.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return OperateResult.CreateFailedResult("没有相应方法类型");
        }
        /// <summary>
        /// 模式切换
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>OperateResult</returns>
        public OperateResult Switch(string cmd)
        {
            LogMessage(string.Format("收到按钮切换：{0}", cmd), EnumLogLevel.Info, true);
            if (SwitchChangeEvent != null)
            {
                SwitchChangeEvent(this, 1, RequestAddr);
            }
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 切换事件通知
        /// </summary>
        public NotifySwitchChange SwitchChangeEvent { get; set; }

    }
}
