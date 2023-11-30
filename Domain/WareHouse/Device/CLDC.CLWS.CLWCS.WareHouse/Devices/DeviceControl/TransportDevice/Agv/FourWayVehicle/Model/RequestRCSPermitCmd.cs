namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
   public class RequestRCSPermitCmd
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 入库请求通行站点1003  或 1015
        /// </summary>
        public string PER_SITE { get; set; }

    }
}
