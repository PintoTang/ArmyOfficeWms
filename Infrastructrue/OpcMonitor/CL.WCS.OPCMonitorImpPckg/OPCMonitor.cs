using System;
using System.Collections.Generic;
using System.Linq;
using CL.WCS.OPCMonitorAbstractPckg;
using System.Threading;
using CL.Framework.ThreadSleepAbstractPckg;
using CL.Framework.OPCClientAbsPckg;
using CL.Framework.DataPoolPckg;
using CLDC.Framework.Log;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CL.Framework.OPCClientImpPckg;
using CL.WCS.SystemConfigPckg;

namespace CL.WCS.OPCMonitorImpPckg
{
    /// <summary>
    ///  OPC监控器
    /// </summary>
    public partial class OPCMonitor : OPCMonitorAbstract
    {
        ThreadManager threadManager;

        OPCClientAbstract opcClient;

        Dictionary<string, List<RegisterInfo>> newRegisterList = new Dictionary<string, List<RegisterInfo>>();

        private Thread readValueThread = null;

        Thread[] callbackThreads;

        volatile bool shouldStop;

        Dictionary<string, List<RegisterInfo>> newRegisterBoolValueList = new Dictionary<string, List<RegisterInfo>>();

        Dictionary<string, string> releaseRegister = new Dictionary<string, string>();

        protected ThreadSleepAbstract threadSleep = new ThreadSleep();
        public ThreadSleepAbstract TI_ThreadSleep
        {
            set { threadSleep = value; }
        }

        private BlockingCollection<DeviceRegisterInfo> callBackDataQueue = new BlockingCollection<DeviceRegisterInfo>();
        public static int MonitorIntervalTime { get; private set; }

        private const int INVALID_VALUE = -0x7FFFFFFF;

        private bool _isConcurrentRead = false;
        private bool IsConcurrentRead
        {
            get
            {
                return _isConcurrentRead;
            }
            set { _isConcurrentRead = value; }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="opcClient">OPCClientAbstract实现对象</param>
        /// <param name="isConcurrentRead">是否并行读取</param>
        //public OPCMonitor(OPCClientAbstract opcClient, IOpcClientFactory opcClientFactory = null)
        //{
        //    this.opcClient = opcClient;
        //    this.callbackThreads = new Thread[] { };
        //    this.shouldStop = false;
        //    this.threadManager = new ThreadManager(opcClientFactory);
        //    this.IsConcurrentRead = opcClientFactory != null;
        //}

        public OPCMonitor(OPCClientAbstract opcClient)
        {
            this.opcClient = opcClient;
            this.callbackThreads = new Thread[] { };
            this.shouldStop = false;
            IOpcClientFactory opcClientFactory = opcClient.GetFactory();
            if (opcClientFactory!=null)
            {
                this.threadManager = new ThreadManager(opcClient.GetFactory());
                IsConcurrentRead = true; 
            }
            else
            {
                IsConcurrentRead = false;
            }
        }

        public void StartMonitor(int threadCountOfBusinessHandle = 10, int monitorIntervalTime = 200)
        {
            shouldStop = false;
            MonitorIntervalTime = monitorIntervalTime;

            readValueThread = new Thread(new ThreadStart(ExecuteReadOperate));
            readValueThread.IsBackground = true;
            readValueThread.Start();

            callbackThreads = new Thread[threadCountOfBusinessHandle];
            for (int i = 0; i < callbackThreads.Length; i++)
            {
                if (callbackThreads[i] == null)
                {
                    callbackThreads[i] = new Thread(new ParameterizedThreadStart(DoWhithCallbackQueue));
                    callbackThreads[i].Name = "Thread" + (i + 1).ToString();
                    callbackThreads[i].IsBackground = true;
                    callbackThreads[i].Start(callbackThreads[i].Name);
                }
            }
        }

        public void StopMonitor()
        {
            shouldStop = true;

            threadManager.Stop();

            if (readValueThread != null)
            {
                readValueThread.Join();
            }

            foreach (Thread t in callbackThreads)
            {
                t.Join();
            }
        }

        /// <summary>
        /// 线程执行方法
        /// </summary>
        private void ExecuteReadOperate()
        {
            while (!shouldStop)
            {
                try
                {
                    List<DeviceAddressInfoGroup> needReadDeviceAddressInfoGroupList = new List<DeviceAddressInfoGroup>();

                    HandleReadBoolValue(needReadDeviceAddressInfoGroupList);

                    HandleReadIntValue(needReadDeviceAddressInfoGroupList);

                    ReleaseRegisterByDeviceNameAndAddress(releaseRegister);

                    if (threadManager!=null&&IsConcurrentRead)
                    {
                        threadManager.TryRemoveThread(needReadDeviceAddressInfoGroupList);
                    }
                }
                catch (Exception ex)
                {
                    Log.getExceptionFile().Info("OPC监控器代码运行出错:", ex);
                }

                threadSleep.Sleep(10);
            }
        }

        private void DistinctDeviceAddressInfoFromRegisterOPCMonitorInfo(List<DeviceAddressInfo> alldeviceInfoList, List<DeviceAddressInfo> distinctDeviceInfoList)
        {
            if (newRegisterList == null || newRegisterList.Count < 1)
                return;
            Dictionary<string, List<RegisterInfo>> newRegisterListCopy = newRegisterList;
            List<string> devicelist = newRegisterListCopy.Keys.ToList();
            for (int i = 0; i < devicelist.Count; i++)
            {
                string strDeviceName = devicelist[i];
                List<RegisterInfo> RegisterInfolist = newRegisterListCopy[strDeviceName];
                for (int a = 0; a < RegisterInfolist.Count; a++)
                {
                    DeviceAddressInfo info = new DeviceAddressInfo();
                    info.deviceName = strDeviceName;
                    info.Datablock = RegisterInfolist[a].Datablock;
                    alldeviceInfoList.Add(info);
                    if (distinctDeviceInfoList.Count > 0)
                    {
                        int count = distinctDeviceInfoList.Count(p => p.Datablock.RealDataBlockAddr == RegisterInfolist[a].Datablock.RealDataBlockAddr && p.deviceName == strDeviceName);
                        if (count == 0)
                        {
                            distinctDeviceInfoList.Add(info);
                        }
                    }
                    else
                    {
                        distinctDeviceInfoList.Add(info);
                    }
                }
            }
        }

        private void DistinctDeviceAddressInfoFromRegisterBoolValue(List<DeviceAddressInfo> deviceInfoList, List<DeviceAddressInfo> deviceInfoListReal)
        {
            Dictionary<string, List<RegisterInfo>> newRegisterBoolValueListCopy = newRegisterBoolValueList;
            var keys = newRegisterBoolValueListCopy.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var value = newRegisterBoolValueListCopy[key];

                for (int index = 0; index < value.Count; index++)
                {
                    var it = value[index];
                    DeviceAddressInfo info = new DeviceAddressInfo();
                    info.deviceName = key;
                    info.Datablock = it.Datablock;
                    deviceInfoList.Add(info);
                    if (deviceInfoListReal.Count > 0)
                    {

                        int count = deviceInfoListReal.Count(p => p.Datablock.RealDataBlockAddr == it.Datablock.RealDataBlockAddr && p.deviceName == key);
                        if (count == 0)
                        {
                            deviceInfoListReal.Add(info);
                        }
                    }
                    else
                    {
                        deviceInfoListReal.Add(info);
                    }
                }
            }
        }

        public void ReleaseRegisterList(string deviceName, Datablock datablock)
        {
            releaseRegister.Add(deviceName, datablock.RealDataBlockAddr);
        }

        public void ReleaseRegisterBoolValueList(string deviceName, Datablock datablock)
        {
            releaseRegister.Add(deviceName,datablock.RealDataBlockAddr);
        }

        public void ReleaseDeviceRegister(string deviceName)
        {
            if (newRegisterList.Keys.Contains(deviceName))
            {
                newRegisterList.Remove(deviceName);
            }
            if (newRegisterBoolValueList.Keys.Contains(deviceName))
            {
                newRegisterBoolValueList.Remove(deviceName);
            }
        }

        private void ReleaseRegisterByDeviceNameAndAddress(Dictionary<string, string> ReleaseRegister)
        {
            if (ReleaseRegister == null || ReleaseRegister.Count <= 0)
            {
                return;
            }
            foreach (KeyValuePair<string, string> item in ReleaseRegister)
            {
                if (newRegisterList.Keys.Contains(item.Key))
                {
                    if (newRegisterList[item.Key].Any(p => p.Datablock.RealDataBlockAddr == item.Value))
                    {
                        newRegisterList[item.Key].RemoveAll(p => p.Datablock.RealDataBlockAddr == item.Value);
                    }
                    if (newRegisterList[item.Key] != null && newRegisterList[item.Key].Count == 0)
                    {
                        newRegisterList.Remove(item.Key);
                    }
                    RemoveRegisterDeviceAddressInfoForInt(item.Key, item.Value);
                }
                if (newRegisterBoolValueList.Keys.Contains(item.Key))
                {
                    if (newRegisterBoolValueList[item.Key].Any(p => p.Datablock.RealDataBlockAddr == item.Value))
                    {
                        newRegisterBoolValueList[item.Key].RemoveAll(p => p.Datablock.RealDataBlockAddr == item.Value);
                    }
                    if (newRegisterBoolValueList[item.Key] != null && newRegisterBoolValueList[item.Key].Count == 0)
                    {
                        newRegisterBoolValueList.Remove(item.Key);
                    }
                    RemoveRegisterDeviceAddressInfoForBool(item.Key, item.Value);
                }
            }
            ReleaseRegister.Clear();
        }

        private void AddToQueueDataPool(RegisterInfo item, int value, string deviceName)
        {
            DeviceRegisterInfo deviceRegisterInfo = new DeviceRegisterInfo();
            deviceRegisterInfo.Item = item;
            deviceRegisterInfo.IntValue = value;
            deviceRegisterInfo.BoolValue = false;
            deviceRegisterInfo.DeviceName = deviceName;
            Log.getMessageFile("OPCMointorDebug").Info(string.Format("(1)监控到{0}注册的变化，{1}-->{2}", deviceName, item.IntLastOpcReadValue, value));
            try
            {
                callBackDataQueue.Add(deviceRegisterInfo);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info("AddToQueueDataPool异常：" + ex.Message);
            }
        }

        private void AddToQueueDataPool(RegisterInfo item, bool value, string deviceName)
        {
            DeviceRegisterInfo deviceRegisterInfo = new DeviceRegisterInfo();
            deviceRegisterInfo.Item = item;
            deviceRegisterInfo.IntValue = INVALID_VALUE;
            deviceRegisterInfo.BoolValue = value;
            deviceRegisterInfo.DeviceName = deviceName;
            Log.getMessageFile("OPCMointorDebug").Info(string.Format("(1)监控到{0}注册的变化，{1}-->{2}", deviceName, item.BoolLastOpcReadValue, value));
            try
            {
                callBackDataQueue.Add(deviceRegisterInfo);
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info("AddToQueueDataPool异常：" + ex.Message);
            }
        }

        private void DoWhithCallbackQueue(object threadName)
        {
            while (!shouldStop)
            {
                foreach (DeviceRegisterInfo doWithData in callBackDataQueue.GetConsumingEnumerable())
                {
                    Callback(doWithData, threadName.ToString());
                }
                threadSleep.Sleep(20);
            }
        }

        private void Callback(DeviceRegisterInfo deviceRegisterInfo, string threadName)
        {
            try
            {
                Log.getMessageFile("OPCMointorDebug").Info(string.Format("--(2){1}监控到{0}进行回调", deviceRegisterInfo.DeviceName, threadName));
                if (deviceRegisterInfo.Item.CallbackContainOpcValueAndAddress != null)
                {
                    deviceRegisterInfo.Item.CallbackContainOpcValueAndAddress(deviceRegisterInfo.IntValue, deviceRegisterInfo.Item.Datablock);
                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("----(3){1}监控到{0}回调完成", deviceRegisterInfo.DeviceName, threadName));
                }
                else if (deviceRegisterInfo.Item.CallbackContainOpcValue != null)
                {
                    deviceRegisterInfo.Item.CallbackContainOpcValue(deviceRegisterInfo.IntValue);
                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("----(3){1}监控到{0}回调完成", deviceRegisterInfo.DeviceName, threadName));
                }
                else if (deviceRegisterInfo.Item.MonitorSpecifiedOpcValueCallback != null)
                {
                    deviceRegisterInfo.Item.MonitorSpecifiedOpcValueCallback();
                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("----(3){1}监控到{0}回调完成", deviceRegisterInfo.DeviceName, threadName));
                }
                else if (deviceRegisterInfo.Item.CallbackContainOpcValueAndAddressAndDeviceName != null)
                {
                    deviceRegisterInfo.Item.CallbackContainOpcValueAndAddressAndDeviceName(deviceRegisterInfo.IntValue, deviceRegisterInfo.Item.Datablock, deviceRegisterInfo.DeviceName);
                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("----(3){1}监控到{0}回调完成", deviceRegisterInfo.DeviceName, threadName));
                }
                else if (deviceRegisterInfo.Item.CallbackContainOpcBoolValue != null)
                {
                    deviceRegisterInfo.Item.CallbackContainOpcBoolValue(deviceRegisterInfo.BoolValue);
                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("----(3){1}监控到{0}回调完成", deviceRegisterInfo.DeviceName, threadName));
                }
            }
            catch (Exception ex)
            {
                Log.getExceptionFile().Info("OPCMonitor执行回调函数出错:", ex);
            }
        }
    }
}
