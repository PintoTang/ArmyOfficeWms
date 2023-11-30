using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using CL.Framework.OPCClientAbsPckg;
using CLDC.Framework.Log;
using CL.Framework.OPCClientImpPckg;
using CL.Framework.FilterLogger;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Infrastructrue.Ioc.DependencyFactory;
using log4net;


namespace CL.Framework.Testing.OPCClientImpPckg
{
    /// <summary>
    /// OPC数据库读写客户端
    /// </summary>
    public class OPCDataBaseRWClient : OPCClientAbstract
    {

        private class OpcDataBaseClientFactory : IOpcClientFactory
        {
            public OPCClientAbstract Create()
            {
                OpcItemAbstract opcServer = DependencyHelper.GetService<OpcItemAbstract>();
                return new OPCDataBaseRWClient(opcServer);
            }
        }

        readonly List<string> saveItemList = new List<string>();
        //同步连接服务对象
        IOpcCommunicationAbstract synServer = null;

        private const string GROUP_NAME = "GN";
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
        public OPCDataBaseRWClient(OpcItemAbstract opcServer)
        {
            IOpcCommunicationAbstract opcCommunicationAbstract = new OPCDataBaseRWServer(ServerType.OPC_SimaticNET, opcServer);
            OPCRWClientInit(opcCommunicationAbstract);
        }

        public OPCDataBaseRWClient(IOpcCommunicationAbstract opcCommunicationAbstract)
        {
            OPCRWClientInit(opcCommunicationAbstract);
        }

        private void OPCRWClientInit(IOpcCommunicationAbstract opcCommunicationAbstract)
        {
            synServer = opcCommunicationAbstract;
            AddGroup(GROUP_NAME);
        }

        /// <summary>
        /// 关闭服务对象连接
        /// </summary>
        private void Close()
        {
            synServer.Close();
        }

        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="groupName">组名称</param>
        private void AddGroup(string groupName)
        {
            synServer.AddGroup(groupName);
        }

        /// <summary>
        /// 添加多个项
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemsName">项列表</param>
        private void TryAddItems(string groupName, List<string> itemsNameList)
        {
            foreach (var itemName in itemsNameList)
            {
                TryAddItem(groupName, itemName);
            }
        }

        /// <summary>
        /// 添加单个项
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        private void TryAddItem(string groupName, string itemName)
        {
            lock (saveItemList)
            {
                if (_CheckAndSaveItem(itemName))
                {
                    try
                    {
                        synServer.AddItem(groupName, itemName);
                    }
                    catch (Exception ex)
                    {
                        Log.getExceptionFile().Info(string.Format("AddItem异常DB块为：{0},错误信息为：{1}", itemName, ex.Message));
                    }
                }
            }
        }

        private bool _CheckAndSaveItem(string itemName)
        {
            if (!saveItemList.Contains(itemName))
            {
                saveItemList.Add(itemName);
                return true;
            }
            return false;
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
                IsConnection = itemName.Contains("S7:[S7 connection_");  //立库软件就没加，所以强制改成True   张胜
                //IsConnection = true;
            }
            catch (Exception ex)
            {
                Log.getMessageFile("TestingOPC").Info(ex.StackTrace.ToString());
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
            return new OpcDataBaseClientFactory();
        }

        public override string ReadString(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            string opcItemName = CreateItem(deviceName, itemName);
            DataItem dataItem = null;
            try
            {
                TryAddItem(GROUP_NAME, opcItemName);
                dataItem = synServer.Read(GROUP_NAME, opcItemName);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC字符串读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}", deviceName, opcItemName, ex.Message));
            }
            string result = string.Empty;
            if (dataItem != null)
            {
                result = dataItem.ItemValue.ToString();
            }

            StringBuilder msg = new StringBuilder();
            msg.Append("接收到单独读指令(Read):\r\n");
            msg.AppendFormat("deviceName[{0}]", deviceName);
            msg.AppendFormat(",itemName[{0}]", itemName);
            msg.AppendFormat("。执行完成的返回值为：{0}", result);
            Log.getEventFile().Info(msg.ToString());
            return result;
        }

        /// <summary>
        /// 单个值的读取
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回读取结果</returns>
        public override int Read(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            int result = 0;
            string opcItemName = CreateItem(deviceName, itemName);
            DataItem dataItem = null;
            try
            {
                TryAddItem(GROUP_NAME, opcItemName);
                dataItem = synServer.Read(GROUP_NAME, opcItemName);
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
                        Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}", deviceName, opcItemName, dataItem.ItemValue.ToString()));
                    }

                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}", deviceName, opcItemName, ex.Message));
            }
            PrintOPCSingleReadLog(deviceName, opcItemName, result);
            ShowOPCReadValueSingleToBoard(deviceName, opcItemName, dataItem);
            return result;
        }

        /// <summary>
        /// float类型的读取
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回读取的结果</returns>
        public override float ReadFloat(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            float result = 0.0f;
            string opcItemName = CreateItem(deviceName, itemName);
            DataItem dataItem = null;
            try
            {
                TryAddItem(GROUP_NAME, opcItemName);
                dataItem = synServer.Read(GROUP_NAME, opcItemName);
                if (dataItem != null)
                {
                    float value = 0.0f;
                    if (!float.TryParse(dataItem.ItemValue.ToString(), out value))
                    {
                        Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}",
                            deviceName, opcItemName, dataItem.ItemValue.ToString()));
                    }
                    result = value;
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, opcItemName, ex.Message));
            }
            PrintOPCSingleLog("Read", deviceName, opcItemName, result.ToString());
            ShowOPCReadValueSingleToBoard(deviceName, opcItemName, dataItem);
            return result;
        }

        private string _lastMsg = String.Empty;
        /// <summary>
        /// 多个值的读取
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
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

            List<int> result = null;

            List<string> opcItemNameList = CreateMultiReadItemList(deviceInfoList);
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到批量读指令(Read):\r\n");
            try
            {
                TryAddItems(GROUP_NAME, opcItemNameList);
                List<DataItem> dataItemList = ReadBySingleMethod(opcItemNameList);

                if (dataItemList != null && dataItemList.Count > 0)
                {
                    result = new List<int>();
                    int value = 0;
                    for (int j = 0; j < dataItemList.Count; j++)
                    {
                        if (!int.TryParse(dataItemList[j].ItemValue.ToString(), out value))
                        {
                            Log.getExceptionFile().Info(string.Format("多个读取List<int>读到无效值默认为0，异常DB块为：{0},转换的值为：{1}", opcItemNameList[j], dataItemList[j].ItemValue.ToString()));
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
                Log.getExceptionFile().Info(ex.Message);
            }
            EventLoger.LogMsg(EnumLogLevel.Info, "Read_deviceInfoList", LoggerType.GetEventFile, msg.ToString());
            if (_lastMsg == msg.ToString())
            {
                //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + " Same");
            }
            else
            {
                //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + " Difference");
                _lastMsg = msg.ToString();
            }
            //Log.getEventFile().Info(msg.ToString());
            return result;
        }

        /// <summary>
        /// 单个读取替代多个读取
        /// </summary>		
        private List<DataItem> ReadBySingleMethod(List<string> opcItemNameList)
        {
            List<DataItem> dataItemList = new List<DataItem>();
            foreach (string opcItemName in opcItemNameList)
            {
                DataItem dataItem = new DataItem();
                try
                {
                    dataItem = synServer.Read(GROUP_NAME, opcItemName);
                }
                catch (Exception ex)
                {
                    dataItem.ItemValue = INVALID_VALUE;
                    Log.getExceptionFile().Info(string.Format("OPC多个读取异常，对应的DB块为：{0}，异常信息为：{1}", opcItemName, ex.Message));
                }
                dataItemList.Add(dataItem);
            }
            return dataItemList;
        }

        /// <summary>
        /// 单个值的写入
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">修改后的值</param>
        public override void Write(string deviceName, string itemName, int value)
        {
            Write(deviceName, itemName, value.ToString());
            ShowOPCWriteValueSingleToBoard(deviceName, CreateItem(deviceName, itemName), value);
        }

        /// <summary>
        /// string类型的写入
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">写入的值</param>
        public override void Write(string deviceName, string itemName, string value)
        {
            manualReset.WaitOne(seconds, true);

            string opcItemName = CreateItem(deviceName, itemName);

            try
            {
                TryAddItem(GROUP_NAME, opcItemName);
                synServer.Write(GROUP_NAME, opcItemName, value);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个写入异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, itemName, ex.Message));
            }
            PrintOPCSingleLog("Write", deviceName, opcItemName, value);
        }

        /// <summary>
        /// float类型的写入
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="value">写入的值</param>
        public override void Write(string deviceName, string itemName, float value)
        {
            Write(deviceName, itemName, value.ToString());
        }

        /// <summary>
        /// 写入多个项的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="Inv">项名称和项值集合</param>
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

            Dictionary<string, object> opcItemValueDic;
            List<string> opcItemNameLst;
            CreateMultiWriteItemListAndDic(deviceName, itemValueDic, out opcItemValueDic, out opcItemNameLst);
            try
            {
                TryAddItems(GROUP_NAME, opcItemNameLst);
                synServer.Write(GROUP_NAME, opcItemValueDic);
            }
            catch (Exception ex)
            {
                string ExAppendStr = AppendOPCAddress(opcItemValueDic);
                Log.getExceptionFile().Info(string.Format("多个写异常，对应的DB块为：{0}，错误信息为：{1}", ExAppendStr, ex.Message));
            }
            foreach (var item in opcItemValueDic)
            {
                string opcItem = item.Key;
                int opcValue = int.Parse((string)item.Value);
                itemValueDicPrintDic.Add(opcItem, opcValue);
            }
            PrintOPCMultiWriteLog(deviceName, itemValueDicPrintDic);
            ShowOPCWriteValueMultiToBoard(deviceName, opcItemValueDic);
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
                if (itemName.Contains("@"))
                {
                    itemNameReal = itemName.Split('@')[1];
                }
                else
                {
                    itemNameReal = itemName;
                }
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
            //string itemName = "";
            itemNameLst = new List<string>();
            string opcConnection = "";
            foreach (var item in itemValueDic)
            {
                if (IsOpcConnection(item.Key.ToString()) == false)
                {
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
                tmp.Add(OpcConnectionReplace(itemNameReal), item.Value.ToString());
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
            //string itemName = "";
            itemNameLst = new List<string>();
            string opcConnection = "";
            foreach (var item in itemValueDic)
            {
                if (IsOpcConnection(item.Key.ToString()) == false)
                {
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
                tmp.Add(OpcConnectionReplace(itemNameReal), item.Value.ToString());
                itemNameLst.Add(OpcConnectionReplace(itemNameReal));
            }
        }

        private void PrintOPCSingleReadLog(string deviceName, string itemName, int result)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到单独读指令(Read):\r\n");
            msg.AppendFormat("deviceName[{0}]", deviceName);
            msg.AppendFormat(",itemName[{0}]", itemName);
            msg.AppendFormat("。执行完成的返回值为：{0}", result);
            Log.getEventFile().Info(msg.ToString());
        }

        private void PrintOPCSingleWriteLog(string deviceName, string itemName, int value)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到单独写指令(Write):\r\n");
            msg.AppendFormat("deviceName[{0}]", deviceName);
            msg.AppendFormat(",itemName[{0}]", itemName);
            msg.AppendFormat(",value[{0}]", value);
            Log.getEventFile().Info(msg.ToString());
        }

        private void PrintOPCMultiWriteLog(string deviceName, Dictionary<string, int> itemValueDic)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("接收到批量写指令(Write):\r\n");
            msg.AppendFormat("deviceName[{0}]", deviceName);
            foreach (var list in itemValueDic)
            {
                msg.AppendFormat(",key[{0}]", list.Key);
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
                msg.AppendFormat(",key[{0}]", list.Key);
                msg.AppendFormat(",value[{0}]\r\n", list.Value);
            }
            Log.getEventFile().Info(msg.ToString());
        }

        private void ShowOPCWriteValueSingleToBoard(string deviceName, string opcWriteItemName, int opcWriteItemValue)
        {
            try
            {
                if (OPCWriteBoardSingleEvent != null)
                {
                    OPCWriteBoardSingleEvent(deviceName, opcWriteItemName, opcWriteItemValue.ToString());
                }
            }
            catch (Exception ex)
            {
                CLDC.Framework.Log.Log.getExceptionFile().Info("OPCWrite观测界面单个写异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 将写入OPC的多个值，打印到opc写窗口
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="tmp">写入OPC的连接字符串和值的字典</param>
        private void ShowOPCWriteValueMultiToBoard(string deviceName, Dictionary<string, object> opcWriteItemValueDic)
        {
            try
            {
                if (OPCWriteBoardMultiEvent != null)
                {
                    OPCWriteBoardMultiEvent(deviceName, opcWriteItemValueDic);
                }
            }
            catch (Exception ex)
            {
                CLDC.Framework.Log.Log.getExceptionFile().Info("OPCWrite观测界面多个写异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 将传给opc的单个读取的信息值，显示到观测界面
        /// </summary>		
        private void ShowOPCReadValueSingleToBoard(string deviceName, string opcReadAddr, DataItem dataItem)
        {
            try
            {
                if (OPCReadBoardSingleEvent != null)
                {
                    if (dataItem != null)
                    {
                        OPCReadBoardSingleEvent(deviceName, opcReadAddr, dataItem.ItemValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                CLDC.Framework.Log.Log.getExceptionFile().Info("OPC读写观测界面异常：单个读异常，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 将传给opc的多个读取的信息值，显示到观测界面
        /// </summary>		
        private void ShowOPCReadValueMultiToBoard(string deviceName, List<string> opcReadAddr, List<int> opcReturnValue)
        {
            try
            {
                List<string> opcReadAddrList = JoinNameAddr(deviceName, opcReadAddr);

                if (OPCReadBoardMultiEvent != null)
                {
                    OPCReadBoardMultiEvent(opcReadAddrList, opcReturnValue);
                }
            }
            catch (Exception ex)
            {
                CLDC.Framework.Log.Log.getExceptionFile().Info("OPC读写观测界面异常：多个读异常，原因：" + ex.Message);
            }
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

        public override bool ReadBool(string deviceName, string itemName)
        {
            manualReset.WaitOne(seconds, true);

            bool result = false;
            string opcItemName = CreateItem(deviceName, itemName);
            DataItem dataItem = null;
            try
            {
                TryAddItem(GROUP_NAME, opcItemName);
                dataItem = synServer.Read(GROUP_NAME, opcItemName);
                if (dataItem != null)
                {
                    bool value = false;
                    if (!bool.TryParse(dataItem.ItemValue.ToString(), out value))
                    {
                        Log.getExceptionFile().Info(string.Format("ReadBool读取到无效值默认为False，对应设备名称为：{0}，对应的DB块为：{1}，转换失败的值为：{2}", deviceName, opcItemName, dataItem.ItemValue.ToString()));
                    }
                    result = value;

                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC字符串读取异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}", deviceName, opcItemName, ex.Message));
            }

            PrintOPCSingleLog("Read", deviceName, opcItemName, result.ToString());

            return result;
        }

        public override void Write(string deviceName, string itemName, bool value)
        {
            Write(deviceName, itemName, value.ToString());
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
            List<bool> result = null;

            List<string> opcItemNameList = CreateMultiReadItemList(deviceInfoList);
            try
            {
                TryAddItems(GROUP_NAME, opcItemNameList);
                List<DataItem> dataItemList = ReadBySingleMethod(opcItemNameList);

                if (dataItemList != null && dataItemList.Count > 0)
                {
                    result = new List<bool>();
                    bool value = false;
                    for (int j = 0; j < dataItemList.Count; j++)
                    {
                        if (!bool.TryParse(dataItemList[j].ItemValue.ToString(), out value))
                        {
                            //Log.getDebugFile().Info(string.Format("BoolList读取到无效值默认为False，异常DB块为：{0},转换的值为：{1}", opcItemNameList[j], dataItemList[j].ItemValue.ToString()));
                        }
                        result.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(ex.Message);
            }
            return result;
        }

        private void PrintOPCSingleLog(string type, string deviceName, string itemName, string result)
        {
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
            msg.AppendFormat("。执行完成的返回值为：{0}\r\n", result);
            Log.getEventFile().Info(msg.ToString());
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

            Dictionary<string, object> opcItemValueDic;
            List<string> opcItemNameLst;
            CreateMultiWriteItemListAndDic(deviceName, itemValueDic, out opcItemValueDic, out opcItemNameLst);
            try
            {
                TryAddItems(GROUP_NAME, opcItemNameLst);
                synServer.Write(GROUP_NAME, opcItemValueDic);
            }
            catch (Exception ex)
            {
                string ExAppendStr = AppendOPCAddress(opcItemValueDic);
                Log.getExceptionFile().Info(string.Format("多个写异常，对应的DB块为：{0}，错误信息为：{1}", ExAppendStr, ex.Message));
            }
            foreach (var item in opcItemValueDic)
            {
                string opcItem = item.Key;
                object opcValue = item.Value;
                itemValueDicPrintDic.Add(opcItem, opcValue);
            }
            PrintOPCMultiWriteLog(deviceName, itemValueDicPrintDic);
            ShowOPCWriteValueMultiToBoard(deviceName, opcItemValueDic);
        }

        public override void Write(string deviceName, string itemName, object value)
        {
            manualReset.WaitOne(seconds, true);

            string opcItemName = CreateItem(deviceName, itemName);

            try
            {
                TryAddItem(GROUP_NAME, opcItemName);
                synServer.Write(GROUP_NAME, opcItemName, value);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info(string.Format("OPC单个写入异常，对应设备名称为：{0}，对应的DB块为：{1}，异常信息为：{2}",
                    deviceName, itemName, ex.Message));
            }
            PrintOPCSingleLog("Write", deviceName, opcItemName, value.ToString());
        }
    }
}
