using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
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
        /// 出入库类型：1-入库；2-出库；3-报损；
        /// </summary>
        [SugarColumn]
        public InOrOutEnum? InOutType { get; set; }

        /// <summary>
        /// 出库单编号
        /// </summary>
        [SugarColumn]
        public string OrderSN { get; set; }

        /// <summary>
        /// 出入库事由
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
        /// 状态：0-新建;1-已启动;2-作业中;3-已完成;
        /// </summary>
        [SugarColumn]
        public InvStatusEnum? Status { get; set; }

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
        /// 任务所属分队
        /// </summary>
        [SugarColumn]
        public string AreaTeam { get; set; }

        /// <summary>
        /// 物资单位
        /// </summary>
        [SugarColumn]
        public string UnitName { get; set; }

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
        /// 物资名称
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public string MaterialDesc { get; set; }
    }
}
