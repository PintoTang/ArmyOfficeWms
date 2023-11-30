using CL.Framework.OPCClientAbsPckg;
using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using CL.Framework.FilterLogger;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using log4net;

namespace CL.Framework.OPCClientImpPckg
{
    /// <summary>
    /// OPC读写客户端
    /// </summary>
    public class OPCRWClient : OPCClientAbstract
    {
        private class OPCRWClientFactory : IOpcClientFactory
        {
            public OPCClientAbstract Create()
            {
                return new OPCRWClient();
            }
        }

        private static readonly IOpcClientFactory factory;
        private OpcClientContainer opcClientContainer;
        private IOpcCommunicationAbstract synServer = null;
        public const string GROUP_NAME = "GN";



        public override event OPCWriteBoardSingle OPCWriteBoardSingleEvent;
        public override event OPCWriteBoardMulti OPCWriteBoardMultiEvent;
        public override event OPCReadBoardSingle OPCReadBoardSingleEvent;
        public override event OPCReadBoardMtlti OPCReadBoardMultiEvent;
        public static readonly MsgPipe EventLoger = new MsgPipe(-1, DiyLog);
        private static void DiyLog(object obj, EnumLogLevel level, LoggerType loggerType, string deviceName)
        {
            ILog logger;
            switch (loggerType)
            {
                case LoggerType.GetDebugFile:
                    logger = Log.getDebugFile();
                    break;
                case LoggerType.GetErrorFile:
                    logger = Log.getErrorFile();
                    break;
                case LoggerType.GetEventFile:
                    logger = Log.getEventFile();
                    break;
                case LoggerType.GetExceptionFile:
                    logger = Log.getExceptionFile();
                    break;
                case LoggerType.GetMessageFile:
                    logger = Log.getMessageFile(deviceName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("loggerType", loggerType, null);
            }
            switch (level)
            {
                case EnumLogLevel.Error:
                    logger.Error(obj);
                    break;
                case EnumLogLevel.Warning:
                    logger.Warn(obj);
                    break;
                case EnumLogLevel.Info:
                    logger.Info(obj);
                    break;
                case EnumLogLevel.Debug:
                    logger.Debug(obj);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level", level, null);
            }
        }


        /// <summary>
        /// 默认以SimaticNET方式构造同步服务对象
        /// </summary>
        public OPCRWClient()
            : this(new OpcClientContainer())
        { }

        public OPCRWClient(OpcClientContainer opcClientContainer)
        {
            this.opcClientContainer = opcClientContainer;
            //this._plcConnectionDictionary = ConnectionStringConfig.ConnectionStrings;
            //this.opcClientContainer.Register(_plcConnectionDictionary.Values.ToArray());

            AddGroup(opcClientContainer.OpcClients.ToArray(), GROUP_NAME);


        }

        /// <summary>
        /// OPC读写客户端
        /// </summary>
        /// <param name="opcCommunicationAbstract"></param>
        public OPCRWClient(IOpcCommunicationAbstract opcCommunicationAbstract)
        {
            OPCRWClientInit(new OpcClient(opcCommunicationAbstract));

        }

        private void OPCRWClientInit(IOpcCommunicationAbstract opcCommunicationAbstract)
        {
            this.synServer = opcCommunicationAbstract;
            AddGroup(GROUP_NAME);
        }

        private IOpcCommunicationAbstract GetOpcClient(string itemName)
        {
            IOpcCommunicationAbstract result;

            if (synServer != null)
            {
                (synServer as OpcClient).IsUsing = true;

                result = synServer;
            }
            else
            {
                result = opcClientContainer.GetOpcClient(itemName);
            }

            return result;
        }

        private List<DataItem> Read(string groupName, List<string> itemNameList)
        {
            Dictionary<string,string> opcConnectionDic=  ConnectionStringConfig.GetOpcConnectionList(groupName, itemNameList);
            List<DataItem> result = new List<DataItem>();
            var opcClientByConnectionString = from itemName in itemNameList
                                              select new { ConnetionString = opcConnectionDic.Values.FirstOrDefault(e => itemName.Contains(e)), ItemName = itemName }
                                                  into g
                                                  group g by g.ConnetionString;

            foreach (var group in opcClientByConnectionString)
            {
                using (IOpcCommunicationAbstract client = GetOpcClient(group.Key))
                {
                    if (client != null)
                    {
                        List<DataItem> temp = client.Read(groupName, group.Select(e => e.ItemName).ToList());
                        result.AddRange(temp);
                    }
                }
            }
            return SortByItemNameList(itemNameList, result);
        }

        private List<DataItem> SortByItemNameList(List<string> itemNameList, List<DataItem> dataItemList)
        {
            return (from itemName in itemNameList
                    let value = dataItemList.First(e => e.ItemName == itemName)
                    select value).ToList();
        }

        private void WriteToOpc(string groupName, Dictionary<string, object> itemValueDictionary)
        {
            List<string> writeItemList=new List<string>();
            foreach (KeyValuePair<string, object> itemValue in itemValueDictionary)
            {
                writeItemList.Add(itemValue.Key);
            }
            Dictionary<string, string> opcConnectionDic = ConnectionStringConfig.GetOpcConnectionList(groupName, writeItemList);
            var opcClientByConnectionString = from kv in itemValueDictionary
                                              select new { ConnetionString = opcConnectionDic.Values.First(e => kv.Key.Contains(e)), ItemName = kv.Key, Value = kv.Value }
                                                  into g
                                                  group g by g.ConnetionString;

            foreach (var group in opcClientByConnectionString)
            {
                using (IOpcCommunicationAbstract client = GetOpcClient(group.Key))
                {
                    if (client != null)
                    {
                        client.Write(groupName, group.ToDictionary(e => e.ItemName, e => e.Value));
                    }
                }
            }
        }

        /// <summary>
        /// 关闭服务对象连接
        /// </summary>
        private void Close()
        {
            foreach (var opcClient in opcClientContainer.OpcClients)
            {
                opcClient.Close();
            }

            if (synServer != null)
            {
                synServer.Close();
            }
        }

        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="groupName">组名称</param>
        private void AddGroup(string groupName)
        {
            AddGroup(new IOpcCommunicationAbstract[] { synServer }, groupName);
        }

        private void AddGroup(IOpcCommunicationAbstract[] opcClients, string groupName)
        {
            try
            {
                foreach (IOpcCommunicationAbstract opcClient in opcClients)
                {
                    if (opcClient != null)
                    {
                        opcClient.AddGroup(groupName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("AddGroup异常,错误信息为：{0}", ex));
            }
        }

        /// <summary>
        /// 获取组名对应的值
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>对应键的值</returns>
        private string GetOpcConnection(string groupName, string itemName)
        {
            return ConnectionStringConfig.GetConnectcionString(groupName, itemName);
        }

        /// <summary>
        /// 判断传入的协议名称是否已加前缀处理
        /// </summary>
        /// <param name="itemName">协议名称</param>
        /// <returns>是否已拼接处理</returns>
        private bool IsOpcConnection(string itemName)
        {
            bool IsConnection = false;
            try
            {
                IsConnection = itemName.Contains("S7:[S7 connection_");
            }
            catch
            {
                return false;
            }
            return IsConnection;
        }

        /// <summary>
        /// 替换协议名称DBD转INT处理
        /// </summary>
        /// <param name="itemName">协议名称</param>
        /// <returns>是否已拼接处理</returns>
        private string OpcConnectionReplace(string itemName)
        {
            if (itemName.StartsWith("Q") || itemName.StartsWith("M"))
            {
                return itemName;
            }
            string itemPrefix = itemName.Substring(itemName.IndexOf(',', 1) + 1, 3);
            switch (itemPrefix)
            {
                case "DBW":
                    return itemName.Replace(itemPrefix, "INT");
                case "INT":
                    return itemName;
                case "DBD":
                    return itemName.Replace(itemPrefix, "DINT");
                case "DIN":
                    return itemName;
                case "DBX":
                    return itemName.Replace(itemPrefix, "STRING");
                case "STR":
                    return itemName;
                case "REA":
                    return itemName;
                default:
                    if (itemPrefix.StartsWith("X"))
                        return itemName;
                    throw new Exception("请检查传入协议地址" + itemName + "是否符合协议规范！");
            }
        }

        ManualResetEvent manualReset = new ManualResetEvent(true);
        int seconds = 0;
        public override void PauseOpcService()
        {
            seconds = -1;
            manualReset.Reset();
        }

        public override void RecoveryOpcService()
        {
            seconds = 0;
            manualReset.Set();
        }

        public override IOpcClientFactory GetFactory()
        {
            return new OPCRWClientFactory();
        }

        /// <summary>
        /// string类型的读取
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns></returns>
        public override string ReadString(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            string opcItemName = string.Empty;
            DataItem dataItem = null;
            try
            {
                opcItemName = CreateItem(deviceName, itemName);

                using (IOpcCommunicationAbstract client = GetOpcClient(opcItemName))
                {
                    if (client != null)
                    {
                        dataItem = client.Read(GROUP_NAME, opcItemName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC字符串读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, opcItemName, ex));
            }
            string result = string.Empty;
            if (dataItem != null)
            {
                result = dataItem.ItemValue.ToString();
            }

            PrintOPCSingleLog("Read", deviceName, opcItemName, result.ToString());

            return result;
        }

        /// <summary>
        /// float类型的读取
        /// </summary>`
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns></returns>
        public override float ReadFloat(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            float result = 0.0f;
            string opcItemName = string.Empty;
            DataItem dataItem = null;
            try
            {
                opcItemName = CreateItem(deviceName, itemName);

                using (IOpcCommunicationAbstract client = GetOpcClient(opcItemName))
                {
                    if (client != null)
                    {
                        dataItem = client.Read(GROUP_NAME, opcItemName);
                    }
                }

                if (dataItem != null)
                {
                    float value = 0.0f;
                    if (float.TryParse(dataItem.ItemValue.ToString(), out value))
                    {
                        result = value;
                    }
                    else
                    {
                        Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                            deviceName, opcItemName, dataItem.ItemValue.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, opcItemName, ex));
            }

            PrintOPCSingleLog("Read", deviceName, opcItemName, result.ToString());

            return result;
        }

        /// <summary>
        /// 单个值的读取
        /// </summary>
        /// <param name="deviceName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回读取结果</returns>
        public override int Read(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            int result = 0;
            string opcItemName = string.Empty;
            DataItem dataItem = null;
            try
            {
                opcItemName = CreateItem(deviceName, itemName);

                using (IOpcCommunicationAbstract client = GetOpcClient(opcItemName))
                {
                    if (client != null)
                    {
                        dataItem = client.Read(GROUP_NAME, opcItemName);
                    }
                }

                if (dataItem != null)
                {
                    int value;
                    if (int.TryParse(dataItem.ItemValue.ToString(), out value))
                    {
                        result = value;
                    }
                    else
                    {
                        result = INVALID_VALUE;
                        Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                            deviceName, opcItemName, dataItem.ItemValue.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, opcItemName, ex));
            }

            PrintOPCSingleLog("Read", deviceName, opcItemName, result.ToString());
            return result;
        }

        /// <summary>
        /// 多个值的读取
        /// </summary>
        /// <param name="deviceInfoList">deviceInfoList</param>
        /// <returns>返回读取结果</returns>
        public override List<int> Read(List<DeviceAddressInfo> deviceInfoList)
        {
            List<DeviceAddressInfo> deviceInfoPrintList = new List<DeviceAddressInfo>();
            manualReset.WaitOne(seconds, true);

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

            List<int> result = new List<int>();

            List<string> opcItemNameList = new List<string>();
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到批量读指令(Read):\r\n");
            try
            {
                opcItemNameList = CreateMultiReadItemList(deviceInfoList);

                List<DataItem> dataItemList = Read(GROUP_NAME, opcItemNameList);
                if (dataItemList != null && dataItemList.Count > 0)
                {
                    for (int j = 0; j < dataItemList.Count; j++)
                    {
                        int value = 0;
                        if (dataItemList[j].ItemValue != null)
                        {
                            if (!int.TryParse(dataItemList[j].ItemValue.ToString(), out value))
                            {
                                Log.getExceptionFile().Info(string.Format("多个读取List<int>读到无效值默认为0，异常DB块为：{0},转换的值为：{1}",
                                    opcItemNameList[j], dataItemList[j].ItemValue.ToString()));

                                value = INVALID_VALUE;
                            }
                        }
                        result.Add(value);
                        DeviceAddressInfo device = deviceInfoList[j] as DeviceAddressInfo;
                        string opcItemName = CreateItem(device.deviceName, device.Datablock.RealDataBlockAddr);
                        msg.AppendFormat("groupName[{0}]", device.deviceName);
                        msg.AppendFormat(",itemName[{0}]", opcItemName);
                        msg.AppendFormat(",执行完成的返回值为:{0};", value).Append(Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
            if (isPrintRealTimeReadLog)
            {
                EventLoger.LogMsg(EnumLogLevel.Info, "Read_deviceInfoList", LoggerType.GetEventFile, msg.ToString());
            }
            return result;
        }

        /// <summary>
        /// 单个值的写入
        /// </summary>
        /// <param name="deviceName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">修改后的值</param>
        public override void Write(string deviceName, string itemName, int value)
        {
            WriteObject(deviceName, itemName, value);
        }


        public override void Write(string deviceName, Dictionary<string, object> itemValueDic)
        {
            Dictionary<string, object> itemValueDicPrintDic = new Dictionary<string, object>();
            manualReset.WaitOne(seconds, true);

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

            Dictionary<string, object> opcItemValueDic = new Dictionary<string, object>();
            List<string> opcItemNameLst;

            try
            {
                CreateMultiWriteItemListAndDic(deviceName, itemValueDic, out opcItemValueDic, out opcItemNameLst);

                WriteToOpc(GROUP_NAME, opcItemValueDic);
            }
            catch (Exception ex)
            {
                string ExAppendStr = AppendOPCAddress(opcItemValueDic);
                Log.getExceptionFile().Info(string.Format("多个写异常，对应的DB块为：{0}，错误信息为：{1}", ExAppendStr, ex));
            }
            foreach (var item in opcItemValueDic)
            {
                string opcItem = item.Key;
                itemValueDicPrintDic.Add(opcItem, item.Value);
            }
            PrintOPCMultiWriteLog(deviceName, itemValueDicPrintDic);
        }

        /// <summary>
        /// 写入多个项的值
        /// </summary>
        /// <param name="deviceName">组名称</param>
        /// <param name="itemValueDic">项名称和项值集合</param>
        public override void Write(string deviceName, Dictionary<string, int> itemValueDic)
        {
            Dictionary<string, int> itemValueDicPrintDic = new Dictionary<string, int>();
            manualReset.WaitOne(seconds, true);

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

            Dictionary<string, object> opcItemValueDic = new Dictionary<string, object>();
            List<string> opcItemNameLst;

            try
            {
                CreateMultiWriteItemListAndDic(deviceName, itemValueDic, out opcItemValueDic, out opcItemNameLst);

                WriteToOpc(GROUP_NAME, opcItemValueDic);
            }
            catch (Exception ex)
            {
                string ExAppendStr = AppendOPCAddress(opcItemValueDic);
                Log.getExceptionFile().Info(string.Format("多个写异常，对应的DB块为：{0}，错误信息为：{1}", ExAppendStr, ex));
            }
            foreach (var item in opcItemValueDic)
            {
                string opcItem = item.Key;
                int opcValue = int.Parse((string)item.Value);
                itemValueDicPrintDic.Add(opcItem, opcValue);
            }
            PrintOPCMultiWriteLog(deviceName, itemValueDicPrintDic);
        }

        private string AppendOPCAddress(Dictionary<string, object> opcItemValueDic)
        {
            string returnMsg = string.Empty;
            foreach (var item in opcItemValueDic)
            {
                returnMsg += item.Key + ",";
            }
            return returnMsg;
        }

        private string CreateItem(string deviceName, string itemName)
        {
            string itemNameReal = string.Empty;
            if (IsOpcConnection(itemName) == false)
            {
                string opcConnection = GetOpcConnection(deviceName, itemName);
                if (itemName.Contains("@"))
                {
                    itemNameReal = itemName.Split('@')[1];
                }
                else
                {
                    itemNameReal = itemName;
                }
                itemNameReal = opcConnection + OpcConnectionReplace(itemNameReal);
            }
            else
            {
                itemNameReal = OpcConnectionReplace(itemName);
            }

            return itemNameReal;
        }

        private List<string> CreateMultiReadItemList(List<DeviceAddressInfo> deviceInfoList)
        {
            List<string> itemNameList = new List<string>();

            foreach (var deviceInfo in deviceInfoList)
            {
                string opcItemName = CreateItem(deviceInfo.deviceName, deviceInfo.Datablock.RealDataBlockAddr);
                itemNameList.Add(opcItemName);
            }
            return itemNameList;
        }

        private void CreateMultiWriteItemListAndDic(
            string deviceName,
            Dictionary<string, int> itemValueDic,
            out Dictionary<string, object> tmp,
            out List<string> itemNameLst)
        {
            string itemNameReal = string.Empty;
            tmp = new Dictionary<string, object>();
            string itemName = "";
            itemNameLst = new List<string>();
            string opcConnection = "";
            foreach (var item in itemValueDic)
            {
                if (IsOpcConnection(item.Key.ToString()) == false)
                {
                    opcConnection = GetOpcConnection(deviceName, item.Key);
                    if (item.Key.ToString().Contains("@"))
                    {
                        itemNameReal = (item.Key.ToString()).Split('@')[1];
                    }
                    else
                    {
                        itemNameReal = item.Key.ToString();
                    }
                    itemNameReal = opcConnection + itemNameReal;
                }
                else
                {
                    itemNameReal = item.Key.ToString();
                }
                tmp.Add(OpcConnectionReplace(itemNameReal),Convert.ToString( item.Value));
                itemNameLst.Add(OpcConnectionReplace(itemNameReal));
            }
        }

        private void CreateMultiWriteItemListAndDic(
           string deviceName,
           Dictionary<string, object> itemValueDic,
           out Dictionary<string, object> tmp,
           out List<string> itemNameLst)
        {
            string itemNameReal = string.Empty;
            tmp = new Dictionary<string, object>();
            string itemName = "";
            itemNameLst = new List<string>();
            string opcConnection = "";
            foreach (var item in itemValueDic)
            {
                if (IsOpcConnection(item.Key.ToString()) == false)
                {
                    opcConnection = GetOpcConnection(deviceName, item.Key);
                    if (item.Key.ToString().Contains("@"))
                    {
                        itemNameReal = (item.Key.ToString()).Split('@')[1];
                    }
                    else
                    {
                        itemNameReal = item.Key.ToString();
                    }
                    itemNameReal = opcConnection + itemNameReal;
                }
                else
                {
                    itemNameReal = item.Key.ToString();
                }
                tmp.Add(OpcConnectionReplace(itemNameReal),Convert.ToString( item.Value));
                itemNameLst.Add(OpcConnectionReplace(itemNameReal));
            }
        }

        private void PrintOPCMultiWriteLog(string deviceName, Dictionary<string, int> itemValueDic)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到批量写指令(Write):\r\n");
            msg.AppendFormat("deviceName[{0}]", deviceName);
            foreach (var list in itemValueDic)
            {
                string opcItemName = CreateItem(deviceName, list.Key);
                msg.AppendFormat(",key[{0}]", opcItemName);
                msg.AppendFormat(",value[{0}]\r\n", list.Value);
            }
            Log.getEventFile().Info(msg.ToString());
        }

        private void PrintOPCMultiWriteLog(string deviceName, Dictionary<string, object> itemValueDic)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到批量写指令(Write):\r\n");
            msg.AppendFormat("deviceName[{0}]", deviceName);
            foreach (var list in itemValueDic)
            {
                string opcItemName = CreateItem(deviceName, list.Key);
                msg.AppendFormat(",key[{0}]", opcItemName);
                msg.AppendFormat(",value[{0}]\r\n", list.Value);
            }
            Log.getEventFile().Info(msg.ToString());
        }


        /// <summary>
        /// 拼接设备名和地址
        /// </summary>		
        private List<string> JoinNameAddr(string deviceName, List<string> opcReadAddr)
        {
            List<string> opcReadAddrList = new List<string>();
            for (int i = 0; i < opcReadAddr.Count; i++)
            {
                opcReadAddrList.Add(deviceName + "|" + opcReadAddr[i]);
            }
            return opcReadAddrList;
        }

        /// <summary>
        /// bool类型的读取
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns></returns>
        public override bool ReadBool(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            bool result = false;
            string opcItemName = string.Empty;
            DataItem dataItem = null;
            try
            {
                opcItemName = CreateItem(deviceName, itemName);

                using (IOpcCommunicationAbstract client = GetOpcClient(opcItemName))
                {
                    if (client != null)
                    {
                        dataItem = client.Read(GROUP_NAME, opcItemName);
                    }
                }

                if (dataItem != null)
                {
                    bool value = false;
                    if (!bool.TryParse(dataItem.ItemValue.ToString(), out value))
                    {
                        Log.getExceptionFile().Info(string.Format("ReadBool读取到无效值默认为False，对应设备名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                            deviceName, opcItemName, dataItem.ItemValue.ToString()));
                    }
                    result = value;
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC字符串读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, opcItemName, ex));
            }

            PrintOPCSingleLog("Read", deviceName, opcItemName, result.ToString());

            return result;
        }

        /// <summary>
        /// bool类型的写入
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="itemName"></param>
        /// <param name="value"></param>
        public override void Write(string deviceName, string itemName, bool value)
        {
            WriteObject(deviceName, itemName, value);
        }

        public override void Write(string deviceName, string itemName, float value)
        {
            WriteObject(deviceName, itemName, value);
        }

        private void WriteObject(string deviceName, string itemName, object value)
        {
            manualReset.WaitOne(seconds, true);

            string opcItemName = string.Empty;

            try
            {
                opcItemName = CreateItem(deviceName, itemName);

                using (IOpcCommunicationAbstract client = GetOpcClient(opcItemName))
                {
                    if (client != null)
                    {
                        client.Write(GROUP_NAME, opcItemName, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个写入异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, itemName, ex));
            }

            PrintOPCSingleLog("Write", deviceName, opcItemName, value.ToString());
        }

        public override List<bool> ReadBoolList(List<DeviceAddressInfo> deviceInfoList)
        {
            manualReset.WaitOne(seconds, true);
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
            List<bool> result = new List<bool>();

            List<string> opcItemNameList = new List<string>();
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到批量读指令(Read):\r\n");
            try
            {
                opcItemNameList = CreateMultiReadItemList(deviceInfoList);

                List<DataItem> dataItemList = Read(GROUP_NAME, opcItemNameList);

                if (dataItemList != null && dataItemList.Count > 0)
                {
                    for (int j = 0; j < dataItemList.Count; j++)
                    {
                        bool value = false;
                        if (dataItemList[j].ItemValue != null)
                        {
                            if (!bool.TryParse(dataItemList[j].ItemValue.ToString(), out value))
                            {
                                //Log.getExceptionFile().Info(string.Format("BoolList读取到无效值默认为False，异常DB块为：{0},转换的值为：{1}", opcItemNameList[j], dataItemList[j].ItemValue.ToString()));
                            }
                        }
                        result.Add(value);

                        DeviceAddressInfo device = deviceInfoList[j] as DeviceAddressInfo;
                        string opcItemName = CreateItem(device.deviceName, device.Datablock.RealDataBlockAddr);
                        msg.AppendFormat("groupName[{0}]", device.deviceName);
                        msg.AppendFormat(",itemName[{0}]", opcItemName);
                        msg.AppendFormat(",执行完成的返回值为:{0};\r\n", value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex);
            }
            if (isPrintRealTimeReadLog)
            {
                EventLoger.LogMsg(EnumLogLevel.Info, "ReadBoolList", LoggerType.GetEventFile, msg.ToString());
            }
            return result;
        }

        private void PrintOPCSingleLog(string type, string deviceName, string itemName, string result)
        {
            if (type == "Read" && !isPrintRealTimeReadLog)
            {
                return;
            }
            StringBuilder msg = new StringBuilder();
            string typeInfo = string.Empty;
            if (type == "Read")
            {
                typeInfo = "接收到单独读指令(Read):\r\n";
            }
            else if (type == "Write")
            {
                typeInfo = "接收到单独写指令(Write):\r\n";
            }
            else
            {
                typeInfo = type;
            }
            msg.Append(typeInfo);
            msg.AppendFormat("deviceName[{0}]", deviceName);
            msg.AppendFormat(",itemName[{0}]", itemName);
            msg.AppendFormat("。执行完成的返回值为：{0}", result);
            Log.getEventFile().Info(msg.ToString());
        }

        public override void Write(string deviceName, string itemName, string value)
        {
            WriteObject(deviceName, itemName, value);
        }

        public override void Write(string deviceName, string itemName, object value)
        {
            WriteObject(deviceName, itemName, value);
        }
    }
}
