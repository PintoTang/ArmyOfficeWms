using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
	/// <summary>
	/// 货架
	/// </summary>
	[SugarTable("t_Shelf")]
	public class Shelf
	{

		/// <summary>
		/// 货架编码
		/// </summary>
		[SugarColumn]
		public string Code { get; set; }

		/// <summary>
		/// 货架名称
		/// </summary>
		[SugarColumn]
		public string Name { get; set; }

		/// <summary>
		/// 货架号
		/// </summary>
		[SugarColumn]
		public int? Number { get; set; }

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
		/// 库区Id
		/// </summary>
		[SugarColumn]
		public long? AreaId { get; set; }

		/// <summary>
		/// 库区编码
		/// </summary>
		[SugarColumn]
		public string AreaCode { get; set; }

		/// <summary>
		/// 所属库区
		/// </summary>
		[SugarColumn]
		public string AreaName { get; set; }

		/// <summary>
		/// 状态：0-禁用；1-正常
		/// </summary>
		[SugarColumn]
		public int? Status { get; set; }

		/// <summary>
		/// 排序号
		/// </summary>
		[SugarColumn]
		public int? DisplayOrder { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public string Remark { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? CellCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? DisabledCellCount { get; set; }

		/// <summary>
		/// 深度，1：单深位，2：双深位，3：三深位，4：四深位，5：五深位
		/// </summary>
		[SugarColumn]
		public int? Depth { get; set; }

		/// <summary>
		/// 货架类型：1-普通货架
		/// </summary>
		[SugarColumn]
		public int? ShelfType { get; set; }

		/// <summary>
		/// 巷道Id
		/// </summary>
		[SugarColumn]
		public long? RoutewayId { get; set; }

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
