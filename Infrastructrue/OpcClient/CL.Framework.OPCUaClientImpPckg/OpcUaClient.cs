using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.OPCUaClientImpPckg
{
    public class OpcUaClient : IOpcUaCommunicationAbstract
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

        private volatile bool _isUsing;
        private HashSet<string> _itemNameHashSet;
        private IOpcUaCommunicationAbstract _opcClient;

        public OpcUaClient(IOpcUaCommunicationAbstract opcClient)
        {
            this._opcClient = opcClient;
            this._opcClient.Notification_Monitored += _opcClient_Notification_Monitored;
            this._isUsing = false;
            this._itemNameHashSet = new HashSet<string>();
        }

        private void _opcClient_Notification_Monitored(string opcUaServerUrl, string nodeId, object value)
        {
            if (_notificationMonitored != null)
            {
                _notificationMonitored(opcUaServerUrl, nodeId, value);
            }
        }

        public bool IsUsing
        {
            get
            {
                return _isUsing;
            }
            set
            {
                _isUsing = value;
            }
        }
        public void ConnectOpcServer(string opcServerUri)
        {
            this._opcClient.ConnectOpcServer(opcServerUri);
        }

        public void DisConnectOpcServer()
        {
            this._opcClient.DisConnectOpcServer();
        }

        public void Dispose()
        {
            _isUsing = false;
        }

        public string Read(string nodeId)
        {
            TryRegisterNodeId(nodeId);

            return _opcClient.Read(nodeId);
        }

        public List<string> Read(List<string> nodeIds)
        {
            TryRegisterNodeId(nodeIds);

            return _opcClient.Read(nodeIds);
        }

        public bool RegisterNodeIds(List<string> nodeIds)
        {
            return _opcClient.RegisterNodeIds(nodeIds);
        }

        public bool UnregisterNodeIds(List<string> nodeIds)
        {
            return _opcClient.UnregisterNodeIds(nodeIds);
        }

        public Subscription SubscribeNodeId(string nodeId)
        {
            return _opcClient.SubscribeNodeId(nodeId);
        }

        public bool UnsubscribeNodeId(Subscription subscriptionObj)
        {
            return _opcClient.UnsubscribeNodeId(subscriptionObj);
        }

        public bool Write(string nodeId, string writeValue)
        {
            return _opcClient.Write(nodeId, writeValue);
        }

        public bool Write(List<string> nodeIds, List<string> writeValues)
        {
            return _opcClient.Write(nodeIds, writeValues);
        }

        private void TryRegisterNodeId(List<string> nodeIds)
        {
            List<string> filterNodeIds = new List<string>();
            foreach(var nodeId in nodeIds)
            {
                if (!Contains(nodeId))
                {
                    filterNodeIds.Add(nodeId);
                }
            }
            RegisterNodeIds(nodeIds);
        }

        private void TryRegisterNodeId(string nodeId)
        {
            if (!Contains(nodeId))
            {
                RegisterNodeIds(new List<string> { nodeId });
            }
        }

        private bool Contains(string nodeId)
        {
            lock (_itemNameHashSet)
            {
                return !_itemNameHashSet.Add(nodeId);
            }
        }
    }
}
