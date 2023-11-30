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
            bool startStatus,
            bool endStatus,
            bool value,
            MonitorType monitorType,
            MonitorSpecifiedOpcValueCallback mse,
            CallbackContainOpcBoolValue mvc)
        {
            RegisterInfo registeInfo = new RegisterInfo();
            registeInfo.BoolStartStatus = startStatus;
            registeInfo.BoolEndStatus = endStatus;
            registeInfo.MonitorSpecifiedOpcValueCallback = mse;
            registeInfo.Datablock = datablock;
            registeInfo.EnumMoniter = monitorType;
            registeInfo.BoolOpcValue = value;
            registeInfo.MonitorSpecifiedOpcValueCallback = mse;
            registeInfo.CallbackContainOpcBoolValue = mvc;
            AddRegisterBoolList(registeInfo, deviceName);
        }

        public void RegisterFromStartToEndStatus(string deviceName, Datablock datablock, bool startStatus, bool endStatus, MonitorSpecifiedOpcValueCallback readopc)
        {
            RegisterOpcMonitor(deviceName, datablock, startStatus, endStatus, false, MonitorType.MonitorFromStartToEndStatusCallback, readopc, null);
        }

        public void RegisterValueChange(string deviceName, Datablock datablock, CallbackContainOpcBoolValue monitervaluechange)
        {
            RegisterOpcMonitor(deviceName, datablock, false, false, false, MonitorType.MonitorValueChange, null, monitervaluechange);
        }

        private void AddRegisterBoolList(RegisterInfo registerInfo, string deviceName)
        {
            lock (newRegisterBoolValueList)
            {
                List<RegisterInfo> list = new List<RegisterInfo>();
                if (!newRegisterBoolValueList.ContainsKey(deviceName))
                {
                    list.Add(registerInfo);
                    newRegisterBoolValueList.Add(deviceName, list);
                }
                else
                {
                    List<RegisterInfo> registerBoolValueTempList = newRegisterBoolValueList[deviceName];
                    for (int i = 0; i < registerBoolValueTempList.Count; i++)
                    {
                        RegisterInfo p = registerBoolValueTempList[i];
                        if (p.Datablock.RealDataBlockAddr == registerInfo.Datablock.RealDataBlockAddr)
                        {
                            newRegisterBoolValueList[deviceName][i] = registerInfo;
                            break;
                        }
                        else if (p.Datablock.RealDataBlockAddr != registerInfo.Datablock.RealDataBlockAddr)
                        {
                            if (!newRegisterBoolValueList[deviceName].Exists(u => u.Datablock.RealDataBlockAddr == registerInfo.Datablock.RealDataBlockAddr))
                            {
                                newRegisterBoolValueList[deviceName].Add(registerInfo);
                                break;
                            }
                        }
                    }
                }
                AddRegisterDeviceAddressInfoForBool(deviceName, registerInfo);

            }
        }

        private List<RegisterInfo> GetNewRegisterBoolValueListValue(string deviceName)
        {
            List<RegisterInfo> value;

            lock (newRegisterBoolValueList)
            {
                if (!newRegisterBoolValueList.TryGetValue(deviceName, out value))
                {
                    value = new List<RegisterInfo>();
                }
            }

            return value;
        }


        private void RemoveRegisterDeviceAddressInfoForBool(string deviceName, string datablockAddr)
        {
            lock (_allDeviceAddressInfoListForBool)
            {
                if (_allDeviceAddressInfoListForBool.Exists(d => d.deviceName.Equals(deviceName) && d.Datablock.RealDataBlockAddr.Equals(datablockAddr)))
                {
                    DeviceAddressInfo needRemovedRegisterInfo =
                        _allDeviceAddressInfoListForBool.First(
                            d =>
                                d.deviceName.Equals(deviceName) &&
                                d.Datablock.RealDataBlockAddr.Equals(datablockAddr));
                    _allDeviceAddressInfoListForBool.Remove(needRemovedRegisterInfo);
                }
            }
        }

        private void AddRegisterDeviceAddressInfoForBool(string deviceName, RegisterInfo newRegisterInfo)
        {
            lock (_allDeviceAddressInfoListForBool)
            {
                if (_allDeviceAddressInfoListForBool.Exists(d => d.deviceName.Equals(deviceName) && d.Datablock.RealDataBlockAddr.Equals(newRegisterInfo.Datablock.RealDataBlockAddr)))
                {
                    //Log.getDebugFile().Info("opc监控器已经存在：" + deviceName + ";注册的OPC地址块为" + newRegisterInfo.Datablock + ";注册的OPC类型为" + newRegisterInfo.EnumMoniter);
                    return;
                }
                else
                {
                    //Log.getDebugFile().Info("opc监控器新增：" + deviceName + ";注册的OPC地址块为" + newRegisterInfo.Datablock + ";注册的OPC类型为" + newRegisterInfo.EnumMoniter);
                    DeviceAddressInfo info = new DeviceAddressInfo();
                    info.deviceName = deviceName;
                    info.Datablock = newRegisterInfo.Datablock;
                    _allDeviceAddressInfoListForBool.Add(info);
                }
            }
        }


        private List<DeviceAddressInfo> _allDeviceAddressInfoListForBool = new List<DeviceAddressInfo>();

        private void HandleReadBoolValue(List<DeviceAddressInfoGroup> needReadDeviceAddressInfoGroupList)
        {
            List<string> deviceNameList = newRegisterBoolValueList.Keys.ToList();

            //由于一个DB块地址可重复注册多次，所以对已注册的OPCMonitor去重复，简化批量读时读取重复DB块地址
            //DistinctDeviceAddressInfoFromRegisterBoolValue(allDeviceAddressInfoList, distinctDeviceAddressInfoList);

            if (threadManager!=null&&IsConcurrentRead)
            {
                List<DeviceAddressInfoGroup> deviceAddressInfoGroups = DeviceAddressInfoGroup.Parse(typeof(bool), _allDeviceAddressInfoListForBool);

                threadManager.TryAddThread(deviceAddressInfoGroups, HandleBoolChanged);

                needReadDeviceAddressInfoGroupList.AddRange(deviceAddressInfoGroups);
            }
            else
            {
                HandleBoolChanged(opcClient, deviceNameList, _allDeviceAddressInfoListForBool, _allDeviceAddressInfoListForBool);
            }
        }

        private void HandleBoolChanged(OPCClientAbstract opcClient, List<string> deviceNameList, List<DeviceAddressInfo> allDeviceAddressInfoList, List<DeviceAddressInfo> distinctDeviceAddressInfoList)
        {
            List<bool> readReal = new List<bool>();
            List<bool> currentOpcReadValue = new List<bool>();

            if (distinctDeviceAddressInfoList.Count != 0)
            {
                readReal = opcClient.ReadBoolList(distinctDeviceAddressInfoList);

                foreach (var item in allDeviceAddressInfoList)
                {
                    int index = 0;
                    foreach (var it in distinctDeviceAddressInfoList)
                    {
                        if (item.deviceName == it.deviceName && item.Datablock.RealDataBlockAddr == it.Datablock.RealDataBlockAddr)
                        {
                            if (readReal.Count > index)
                            {
                                item.Datablock.RealValue = readReal[index].ToString();
                                currentOpcReadValue.Add(readReal[index]);
                            }
                            else
                            {
                                item.Datablock.RealValue = "False";
                                currentOpcReadValue.Add(false);
                            }
                            break;
                        }
                        index++;
                    }
                }
            }

            //根据注册时不同的类型和读取到的OPC信息对业务进行处理
            ProcessOPCMonitorBussiness(deviceNameList, currentOpcReadValue);
        }

        private void ProcessOPCMonitorBussiness(List<string> devicelist, List<bool> currentOpcReadValue)
        {
            //处理之前先判断注册的链表长度与opc读取到的值链表长度是否一致
            int registerCount = 0;

            foreach (var device in devicelist.ToArray())
            {
                registerCount = GetNewRegisterBoolValueListValue(device).Count + registerCount;
            }
            if (currentOpcReadValue.Count != registerCount)
            {
                return;
            }
            //开始判断业务
            int resultindex = 0;
            if (currentOpcReadValue.Count > 0)
            {
                foreach (var device in devicelist.ToArray())
                {
                    List<RegisterInfo> listinfo = GetNewRegisterBoolValueListValue(device);
                    int flag = 0;
                    foreach (var item in listinfo.ToList())
                    {
                        switch (item.EnumMoniter)
                        {
                            case MonitorType.MonitorFromStartToEndStatusCallback:
                                if (currentOpcReadValue[resultindex] == item.BoolEndStatus && item.BoolLastOpcReadValue == item.BoolStartStatus)
                                {
                                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("Bool监控到{0}进行回调,从期望值{1}到当前值{2}", device, item.BoolStartStatus, item.BoolEndStatus));
                                    item.MonitorSpecifiedOpcValueCallback.BeginInvoke(null, null);
                                    Log.getMessageFile("OPCMointorDebug").Info(string.Format("Bool监控到{0}进行回调完成", device));
                                }
                                break;
                            case MonitorType.MonitorValueChange:
                                if (item.BoolLastOpcReadValue != currentOpcReadValue[resultindex])
                                {
                                    AddToQueueDataPool(item, currentOpcReadValue[resultindex], device);
                                }
                                break;
                            default:
                                break;
                        }
                        item.BoolLastOpcReadValue = currentOpcReadValue[resultindex];
                        lock (newRegisterBoolValueList)
                        {
                            var oldRegisterInfo = newRegisterBoolValueList[device].Find(d =>
                                    d.Datablock.RealDataBlockAddr == item.Datablock.RealDataBlockAddr &&
                                    d.EnumMoniter == item.EnumMoniter);
                            if (oldRegisterInfo != null)
                            {
                                oldRegisterInfo.BoolLastOpcReadValue = currentOpcReadValue[resultindex];
                            }
                        }
                        listinfo[flag] = item;
                        flag++;
                        resultindex++;
                    }
                }
            }
        }
    }
}
