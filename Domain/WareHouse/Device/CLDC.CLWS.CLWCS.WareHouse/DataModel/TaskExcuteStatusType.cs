using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    public enum TaskExcuteStatusType
    {
		/// <summary>
		/// 出库取空
		/// </summary>
		[Description("出库取空")]
		OutButEmpty=1,
		/// <summary>
		/// 出库异常,如：出深浅有异常
		/// </summary>
		[Description("出深浅有")]
		OutDepthButShallowExist=2,
		/// <summary>
		/// 入深浅有异常
		/// </summary>
		[Description("入深浅有")]
		InDepthButShallowExist=3,
		/// <summary>
		/// 双重入库异常
		/// </summary>
		[Description("双重入库")]
		InButExist=4,
		/// <summary>
		/// 正常--无异常
		/// </summary>
		[Description("无异常")]
		NoException=5,
		/// <summary>
		/// 设备异常
		/// </summary>
		[Description("设备异常")]
		DeviceException=6,
		/// <summary>
		/// 入库异常（无法区分入深浅有，双重入库异常）
		/// </summary>
		[Description("入库异常")]
		InException=7,
		/// <summary>
		/// 出库异常（无法区分出深浅有）
		/// </summary>
		[Description("出库异常")]
		OutException=8,
		/// <summary>
		/// 不可重置异常，需人工干预
		/// </summary>
		[Description("不可重置异常")]
		UnresetException=9,
		/// <summary>
		/// 未知异常
		/// </summary>
		[Description("未知异常")]
		UnknowException=10
	}
}
