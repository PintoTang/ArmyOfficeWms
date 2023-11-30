using System;
using System.Collections.Generic;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.ClouRcs.ViewModel
{
    public class ClouAgvRcsViewModel : TransportDeviceViewModel
    {
        private readonly ClouAgvRcsControl _deviceControl;
        private readonly IWebNetInvoke _webNetInvoke;
        public ClouAgvRcsViewModel(TransportDeviceBaseAbstract device)
            : base(device)
        {
            Device = device;
            _deviceControl = (ClouAgvRcsControl)Device.DeviceControl;
            _webNetInvoke = _deviceControl.WebNetInvoke;
            InitWebApiViewModel();
        }
        private readonly Dictionary<string, object> _dicMethodCmd = new Dictionary<string, object>();
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
                    _dicMethodCmd.Add(ClouAgvRcsApiEnum.OrderAdd.ToString(), new OrderAddView(orderAddCmdDemo));
                });

        }

        private OrderAddCmd orderAddCmdDemo
        {
            get
            {
                return new OrderAddCmd()
                {
                    orderSource = "ClouWCS",
                    planBeginTime=DateTime.Now,
                    planEndTime = DateTime.Now,
                    priority=0,
                    orderCode="orderCode",
                    details = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            actionExtra="",
                            beginStationCode="InAndOutPort:1_1_1",
                            endStationCode="Cell:1_1_1_0_A01",
                            orderType=10,
                            productCode="barcode",
                            productName="电能表",
                            qty=1
                        }
                    },
                };
            }
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
                opResult = _webNetInvoke.ServiceRequest<ClouRcsResSucMsg>(cmd.HttpUrl, cmd.MethodName, cmd.InvokeCmd);
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
                    foreach (var value in Enum.GetValues(typeof(ClouAgvRcsApiEnum)))
                    {
                        ClouAgvRcsApiEnum em = (ClouAgvRcsApiEnum)value;
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
