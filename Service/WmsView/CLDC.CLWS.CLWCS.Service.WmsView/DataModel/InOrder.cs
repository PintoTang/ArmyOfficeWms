using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
	/// <summary>
	/// 入库单
	/// </summary>
	[SugarTable("t_InOrder")]
	public class InOrder
	{

		/// <summary>
		/// 入库单编号
		/// </summary>
		[SugarColumn]
		public string OrderSN { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? TaskType { get; set; }

		/// <summary>
		/// 库房编码
		/// </summary>
		[SugarColumn]
		public string WarehouseCode { get; set; }

		/// <summary>
		/// 库房名称
		/// </summary>
		[SugarColumn]
		public string WarehouseName { get; set; }

		/// <summary>
		/// 物料Id
		/// </summary>
		[SugarColumn]
		public long? MaterialId { get; set; }

		/// <summary>
		/// 物料号
		/// </summary>
		[SugarColumn]
		public string MaterialCode { get; set; }

		/// <summary>
		/// 物料名称
		/// </summary>
		[SugarColumn]
		public string MaterialDesc { get; set; }

		/// <summary>
		/// 物料类别：1-容器；2-成品；3-半成品；4-原材料
		/// </summary>
		[SugarColumn]
		public int? MaterialCategory { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public decimal? Qty { get; set; }

		/// <summary>
		/// 物料基本单位Id
		/// </summary>
		[SugarColumn]
		public long? UnitId { get; set; }

		/// <summary>
		/// 物料基本单位
		/// </summary>
		[SugarColumn]
		public string UnitName { get; set; }

		/// <summary>
		/// 批次号
		/// </summary>
		[SugarColumn]
		public string BatchNo { get; set; }

		/// <summary>
		/// 生产日期
		/// </summary>
		[SugarColumn]
		public DateTime? ProductDate { get; set; }

		/// <summary>
		/// 有效期
		/// </summary>
		[SugarColumn]
		public DateTime? ExpireDate { get; set; }

		/// <summary>
		/// 生产厂家
		/// </summary>
		[SugarColumn]
		public string ProduceFactory { get; set; }

		/// <summary>
		/// 最早入库时间
		/// </summary>
		[SugarColumn]
		public DateTime? FirstInStoreDate { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn]
		public string Remark { get; set; }

		/// <summary>
		/// 状态：0-新建;1-已启动;2-作业中;3-已完成;
		/// </summary>
		[SugarColumn]
		public int? Status { get; set; }

		/// <summary>
		/// 存储区域编码
		/// </summary>
		[SugarColumn]
		public string AreaCode { get; set; }

		/// <summary>
		/// 存储区域名称
		/// </summary>
		[SugarColumn]
		public string AreaName { get; set; }

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
