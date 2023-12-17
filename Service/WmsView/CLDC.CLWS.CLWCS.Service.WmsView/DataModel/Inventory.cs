using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
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
        /// 出入库事由
        /// </summary>
        [SugarColumn]
        public string Reason { get; set; }

        /// <summary>
        /// 物资编码
        /// </summary>
        [SugarColumn]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物资名称
        /// </summary>
        [SugarColumn]
        public string MaterialDesc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public int? Qty { get; set; }

        /// <summary>
        /// 设备码
        /// </summary>
        [SugarColumn]
        public string Barcode { get; set; }

        /// <summary>
        /// 物资单位
        /// </summary>
        [SugarColumn]
        public string UnitName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn]
        public string Remark { get; set; }

        /// <summary>
        /// 状态：1-在库;2-出库;3-报损;
        /// </summary>
        [SugarColumn]
        public InvStatusEnum? Status { get; set; }

        /// <summary>
        /// 任务分类编码
        /// </summary>
        [SugarColumn]
        public string AreaCode { get; set; }

        /// <summary>
        /// 任务分类名称
        /// </summary>
        [SugarColumn]
        public string AreaName { get; set; }

        /// <summary>
        /// 任务所属分队
        /// </summary>
        [SugarColumn]
        public string AreaTeam { get; set; }        

        /// <summary>
        /// 货架编号
        /// </summary>
        [SugarColumn]
        public string ShelfCode { get; set; }

        /// <summary>
        /// 货架名称
        /// </summary>
        [SugarColumn]
        public string ShelfName { get; set; }

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
