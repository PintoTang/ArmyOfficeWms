using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.MNLMCha.Model
{
   
    /// <summary>
    /// 木牛流马agv行走路径
    /// </summary>
    public class MNLMChaAgvCarWalkingPath
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr{get;set;}
        /// <summary>
        /// 位置编号
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
}
