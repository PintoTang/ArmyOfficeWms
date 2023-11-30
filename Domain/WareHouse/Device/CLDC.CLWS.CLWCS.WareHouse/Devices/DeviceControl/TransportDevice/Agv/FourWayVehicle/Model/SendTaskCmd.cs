using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
   public class SendTaskCmd
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public int TASK_TYPE { get; set; }
        /// <summary>
        /// 指令优先级
        /// </summary>
        public int PRI { get; set; }
        /// <summary>
        /// 包装条码
        /// </summary>
        /// 
        public string PACKAGE_BARCODE { get; set; }
        /// <summary>
        /// 起始地址
        /// </summary>
        /// 
        public string START_ADDR { get; set; }
        /// <summary>
        /// 目的地址
        /// </summary>
        public string END_ADDR { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string EXT1 { get; set; }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string EXT2 { get; set; }
    }
    public enum RequestTypeEnum
    {
        [Description("不请求")]
        UnRequest = 0,
        [Description("请求")]
        Request = 1
    }


}
