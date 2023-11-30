using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.CmdDataModelPckg
{
    /// <summary>
    /// 上层系统的任务类型
    /// </summary>
    public enum SourceTaskEnum
    {
        /// <summary>
        /// 入库
        /// </summary>
        [Description("入库")]
        In = 0,
        /// <summary>
        /// 出库
        /// </summary>
        [Description("出库")]
        Out = 1,
        /// <summary>
        /// 移库
        /// </summary>
        [Description("移库")]
        Move = 2,
        /// <summary>
        /// 盘点
        /// </summary>
        [Description("盘点入库")]
        InventoryIn = 3,
        /// <summary>
        /// 盘点
        /// </summary>
        [Description("盘点出库")]
        InventoryOut = 4,
        /// <summary>
        /// 拣选
        /// </summary>
        [Description("拣选入库")]
        PickIn = 5,
        /// <summary>
        /// 拣选
        /// </summary>
        [Description("拣选出库")]
        PickOut = 6,
        /// <summary>
        /// 手动上架
        /// </summary>
        [Description("手动上架")]
        HandUpLoad = 7,
        /// <summary>
        /// 手动下架
        /// </summary>
        [Description("手动下架")]
        HandDownLoad = 8,
        /// <summary>
        /// 未知任务类型
        /// </summary>
        [Description("未知任务类型")]
        UnKnow,

        [Description("缓存位")]
        ToCacheCell=10,

        /// <summary>
        /// 品质确认入库
        /// </summary>
        [Description("品质确认入库")]
        QualityConfirmIn = 11,

        /// <summary>
        /// 品质确认出库
        /// </summary>
        [Description("品质确认出库")]
        QualityConfirmOut = 12,

        /// <summary>
        /// 人工空托盘出库
        /// </summary>
        [Description("人工空托盘出库")]
        ManualEmptyTrayOut = 13,


        /// <summary>
        /// 人工出库
        /// </summary>
        [Description("人工出库")]
        ManualOut = 14,

        /// <summary>
        /// 退货出库
        /// </summary>
        [Description("退货出库")]
        ReturnGoods = 15,


        /// <summary>
        /// 报废出库
        /// </summary>
        [Description("报废出库")]
        ScrapOut = 16,


        /// <summary>
        /// 库存整理入库
        /// </summary>
        [Description("库存整理入库")]
        InventoryArrangeIn = 17,

        /// <summary>
        /// 库存整理出库
        /// </summary>
        [Description("库存整理出库")]
        InventoryArrangeOut = 18,
    }
}
