using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.Model
{
    public sealed class DePalletizeCmd
    {
        /// <summary>
        /// 上层服务的任务号
        /// </summary>
        public string UpperTaskCode { get; set; }
        /// <summary>
        /// 设备的任务号
        /// </summary>
        public string DeviceTaskCode { get; set; }

        /// <summary>
        /// 拆盘的容器类型
        /// </summary>
        public int ContainerType { get; set; }

        /// <summary>
        /// 当前跺的数量
        /// </summary>
        public int PileNum { get; set; }

        /// <summary>
        /// 需要拆的数量
        /// </summary>
        public int DePalletizeCount { get; set; }

        public override string ToString()
        {
           return  this.ToJson();
        }
    }
}
