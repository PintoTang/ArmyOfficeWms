using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 上层系统 枚举
    /// </summary>
    public enum UpperSystemEnum
    {

        [Description("仓储管理系统")]
        Wms = 0,
        [Description("物联网管理系统")]
        IoT = 1,
        [Description("3D模拟仿真系统")]
        Simulate3D = 2
    }
}
