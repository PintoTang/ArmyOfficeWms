using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace CLDC.CLWS.CLWCS.Infrastructrue.Sockets.Client.ViewModel
{
    public class SocketClientViewModel : ViewModelBase
    {
        public SocketAsyncClientAbstract DataModel { get; set; }
        public SocketClientViewModel(SocketAsyncClientAbstract socketClient)
        {
            DataModel = socketClient;
            DataModel.ReceivedMessageNotifyEvent += DataModel_ReceivedMessageNotifyEvent;
            DataModel.NotifyConnectedStatusChangeEvent += DataModel_NotifyConnectedStatusChangeEvent;
            DataModel.MessageReportEvent+= DataModelOnMessageReportEvent;
            IsConnected = DataModel.IsConnected;
        }

        private void DataModelOnMessageReportEvent(string message,EnumLogLevel messageLevel)
        {
            RecieveContent = string.Empty;
            RecieveContent = message;
        }

        void DataModel_NotifyConnectedStatusChangeEvent(bool isNormal)
        {
            IsConnected = isNormal;
        }

        void DataModel_ReceivedMessageNotifyEvent(byte[] datas)
        {
            RecieveContent = string.Empty;
            RecieveContent = Encoding.ASCII.GetString(datas);
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                RaisePropertyChanged();
            }
        }

        public string RemoteIp { get { return DataModel.RemoteIp; } }

        public int RemotePort { get { return DataModel.RemotePort; } }

        public string LocalIp { get { return DataModel.LocalIp; } }

        public int LocalPort { get { return DataModel.LocalPort; } }

        public string RecieveContent
        {
            get { return _recieveContent; }
            set
            {
                _recieveContent = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<string> _sendCommand;
        private bool _isConnected;
        private string _recieveContent;
        private RelayCommand _clearCommand;
        private RelayCommand _connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new RelayCommand(Connect);
                }
                return _connectCommand;
            }
        }

        private RelayCommand _disConnectCommand;
        public RelayCommand DisConnectCommand
        {
            get
            {
                if (_disConnectCommand == null)
                {
                    _disConnectCommand = new RelayCommand(DisConnect);
                }
                return _disConnectCommand;
            }
        }

        private void DisConnect()
        {
            DataModel.DisconnectService();
        }

        private void Connect()
        {
            return;
        }

        public RelayCommand<string> SendCommand
        {
            get
            {
                if (_sendCommand == null)
                {
                    _sendCommand = new RelayCommand<string>(Send);
                }
                return _sendCommand;
            }
        }

        private void Send(string content)
        {
            byte[] contentByets = Encoding.ASCII.GetBytes(content);
            OperateResult sendResult = DataModel.Send(contentByets);
            if (sendResult.IsSuccess)
            {
                RecieveContent = "发送成功";
            }
            else
            {
                RecieveContent = string.Format("发送失败：{0}", sendResult.Message);
            }
        }

        public RelayCommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                {
                    _clearCommand = new RelayCommand(Clear);
                }
                return _clearCommand;
            }
        }

        private void Clear()
        {
            RecieveContent = string.Empty;
        }
    }
}
