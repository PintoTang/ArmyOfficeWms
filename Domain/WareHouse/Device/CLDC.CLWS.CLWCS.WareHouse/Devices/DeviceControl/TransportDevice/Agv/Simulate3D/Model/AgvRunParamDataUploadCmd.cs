using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model
{
    public class AgvRunParamDataUploadCmd
    {
        /// <summary>
        /// agv编号
        /// </summary>
        public string AGVCODE { get; set; }
        /// <summary>
        /// 任务数量
        /// </summary>
        public int TASK_COUNT { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public string BATTERY { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string SPEED { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public string POSITION_X { get; set; }
        /// <summary>
        ///  Y坐标
        /// </summary>
        public string POSITION_Y { get; set; }
        /// <summary>
        ///  Y坐标
        /// </summary>
        public string POSITION_Z { get; set; }
    }
}
