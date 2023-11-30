using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TaskDevice.RfidDoor.YChuangRfidDoor.ViewModel
{
    public class YChuangRfidDoorViewModel : DeviceViewModelAbstract<StackRfidDeviceAbstract>
    {
        public string HttpUrl
        {
            get
            {
                return _deviceControl.Http;
            }
        }
        private readonly YChuangRfidDoorControl _deviceControl;
        public YChuangRfidDoorViewModel(StackRfidDeviceAbstract device)
            : base(device)
        {
            Device = device;
            _deviceControl = (YChuangRfidDoorControl)Device.DeviceControl;
            InitWebApiViewModel();
            InitTaskViewModel();
        }

        private void InitTaskViewModel()
        {
            TaskViewModel = new StringCharTaskViewModel();
            TaskViewModel.UnFinishTaskList = Device.UnFinishedTask.DataPool;
            TaskViewModel.FinishTask = Device.FinishTask;
            TaskViewModel.DoTask = Device.DoTask;

        }
        public StringCharTaskViewModel TaskViewModel { get; set; }
        public WebApiViewModel WebApiViewModel { get; set; }
        private void InitWebApiViewModel()
        {
            WebApiViewModel = new WebApiViewModel(Device.Name, HttpUrl, InvokeApi);
            WebApiViewModel.DicApiNameList = DicApiNameList;
            WebApiViewModel.DicMethodCmd = DicMethodCmd;

        }
        private Dictionary<string, string> _dicApiNameList = new Dictionary<string, string>();
        /// <summary>
        /// 接口方法列表
        /// </summary>
        public Dictionary<string, string> DicApiNameList
        {
            get
            {
                if (_dicApiNameList.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(RfidDoorApiEnum)))
                    {
                        RfidDoorApiEnum em = (RfidDoorApiEnum)value;
                        _dicApiNameList.Add(em.ToString(), em.GetDescription());
                    }
                }
                return _dicApiNameList;
            }
            set
            {
                _dicApiNameList = value;
            }
        }

        private Dictionary<string, object> _dicMethodCmd = new Dictionary<string, object>();
        public Dictionary<string, object> DicMethodCmd
        {
            get
            {
                if (_dicMethodCmd.Count == 0)
                {
                    InitMethodAndCmd();
                }
                return _dicMethodCmd;
            }
        }

        private void InitMethodAndCmd()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dicMethodCmd.Add(RfidDoorApiEnum.SendScanTask.ToString(), new SendScanTaskCmdView(new SendScanTaskCmd { DEVICE_NO = Device.ExposedUnId }));
                    _dicMethodCmd.Add(RfidDoorApiEnum.SendShow.ToString(), new SendShowCmdView(new SendShowCmd { DEVICE_NO = Device.ExposedUnId, LEVEL = MessageLevelTypeEnum.Nomal, MESSAGE = "测试", VOICE = "语音测试" }));
                    _dicMethodCmd.Add(RfidDoorApiEnum.SendStackTask.ToString(), new SendStackTaskCmdView(new SendStackTaskCmd { DEVICE_NO = Device.ExposedUnId, TASK_TYPE = StackActionTypeEnum.Stack, BOX_COUNT = 15, BOX_TYPE = BoxTypeEnum.SingleMeter, SEP_COUNT = 3 }));
                    _dicMethodCmd.Add(RfidDoorApiEnum.SendSwitchTask.ToString(), new SendSwitchTaskView(new SendSwitchTaskCmd { DEVICE_NO = Device.ExposedUnId, TASK_TYPE = DoorActionTypeEnum.CloseDoor }));
                });
        }

        private string InvokeApi(WebApiInvokeCmd cmd)
        {
            OperateResult opResult = ExecInvokeMethod(cmd);
            return opResult.Message;

        }

        public OperateResult ExecInvokeMethod(WebApiInvokeCmd cmd)
        {
            OperateResult opResult = new OperateResult();
            try
            {
                opResult = _deviceControl.SendCmd<SyncResReMsg>(cmd);
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = OperateResult.ConvertException(ex);
            }
            return opResult;
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}
