using SqlSugar;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
    /// <summary>
    /// 入库单明细
    /// </summary>
    [SugarTable("t_InOrderDetail")]
    public class InOrderDetail
    {
        /// <summary>
        /// 入库单Id
        /// </summary>
        [SugarColumn(IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 入库单编号
        /// </summary>
        [SugarColumn]
        public string OrderSN { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        [SugarColumn]
        public string Barcode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn]
        public string Remark { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn]
        public bool? IsDeleted { get; set; }

    }
}
