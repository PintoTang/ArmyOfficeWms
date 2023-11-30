using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.FourWayVehicle.ViewModel
{
    public class FourWayVehicleRcsViewModel : TransportDeviceViewModel
    {
        private readonly FourWayVehicleRcsControl _deviceControl;
        private readonly IWebNetInvoke _webNetInvoke;
        public FourWayVehicleRcsViewModel(TransportDeviceBaseAbstract device)
            : base(device)
        {
            Device = device;
            _deviceControl = (FourWayVehicleRcsControl)Device.DeviceControl;
            _webNetInvoke = _deviceControl.WebNetInvoke;
            InitWebApiViewModel();
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
            //处理问题：判断条件，调用线程必须为STA，因为许多UI组件都需要
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dicMethodCmd.Add(FourWayVehicleRcsApiEnum.DeleteTask.ToString(), new DeleteTaskView(new DeleteTaskCmd()));
                    _dicMethodCmd.Add(FourWayVehicleRcsApiEnum.SelectTaskInfo.ToString(), new SelectTaskInfoView(new SelectTaskInfoCmd()));
                    _dicMethodCmd.Add(FourWayVehicleRcsApiEnum.RequestRCSPermit.ToString(), new RequestRCSPermitView(new RequestRCSPermitCmd()));
                    _dicMethodCmd.Add(FourWayVehicleRcsApiEnum.ReportRCSPermitFinish.ToString(), new ReportRCSPermitFinishView(new ReportRCSPermitFinishCmd()));
                    _dicMethodCmd.Add(FourWayVehicleRcsApiEnum.SendTask.ToString(), new SendTaskView(new SendTaskCmd()));
                    _dicMethodCmd.Add(FourWayVehicleRcsApiEnum.SendChangeTask.ToString(), new SendChangeTaskView(new SendChangeTaskCmd()));
                    //_dicMethodCmd.Add(FourWayVehicleRcsApiEnum.SendModeSwitch.ToString(), new SendChangeTaskView(new SendChangeTaskCmd()));
                });

        }
        public WebApiViewModel WebApiViewModel { get; set; }
        private void InitWebApiViewModel()
        {
            WebApiViewModel = new WebApiViewModel(Device.Name, HttpUrl, InvokeApi);
            WebApiViewModel.DicApiNameList = DicApiNameList;
            WebApiViewModel.DicMethodCmd = DicMethodCmd;

        }
        //HttpUrl地址
        public string HttpUrl
        {
            get
            {
                return _deviceControl.Http;
            }
        }
        public OperateResult<string> ExecInvokeMethod(WebApiInvokeCmd cmd)
        {
            OperateResult<string> opResult = new OperateResult<string>();
            try
            {
                //组Json
                opResult = _webNetInvoke.ServiceRequest<SyncResReMsg>(cmd.HttpUrl, cmd.MethodName, cmd.InvokeCmd);
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Content = OperateResult.ConvertException(ex);
            }
            return opResult;
        }

        private string InvokeApi(WebApiInvokeCmd cmd)
        {
            OperateResult<string> opResult = ExecInvokeMethod(cmd);
            if (opResult.IsSuccess)
            {
                //返回数据
                return opResult.Content;
            }
            else
            {
                return opResult.Message;
            }

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
                    foreach (var value in Enum.GetValues(typeof(FourWayVehicleRcsApiEnum)))
                    {
                        FourWayVehicleRcsApiEnum em = (FourWayVehicleRcsApiEnum)value;
                        _dicApiNameList.Add(em.ToString(), em.GetDescription());
                    }
                }
                return _dicApiNameList;
            }
            set
            {
                _dicApiNameList = value;
                RaisePropertyChanged("DicApiNameList");
            }
        }
    }
}
