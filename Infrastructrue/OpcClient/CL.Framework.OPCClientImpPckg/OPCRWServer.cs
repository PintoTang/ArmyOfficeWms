using System;
using System.Collections.Generic;
using OpcRcw.Da;
using System.Runtime.InteropServices;
using CLDC.Framework.Log;


namespace CL.Framework.OPCClientImpPckg
{
    /// <summary>
    /// OPC底层服务类
    /// </summary>
    public class OPCRWServer : IOpcCommunicationAbstract
    {
        //默认读取缓存大小为2
        private short READ_INT_BUFFER_SIZE = 2;

        private readonly object threadLock = new object();

        //服务对象
        private IOPCServer opcServer;
        //管理组对象
        private IDictionary<string, DataGroup> groups;
        //管理项对象
        private IDictionary<string, DataItem> items;
        //组与项关系映射对象
        private IDictionary<string, IList<string>> groupItemMappings;

        private string _serverName = "OPC.SimaticNET";
        private int _localID = 0x407;
        private int _dwRequestedUpdateRate = 250;

        private const int INVALID_VALUE = -0x7FFFFFFF;

        /// <summary>
        /// 初始化
        /// </summary>
        public OPCRWServer(ServerType serverType)
        {
            _serverName = GetServerNameForEnum(serverType);
            groups = new Dictionary<string, DataGroup>();
            items = new Dictionary<string, DataItem>();
            groupItemMappings = new Dictionary<string, IList<string>>();

            Open();
        }

        void IDisposable.Dispose()
        {

        }

        /// <summary>
        /// 根据item返回请求PLC服务数据类型(字符串使用20个字节缓存，bool和int使用4字节缓存)
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        private short GetRequestedDataType(string itemName)
        {
            short value = READ_INT_BUFFER_SIZE;
            string strItemName = itemName.ToLower();
            if (strItemName.Contains("string"))
            {
                value = 8;
            }
            else if (strItemName.Contains("real"))
            {
                value = 5;
            }
            else if (strItemName.Contains("dint") || strItemName.Contains("w"))
            {
                value = 3;
            }
            else if (strItemName.Contains("x") || strItemName.Contains("q") || strItemName.Contains("m"))
            {
                value = 11;
            }
            return value;
        }

        /// <summary>
        /// 打开服务
        /// </summary>
        private void Open()
        {
            try
            {
                //获取当前服务器名称对应的标识符
                Type svrComponenttyp = Type.GetTypeFromProgID(_serverName);
                //根据标识符创建服务器对象
                opcServer = (IOPCServer)Activator.CreateInstance(svrComponenttyp);
            }
            catch (Exception ex)
            {
                string msg = string.Format("连接服务名为：{0} 的OPC服务失败，请检查OPC服务配置环境  \r\n失败详情：{1}", _serverName, ex.StackTrace.ToString());
                Log.getExceptionFile().Info(ex);
                Log.getExceptionFile().Info(msg);
            }
        }

        /// <summary>
        /// 断开服务
        /// </summary>
        public void Close()
        {
            //释放组与项映射
            groupItemMappings = null;
            //释放项
            items = null;
            //释放组
            foreach (var group in groups.Values)
            {
                opcServer.RemoveGroup(group.GroupID, 0);
            }
            groups = null;
            //释放服务对象 
            if (opcServer != null)
            {
                Marshal.ReleaseComObject(opcServer);
            }
            opcServer = null;
        }

        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="groupName">组名称</param>
        public void AddGroup(string groupName)
        {
            //判断是否连上服务
            if (opcServer == null)
            {
                throw new ApplicationException("OPC异常:没有打开服务!");
            }
            //是否存在组
            if (groups.ContainsKey(groupName))
            {
                return; //存在该组就不再添加该组
            }
            //激活标志,1为激活，2为未激活
            int bActive = 1;
            //Client handle 标签句柄
            int hClientGroup = 0;
            //PLC回传频率
            int pRevUpdateRate;
            int TimeBias = 0;
            float deadband = 0;
            //组对象
            object group;
            //返回的组ID
            int svrGroupID;
            //自定义组对象
            DataGroup dataGroup;

            GCHandle hTimeBias;
            GCHandle hDeadband;

            hTimeBias = GCHandle.Alloc(TimeBias, GCHandleType.Pinned);
            hDeadband = GCHandle.Alloc(deadband, GCHandleType.Pinned);

            try
            {
                //IOPCItemMgt是项的操作对象
                Guid iidRequiredInterface = typeof(IOPCItemMgt).GUID;

                //*  groupName 组名
                //*  bActive 是否激活组
                //*  _dwRequestedUpdateRate 请求更新时间
                //*  _localID 语言名
                //*  svrGroupID 返回的组对象的ID
                //*  pRevUpdateRate 返回的更新频率
                //*  group 添加组
                opcServer.AddGroup(groupName,
                                    bActive,
                                    _dwRequestedUpdateRate,
                                    hClientGroup,
                                    hTimeBias.AddrOfPinnedObject(),
                                    hDeadband.AddrOfPinnedObject(),
                                    _localID,
                                    out svrGroupID,
                                    out pRevUpdateRate,
                                    ref iidRequiredInterface,
                                    out group);

                //初始化组对象
                dataGroup = new DataGroup();
                dataGroup.GroupID = svrGroupID;
                dataGroup.GroupName = groupName;
                dataGroup.GroupObject = group;

                //添加到组集合
                groups.Add(groupName, dataGroup);
                groupItemMappings.Add(groupName, null);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("OPC异常:添加组失败！" + groupName + ex);
            }
            finally
            {
                //释放内存
                if (hDeadband.IsAllocated)
                {
                    hDeadband.Free();
                }
                if (hTimeBias.IsAllocated)
                {
                    hTimeBias.Free();
                }
            }
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        public void AddItem(string groupName, string itemName)
        {
            //判断是否连上服务
            if (opcServer == null)
            {
                throw new ApplicationException("OPC异常:没有打开服务!");
            }
            //判断是否存在组
            if (!groups.ContainsKey(groupName))
            {
                throw new ApplicationException("OPC异常:不存在该组!");
            }
            //判断是否存在组与项的关系映射
            if (groupItemMappings[groupName] != null && groupItemMappings[groupName].Contains(itemName))
            {
                throw new ApplicationException("OPC异常:已存在该组与项的关系映射!");
            }
            //定义一个Item
            OPCITEMDEF[] pItemArray = new OPCITEMDEF[1];
            //可选的通道路径，对于Simatiic Net不需要
            pItemArray[0].szAccessPath = string.Empty;
            //Item标识符
            pItemArray[0].szItemID = itemName;
            //激活标志
            pItemArray[0].bActive = 1;
            //Client handle 标签句柄 , 在OnDataChange中会用到，不同的Item不一样
            pItemArray[0].hClient = 1;
            pItemArray[0].dwBlobSize = 0;
            pItemArray[0].pBlob = IntPtr.Zero;
            //添加项时的数据类型
            pItemArray[0].vtRequestedDataType = GetRequestedDataType(itemName);

            IntPtr pResults = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;
            //自定义项
            DataItem dataItem;
            string pstrError = string.Empty;

            try
            {
                // 添加项到组
                ((IOPCItemMgt)groups[groupName].GroupObject).AddItems(1, pItemArray, out pResults, out pErrors);
                int[] errors = new int[1];
                //从非托管去复制错误到托管区
                Marshal.Copy(pErrors, errors, 0, 1);

                if (errors[0] == 0)
                {
                    //获取结果对象
                    OPCITEMRESULT result = (OPCITEMRESULT)Marshal.PtrToStructure(pResults, typeof(OPCITEMRESULT));

                    dataItem = new DataItem();
                    dataItem.ItemID = result.hServer;
                    //保存到项集合
                    items.Add(itemName, dataItem);

                    //保存到组与项映射集合
                    if (groupItemMappings[groupName] != null)
                    {
                        //不为空组时添加项映射
                        groupItemMappings[groupName].Add(itemName);
                    }
                    else
                    {
                        //为空组时添加项映射
                        groupItemMappings[groupName] = new List<string> { itemName };
                    }
                }
                else
                {
                    opcServer.GetErrorString(errors[0], _localID, out pstrError);
                    throw new ApplicationException(pstrError);
                }
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("OPC 添加项失败，组：{0} 项：{1} \r\n失败原因：{2}", groupName, itemName, ex.StackTrace + ex.Message);
                throw new ApplicationException(exMsg);
            }
            finally
            {
                // 释放非托管区读取结果的内存
                Marshal.DestroyStructure(pResults, typeof(OPCITEMRESULT));

                // 释放非托管内存
                if (pResults != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pResults);
                    pResults = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }
        }

        private bool Check(string groupName, string itemName, out string msg)
        {
            msg = string.Empty;
            //判断是否连上服务
            if (opcServer == null)
            {
                msg = "OPC异常:没有打开服务！";
                return false;
            }
            //判断是否存在组和组与项的关系映射
            if (!groups.ContainsKey(groupName))
            {
                msg = string.Format("不存在组:{0}", groupName);
                return false;
            }
            if (!groupItemMappings.ContainsKey(groupName))
            {
                msg = string.Format("OPC异常：不存在组：{0} 和 项：{1} 的关系", groupName, itemName);
                return false;
            }
            if (groupItemMappings[groupName] == null)
            {
                msg = string.Format("OPC异常：组:{0} 项:{1} 关系中，项的值为空",groupName,itemName);
                return false;
            }
            if (!groupItemMappings[groupName].Contains(itemName))
            {
                msg = string.Format("OPC异常：组：{0} 不包含项：{1}", groupName, itemName);
                return false;
            }
            //不存在项
            if (!items.ContainsKey(itemName))
            {
                msg = string.Format("OPC异常:不存在该地址项:{0}", itemName);
                return false;
            }
            return true;
        }

        private readonly object ReadLock = new object();
        /// <summary>
        /// 读取item的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回读取结果</returns>
        public DataItem Read(string groupName, string itemName)
        {
            lock (ReadLock)
            {
                string msg = string.Empty;
                bool check = Check(groupName, itemName, out msg);
                if (!check)
                {
                    throw new ApplicationException(msg);
                }
                //读写对象
                IOPCSyncIO pIOPCSyncIO;
                // 访问非托管内存指针
                IntPtr pItemValues = IntPtr.Zero;
                IntPtr pErrors = IntPtr.Zero;
                string pstrError = string.Empty;

                try
                {
                    //通过组对象转为读取对象
                    pIOPCSyncIO = (IOPCSyncIO)groups[groupName].GroupObject;
                    // 同步读取数据
                    pIOPCSyncIO.Read(OPCDATASOURCE.OPC_DS_DEVICE, 1, new int[] { items[itemName].ItemID }, out pItemValues, out pErrors);
                    //将错误从非托管区读到托管区
                    int[] errors = new int[1];
                    Marshal.Copy(pErrors, errors, 0, 1);

                    if (errors[0] == 0)
                    {
                        OPCITEMSTATE pItemState = (OPCITEMSTATE)Marshal.PtrToStructure(pItemValues, typeof(OPCITEMSTATE));
                        // 获取项中的值
                        items[itemName].ItemValue = pItemState.vDataValue;
                        //获取项中的质量码
                        items[itemName].Quality = pItemState.wQuality;
                        //获取时间
                        items[itemName].TimeStamp = ToDateTime(pItemState.ftTimeStamp);
                    }
                    else
                    {
                        //有异常时抛出
                        opcServer.GetErrorString(errors[0], _localID, out pstrError);
                        throw new ApplicationException(pstrError);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("OPC异常:单个读取数据失败！组名" + groupName + "项名" + itemName + "\n" + ex);
                }
                finally
                {
                    //释放内存
                    Marshal.DestroyStructure(pItemValues, typeof(OPCITEMSTATE));

                    //  释放非托管内存
                    if (pItemValues != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pItemValues);
                        pItemValues = IntPtr.Zero;
                    }
                    if (pErrors != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pErrors);
                        pErrors = IntPtr.Zero;
                    }
                }
                return items[itemName];
            }
        }

        private readonly object ReadListLock = new object();
        /// <summary>
        /// 读取item的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemNameList">项名称</param>
        /// <returns>返回读取结果</returns>
        public List<DataItem> Read(string groupName, List<string> itemNameList)
        {
            lock (ReadListLock)
            {
                string msg = string.Empty;
                int[] itemIDList = new int[itemNameList.Count];
                for (int i = 0; i < itemNameList.Count; i++)
                {
                    bool check = Check(groupName, itemNameList[i], out msg);
                    if (!check)
                    {
                        throw new ApplicationException(msg);
                    }
                    itemIDList[i] = items[itemNameList[i]].ItemID;
                }

                //读写对象
                IOPCSyncIO pIOPCSyncIO;
                // 访问非托管内存指针
                IntPtr pItemValues = IntPtr.Zero;
                IntPtr pErrors = IntPtr.Zero;
                string pstrError = string.Empty;
                List<DataItem> result = new List<DataItem>();
                IntPtr pos;
                try
                {
                    //通过组对象转为读取对象
                    pIOPCSyncIO = (IOPCSyncIO)groups[groupName].GroupObject;
                    // 同步读取数据
                    pIOPCSyncIO.Read(OPCDATASOURCE.OPC_DS_DEVICE, itemNameList.Count, itemIDList, out pItemValues, out pErrors);
                    //将错误从非托管区读到托管区
                    int[] errors = new int[itemNameList.Count];
                    Marshal.Copy(pErrors, errors, 0, itemNameList.Count);
                    OPCITEMSTATE[] pItemState = new OPCITEMSTATE[itemNameList.Count];
                    pos = pItemValues;
                    for (int i = 0; i < itemNameList.Count; i++)
                    {
                        DataItem item = items[itemNameList[i]];

                        item.ItemName = itemNameList[i];

                        if (errors[i] == 0)
                        {
                            pItemState[i] = (OPCITEMSTATE)Marshal.PtrToStructure(pos, typeof(OPCITEMSTATE));

                            item.ItemValue = pItemState[i].vDataValue;
                            if (item.ItemValue == null)
                            {
                                item.ItemValue = INVALID_VALUE;
                            }

                            //获取项中的质量码
                            item.Quality = pItemState[i].wQuality;
                            //获取时间
                            item.TimeStamp = ToDateTime(pItemState[i].ftTimeStamp);
                            pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMSTATE)));

                        }
                        else
                        {
                            pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMSTATE)));
                            opcServer.GetErrorString(errors[0], _localID, out pstrError);
                            //Log.getExceptionFile().Info("第" + i + "OPCServer读取错误：" + pstrError);
                            item.ItemValue = INVALID_VALUE;
                        }
                        result.Add(item);
                    }

                }
                catch (Exception ex)
                {
                    throw new ApplicationException("OPC异常:多个读取数据失败！ 详细信息" + ex);
                }
                finally
                {
                    try
                    {
                        pos = IntPtr.Zero;

                        Marshal.DestroyStructure(pItemValues, typeof(OPCITEMSTATE));

                        //  释放非托管内存
                        if (pItemValues != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(pItemValues);
                            pItemValues = IntPtr.Zero;
                        }
                        if (pErrors != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(pErrors);
                            pErrors = IntPtr.Zero;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException("OPC异常:多个读取数据释放内存异常" + e);
                    }

                }
                return result;
            }
        }

        private readonly object WriteLock = new object();
        /// <summary>
        /// 写入单个项的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="setValue">写入的值</param>
        public void Write(string groupName, string itemName, object setValue)
        {
            lock (WriteLock)
            {
                string msg = string.Empty;
                bool check = Check(groupName, itemName, out msg);
                if (!check)
                {
                    throw new ApplicationException(msg);
                }
                // 访问非托管内存
                IntPtr pErrors = IntPtr.Zero;
                //读写对象
                IOPCSyncIO pIOPCSyncIO;

                object[] values = new object[1];
                values[0] = setValue;
                string pstrError = string.Empty;

                try
                {
                    //通过组对象转为读取对象
                    pIOPCSyncIO = (IOPCSyncIO)groups[groupName].GroupObject;
                    //写入数据
                    pIOPCSyncIO.Write(1, new int[] { items[itemName].ItemID }, values, out pErrors);

                    int[] errors = new int[1];
                    Marshal.Copy(pErrors, errors, 0, 1);

                    if (errors[0] != 0)
                    {
                        //有异常时抛出
                        opcServer.GetErrorString(errors[0], _localID, out pstrError);
                        throw new ApplicationException(pstrError);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("OPC异常:写入数据失败! 组名" + groupName + "项名" + itemName + "\n" + ex);
                }
                finally
                {
                    // 释放非托管内存
                    if (pErrors != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pErrors);
                        pErrors = IntPtr.Zero;
                    }
                }
            }
        }

        private readonly object WriteListLock = new object();
        /// <summary>
        /// 写入多个项的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemValueList">项和项值的键值对集合</param>
        /// <returns></returns>
        public void Write(string groupName, Dictionary<string, object> itemValueDic)
        {
            lock (WriteListLock)
            {
               

                foreach (KeyValuePair<string, object> itemValue in itemValueDic)
                {
                    //判断是否存在组和组与项的关系映射
                    string msg = string.Empty;
                    bool check = Check(groupName, itemValue.Key, out msg);
                    if (!check)
                    {
                        throw new ApplicationException(msg);
                    }
                }

                // 访问非托管内存
                IntPtr pErrors = IntPtr.Zero;
                //读写对象
                IOPCSyncIO pIOPCSyncIO;

                object[] values = new object[1];
                string pstrError = string.Empty;

                List<int> list1 = new List<int>();
                List<object> list2 = new List<object>();

                foreach (var IV in itemValueDic)
                {
                    list1.Add(items[IV.Key].ItemID);
                    list2.Add(IV.Value);
                }
                int[] ItemIDList = list1.ToArray();
                object[] ItemsValueList = list2.ToArray();


                try
                {
                    //通过组对象转为读取对象
                    pIOPCSyncIO = (IOPCSyncIO)groups[groupName].GroupObject;
                    //写入数据
                    pIOPCSyncIO.Write(ItemIDList.Length, ItemIDList, ItemsValueList, out pErrors);

                    int[] errors = new int[ItemIDList.Length];
                    Marshal.Copy(pErrors, errors, 0, errors.Length);

                    if (errors[0] != 0)
                    {
                        //有异常时抛出
                        opcServer.GetErrorString(errors[0], _localID, out pstrError);
                        throw new ApplicationException(pstrError);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("OPC异常:写入数据失败! " + ex);
                }
                finally
                {
                    // 释放非托管内存
                    if (pErrors != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pErrors);
                        pErrors = IntPtr.Zero;
                    }
                }
            }
        }

        public void Write(string groupName, Dictionary<string, int> itemValueDic)
        {
            lock (WriteListLock)
            {
                foreach (KeyValuePair<string, int> itemValue in itemValueDic)
                {
                    //判断是否存在组和组与项的关系映射
                    string msg = string.Empty;
                    bool check = Check(groupName, itemValue.Key, out msg);
                    if (!check)
                    {
                        throw new ApplicationException(msg);
                    }
                }
                // 访问非托管内存
                IntPtr pErrors = IntPtr.Zero;
                //读写对象
                IOPCSyncIO pIOPCSyncIO;

                object[] values = new object[1];
                string pstrError = string.Empty;

                List<int> list1 = new List<int>();
                List<object> list2 = new List<object>();

                foreach (var IV in itemValueDic)
                {
                    list1.Add(items[IV.Key].ItemID);
                    list2.Add(IV.Value);
                }
                int[] ItemIDList = list1.ToArray();
                object[] ItemsValueList = list2.ToArray();


                try
                {
                    //通过组对象转为读取对象
                    pIOPCSyncIO = (IOPCSyncIO)groups[groupName].GroupObject;
                    //写入数据
                    pIOPCSyncIO.Write(ItemIDList.Length, ItemIDList, ItemsValueList, out pErrors);

                    int[] errors = new int[ItemIDList.Length];
                    Marshal.Copy(pErrors, errors, 0, errors.Length);

                    if (errors[0] != 0)
                    {
                        //有异常时抛出
                        opcServer.GetErrorString(errors[0], _localID, out pstrError);
                        throw new ApplicationException(pstrError);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("OPC异常:写入数据失败! " + ex);
                }
                finally
                {
                    // 释放非托管内存
                    if (pErrors != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(pErrors);
                        pErrors = IntPtr.Zero;
                    }
                }
            }
        }




        /// <summary>
        /// 将枚举转成字符串服务名
        /// </summary>
        /// <param name="svrType">服务类型</param>
        private string GetServerNameForEnum(ServerType svrType)
        {
            return svrType.ToString().Replace('_', '.');
        }

        /// <summary>
        /// 转换 FILETIME 为 DateTime
        /// </summary>
        private DateTime ToDateTime(OpcRcw.Da.FILETIME ft)
        {
            long highbuf = (long)ft.dwHighDateTime;
            long buffer = (highbuf << 32) + ft.dwLowDateTime;
            return DateTime.FromFileTimeUtc(buffer);
        }

    }

    /// <summary>
    /// 组
    /// </summary>
    public class DataGroup
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public int GroupID { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 组对象
        /// </summary>
        public object GroupObject { get; set; }
    }

    /// <summary>
    /// 项
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// 项ID
        /// </summary>
        public int ItemID { get; set; }
        /// <summary>
        /// 项名
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 项的返回值
        /// </summary>
        public object ItemValue { get; set; }
        /// <summary>
        /// 项质量码
        /// </summary>
        public short Quality { get; set; }
        /// <summary>
        /// 项最后返回时间
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
    /// <summary>
    /// 变量处理类
    /// </summary>
    [ComVisible(true), StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class DUMMY_VARIANT
    {
        /// <summary>
        /// 清空变量
        /// </summary>
        /// <param name="addrofvariant">变量指针</param>
        [DllImport("oleaut32.dll")]
        public static extern int VariantClear(IntPtr addrofvariant);
    }

    /// <summary>
    /// 服务类型枚举
    /// </summary>
    public enum ServerType
    {
        /// <summary>
        /// "SimaticNET"
        /// </summary>
        OPC_SimaticNET,
        /// <summary>
        /// "SimaticHMI_PTPRO"
        /// </summary>
        OPC_SimaticHMI_PTPRO,
        /// <summary>
        /// "SimaticNET_DP"
        /// </summary>
        OPC_SimaticNET_DP,
        /// <summary>
        /// "SimaticNET_PD"
        /// </summary>
        OPC_SimaticNET_PD,
        /// <summary>
        /// "WinCC"
        /// </summary>
        OPCServer_WinCC
    }
}
