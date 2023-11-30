using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Agv.Common
{
    public struct AgvDeviceRequest
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备的当前任务号
        /// </summary>
        public string DeviceTaskCode { get; set; }

        /// <summary>
        /// 当前地址
        /// </summary>
        public string CurrentAddr { get; set; }

        /// <summary>
        /// 是否等待下步地址
        /// </summary>
        public bool IsWaitNextAddr { get; set; }

        /// <summary>
        /// Json
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
