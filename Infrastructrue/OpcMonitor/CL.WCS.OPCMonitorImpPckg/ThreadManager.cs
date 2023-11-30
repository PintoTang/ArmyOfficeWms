using CL.Framework.OPCClientAbsPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CLDC.Framework.Log;
using System.Diagnostics;

namespace CL.WCS.OPCMonitorImpPckg
{
    class ThreadManager
    {
        private class ThreadWrapper
        {
            public ThreadWrapper()
            {
                this.tokenSource = new CancellationTokenSource();
            }

            public DeviceAddressInfoGroup DeviceAddressInfoGroup { get; set; }

            public void Run(OPCClientAbstract client, Action<OPCClientAbstract, List<string>, List<DeviceAddressInfo>, List<DeviceAddressInfo>> action,
                   DeviceAddressInfoGroup deviceAddressInfoGroup)
            {
                this.DeviceAddressInfoGroup = deviceAddressInfoGroup;

                Task.Factory.StartNew(() =>
                    {
                        while (!tokenSource.Token.IsCancellationRequested)
                        {
                            try
                            {
                                action(client, this.DeviceAddressInfoGroup.DeviceNameList,
                                    this.DeviceAddressInfoGroup.AllDeviceAddressInfoList,
                                    this.DeviceAddressInfoGroup.DistinctDeviceAddressInfoList);
                            }
                            catch (Exception e)
                            {
                                Log.getExceptionFile().Info(e);
                            }

                            Thread.Sleep(OPCMonitor.MonitorIntervalTime);
                        }
                    }, TaskCreationOptions.LongRunning);
            }

            public void Stop()
            {
                tokenSource.Cancel();
            }

            private CancellationTokenSource tokenSource;
        }

        public ThreadManager(IOpcClientFactory opcClientFactory)
        {
            this.threadDictionary = new ConcurrentDictionary<string, ThreadWrapper>();
            this.opcClientFactory = opcClientFactory;
        }

        public void TryAddThread(List<DeviceAddressInfoGroup> deviceAddressInfoGroupList,
            Action<OPCClientAbstract, List<string>, List<DeviceAddressInfo>, List<DeviceAddressInfo>> action
            )
        {
            deviceAddressInfoGroupList.ForEach(e =>
            {
                string key = GetKey(e.ConnectionString, e.ValueType);

                if (threadDictionary.TryAdd(key, new ThreadWrapper()))
                {
                    threadDictionary[key].Run(opcClientFactory.Create(), action, e);
                }
                else
                {
                    ThreadWrapper value = threadDictionary[key];
	                if (value.DeviceAddressInfoGroup==null)
	                {
		                value.DeviceAddressInfoGroup = e;
						return;
	                }
                    if (!e.AllDeviceAddressInfoList.SequenceEqual(value.DeviceAddressInfoGroup.AllDeviceAddressInfoList))
                    {
                        value.DeviceAddressInfoGroup = e;
                    }
                }
            });
        }

        public void TryRemoveThread(List<DeviceAddressInfoGroup> needReadDeviceAddressInfoGroupList)
        {
            List<string> needToRemoveKey = threadDictionary.Keys
                .Except(needReadDeviceAddressInfoGroupList.Select(e => GetKey(e.ConnectionString, e.ValueType)))
                .ToList();

            needToRemoveKey.ForEach(e =>
            {
                ThreadWrapper threadWrapper;

                if (threadDictionary.TryRemove(e, out threadWrapper))
                {
                    threadWrapper.Stop();
                }
            });


        }

        public void Stop()
        {
            threadDictionary.Values.ToList().ForEach(e => e.Stop());
            threadDictionary.Clear();
        }

        private string GetKey(string connectionString, Type readType)
        {
            return connectionString + readType;
        }

        private ConcurrentDictionary<string, ThreadWrapper> threadDictionary;
        private IOpcClientFactory opcClientFactory;
    }
}