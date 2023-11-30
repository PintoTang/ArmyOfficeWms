using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CL.Framework.OPCUaClientImpPckg
{
    public class OpcUaClientContainer
    {

        /// <summary>
        /// Opc通讯客户端池
        /// </summary>
        private class OpcClientPool
        {
            public OpcClientPool(string opcUaServerUrl, IEnumerable<IOpcUaCommunicationAbstract> opcClients)
            {
                this.OpcUaServerUrl = opcUaServerUrl;
                this.OpcClientList = new List<IOpcUaCommunicationAbstract>(opcClients);
            }

            public List<IOpcUaCommunicationAbstract> OpcClientList { get; set; }

            public string OpcUaServerUrl { get; private set; }
        }

        public OpcUaClientContainer()
        {
            this.poolList = new List<OpcClientPool>();
        }
        /// <summary>
        /// 获取说有Opc通讯客户端
        /// </summary>
        public IReadOnlyCollection<IOpcUaCommunicationAbstract> OpcClients
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
        public void Register(params string[] opcUaServerUrls)
        {
            lock (this.poolList)
            {
                var poolListTemp = new ConcurrentBag<OpcClientPool>();
                Parallel.ForEach(opcUaServerUrls.Distinct(), opcUaServerUrl =>
                {
                    bool isExist = this.poolList.Exists(p => p.OpcUaServerUrl.Equals(opcUaServerUrl));
                    if (isExist)
                    {
                        return;
                    }
                    int count = 5;
                    var temp = new ConcurrentBag<IOpcUaCommunicationAbstract>();

                    Parallel.For(0, count, i =>
                    {
                        IOpcUaCommunicationAbstract opcClient = CreateOpcClient(opcUaServerUrl);
                        if (opcClient != null)
                        {
                            temp.Add(opcClient);
                        }
                    });
                    if (temp.Count > 0)
                    {
                        poolListTemp.Add(new OpcClientPool(opcUaServerUrl, temp));
                    }
                });
                if (poolListTemp.Count > 0)
                {
                    this.poolList.AddRange(poolListTemp);
                }
            }

        }

        /// <summary>
        /// 根据项名获取Opc通讯客户端
        /// </summary>
        /// <param name="itemName">项名</param>
        /// <returns>Opc通讯客户端</returns>
        public IOpcUaCommunicationAbstract GetOpcClient(string opcUaServerUrl)
        {
            OpcClientPool pool = null;
            if (poolList.Exists(e => e.OpcUaServerUrl==opcUaServerUrl))
            {
                pool = poolList.FirstOrDefault(e => e.OpcUaServerUrl == opcUaServerUrl);
            }
            else
            {
                Register(opcUaServerUrl);
                pool = poolList.FirstOrDefault(e => e.OpcUaServerUrl == opcUaServerUrl);
            }
            return GetOpcClient(pool,opcUaServerUrl);
        }

        public bool IsExistOpcClient(string opcUaServerUrl)
        {
            return poolList.Exists(p => p.OpcUaServerUrl.Equals(opcUaServerUrl));
        }


        protected virtual IOpcUaCommunicationAbstract CreateOpcClient(string opcUaServerUrl)
        {
            IOpcUaCommunicationAbstract opcService = null;
            try
            {
                opcService = new OpcUaClient(new OpcUaRWServer());
                opcService.ConnectOpcServer(opcUaServerUrl);
            }
            catch (Exception ex)
            {
            }
            return opcService;
        }

        private IOpcUaCommunicationAbstract GetOpcClient(OpcClientPool pool,string opcUaServerUrl)
        {
            IOpcUaCommunicationAbstract result;
            DateTime dt = DateTime.Now.AddMilliseconds(50);
            if (pool == null)
            {
                result = CreateOpcClient(opcUaServerUrl);
            }
            else
            {
                lock (pool.OpcClientList)
                {
                    do
                    {
                        result = pool.OpcClientList.FirstOrDefault(e => !(e as OpcUaClient).IsUsing);

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
                        result = CreateOpcClient(opcUaServerUrl);

                        pool.OpcClientList.Add(result);

                        CreateOpcClientAsync(pool,opcUaServerUrl);
                    }

                    (result as OpcUaClient).IsUsing = true;
                }

            }

            return result;
        }

        private void CreateOpcClientAsync(OpcClientPool pool,string opcUaServerUrl)
        {
            Task.Run(() =>
            {
                var temp = new ConcurrentBag<IOpcUaCommunicationAbstract>();
                int to;

                lock (pool.OpcClientList) to = (int)(pool.OpcClientList.Count * 1.5);
                Parallel.For(0, to, i =>
                {
                    IOpcUaCommunicationAbstract client = CreateOpcClient(opcUaServerUrl);

                    temp.Add(client);
                });

                lock (pool.OpcClientList) pool.OpcClientList.AddRange(temp);
            });
        }

        private const int maxOpcClientCount = 100;
        private List<OpcClientPool> poolList;
    }
}
