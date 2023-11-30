using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using CLDC.Framework.Log;
using CL.Framework.OPCClientAbsPckg;

namespace CL.Framework.OPCUaClientImpPckg
{
    public class OpcUaRWClient: OPCClientAbstract
    {
        private class OPCUaRWClientFactory : IOpcClientFactory
        {
            public OPCClientAbstract Create()
            {
                return new OpcUaRWClient();
            }
        }

        #region 私有变量
        private OpcUaClientContainer _opcClientContainer;
        private IOpcUaCommunicationAbstract _synServer = null;

        #endregion

        #region 构造方法
        public OpcUaRWClient()
            : this(new OpcUaClientContainer())
        {
            
            
        }
        public OpcUaRWClient(OpcUaClientContainer opcClientContainer)
        {
            this._opcClientContainer = opcClientContainer;
        }
        public OpcUaRWClient(IOpcUaCommunicationAbstract opcCommunicationAbstract)
        {
            OPCRWClientInit(new OpcUaClient(opcCommunicationAbstract));

        }
        #endregion

        #region 私有方法
        private void OPCRWClientInit(IOpcUaCommunicationAbstract opcCommunicationAbstract)
        {
            this._synServer = opcCommunicationAbstract;
            this._synServer.Notification_Monitored += OpcUa_Notification_Monitored;
        }

        private IOpcUaCommunicationAbstract GetOpcClient(string opcUaServerUrl)
        {
            IOpcUaCommunicationAbstract result;

            if (_synServer != null)
            {
                (_synServer as OpcUaClient).IsUsing = true;

                result = _synServer;
            }
            else
            {
                result = _opcClientContainer.GetOpcClient(opcUaServerUrl);
                result.Notification_Monitored += OpcUa_Notification_Monitored;
            }

            return result;
        }

        private void OpcUa_Notification_Monitored(string opcUaServerUrl, string nodeId, object value)
        {
            if (_notificationMonitored != null)
            {
                _notificationMonitored(opcUaServerUrl, nodeId, value);
            }
        }

        private string ReadNode(string opcUaServerUrl,string nodeIdStr)
        {
            string value = "";
            using (IOpcUaCommunicationAbstract client = GetOpcClient(opcUaServerUrl))
            {
                if (client != null)
                {
                    value = client.Read(nodeIdStr);
                }
            }
            return value;
        }

        private List<string> ReadNodes(string opcUaServerUrl, List<string> nodeIds)
        {
            List<string> values = null;
            using (IOpcUaCommunicationAbstract client = GetOpcClient(opcUaServerUrl))
            {
                if (client != null)
                {
                    values = client.Read(nodeIds);
                }
            }
            return values;
        }
        private bool WriteNode(string opcUaServerUrl,string nodeIdStr,string valueStr)
        {
            using (IOpcUaCommunicationAbstract client = GetOpcClient(opcUaServerUrl))
            {
                if (client != null)
                {
                    return client.Write(nodeIdStr, valueStr);
                }
            }
            return false;
        }

        private bool WriteNodes(string opcUaServerUrl,Dictionary<string,object> dic)
        {
            using (IOpcUaCommunicationAbstract client = GetOpcClient(opcUaServerUrl))
            {
                if (client != null)
                {
                    return client.Write(dic.Keys.ToList(),dic.Values.Select(t=>t.ToString()).ToList());
                }
            }
            return false;
        }
        #endregion

        #region 公共方法
        public override event OPCWriteBoardSingle OPCWriteBoardSingleEvent;
        public override event OPCWriteBoardMulti OPCWriteBoardMultiEvent;
        public override event OPCReadBoardSingle OPCReadBoardSingleEvent;
        public override event OPCReadBoardMtlti OPCReadBoardMultiEvent;

        private Action<string, string, object> _notificationMonitored;
        public override event Action<string, string, object> Notification_Monitored
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
        #endregion


        #region 重写方法
        public override string ReadString(string opcUaServerUrl, string nodeIdStr)
        {
            try
            {
                string value = ReadNode(opcUaServerUrl, nodeIdStr);
                return value;
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info("OpcUaRWClient.ReadOpcUaData(List<string> nodeIdStr) \r\n " + ex.ToString());
                return "";
            }
        }

        public override float ReadFloat(string opcUaServerUrl, string nodeIdStr)
        {
            try
            {
                string str  = ReadNode(opcUaServerUrl, nodeIdStr);
                if (!float.TryParse(str, out float value))
                {
                    Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应地址名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                        opcUaServerUrl, nodeIdStr, str));
                    return 0;
                }
                return value;
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    opcUaServerUrl, nodeIdStr, ex));
                return 0;
            }
        }

        public override bool ReadBool(string opcUaServerUrl, string nodeIdStr)
        {
            try
            {
                string str = ReadNode(opcUaServerUrl, nodeIdStr);

                if (!bool.TryParse(str, out bool value))
                {
                    Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应地址名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                           opcUaServerUrl, nodeIdStr, str));
                    return false;
                }
                return value;
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    opcUaServerUrl, nodeIdStr, ex));
                return false;
            }
        }

        public override int Read(string opcUaServerUrl, string nodeIdStr)
        {
            try
            {
                string str = ReadNode(opcUaServerUrl, nodeIdStr);

                if (!int.TryParse(str, out int value))
                {
                    Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应地址名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                           opcUaServerUrl, nodeIdStr, str));
                    return 0;
                }
                return value;
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    opcUaServerUrl, nodeIdStr, ex));
                return 0;
            }
        }

        public override List<int> Read(List<DeviceAddressInfo> deviceInfoList)
        {
            try
            {
                if (null == deviceInfoList)
                {
                    Log.getDebugFile().Info("OPC批量Read()输入无效参数。参数deviceInfoList为null");
                    return null;
                }

                if (0 == deviceInfoList.Count)
                {
                    Log.getDebugFile().Info("OPC批量Read()输入无效参数。参数deviceInfoList的长度为0");
                    return null;
                }
                Dictionary<string, int> dic = new Dictionary<string, int>();
                var groups=deviceInfoList.GroupBy(t => t.Datablock.Connection);
                foreach(var group in groups)
                {
                    var nodeIds = group.Select(t => t.Datablock.RealDataBlockAddr).ToList();
                    List<string> strs = ReadNodes(group.Key, nodeIds);
                    if(strs!=null && strs.Count > 0)
                    {
                        for(int j = 0; j < nodeIds.Count; j++)
                        {
                            string nodeId = nodeIds[j];
                            string valueStr = strs[j];
                            int value = 0;
                            if (!string.IsNullOrEmpty(valueStr))
                            {
                                if (!int.TryParse(valueStr, out value))
                                {
                                    Log.getExceptionFile().Info(string.Format("多个读取List<int>读到无效值默认为0，异常DB块为：{0},转换的值为：{1}",
                                        nodeId, valueStr));

                                    value = INVALID_VALUE;
                                }
                            }
                            dic[group.Key+"_"+nodeId]=value;
                        }
                    }
                }
                List<int> list = new List<int>();
                foreach(var deviceInfo in deviceInfoList)
                {
                    string key = deviceInfo.Datablock.Connection + "_" + deviceInfo.Datablock.RealDataBlockAddr;
                    if (dic.ContainsKey(key))
                    {
                        list.Add(dic[key]);
                    }
                    else
                    {
                        list.Add(INVALID_VALUE);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
                return null;
            }
        }

        public override List<bool> ReadBoolList(List<DeviceAddressInfo> deviceInfoList)
        {
            if (null == deviceInfoList)
            {
                Log.getDebugFile().Info("OPC批量Read()输入无效参数。参数deviceInfoList为null");
                return null;
            }

            if (0 == deviceInfoList.Count)
            {
                Log.getDebugFile().Info("OPC批量Read()输入无效参数。参数deviceInfoList的长度为0");
                return null;
            }
            try
            {
                Dictionary<string, bool> dic = new Dictionary<string, bool>();
                var groups = deviceInfoList.GroupBy(t => t.Datablock.Connection);
                foreach (var group in groups)
                {
                    var nodeIds = group.Select(t => t.Datablock.RealDataBlockAddr).ToList();
                    List<string> strs = ReadNodes(group.Key, nodeIds);
                    if (strs != null && strs.Count > 0)
                    {
                        for (int j = 0; j < nodeIds.Count; j++)
                        {
                            string nodeId = nodeIds[j];
                            string valueStr = strs[j];
                            bool value = false;
                            if (!string.IsNullOrEmpty(valueStr))
                            {
                                if (!bool.TryParse(valueStr, out value))
                                {
                                    Log.getExceptionFile().Info(string.Format("多个读取List<int>读到无效值默认为0，异常DB块为：{0},转换的值为：{1}",
                                        nodeId, valueStr));
                                }
                            }
                            dic[group.Key + "_" + nodeId] = value;
                        }
                    }
                }
                List<bool> list = new List<bool>();
                foreach (var deviceInfo in deviceInfoList)
                {
                    string key = deviceInfo.Datablock.Connection + "_" + deviceInfo.Datablock.RealDataBlockAddr;
                    if (dic.ContainsKey(key))
                    {
                        list.Add(dic[key]);
                    }
                    else
                    {
                        list.Add(false);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
                return null;
            }
        }

        public override void Write(string opcUaServerUrl, string nodeIdStr, int value)
        {
            try
            {
                WriteNode(opcUaServerUrl, nodeIdStr, value.ToString());
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void Write(string opcUaServerUrl, Dictionary<string, int> itemValueDic)
        {
            if (null == itemValueDic)
            {
                Log.getDebugFile().Info("OPC批量Write()输入无效参数。参数itemValueDic为null");
                return;
            }

            if (0 == itemValueDic.Count)
            {
                Log.getDebugFile().Info("OPC批量Write()输入无效参数。参数itemValueDic的长度为0");
                return;
            }

            try
            {
                WriteNodes(opcUaServerUrl, itemValueDic.ToDictionary(t=>t.Key,t=>(object)t.Value));
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void Write(string opcUaServerUrl, Dictionary<string, object> itemValueDic)
        {
            if (null == itemValueDic)
            {
                Log.getDebugFile().Info("OPC批量Write()输入无效参数。参数itemValueDic为null");
                return;
            }

            if (0 == itemValueDic.Count)
            {
                Log.getDebugFile().Info("OPC批量Write()输入无效参数。参数itemValueDic的长度为0");
                return;
            }

            try
            {
                WriteNodes(opcUaServerUrl, itemValueDic);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void Write(string opcUaServerUrl, string nodeIdStr, bool value)
        {
            try
            {
                WriteNode(opcUaServerUrl, nodeIdStr,value.ToString());
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void Write(string opcUaServerUrl, string nodeIdStr, string value)
        {
            try
            {
                WriteNode(opcUaServerUrl, nodeIdStr, value);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void Write(string opcUaServerUrl, string nodeIdStr, float value)
        {
            try
            {
                WriteNode(opcUaServerUrl, nodeIdStr, value.ToString());
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void Write(string opcUaServerUrl, string nodeIdStr, object value)
        {
            try
            {
                WriteNode(opcUaServerUrl, nodeIdStr, value.ToString());
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
        }

        public override void PauseOpcService()
        {

        }

        public override void RecoveryOpcService()
        {

        }

        public override IOpcClientFactory GetFactory()
        {
            return new OPCUaRWClientFactory();
        }

        public override bool SubscribeNodeId(string opcUaServerUrl, string nodeId)
        {
            using (IOpcUaCommunicationAbstract client = GetOpcClient(opcUaServerUrl))
            {
                if (client != null)
                {
                    var sub= client.SubscribeNodeId(nodeId);
                    return sub != null;
                }
            }
            return false;
        }
        #endregion
    }
}
