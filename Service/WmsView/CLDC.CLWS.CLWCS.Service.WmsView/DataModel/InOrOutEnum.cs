using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.Service.WmsView.DataModel
{
    public enum InOrOutEnum
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        入库 = 1,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        出库 = 2,
        /// <summary>
        /// 报损
        /// </summary>
        [Description("报损")]
        报损 = 3,
    }
}
