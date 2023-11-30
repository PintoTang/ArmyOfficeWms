using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model
{
    public class SendModeSwitchCmd
    {
        public List<SiteMode> SiteModes { get; set; }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator SendModeSwitchCmd(string json)
        {
            return json.ToObject<SendModeSwitchCmd>();
        }
    }
    /// <summary>
    /// 站点模式信息
    /// </summary>
    public struct SiteMode
    {
        /// <summary>
        /// 站点
        /// </summary>
        public string SITE { get; set; }
        /// <summary>
        /// 出入库模式
        /// </summary>
        public int MODE { get; set; }
    }

}
