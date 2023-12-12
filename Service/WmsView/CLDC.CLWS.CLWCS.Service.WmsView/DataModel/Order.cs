using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
    /// <summary>
    /// 任务单
    /// </summary>
    [SugarTable("t_Order")]
	public class Order
	{
        /// <summary>
        /// 入库单Id
        /// </summary>
        [SugarColumn]
        public long Id { get; set; }

        /// <summary>
        /// 出入库类型：1-入库；2-出库；
        /// </summary>
        [SugarColumn]
        public InOrOutEnum? InOutType { get; set; }

        /// <summary>
        /// 出库单编号
        /// </summary>
        [SugarColumn]
        public string OrderSN { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public int? TaskType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public string Reason { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public int? Qty { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn]
        public string Remark { get; set; }

        /// <summary>
        /// 单据状态
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


        /// <summary>
        /// 装备
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string MaterialDesc { get; set; }
    }
}
