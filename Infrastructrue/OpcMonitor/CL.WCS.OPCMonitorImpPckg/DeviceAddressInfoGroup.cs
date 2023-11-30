using CL.Framework.OPCClientAbsPckg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.OPCClientImpPckg;

namespace CL.WCS.OPCMonitorImpPckg
{
    class DeviceAddressInfoGroup
    {
        public string ConnectionString { get; private set; }

        public List<DeviceAddressInfo> AllDeviceAddressInfoList { get; private set; }

        public List<string> DeviceNameList
        {
            get
            {
                return AllDeviceAddressInfoList.Select(e => e.deviceName).Distinct().ToList();
            }
        }

        public List<DeviceAddressInfo> DistinctDeviceAddressInfoList
        {
            get
            {
                return AllDeviceAddressInfoList.Distinct().ToList();
            }
        }

        public Type ValueType { get; private set; }

        public static List<DeviceAddressInfoGroup> Parse(Type ValueType, List<DeviceAddressInfo> allDeviceAddressInfoList)
        {
            lock (allDeviceAddressInfoList)
            {
                return (from deviceAddressInfo in allDeviceAddressInfoList
                        group deviceAddressInfo by ConnectionStringConfig.GetConnectcionString(deviceAddressInfo)
                            into g
                            select new DeviceAddressInfoGroup()
                            {
                                ConnectionString = g.Key,
                                ValueType = ValueType,
                                AllDeviceAddressInfoList = g.ToList()
                            }).ToList();
            }
        }
    }
}
