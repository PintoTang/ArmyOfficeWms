using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class Coords
    {
        public string Type;
        public string Id;
        public string Name;
        public string DeviceName;
        public string Class;
        public string NameSpace;
        public int WorkSize;
        public CoordBusinessHandle CoordBusinessHandle;
        public CoordConfigInfo CoordConfigInfo;
    }
    public class CoordBusinessHandle
    {
        public string Type;
        public string Class;
        public string NameSpace;
        public CoordBusinessHandleConfig CoordBusinessHandleConfig;
      
    }
    public class CoordBusinessHandleConfig
    {
        public string CurrAddress;
        public string DestAddress;
        public string Type;

    }
    public class CoordConfigInfo
    {
        public AddrPrefixs AddrPrefixs;
        public DeviceDatas Devices;
    }
    public class DeviceDatas
    {
        public List<DeviceData> DeviceDataLst = new List<DeviceData>();
    }
    public class DeviceData
    {
        public int DeviceId;
    }
    public class AddrPrefixs
    {
        public List<Prefixs> PrefixsLst = new List<Prefixs>();
    }
    public class Prefixs
    {
        public string Type;
        public string Address;
    }


    public class CoorDevice
    {
        public int DeviceId;
        public string Type;
        public bool IsCheckReady;
        public bool IsCheckBarcode;
        public string DestAddress;
    }
}
