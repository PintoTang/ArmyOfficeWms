using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
	/// <summary>
	/// 任务分类
	/// </summary>
	[SugarTable("t_Area")]
	public class Area
	{
        /// <summary>
        /// 任务分类Id
        /// </summary>
        [SugarColumn]
        public long Id { get; set; }

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
        /// 所在行
        /// </summary>
        [SugarColumn]
        public int? ROW { get; set; }

        /// <summary>
        /// 所在列
        /// </summary>
        [SugarColumn]
        public int? COLUMN { get; set; }

        /// <summary>
        /// 状态：0-禁用；1-启用；
        /// </summary>
        [SugarColumn]
        public int? Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public int? ShelfCount { get; set; }

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
