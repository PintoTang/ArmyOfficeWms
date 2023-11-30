
using CL.Framework.CmdDataModelPckg;
using CL.Framework.OPCClientAbsPckg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CL.Framework.OPCClientImpPckg
{
    /// <summary>
    /// 连接字符串配置
    /// </summary>
    public static class ConnectionStringConfig
    {
        private static Dictionary<string, string> connectionStrings;

        static ConnectionStringConfig()
        {
            connectionStrings = new Dictionary<string, string>();
        }

        /// <summary>
        /// 获取所有连接字符串
        /// </summary>
        public static Dictionary<string, string> ConnectionStrings
        {
            get
            {
                lock (connectionStrings)
                {
                    if (connectionStrings.Count <= 0)
                    {
                        Load("Config/DevicePLCConnectionConfig.xml");
                    }
                }
                return connectionStrings;
            }
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="deviceAddressInfo">DeviceAddressInfo对象</param>
        /// <returns>连接字符串</returns>
        public static string GetConnectcionString(DeviceAddressInfo deviceAddressInfo)
        {
            return GetConnectcionString(deviceAddressInfo.deviceName, deviceAddressInfo.Datablock.RealDataBlockAddr);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <param name="itemName">项名</param>
        /// <returns>连接字符串</returns>
        public static string GetConnectcionString(string deviceName, string itemName)
        {
            return GetOpcConnection(connectionStrings, deviceName, itemName);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="itemName">项名</param>
        /// <returns>连接字符串</returns>
        public static string GetConnectcionString(string itemName)
        {
            return connectionStrings.Values.First(e => itemName.Contains(e));
        }

        private static void Load(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList nodeList = xmlDoc.SelectNodes("/configuration/appSettings/add");

            foreach (XmlNode node in nodeList)
            {
                if (!connectionStrings.ContainsKey(node.Attributes["key"].Value.ToUpper()))
                {
                    connectionStrings.Add(node.Attributes["key"].Value.ToLower(), node.Attributes["value"].Value);
                }
            }
        }

        public static Dictionary<string, string> GetOpcConnectionList(string groupName, List<string> itemNameList)
        {
            Dictionary<string, string> opcConnectionDic = new Dictionary<string, string>();
            foreach (string itemName in itemNameList)
            {
                GetOpcConnection(opcConnectionDic, groupName, itemName);
            }
            return opcConnectionDic;
        }

        public static string GetOpcConnection(Dictionary<string, string> plcConnectionDictionary, string groupName, string itemName)
        {
            string key = string.Empty;
            if (itemName.Contains("S7:[S7 connection_"))
            {
                int index = itemName.LastIndexOf(']');
                string connection = itemName.Substring(0, index + 1);
                if (plcConnectionDictionary.ContainsKey(connection))
                {
                    plcConnectionDictionary[connection] = connection;
                }
                else
                {
                    plcConnectionDictionary.Add(connection, connection);
                }
                return connection;
            }

            if (itemName.Contains("@"))
            {
                Addr realOpcAddr = new Addr(itemName.Split('@')[0]);

                key = plcConnectionDictionary.Keys.FirstOrDefault(p => p.Contains(":") && new Addr(p).IsContain(new Addr(realOpcAddr.FullName.ToLower())));

                if (!string.IsNullOrWhiteSpace(key))
                {
                    key = key.ToLower();

                    return plcConnectionDictionary[key];
                }
            }
            if (plcConnectionDictionary.Keys.Contains(groupName, StringComparer.OrdinalIgnoreCase))
            {
                key = groupName.ToLower();

                return plcConnectionDictionary[key];
            }
            if (plcConnectionDictionary.Keys.Contains(groupName.Split('#')[0], StringComparer.OrdinalIgnoreCase))
            {
                key = groupName.Split('#')[0].ToLower();

                return plcConnectionDictionary[key];
            }
            if (plcConnectionDictionary.Keys.Contains(groupName.Split(':')[0], StringComparer.OrdinalIgnoreCase))
            {
                key = groupName.Split(':')[0].ToLower();

                return plcConnectionDictionary[key];
            }
            return string.Empty;
        }
    }
}