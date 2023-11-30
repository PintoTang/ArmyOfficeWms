using CL.Framework.OPCClientAbsPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.OPCMonitorImpPckg
{
    public partial class OPCMonitor
    {
        private void RegisterOpcMonitor(
            string deviceName,
            Datablock datablock,
            int startStatus,
            int endStatus,
            int value,
            MonitorType monitorType,
            MonitorSpecifiedOpcValueCallback mse,
            CallbackContainOpcValue mvc,
            CallbackContainOpcValueAndAddress mdo,
            CallbackContainOpcValueAndAddressAndDeviceName vad)
        {
            RegisterInfo registerInfo = new RegisterInfo();
            registerInfo.IntStartStatus = startStatus;
            registerInfo.IntEndStatus = endStatus;
            registerInfo.MonitorSpecifiedOpcValueCallback = mse;
            registerInfo.Datablock = datablock;
            registerInfo.EnumMoniter = monitorType;
            registerInfo.MonitorSpecifiedOpcValueCallback = mse;
            registerInfo.CallbackContainOpcValue = mvc;
            registerInfo.CallbackContainOpcValueAndAddress = mdo;
            registerInfo.IntopcValue = value;
            registerInfo.CallbackContainOpcValueAndAddressAndDeviceName = vad;
            AddRegisterList(registerInfo, deviceName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="datablock"></param>
        /// <param name="endStatus"></param>
        /// <param name="mvc"></param>
        public void RegisterReadEndValueOnceCallBack(string deviceName, Datablock datablock, int endStatus, CallbackContainOpcValue mvc)
        {
            RegisterOpcMonitor(deviceName, datablock, 0, endStatus, 0, MonitorType.MonitorReadEndValueOnceCallBack, null, mvc, null, null);
        }

        /// <summary>
        ///  当OPC监控器读取到两个值不一致时,回调包含读取到的OPC的值和DB块地址的委托
        /// </summary>
        public void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcValueAndAddress mdo)
        {
            RegisterOpcMonitor(deviceName, datablock, 0, 0, 0, MonitorType.MonitorValueChange, null, null, mdo, null);
        }

        /// <summary>
        /// 当OPC监控器读取到两个值不一致时,回调包含读取到的OPC的值委托
        /// </summary>
        public void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcValue mvc)
        {
            RegisterOpcMonitor(deviceName, datablock, 0, 0, 0, MonitorType.MonitorValueChange, null, mvc, null, null);
        }

        public void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcValueAndAddressAndDeviceName vad)
        {
            RegisterOpcMonitor(deviceName, datablock, 0, 0, 0, MonitorType.MonitorValueChange, null, null, null, vad);
        }

        /// <summary>
        /// 当OPC监控器读到从开始状态变化到终止状态时回调委托
        /// </summary>
        public void RegisterFromStartToEndStatus(string deviceName, Datablock datablock, int startStatus, int endStatus, MonitorSpecifiedOpcValueCallback mse)
        {
            RegisterOpcMonitor(deviceName, datablock, startStatus, endStatus, 0, MonitorType.MonitorFromStartToEndStatusCallback, mse, null, null, null);
        }

        /// <summary>
        /// 当OPC监控器读到到终止状态时回调委托
        /// </summary>
        public void RegisterReadEndStatus(string deviceName, Datablock datablock, int endStatus, MonitorSpecifiedOpcValueCallback mso)
        {
            RegisterOpcMonitor(deviceName, datablock, 0, endStatus, 0, MonitorType.MonitorOpcReadEndStatusCallback, mso, null, null, null);
        }

        /// <summary>
        /// 当OPC监控器读到到终止状态时回调委托
        /// </summary>
        public void RegisterReadEndStatus(string deviceName, Datablock datablock, int endStatus, CallbackContainOpcValueAndAddressAndDeviceName vad)
        {
            RegisterOpcMonitor(deviceName, datablock, 0, endStatus, 0, MonitorType.MonitorOpcReadEndStatusCallback, null, null, null, vad);
        }
        /// <summary>
        /// 当OPC监控器读取到的值不等于初始值时，并且上一个值为初始值时才回调委托
        /// </summary>
        public void RegisterNotEqualStartValue(string deviceName, Datablock datablock, int startValue, CallbackContainOpcValue mvc)
        {
            RegisterOpcMonitor(deviceName, datablock, startValue, 0, 0, MonitorType.MonitorNotEqualStartValueCallback, null, mvc, null, null);
        }

        /// <summary>
        /// 当OPC监控器读取到的值不等于初始值时，并且上一个值为初始值时才回调委托
        /// </summary>
        public void RegisterNotEqualStartValue(string deviceName, Datablock datablock, int startValue, CallbackContainOpcValueAndAddress mdo)
        {
            RegisterOpcMonitor(deviceName, datablock, startValue, 0, 0, MonitorType.MonitorNotEqualStartValueCallback, null, null, mdo, null);
        }

        /// <summary>
        /// 当OPC监控器读取到的值不等于初始值时，并且上一个值为初始值时才回调委托
        /// </summary>
        public void RegisterNotEqualStartValue(string deviceName, Datablock datablock, int startValue, CallbackContainOpcValueAndAddressAndDeviceName vad)
        {
            RegisterOpcMonitor(deviceName, datablock, startValue, 0, 0, MonitorType.MonitorNotEqualStartValueCallback, null, null, null, vad);
        }

        /// <summary>
        /// 增加注册信息
        /// </summary>
        /// <param name="registerInfo"></param>
        /// <param name="deviceName"></param>
        private void AddRegisterList(RegisterInfo registerInfo, string deviceName)
        {
            lock (newRegisterList)
            {
                List<RegisterInfo> list = new List<RegisterInfo>();
                if (!newRegisterList.ContainsKey(deviceName))
                {
                    list.Add(registerInfo);
                    newRegisterList.Add(deviceName, list);
                }
                else
                {
                    List<RegisterInfo> registerInfoTempList = newRegisterList[deviceName];
                    for (int i = 0; i < registerInfoTempList.Count; i++)
                    {
                        RegisterInfo p = registerInfoTempList[i];
                        if (p.Datablock.RealDataBlockAddr == registerInfo.Datablock.RealDataBlockAddr && registerInfo.EnumMoniter == MonitorType.MonitorFromStartToEndStatusCallback)
                        {
                            newRegisterList[deviceName][i] = registerInfo;
                            break;
                        }
                        else if (p.Datablock.RealDataBlockAddr != registerInfo.Datablock.RealDataBlockAddr || p.EnumMoniter != registerInfo.EnumMoniter)
                        {
                            NewRegisterListHandlerBusiness(registerInfo, deviceName);
                            break;
                        }
                        else
                        {
                            Log.getDebugFile().Info("opc监控器重复注册：" + deviceName + ";注册的OPC地址块为" + registerInfo.Datablock + ";注册的OPC类型为" + registerInfo.EnumMoniter);
                        }
                    }
                }
                //此处添加需要监控的列表
                AddRegisterDeviceAddressInfoForInt(deviceName, registerInfo);
            }
        }

        private void NewRegisterListHandlerBusiness(RegisterInfo registerInfo, string deviceName)
        {
            if (!IsContaindatablockAndSameMoniterType(registerInfo, deviceName))
            {
                newRegisterList[deviceName].Add(registerInfo);
            }
            else
            {
                var itemIndex = newRegisterList[deviceName].FindIndex(x => x.Datablock.RealDataBlockAddr == registerInfo.Datablock.RealDataBlockAddr);
                newRegisterList[deviceName][itemIndex] = registerInfo;
            }
        }

        private bool IsContaindatablockAndSameMoniterType(RegisterInfo registerInfo, string deviceName)
        {
            List<RegisterInfo> registerInfoTempList = newRegisterList[deviceName];
            for (int i = 0; i < registerInfoTempList.Count; i++)
            {
                if (registerInfoTempList[i].Datablock.RealDataBlockAddr == registerInfo.Datablock.RealDataBlockAddr && registerInfoTempList[i].EnumMoniter == registerInfo.EnumMoniter)
                {
                    return true;
                }
            }
            return false;
        }

        private void RemoveRegisterDeviceAddressInfoForInt(string deviceName, string  datablockAddr)
        {
            lock (_allDeviceAddressInfoListForInt)
            {
                if (_allDeviceAddressInfoListForInt.Exists(d => d.deviceName.Equals(deviceName) && d.Datablock.RealDataBlockAddr.Equals(datablockAddr)))
                {
                    DeviceAddressInfo needRemovedRegisterInfo =
                        _allDeviceAddressInfoListForInt.First(
                            d =>
                                d.deviceName.Equals(deviceName) &&
                                d.Datablock.RealDataBlockAddr.Equals(datablockAddr));
                    _allDeviceAddressInfoListForInt.Remove(needRemovedRegisterInfo);
                }
            }
        }

        private void AddRegisterDeviceAddressInfoForInt(string deviceName, RegisterInfo newRegisterInfo)
        {
            lock (_allDeviceAddressInfoListForInt)
            {
                if (_allDeviceAddressInfoListForInt.Exists(d => d.deviceName.Equals(deviceName) && d.Datablock.RealDataBlockAddr.Equals(newRegisterInfo.Datablock.RealDataBlockAddr)))
                {
                    Log.getDebugFile().Info("opc监控器已经存在：" + deviceName + ";注册的OPC地址块为" + newRegisterInfo.Datablock + ";注册的OPC类型为" + newRegisterInfo.EnumMoniter);
                    return;
                }
                else
                {
                    Log.getDebugFile().Info("opc监控器新增：" + deviceName + ";注册的OPC地址块为" + newRegisterInfo.Datablock + ";注册的OPC类型为" + newRegisterInfo.EnumMoniter);
                    DeviceAddressInfo info = new DeviceAddressInfo();
                    info.deviceName = deviceName;
                    info.Datablock = newRegisterInfo.Datablock;
                    _allDeviceAddressInfoListForInt.Add(info);
                }
            }
        }


        private  List<DeviceAddressInfo> _allDeviceAddressInfoListForInt=new List<DeviceAddressInfo>();

        private void HandleReadIntValue(List<DeviceAddressInfoGroup> needReadDeviceAddressInfoGroupList)
        {
            List<string> deviceNameList = newRegisterList.Keys.ToList();

            //由于一个DB块地址可重复注册多次，所以对已注册的OPCMonitor去重复，简化批量读时读取重复DB块地址
            //DistinctDeviceAddressInfoFromRegisterOPCMonitorInfo(allDeviceAddressInfoList, distinctDeviceAddressInfoList);

            if (threadManager != null && IsConcurrentRead)
            {
                List<DeviceAddressInfoGroup> deviceAddressInfoGroups = DeviceAddressInfoGroup.Parse(typeof(int), _allDeviceAddressInfoListForInt);

                threadManager.TryAddThread(deviceAddressInfoGroups, HandleIntChanged);

                needReadDeviceAddressInfoGroupList.AddRange(deviceAddressInfoGroups);
            }
            else
            {
                HandleIntChanged(opcClient, deviceNameList, _allDeviceAddressInfoListForInt, _allDeviceAddressInfoListForInt);
            }
        }

        private void HandleIntChanged(OPCClientAbstract opcClient, List<string> deviceNameList, List<DeviceAddressInfo> allDeviceAddressInfoList, List<DeviceAddressInfo> distinctDeviceAddressInfoList)
        {
            List<int> readReal = new List<int>();
            List<int> currentOpcReadValue = new List<int>();
            //批量读取DB块地址，并将读取到值
            deviceNameList.Sort();
            allDeviceAddressInfoList.Sort((x, y) => { return x.deviceName.CompareTo(y.deviceName); });
            distinctDeviceAddressInfoList.Sort((x, y) => { return x.deviceName.CompareTo(y.deviceName); });
            if (distinctDeviceAddressInfoList.Count != 0)
            {
                readReal = opcClient.Read(distinctDeviceAddressInfoList);

                if (readReal.Count != 0)
                {
                    foreach (var item in allDeviceAddressInfoList)
                    {
                        bool matchSuccess = false;
                        int index = 0;
                       
                        foreach (var it in distinctDeviceAddressInfoList)
                        {
                            if (item.deviceName == it.deviceName && item.Datablock.RealDataBlockAddr == it.Datablock.RealDataBlockAddr)
                            {
                                currentOpcReadValue.Add(readReal[index]);
                                item.Datablock.RealValue = readReal[index].ToString();
                                matchSuccess = true;
                                break;
                            }
                            index++;
                        }

                        if (!matchSuccess)
                        {
                            //重复注册的DB块才会进来
                            index = 0;
                            foreach (var it in distinctDeviceAddressInfoList)
                            {
                                string noConnectionDeviceName = it.deviceName.Split('@')[0];

                                if (item.deviceName == noConnectionDeviceName && item.Datablock.RealDataBlockAddr == it.Datablock.RealDataBlockAddr)
                                {
                                    currentOpcReadValue.Add(readReal[index]);
                                    item.Datablock.RealValue = readReal[index].ToString();
                                    break;
                                }
                                index++;
                            }
                        }
                    }
                }
            }
            //根据注册时不同的类型和读取到的OPC信息对业务进行处理
            ProcessOPCMonitorBussinessByRegisterType(deviceNameList, currentOpcReadValue);
        }

        private void ProcessOPCMonitorBussinessByRegisterType(List<string> devicelist, List<int> currentOpcReadValue)
        {
            //处理之前先判断注册的链表长度与opc读取到的值链表长度是否一致
            lock (newRegisterList)
            {
                int registerCount = 0;
                foreach (var device in devicelist.ToArray())
                {
                    registerCount = newRegisterList[device].Count + registerCount;
                }
                if (currentOpcReadValue.Count != registerCount)
                {
                    Log.getDebugFile().Info("比较变化时，发现读到的OPC值与注册的OPC值个数不一致，忽略");
                    return;
                }
                //开始判断业务
                int resultindex = 0;
                if (currentOpcReadValue.Count > 0)
                {
                    foreach (var device in devicelist.ToArray())
                    {
                        List<RegisterInfo> listinfo = newRegisterList[device];
                        foreach (var item in listinfo.ToList())
                        {
                            if (OPCClientAbstract.INVALID_VALUE != currentOpcReadValue[resultindex])
                            {
                                switch (item.EnumMoniter)
                                {
                                    case MonitorType.MonitorOpcReadEndStatusCallback:
                                        if (item.IntLastOpcReadValue != item.IntEndStatus && currentOpcReadValue[resultindex] == item.IntEndStatus)
                                        {
                                            AddToQueueDataPool(item, currentOpcReadValue[resultindex], device);
                                        }
                                        break;
                                    case MonitorType.MonitorFromStartToEndStatusCallback:
                                        if (currentOpcReadValue[resultindex] == item.IntEndStatus && item.IntLastOpcReadValue == item.IntStartStatus)
                                        {
                                            AddToQueueDataPool(item, currentOpcReadValue[resultindex], device);
                                        }
                                        break;
                                    case MonitorType.MonitorValueChange:
                                        if (item.IntLastOpcReadValue != currentOpcReadValue[resultindex])
                                        {
                                            AddToQueueDataPool(item, currentOpcReadValue[resultindex], device);
                                        }
                                        break;
                                    case MonitorType.MonitorNotEqualStartValueCallback:
                                        if (item.IntLastOpcReadValue != currentOpcReadValue[resultindex] && item.IntLastOpcReadValue == item.IntStartStatus)
                                        {
                                            AddToQueueDataPool(item, currentOpcReadValue[resultindex], device);
                                        }
                                        break;
                                    case MonitorType.MonitorReadEndValueOnceCallBack:
                                        if (currentOpcReadValue[resultindex] == item.IntEndStatus)
                                        {
                                            //解注册
                                            releaseRegister.Add(device, item.Datablock.RealDataBlockAddr);
                                            AddToQueueDataPool(item, currentOpcReadValue[resultindex], device);
                                        }
                                        break;

                                }
                                item.IntLastOpcReadValue = currentOpcReadValue[resultindex];
                            }
                            resultindex++;
                        }
                    }
                }
            }
        }
    }
}
