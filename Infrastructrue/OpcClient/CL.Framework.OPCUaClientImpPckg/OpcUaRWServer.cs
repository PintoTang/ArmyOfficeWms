using CLDC.CLWS.CLWCS.Framework;
using CLDC.Framework.Log;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.OPCUaClientImpPckg
{
    public class OpcUaRWServer : IOpcUaCommunicationAbstract
    {
        private Action<string, string, object> _notificationMonitored;
        public event Action<string, string, object> Notification_Monitored
        {
            add
            {
                if (_notificationMonitored == null)
                {
                    _notificationMonitored += value;
                }
            }
            remove
            {
                _notificationMonitored -= value;
            }
        }

        #region 私有变量
        private Session _mySession;
        private UAClientHelperAPI _myClientHelperAPI;
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public OpcUaRWServer()
        {
            _myClientHelperAPI = new UAClientHelperAPI();
            _myClientHelperAPI.ItemChangedNotification += Notification_MonitoredItem;
            _myClientHelperAPI.KeepAliveNotification += new KeepAliveEventHandler(Notification_KeepAlive);
        }


        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
            object value = notification.Value.WrappedValue.Value;
            string nodeId = monitoredItem.ResolvedNodeId.ToString();
            string opcUaServerUrl = monitoredItem.Subscription.Session.ConfiguredEndpoint.EndpointUrl.AbsoluteUri;
            if (_notificationMonitored != null)
            {
                _notificationMonitored(opcUaServerUrl,nodeId,value);
            }
        }

        private string _opcServerUri = "";
        public void ConnectOpcServer(string opcServerUri)
        {
            _opcServerUri = opcServerUri;
            if (_mySession != null && !_mySession.Disposed)
            {
                _myClientHelperAPI.Disconnect();
            }
            try
            {
                AsyncHelper.RunSync(()=>_myClientHelperAPI.Connect(opcServerUri, false));//是否是安全连接，false为无安全连接，暂时为不安全的连接
                _mySession = _myClientHelperAPI.Session;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Notification_KeepAlive(ISession sender, KeepAliveEventArgs e)
        {
            if (e.CurrentState == ServerState.Shutdown|| e.CurrentState==ServerState.Failed)
            {
            }
        }

        public void DisConnectOpcServer()
        {
            try
            {
                _myClientHelperAPI.Disconnect();
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info("OpcUaRWClient.DisConnectOpcServer \r\n " + ex.ToString());
            }
        }

        public void Dispose()
        {
            DisConnectOpcServer();
        }

        private readonly object ReadLock = new object();
        private readonly object ReadListLock = new object();
        public string Read(string nodeId)
        {
            lock (ReadLock)
            {
                try
                {
                    var values = _myClientHelperAPI.ReadValues(new List<string> { nodeId });
                    return values.ElementAt(0);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"OPCUAD读取异常:单个读取数据失败！项名{nodeId}:{ex}");
                }
            }
        }

        public List<string> Read(List<string> nodeIds)
        {
            lock (ReadListLock)
            {
                try
                {
                    var values = _myClientHelperAPI.ReadValues(nodeIds);
                    return values;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"OPCUAD读取异常:多个读取数据失败！{ex}");
                }
            }
        }

        public bool RegisterNodeIds(List<string> nodeIds)
        {
            try
            {
                _myClientHelperAPI.RegisterNodeIds(nodeIds);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UnregisterNodeIds(List<string> nodeIds)
        {
            try
            {
                _myClientHelperAPI.UnregisterNodeIds(nodeIds);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Subscription SubscribeNodeId(string nodeId)
        {
            try
            {
                Subscription subscription = _myClientHelperAPI.Subscribe(10);
                var myMonitoredItem = _myClientHelperAPI.AddMonitoredItem(subscription, nodeId, 1);
                return subscription;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"OPCUAD订阅异常:多个订阅数据失败！{ex}");
            }
        }

        public bool UnsubscribeNodeId(Subscription subscriptionObj)
        {
            try
            {
                _myClientHelperAPI.RemoveSubscription(subscriptionObj);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"OPCUAD取消订阅异常:取消订阅数据失败！{ex}");
            }
        }

        public bool Write(string nodeId, string writeValue)
        {
            try
            {
                string returnVal = _myClientHelperAPI.WriteValues(new List<string> { writeValue }, new List<string> { nodeId });
                return returnVal.ToLower()=="good";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"OPCUAD写入异常:单个写入数据失败！{nodeId}--{writeValue}:{ex}");
            }
        }

        public bool Write(List<string> nodeIds, List<string> writeValues)
        {
            try
            {
                string returnVal = _myClientHelperAPI.WriteValues(writeValues, nodeIds);
                return returnVal.ToLower() == "good";
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"OPCUAD写入异常:多个写入数据失败！{ex}");
            }
        }
    }
}
