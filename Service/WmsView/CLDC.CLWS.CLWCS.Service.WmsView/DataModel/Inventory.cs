using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
	/// <summary>
	/// 库存
	/// </summary>
	[SugarTable("t_Inventory")]
	public class Inventory
    {
        /// <summary>
        /// 库存Id
        /// </summary>
        [SugarColumn(IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 关联单据编号
        /// </summary>
        [SugarColumn]
		public string OrderSN { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? TaskType { get; set; }

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
		public int? Qty { get; set; }

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
        /// 装备条码
        /// </summary>
        [SugarColumn]
        public string Barcode { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [SugarColumn]
		public DateTime? ExpireDate { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn]
		public string Remark { get; set; }

        /// <summary>
        /// 事由
        /// </summary>
        [SugarColumn]
        public string Reason { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        [SugarColumn]
        public string ShelfCode { get; set; }

        /// <summary>
        /// 货架名称
        /// </summary>
        [SugarColumn]
        public string ShelfName { get; set; }


        /// <summary>
        /// 状态：0-其它;1-在库;2-出库;3-报损;
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
