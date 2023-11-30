using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    public enum LoggerType
    {
        GetDebugFile,
        GetErrorFile,
        GetEventFile,
        GetExceptionFile,
        GetMessageFile
    }
	/// <summary>
	/// 日志错误等级
	/// </summary>
	[Flags]
	public enum EnumLogLevel
	{
		/// <summary>
		/// 错误
		/// </summary>
        [Description("错误")]
		Error=0,
		/// <summary>
		/// 警告
		/// </summary>
        [Description("警告")]
        Warning=1,
		/// <summary>
		/// 信息
		/// </summary>
        [Description("提示")]
		Info=2,
		/// <summary>
		/// 调试
		/// </summary>
		[Description("调试")]
		Debug=3,
	}
}
