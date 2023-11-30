using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Station.Common
{
    public abstract class StationDeviceBusinessAbstract : DeviceBusinessBaseAbstract, IChangeMode
    {
        public  int CurDestValue { get; set; }
        public int CurOrderValue { get; set; }
        public DeviceModeEnum CurMode { get; set; }

        public abstract OperateResult IsCanChangeMode(DeviceModeEnum destMode);


        public abstract OperateResult ChangeMode(DeviceModeEnum destMode);

        public abstract bool CheckMode(DeviceModeEnum destMode);




        public abstract bool IsNeedHandleOrderValue(int newValue);


        public abstract bool IsNeedHandleDestValue(int newValue);

        public virtual void HandleFault(int deviceId, string deviceName, string faultDesc, string handlingSuggest)
        {
            
        }

    }
}
