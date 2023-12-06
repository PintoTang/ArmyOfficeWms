using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.Service.WmsView.DataModel
{
    public enum ReasonEnum
    {
        /// <summary>
        /// 新购物资入库
        /// </summary>
        [Description("新购物资入库")]
        新购物资入库 = 1,

        /// <summary>
        /// 演练后归还入库
        /// </summary>
        [Description("演练后归还入库")]
        演练后归还入库 = 2,

        /// <summary>
        /// 演练出库
        /// </summary>
        [Description("演练出库")]
        演练出库 = 3,
    }
}
