using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
	/// <summary>
	/// 库区
	/// </summary>
	[SugarTable("t_Area")]
	public class Area
	{

		/// <summary>
		/// 库区编码
		/// </summary>
		[SugarColumn]
		public string Code { get; set; }

		/// <summary>
		/// 库区名称
		/// </summary>
		[SugarColumn]
		public string Name { get; set; }

		/// <summary>
		/// 库房Id
		/// </summary>
		[SugarColumn]
		public long? WarehouseId { get; set; }

		/// <summary>
		/// 仓库编码
		/// </summary>
		[SugarColumn]
		public string WarehouseCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public string WarehouseName { get; set; }

		/// <summary>
		/// 是否物理库区
		/// </summary>
		[SugarColumn]
		public bool? IsRealArea { get; set; }

		/// <summary>
		/// 状态：0-禁用；1-正常；
		/// </summary>
		[SugarColumn]
		public int? Status { get; set; }

		/// <summary>
		/// 库区类型：1-自动货架区
		/// </summary>
		[SugarColumn]
		public int? AreaType { get; set; }

		/// <summary>
		/// 对应ERP系统的库区编码
		/// </summary>
		[SugarColumn]
		public string UpperSysCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? ShelfCount { get; set; }

		/// <summary>
		/// 库区面积
		/// </summary>
		[SugarColumn]
		public decimal? BuiltArea { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn]
		public string Remark { get; set; }

		/// <summary>
		/// 排序号
		/// </summary>
		[SugarColumn]
		public int? DisplayOrder { get; set; }

		/// <summary>
		/// 创建者Id
		/// </summary>
		[SugarColumn]
		public long? CreatedUserId { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		[SugarColumn]
		public string CreatedUserName { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn]
		public DateTime? CreatedTime { get; set; }

		/// <summary>
		/// 修改者Id
		/// </summary>
		[SugarColumn]
		public long? ModifiedUserId { get; set; }

		/// <summary>
		/// 修改人
		/// </summary>
		[SugarColumn]
		public string ModifiedUserName { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		[SugarColumn]
		public DateTime? ModifiedTime { get; set; }

		/// <summary>
		/// 是否删除
		/// </summary>
		[SugarColumn]
		public bool? IsDeleted { get; set; }

		/// <summary>
		/// 租户Id
		/// </summary>
		[SugarColumn]
		public long? TenantId { get; set; }
	}
}
