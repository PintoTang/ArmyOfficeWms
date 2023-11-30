using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.OPCUaClientImpPckg
{
    /// <summary>
    /// OpcUa通讯接口
    /// </summary>
    public interface IOpcUaCommunicationAbstract : IDisposable
    {
        event Action<string,string,object> Notification_Monitored;
        /// <summary>
        /// 连接opc服务
        /// </summary>
        /// <param name="opcServerUri"></param>
        void ConnectOpcServer(string opcServerUri);

        /// <summary>
        /// 断开opc连接
        /// </summary>
        void DisConnectOpcServer();

        /// <summary>
        /// 读取PLC中的数据，传入参数为NodeId,读取单个数据
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        string Read(string nodeId);

        /// <summary>
        /// 读取PLC中的数据，传入参数为NodeId,读取多个数据
        /// </summary>
        /// <param name="nodeIds"></param>
        /// <returns></returns>
        List<string> Read(List<string> nodeIds);

        /// <summary>
        /// 向OPC UA 写入单个数据
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="writeValue"></param>
        /// <returns></returns>
        bool Write(string nodeId, string writeValue);

        /// <summary>
        /// 向OPC UA 写入多个数据
        /// </summary>
        /// <param name="nodeIds"></param>
        /// <param name="writeValues"></param>
        /// <returns></returns>
        bool Write(List<string> nodeIds, List<string> writeValues);

        /// <summary>
        /// 订阅NodeId
        /// </summary>
        /// <param name="nodeIdStr"></param>
        /// <returns></returns>
        Subscription SubscribeNodeId(string nodeIdStr);

        /// <summary>
        /// 取消订阅NodeId
        /// </summary>
        /// <param name="subscriptionObj"></param>
        bool UnsubscribeNodeId(Subscription subscriptionObj);

        /// <summary>
        /// 注册多个NodeId,NodeId注册后可提高访问效率
        /// </summary>
        /// <param name="nodeIds"></param>
        /// <returns></returns>
        bool RegisterNodeIds(List<string> nodeIds);

        /// <summary>
        /// 解除注册NodeId
        /// </summary>
        /// <param name="nodeIds"></param>
        bool UnregisterNodeIds(List<string> nodeIds);
    }
}
