namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
   public class ReportRCSPermitFinishCmd
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 通行完成 1005  或 1014 
        /// </summary>
        public string PER_SITE { get; set; }

    }
}
