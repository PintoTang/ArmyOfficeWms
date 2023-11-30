using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
	/// <summary>
	/// 日志记录表，目前只记录与唯智交互输入和输出的数据日志
	/// </summary>
	[SugarTable("T_AC_LogRecord", "日志记录表，目前只记录与唯智交互输入和输出的数据日志")]
	public class LogRecord
	{
		public LogRecord()
        {
			LR_CreateDate = DateTime.Now;
        }
		/// <summary>
		/// 日志记录表GUID，系统默认生成GUID
		/// </summary>
		[SugarColumn(ColumnName = "LR_GUID", Length = 32, IsPrimaryKey = true, DefaultValue = "NEWID()", IsNullable = false, ColumnDescription = "日志记录表GUID，系统默认生成GUID")]
		public string LR_GUID { get; set; }

		/// <summary>
		/// 仓库代码
		/// </summary>
		[SugarColumn(ColumnName = "WH_Code", Length = 20, IsNullable = false, ColumnDescription = "仓库代码")]
		public string WH_Code { get; set; }

		/// <summary>
		/// 该日志触发的系统，如输入参数是WCS系统提交的参数，输出参数是WMS返回的参数值。1：WCS；2：WMS
		/// </summary>
		[SugarColumn(ColumnName = "LR_TriggerSystem", IsNullable = false, ColumnDescription = "该日志触发的系统，如输入参数是WCS系统提交的参数，输出参数是WMS返回的参数值。1：WCS；2：WMS")]
		public int? LR_TriggerSystem { get; set; }

		/// <summary>
		/// 方法名称，数据来源是该方法被调用
		/// </summary>
		[SugarColumn(ColumnName = "LR_MethodName", Length = 128, IsNullable = false, ColumnDescription = "方法名称，数据来源是该方法被调用")]
		public string LR_MethodName { get; set; }

		/// <summary>
		/// 输入或输出的参数的值，格式为Json的字符，当函数无返回值是为空
		/// </summary>
		[SugarColumn(ColumnName = "LR_ParamValue", Length = 8000, IsNullable = true, ColumnDescription = "输入或输出的参数的值，格式为Json的字符，当函数无返回值是为空")]
		public string LR_ParamValue { get; set; }

		/// <summary>
		/// 数据接收时间，默认为系统时间，精确到3位毫秒
		/// </summary>
		[SugarColumn(ColumnName = "LR_CreateDate", DefaultValue = "getdate()", IsNullable = false, ColumnDescription = "数据接收时间，默认为系统时间，精确到3位毫秒")]
		public DateTime? LR_CreateDate { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn(ColumnName = "LR_Note", Length = 8000, IsNullable = true, ColumnDescription = "备注")]
		public string LR_Note { get; set; }
	}
}
