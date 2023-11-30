using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model
{
    public enum Simulate3DIOCEnum
    {
        [Description("设备状态数据")]
        DeviceStautsDataUpload,
        [Description("货物物流轨迹")]
        GoodsTrackDataUpload,
        [Description("车辆运行参数数据")]
        AgvRunParamDataUpload,
        [Description("设备动作数据")]
        DeviceActionDataUpload,
        [Description("清空货物")]
        ClearGoodsDataUpload,
       [Description("机器人工作状态")]
        RobotTrackDataUpload,

       [Description("设备状态")]
        AddDeviceFaultRec,
    }
}
