using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// 设备模式枚举
    /// </summary>
	public enum DeviceModeEnum
	{
		[Description("默认")]
		Default = 0,

		[Description("入库")]
		In = 1,

		[Description("出库")]
		Out = 2,

		[Description("盘点")]
		Stock = 3,
        [Description("拣选")]
        Picking=4,
        [Description("可出可入")]
        InAndOut=5,
		[Description("下料完成模式")]
		DownMaterialFinish = 7,

		[Description("请求空托盘")]
		ReqEmptyTray = 10,
	}
}
