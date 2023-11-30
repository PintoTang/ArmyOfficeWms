using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
	/// <summary>
	/// 
	/// </summary>
	[SugarTable("t_RobotReadyRecord", "")]
	public class RobotReadyRecord
	{

		/// <summary>
		/// Id主键
		/// </summary>
		[SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true, IsNullable = false, ColumnDescription = "Id主键")]
		public int Id { get; set; }

		/// <summary>
		/// 当前任务号
		/// </summary>
		[SugarColumn(ColumnName = "OrderNO", Length = 50, IsNullable = true, ColumnDescription = "当前任务号")]
		public string OrderNO { get; set; }

		/// <summary>
		/// 容器1条码
		/// </summary>
		[SugarColumn(ColumnName = "ContainerNO1", Length = 50, IsNullable = true, ColumnDescription = "容器1条码")]
		public string ContainerNO1 { get; set; }

		/// <summary>
		/// 地址1
		/// </summary>
		[SugarColumn(ColumnName = "Addr1", Length = 50, IsNullable = true, ColumnDescription = "地址1")]
		public string Addr1 { get; set; }

		/// <summary>
		/// 地址1是否就绪：0-未就绪；1-已就绪
		/// </summary>
		[SugarColumn(ColumnName = "Ready1", IsNullable = false, ColumnDescription = "地址1是否就绪：0-未就绪；1-已就绪")]
		public int? Ready1 { get; set; }

		/// <summary>
		/// 就绪时间
		/// </summary>
		[SugarColumn(ColumnName = "ReadyTime1", IsNullable = true, ColumnDescription = "就绪时间")]
		public DateTime? ReadyTime1 { get; set; }

		/// <summary>
		/// 地址2
		/// </summary>
		[SugarColumn(ColumnName = "Addr2", Length = 50, IsNullable = true, ColumnDescription = "地址2")]
		public string Addr2 { get; set; }

		/// <summary>
		/// 地址2是否就绪：0-未就绪；1-已就绪
		/// </summary>
		[SugarColumn(ColumnName = "Ready2", IsNullable = false, ColumnDescription = "地址2是否就绪：0-未就绪；1-已就绪")]
		public int? Ready2 { get; set; }

		/// <summary>
		/// 容器2条码
		/// </summary>
		[SugarColumn(ColumnName = "ContainerNO2", Length = 50, IsNullable = true, ColumnDescription = "容器2条码")]
		public string ContainerNO2 { get; set; }

		/// <summary>
		/// 就绪时间
		/// </summary>
		[SugarColumn(ColumnName = "ReadyTime2", IsNullable = true, ColumnDescription = "就绪时间")]
		public DateTime? ReadyTime2 { get; set; }

		/// <summary>
		/// 状态：0-未开始；1-执行中；2-已完成
		/// </summary>
		[SugarColumn(ColumnName = "Status",  IsNullable = false, ColumnDescription = "状态：0-未开始；1-执行中；2-已完成")]
		public int? Status { get; set; }

		/// <summary>
		/// 当前步骤
		/// </summary>
		[SugarColumn(ColumnName = "StepIndex", IsNullable = false, ColumnDescription = "当前步骤")]
		public int? StepIndex { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "CreateDate", IsNullable = false, ColumnDescription = "创建时间")]
		public DateTime? CreateDate { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn(ColumnName = "ModifyDate", IsNullable = false, ColumnDescription = "修改时间")]
		public DateTime? ModifyDate { get; set; }
	}
}

