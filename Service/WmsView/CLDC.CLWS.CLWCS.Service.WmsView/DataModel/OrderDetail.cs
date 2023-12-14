using SqlSugar;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
    /// <summary>
    /// 任务单明细
    /// </summary>
    [SugarTable("t_OrderDetail")]
    public class OrderDetail
    {
        /// <summary>
        /// 入库单Id
        /// </summary>
        [SugarColumn(IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
		/// 
		/// </summary>
		[SugarColumn]
        public string OrderSN { get; set; }

        /// <summary>
        /// 设备码
        /// </summary>
        [SugarColumn]
        public string Barcode { get; set; }

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
        /// 物料基本单位
        /// </summary>
        [SugarColumn]
        public string UnitName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public string ShelfCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public string ShelfName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public bool? IsDeleted { get; set; }

    }
}
