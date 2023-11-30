using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    /// <summary>
    /// 货物的外观属性
    /// </summary>
    public struct SizeProperties
    {
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public double Lenght { get; set; }
    }
}
