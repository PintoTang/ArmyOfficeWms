using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.ViewModel
{
    public class WebApiViewModel : ViewModelBase
    {
        public WebApiViewModel(string title, string httpUrl, Func<WebApiInvokeCmd, string> invokeApi)
        {
            this.Title = title;
            this.HttpUrl = httpUrl;
            this.InvokeApiAction += invokeApi;
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }


        public Dictionary<string, object> DicMethodCmd { get; set; }

        public Dictionary<string, string> DicApiNameList { get; set; }

        public string HttpUrl
        {
            get { return _httpUrl; }
            set
            {
                _httpUrl = value;
                RaisePropertyChanged();
            }
        }

        public object RequestView
        {
            get { return _requestView; }
            set
            {
                _requestView = value;
                RaisePropertyChanged();

            }
        }

        public string ReponseValue
        {
            get { return _reponseValue; }
            set
            {
                _reponseValue = value;
                RaisePropertyChanged();
            }
        }


        private object GetCmdByMethodName(string methodName)
        {
            if (DicMethodCmd != null)
            {
                if (DicMethodCmd.ContainsKey(methodName))
                {
                    return DicMethodCmd[methodName];
                }
            }
            return string.Empty;
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
                    RequestView = GetCmdByMethodName(value);
                }
                RaisePropertyChanged();
            }
        }



        public Func<WebApiInvokeCmd, string> InvokeApiAction { get; set; }

        private RelayCommand<object> _invokeApiCommand;
        private string _title;
        private string _httpUrl;
        private object _requestView;
        private string _reponseValue;

        public RelayCommand<object> InvokeApiCommand
        {
            get
            {
                if (_invokeApiCommand == null)
                {
                    _invokeApiCommand = new RelayCommand<object>(InvokeApi);
                }
                return _invokeApiCommand;
            }
        }

        private async void InvokeApi(object obj)
        {
            IInvokeCmd cmdViewModel = obj as IInvokeCmd;
            if (cmdViewModel==null)
            {
                return;
            }
            if (InvokeApiAction == null)
            {
                return;
            }
            ReponseValue = string.Empty;
            WebApiInvokeCmd cmd = new WebApiInvokeCmd(HttpUrl, SelectedApiName, cmdViewModel.GetCmd());

             Task<string> taskReult= InvokeApiAsync(cmd);
            ReponseValue =await taskReult;

            SnackbarQueue.Enqueue(string.Format("调用接口 {0} 完成", SelectedApiName));
        }

        private async Task<string> InvokeApiAsync(WebApiInvokeCmd cmd)
        {
           string result = await Task.Run(() => InvokeApiAction(cmd));
            return result;
        }

    }
}
