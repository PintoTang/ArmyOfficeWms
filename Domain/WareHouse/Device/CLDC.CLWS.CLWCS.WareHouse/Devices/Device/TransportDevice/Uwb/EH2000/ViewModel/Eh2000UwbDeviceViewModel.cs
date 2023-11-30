using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Uwb.Eh2000.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Uwb.EH2000.ViewModel
{
    public sealed class Eh2000UwbDeviceViewModel : TransportDeviceViewModel
    {
        public Eh2000UwbDeviceViewModel(Eh2000UwbDevice device)
            : base(device)
        {
            Device = device;
            _deviceControl = (Eh2000UwbControl)Device.DeviceControl;
            InitWebApiViewModel();
        }
        private readonly Eh2000UwbControl _deviceControl;
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
                    foreach (var value in Enum.GetValues(typeof(Eh2000UwbApiEnum)))
                    {
                        Eh2000UwbApiEnum em = (Eh2000UwbApiEnum)value;
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
                    _dicMethodCmd.Add(Eh2000UwbApiEnum.publishEink.ToString(), new Eh2000UwbCmdView(DemoPublishEinkCmd));
                });
        }

        private string DemoPublishEinkCmd
        {
            get
            {
                PublishEinkCmd cmd = new PublishEinkCmd { card_ids = Device.ExposedUnId, message = "测试", shake = EinkShakeTypeEnum.HasShake, type = EinkNotifyTypeEnum.Notice, voice = EinkVoiceTypeEnum.NoVoice };
                return cmd.ToString();
            }
        }

        public string HttpUrl
        {
            get
            {
                return _deviceControl.Http;
            }
        }

        private string InvokeApi(WebApiInvokeCmd cmd)
        {
            OperateResult opResult = ExecInvokeMethod(cmd);
            return opResult.Message;

        }

        private OperateResult ExecInvokeMethod(WebApiInvokeCmd cmd)
        {
            OperateResult opResult = new OperateResult();
            try
            {
                opResult = _deviceControl.EinkApi(cmd);
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = OperateResult.ConvertException(ex);
            }
            return opResult;
        }
    }
}
