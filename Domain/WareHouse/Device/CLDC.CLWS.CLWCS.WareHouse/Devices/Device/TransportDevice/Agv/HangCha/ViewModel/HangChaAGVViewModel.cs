using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;
using Newtonsoft.Json;
using System.Windows;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.HangCha.ViewModel
{
    public class HangChaAGVViewModel : TransportDeviceViewModel
    {
        private readonly IWebNetInvoke _webNetInvoke;
        public HangChaAGVViewModel(AgvDeviceAbstract device)
            : base(device)
        {
            Device = device;
            _deviceControl = (HangChaAgvControl) Device.DeviceControl;
            _webNetInvoke = _deviceControl.WebNetInvoke;
            InitWebApiViewModel();
        }

        public WebApiViewModel WebApiViewModel { get; set; }
        private void InitWebApiViewModel()
        {
            WebApiViewModel = new WebApiViewModel(Device.Name, HttpUrl, InvokeApi);
            WebApiViewModel.DicApiNameList = DicApiNameList;
            WebApiViewModel.DicMethodCmd = DicMethodCmd;

        }

        private Dictionary<string, object> _dicMethodCmd = new Dictionary<string, object>();
        public Dictionary<string, object> DicMethodCmd
        {
            get
            {
                if (_dicMethodCmd.Count==0)
                {
                    InitMethodAndCmd();
                }
                return _dicMethodCmd;
            }
        }
        /// <summary>
        /// 请求下发任务 模板 
        /// </summary>
        /// <returns></returns>
        private SendAddTaskCmd DemoSendAddTaskCmd
        {
            get
            {
                return new SendAddTaskCmd
                {
                    businessType = "01"
                };
            }
        }

        /// <summary>
        /// 查询任务状态
        /// </summary>
        /// <returns></returns>
        private SendTaskStateCmd DemoSendTaskStateCmd
        {
            get
            {
                return new SendTaskStateCmd
                {
                    taskNo = ""
                };
            }
        }

        /// <summary>
        /// 查询车辆状态
        /// </summary>
        /// <returns></returns>
        private string CreateInputTemplate_carInfo()
        {
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            dicData.Add("taskNo", "");
            return JsonConvert.SerializeObject(dicData);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        private string CreateInputTemplate_deleteTask()
        {
            Dictionary<string, string> dicData = new Dictionary<string, string>();
            dicData.Add("taskNo", "");
            return JsonConvert.SerializeObject(dicData);
        }
        /// <summary>
        /// 车辆暂停或启动
        /// </summary>
        /// <returns></returns>
        private SendCarOperationCmd DemoSendCarOperationCmd
        {
            get
            {
                return new SendCarOperationCmd
                {
                    carNo = "1"
                };
            }
        }

        private void InitMethodAndCmd()
        {
            //处理问题：判断条件，调用线程必须为STA，因为许多UI组件都需要
            if (Application.Current != null && Application.Current.Dispatcher != null)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dicMethodCmd.Add(HangChaAgvApiEnum.addTask.ToString(), new SendAddTaskCmdView(DemoSendAddTaskCmd));
                    _dicMethodCmd.Add(HangChaAgvApiEnum.taskState.ToString(), new SendTaskStateCmdView(DemoSendTaskStateCmd));
                    _dicMethodCmd.Add(HangChaAgvApiEnum.carInfo.ToString(), new SendTaskStateCmdView(DemoSendTaskStateCmd));
                    _dicMethodCmd.Add(HangChaAgvApiEnum.deleteTask.ToString(), new SendTaskStateCmdView(DemoSendTaskStateCmd));
                    _dicMethodCmd.Add(HangChaAgvApiEnum.carOperationControl.ToString(), new SendCarOperationCmdView(DemoSendCarOperationCmd));

                });

        }

        private readonly HangChaAgvControl _deviceControl;

        #region 属性
    
        //HttpUrl地址
        public string HttpUrl
        {
            get
            {
                return _deviceControl.Http; 
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
                    foreach (var value in Enum.GetValues(typeof(HangChaAgvApiEnum)))
                    {
                        HangChaAgvApiEnum em = (HangChaAgvApiEnum)value;
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
        private TransportMessage _tranSportMsg;
        public TransportMessage TranSportMsg
        {
            get
            {
                return _tranSportMsg;
            }
            set
            {
                _tranSportMsg = value;
                RaisePropertyChanged("TranSportMsg");
            }
        }

        #endregion

      
        private string InvokeApi(WebApiInvokeCmd cmd)
        {
            OperateResult<string> opResult = ExecInvokeMethod(cmd);
            if (opResult.IsSuccess)
            {
                //返回数据
                return   opResult.Content;
            }
            else
            {
                return  opResult.Message;
            }

        }

        public OperateResult<string> ExecInvokeMethod(WebApiInvokeCmd cmd)
        {
            OperateResult<string> opResult = new OperateResult<string>();
            try
            {
                //组Json
                opResult = _webNetInvoke.ServiceRequest<SyncResReMsgForHangChaAgv>(cmd.HttpUrl, cmd.MethodName, cmd.InvokeCmd);
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Content = OperateResult.ConvertException(ex);
            }
            return opResult;
        }


        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            return;
        }
    }
}
