namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    /// <summary>
    /// 请求下发任务
    /// </summary>
    public class SendAddTaskCmd
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string taskNo { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string businessType { get; set; }
        /// <summary>
        /// 调用系统名称
        /// </summary>
        public string sysName { get; set; }
        /// <summary>
        /// 调用设备名称
        /// </summary>

        public string deviceName { get; set; }
        /// <summary>
        /// 任务起点
        /// </summary>
        public string locationFrom { get; set; }
        /// <summary>
        /// 任务终点
        /// </summary>
        public string locationTo { get; set; }
        /// <summary>
        /// 任务优先级
        /// </summary>
        public string priority { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
        public string extParam { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
        public string ext1 { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
        public string ext2 { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
        public string ext3 { get; set; }
    }
}
