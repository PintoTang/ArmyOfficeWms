using System;
using System.Collections.Generic;
using System.Data;
using CLDC.Framework.Log;
using CL.Framework.OPCClientImpPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;


namespace CL.Framework.Testing.OPCClientImpPckg
{
    /// <summary>
    /// OPC底层服务类
    /// </summary>
    public class OPCDataBaseRWServer : IOpcCommunicationAbstract
    {
        //读取字符串信息时，最大读取字符串的长度。
        //通过修改该值，可以改变相应的读取缓存大小，可以读出更长的字符串
        private const int MAX_READ_STRING_BUFFER_SIZE = 20;

        private readonly object threadLock = new object();


        //管理组对象
        private IDictionary<string, DataGroup> groups;
        //管理项对象
        private IDictionary<string, DataItem> items;
        //组与项关系映射对象
        private IDictionary<string, IList<string>> groupItemMappings;

        private string _serverName = "OPC.SimaticNET";

        private OpcItemAbstract _opcServer;

        /// <summary>
        /// 初始化
        /// </summary>
        public OPCDataBaseRWServer(ServerType serverType, OpcItemAbstract opcServer)
        {
            _serverName = GetServerNameForEnum(serverType);
            _opcServer = opcServer;
            groups = new Dictionary<string, DataGroup>();
            items = new Dictionary<string, DataItem>();
            groupItemMappings = new Dictionary<string, IList<string>>();
            Open();
        }



        /// <summary>
        /// 打开服务
        /// </summary>
        private void Open()
        {
            ConsoleManager.WriteLog(123, 123, "调用打开OPC服务");
        }

        /// <summary>
        /// 断开服务
        /// </summary>
        public void Close()
        {
            ConsoleManager.WriteLog(123, 123, "调用关闭OPC服务");
        }

        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="groupName">组名称</param>
        public void AddGroup(string groupName)
        {
            lock (threadLock)
            {
                if (string.IsNullOrEmpty(groupName))  //如果组名称是空直接返回
                {
                    ConsoleManager.WriteLog(123, 123, "组名称为空!");
                    return;
                }

                if (groups.Count > 0 && groups.ContainsKey(groupName))  //如果内存里面已存在直接返回
                {
                    ConsoleManager.WriteLog(123, 123, "添加相同的OPC地址");
                    return;
                }
                DataGroup dataGroup;
                OpcGroupManagerEntity dbItem = _opcServer.ReadGroup(groupName);
                
                //根据组名称获取组对象
                if (dbItem != null)  //如果数据库中存在 就不添加数据库直接在内存中添加
                {
                    string groupID = dbItem.GROUPID.ToString();
                    dataGroup = new DataGroup();
                    dataGroup.GroupID = int.Parse(groupID);
                    dataGroup.GroupName = groupName;
                    //dataGroup.GroupObject = group;
                    groups.Add(groupName, dataGroup);
                    groupItemMappings.Add(groupName, null);

                }
                else  //如果数据库中不存在相同的组名称 添加数据库同时添加到内存中
                {
                    bool result = _opcServer.AddGroup(groupName); //如果没有就先添加数据库 再获取对应的groupID
                    if (!result)
                    {
                        ConsoleManager.WriteLog(123, 123, "操作数据库失败:网络异常或配置连接数据库配置有问题");
                        return;
                    }
                    OpcGroupManagerEntity newGroup = _opcServer.ReadGroup(groupName);
                    
                    string groupID = newGroup.GROUPID.ToString();
                    dataGroup = new DataGroup();
                    dataGroup.GroupID = int.Parse(groupID);
                    dataGroup.GroupName = groupName;
                    groups.Add(groupName, dataGroup);
                    groupItemMappings.Add(groupName, null);
                    

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
            lock (threadLock)
            {
                lock (threadLock)
                {
                    //判断是否连上服务

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


                    //自定义项
                    string pstrError = string.Empty;

                    try
                    {
                        bool result = _opcServer.Write(groupName, itemName);
                        if (!result)
                        {
                            ConsoleManager.WriteLog(123, 123, "操作数据库失败:网络异常或配置连接数据库配置有问题");
                            return;
                        }
                        //获取刚刚添加的数据，添加到内存中
                        var dtItem = _opcServer.Read(groupName, itemName);
                        {
                            DataItem dataItem = new DataItem();
                            string tempItemID = dtItem.ITEMID.ToString();
                            dataItem.ItemID = int.Parse(tempItemID);
                            dataItem.ItemName = itemName;
                            items.Add(itemName, dataItem); //把数据添加到内存中
                        }
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
                    catch (Exception ex)
                    {
                        throw new ApplicationException("OPC异常:添加项失败！" + ex.Message);
                    }
                    finally
                    {

                    }
                }


            }
        }

        /// <summary>
        /// 读取item的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回读取结果</returns>
        public DataItem Read(string groupName, string itemName)
        {
            lock (threadLock)
            {
                List<string> lstItemName = new List<string>();
                lstItemName.Add(itemName);
                var dtItems = _opcServer.Read(groupName, lstItemName);
                
                List<DataItem> dataItemlist = new List<DataItem>();
                foreach (var item in dtItems)
                {
                    DataItem di = new DataItem();
                    di.ItemID = int.Parse(item.ITEMID.ToString());
                    di.ItemName = item.ITEMNAME;
                    di.ItemValue = item.VALUE;
                    dataItemlist.Add(di);
                }
                return dataItemlist[0];
                

            }
        }

        /// <summary>
        /// 写入单个项的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <param name="setValue">写入的值</param>
        public void Write(string groupName, string itemName, string setValue)
        {
            lock (threadLock)
            {
                try
                {
                    bool result = _opcServer.Update(groupName, itemName, setValue);
                    if (!result)
                    {
                        ConsoleManager.WriteLog(123, 123, "操作数据库失败:未读到数据");
                    }
                }
                catch
                {

                    ConsoleManager.WriteLog(123, 123, "操作数据库失败:网络异常或配置连接数据库配置有问题");

                }
            }
        }


        /// <summary>
        /// 写入多个项的值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemValueList">项和项值的键值对集合</param>
        /// <returns></returns>
        public void Write(string groupName, Dictionary<string, object> itemValueDic)
        {
            lock (threadLock)
            {
                try
                {
                    _opcServer.Write(groupName, itemValueDic);
                    //return true;
                }
                catch (Exception ex)
                {
                    ConsoleManager.WriteLog(123, 123, "操作数据库失败:网络异常或配置连接数据库配置有问题" + ex.Message);
                    //return false;
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

        public List<DataItem> Read(string groupName, List<string> itemNameList)
        {
            return new List<DataItem>();
        }

        public void Dispose()
        {
            return;
        }


        public void Write(string groupName, string itemName, object value)
        {
            lock (threadLock)
            {
                try
                {
                    bool result = _opcServer.Update(groupName, itemName, value.ToString());
                    if (!result)
                    {
                        ConsoleManager.WriteLog(123, 123, "操作数据库失败:未读到数据");
                    }
                }
                catch
                {

                    ConsoleManager.WriteLog(123, 123, "操作数据库失败:网络异常或配置连接数据库配置有问题");

                }
            }
        }

        public void Write(string groupName, Dictionary<string, int> itemValueDic)
        {
            lock (threadLock)
            {
                try
                {
                    _opcServer.Write(groupName, itemValueDic);
                    //return true;
                }
                catch (Exception ex)
                {
                    ConsoleManager.WriteLog(123, 123, "操作数据库失败:网络异常或配置连接数据库配置有问题" + ex.Message);
                    //return false;
                }
            }
        }
    }




}
