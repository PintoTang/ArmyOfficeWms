using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class DeviceInfoConfigManage
    {
        private static List<DeviceInfoConfig> _deviceInfoLst = new List<DeviceInfoConfig>();
        private static DeviceInfoConfigManage deviceInfoManage;
        public static DeviceInfoConfigManage Instance
        {
            get
            {
                if (deviceInfoManage == null)
                {
                    deviceInfoManage = new DeviceInfoConfigManage();
                }
                return deviceInfoManage;
            }
        }

        public void  AddDevice(DeviceInfoConfig deviceInfo)
        {
            _deviceInfoLst.Add(deviceInfo);
        }

        public List<DeviceInfoConfig> GetAllDeviceInfoData()
        {
            return _deviceInfoLst;
        }
    }
}
