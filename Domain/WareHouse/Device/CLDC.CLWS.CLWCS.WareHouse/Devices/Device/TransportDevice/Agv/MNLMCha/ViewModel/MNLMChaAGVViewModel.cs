using System;
using System.Collections.Generic;
using System.IO;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.MNLMCha.ViewModel
{
    public class MNLMChaAGVViewModel : TransportDeviceViewModel
    {
        private readonly IWebNetInvoke _webNetInvoke;
        public MNLMChaAGVViewModel(MNLMChaAgv device)
            : base(device)
        {
            Device = device;
            _deviceControl = (MNLMChaAgvControl) Device.DeviceControl;
            _webNetInvoke = _deviceControl.WebNetInvoke;

        }

        private readonly MNLMChaAgvControl _deviceControl;

        #region 属性
    
        //HttpUrl地址
        public string HttpUrl
        {
            get
            {
                return _deviceControl.Http; 
            }
        }
        private string _requestValue;
        //输入参数
        public string RequestValue
        {
            get { return _requestValue; }
            set
            {
                if (_requestValue != value)
                {
                    _requestValue = value;
                    RaisePropertyChanged("RequestValue");
                }
            }
        }
        private string _reponseValue;
        //输出参数
        public string ReponseValue
        {
            get { return _reponseValue; }
            set
            {
                if (_reponseValue != value)
                {
                    _reponseValue = value;
                    RaisePropertyChanged("ReponseValue");
                }
            }
        }
        private string _selectedApiName;
        /// <summary>
        /// 当前选择的接口名称
        /// </summary>
        public string SelectedApiName
        {
            get { return _selectedApiName; }
            set
            {
                if (_selectedApiName != value)
                {
                    _selectedApiName = value;
                    RequestValue = GetInputParms();//调用 切换输入框格式
                }
                RaisePropertyChanged("SelectedApiName");
            }
        }
        private string GetInputParms()
        {
            string strJson = "";
            if (string.IsNullOrEmpty(SelectedApiName)) return strJson;
            if (SelectedApiName.Equals("SendTask"))
            {
                strJson = CreateInputTemplate_SendTask();
            }
            else if (SelectedApiName.Equals("SelectTaskStatus"))
            {
                strJson = CreateInputTemplate_SelectTaskStatus();
            }
            else if (SelectedApiName.Equals("DeleteTask"))
            {
                strJson = CreateInputTemplate_DeleteTask();
            }
            else
            {
                //待对接接口
            }
            return ConvertJsonString(strJson);
        }
        /// <summary>
        /// 请求下发任务 模板 
        /// </summary>
        /// <returns></returns>
        private string CreateInputTemplate_SendTask()
        {
            return JsonConvert.SerializeObject(new MNLMChaTaskMode());
        }
        /// <summary>
        /// 查询任务状态
        /// </summary>
        /// <returns></returns>
        private string CreateInputTemplate_SelectTaskStatus()
        {
            Dictionary<string, string> dataDic = new Dictionary<string, string>();
            dataDic.Add("TASK_NO", "");
            return JsonConvert.SerializeObject(dataDic);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <returns></returns>
        private string CreateInputTemplate_DeleteTask()
        {
            Dictionary<string, string> dataDic = new Dictionary<string, string>();
            dataDic.Add("TASK_NO", "");
            return JsonConvert.SerializeObject(dataDic);
        }

        private Dictionary<string, string> _ApiNameListDic = new Dictionary<string, string>();
        /// <summary>
        /// 接口方法列表
        /// </summary>
        public Dictionary<string, string> ApiNameListDic
        {
            get
            {
                if (_ApiNameListDic.Count == 0)
                {
                    _ApiNameListDic.Add("SendTask", "请求下发任务");
                    _ApiNameListDic.Add("SelectTaskStatus", "查询任务状态");
                    _ApiNameListDic.Add("DeleteTask", "删除任务");
                }
                return _ApiNameListDic;
            }
            set
            {
                _ApiNameListDic = value;
                RaisePropertyChanged("ApiNameListDic");
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

        #region Event RelayCommand
        private RelayCommand _invokeApiCommand;

        public RelayCommand InvokeApiCommand
        {
            get
            {
                if (_invokeApiCommand == null)
                {
                    _invokeApiCommand = new RelayCommand(InvokeApi);
                }
                return _invokeApiCommand;
            }
        }
        private RelayCommand _clearInputCommand;
        /// <summary>
        /// 清理input数据 
        /// </summary>
        public RelayCommand ClearInputCommand
        {
            get
            {
                if (_clearInputCommand == null)
                {
                    _clearInputCommand = new RelayCommand(ClearInputParmsData);
                }
                return _clearInputCommand;
            }
        }
        private RelayCommand _clearOutputCommand;
        /// <summary>
        /// 清理input数据 
        /// </summary>
        public RelayCommand ClearOutputCommand
        {
            get
            {
                if (_clearOutputCommand == null)
                {
                    _clearOutputCommand = new RelayCommand(ClearOutputParmsData);
                }
                return _clearOutputCommand;
            }
        }
        #endregion

        public void InvokeApi()
        {
            OperateResult<string> opResult = ExecInvokeMethod();
            if (opResult.IsSuccess)
            {
                //返回数据 
                ReponseValue = ConvertJsonString(opResult.Content);
            }
            else
            {
                //S_OutputParms =ConvertJsonString( opResult.Message);
                ReponseValue = opResult.Message;
            }

        }

        public OperateResult<string> ExecInvokeMethod()
        {
            OperateResult<string> opResult = new OperateResult<string>();
            try
            {
                //组Json
                opResult = _webNetInvoke.ServiceRequest<SyncResReMsg>(HttpUrl, SelectedApiName, RequestValue);
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Content = OperateResult.ConvertException(ex);
            }
            return opResult;
        }

        public void ClearInputParmsData()
        {
            RequestValue = "";
        }
        public void ClearOutputParmsData()
        {
            ReponseValue = "";
        }

        /// <summary>
        /// JSON格式化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ConvertJsonString(string str)
        {
            try
            {

                //格式化json字符串
                JsonSerializer serializer = new JsonSerializer();
                TextReader tr = new StringReader(str);
                JsonTextReader jtr = new JsonTextReader(tr);
                object obj = serializer.Deserialize(jtr);
                if (obj != null)
                {
                    StringWriter textWriter = new StringWriter();
                    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                    {
                        Formatting = Newtonsoft.Json.Formatting.Indented,
                        Indentation = 4,
                        IndentChar = ' '
                    };
                    serializer.Serialize(jsonWriter, obj);
                    return textWriter.ToString();
                }
                else
                {
                    return str;
                }
            }
            catch (Exception ex)
            {
                return str;
            }
        }

        public override void NotifyAttributeChange(string attributeName, object newValue)
        {
            return;
        }
    }
}
