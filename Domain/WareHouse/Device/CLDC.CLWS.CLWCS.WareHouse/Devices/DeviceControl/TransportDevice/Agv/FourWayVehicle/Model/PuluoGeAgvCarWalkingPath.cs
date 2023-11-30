using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    /// <summary>
    /// 普罗格agv行走路径
    /// </summary>
    public class PuluoGeAgvCarWalkingPath
    {
        /// <summary>
        /// WCS地址
        /// </summary>
        public string Addr { get; set; }
        /// <summary>
        /// x轴坐标
        /// </summary>
        public string Position_X { get; set; }
        /// <summary>
        /// y轴坐标
        /// </summary>
        public string Position_Y { get; set; }
        /// <summary>
        /// Z层
        /// </summary>
        public string Position_Z { get; set; }

        /// <summary>
        /// RCS位置编号 （Z,X,Y）十位数坐标
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
}
