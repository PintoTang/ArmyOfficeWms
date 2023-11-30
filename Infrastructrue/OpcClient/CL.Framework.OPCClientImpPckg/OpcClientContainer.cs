using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using CLDC.Framework.Log;

namespace CL.Framework.OPCClientImpPckg
{
    /// <summary>
    /// Opc通讯客户端容器
    /// </summary>
    public class OpcClientContainer
    {
        /// <summary>
        /// Opc通讯客户端池
        /// </summary>
        private class OpcClientPool
        {
            public OpcClientPool(string connectionString, IEnumerable<IOpcCommunicationAbstract> opcClients)
            {
                this.ConnectionString = connectionString;
                this.OpcClientList = new List<IOpcCommunicationAbstract>(opcClients);
            }

            public List<IOpcCommunicationAbstract> OpcClientList { get; set; }

            public string ConnectionString { get; private set; }
        }
        /// <summary>
        /// 默认构造
        /// </summary>
        public OpcClientContainer()
        {
            this.poolList = new List<OpcClientPool>();
        }

        /// <summary>
        /// 获取说有Opc通讯客户端
        /// </summary>
        public IReadOnlyCollection<IOpcCommunicationAbstract> OpcClients
        {
            get
            {
                return (from values in poolList
                        from value in values.OpcClientList.ToArray()
                        select value).ToArray();
            }
        }

        /// <summary>
        /// 依连接字符串注册Opc通讯客户端
        /// </summary>
        /// <param name="connectionStrings">连接字符串</param>
        public void Register(params string[] connectionStrings)
        {
            lock (this.poolList)
            {
                var poolListTemp = new ConcurrentBag<OpcClientPool>();
                Parallel.ForEach(connectionStrings.Distinct(), connectionString =>
                {
                    bool isExist = this.poolList.Exists(p => p.ConnectionString.Equals(connectionString));
                    if (isExist)
                    {
                        return;
                    }
                    int count = 5;
                    var temp = new ConcurrentBag<IOpcCommunicationAbstract>();

                    Parallel.For(0, count, i =>
                    {
                        IOpcCommunicationAbstract opcClient = CreateOpcClient();
                        if (opcClient != null)
                        {
                            opcClient.AddGroup(OPCRWClient.GROUP_NAME);
                            temp.Add(opcClient);
                        }
                    });
                    if (temp.Count > 0)
                    {
                        poolListTemp.Add(new OpcClientPool(connectionString, temp));
                    }
                });
                if (poolListTemp.Count > 0)
                {
                    this.poolList.AddRange(poolListTemp);
                }
            }

        }


        private string GetOpcConnection(string itemName)
        {
            if (!string.IsNullOrEmpty(itemName)&&itemName.Contains("S7:[S7 connection_"))
            {
                int index = itemName.LastIndexOf(']');
                string connection = itemName.Substring(0, index + 1);
                return connection;
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 根据项名获取Opc通讯客户端
        /// </summary>
        /// <param name="itemName">项名</param>
        /// <returns>Opc通讯客户端</returns>
        public IOpcCommunicationAbstract GetOpcClient(string itemName)
        {
            OpcClientPool pool = null;
            if (poolList.Exists(e => itemName.Contains(e.ConnectionString)))
            {
                pool = poolList.FirstOrDefault(e => itemName.Contains(e.ConnectionString));
            }
            else
            {
                string connection = GetOpcConnection(itemName);
                if (!string.IsNullOrEmpty(connection))
                {
                    Register(connection);
                    pool = poolList.FirstOrDefault(e => connection.Contains(e.ConnectionString));
                }
            }
            return GetOpcClient(pool);
        }

        public bool IsExistOpcClient(string connection)
        {
            return poolList.Exists(p => p.ConnectionString.Equals(connection));
        }


        protected virtual IOpcCommunicationAbstract CreateOpcClient()
        {
            IOpcCommunicationAbstract opcService = null;
            try
            {
                opcService = new OpcClient(new OPCRWServer(ServerType.OPC_SimaticNET));
            }
            catch (Exception ex)
            {
            }
            return opcService;
        }

        private IOpcCommunicationAbstract GetOpcClient(OpcClientPool pool)
        {
            IOpcCommunicationAbstract result;
            DateTime dt = DateTime.Now.AddMilliseconds(50);
            if (pool == null)
            {
                result = CreateOpcClient();
                if (result != null)
                {
                    result.AddGroup(OPCRWClient.GROUP_NAME);
                }
            }
            else
            {
                lock (pool.OpcClientList)
                {
                    do
                    {
                        result = pool.OpcClientList.FirstOrDefault(e => !(e as OpcClient).IsUsing);

                        if (pool.OpcClientList.Count >= maxOpcClientCount)
                        {
                            Thread.Sleep(1);
                        }
                        else
                        {
                            if (DateTime.Now > dt) break;
                        }
                    } while (result == null);

                    if (result == null)
                    {
                        result = CreateOpcClient();

                        result.AddGroup(OPCRWClient.GROUP_NAME);

                        pool.OpcClientList.Add(result);

                        CreateOpcClientAsync(pool);
                    }

                    (result as OpcClient).IsUsing = true;
                }

            }

            return result;
        }

        private void CreateOpcClientAsync(OpcClientPool pool)
        {
            Task.Run(() =>
            {
                var temp = new ConcurrentBag<IOpcCommunicationAbstract>();
                int to;

                lock (pool.OpcClientList) to = (int)(pool.OpcClientList.Count * 1.5);
                Parallel.For(0, to, i =>
                {
                    IOpcCommunicationAbstract client = CreateOpcClient();

                    client.AddGroup(OPCRWClient.GROUP_NAME);

                    temp.Add(client);
                });

                lock (pool.OpcClientList) pool.OpcClientList.AddRange(temp);
            });
        }

        private const int maxOpcClientCount = 100;
        private List<OpcClientPool> poolList;
    }
}