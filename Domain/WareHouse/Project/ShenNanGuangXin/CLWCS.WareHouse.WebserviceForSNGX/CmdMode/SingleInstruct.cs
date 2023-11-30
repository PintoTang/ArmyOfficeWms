using System.Collections.Generic;
using System.ComponentModel;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{

    public class SendInstructCmd
    {
        public List<SingleInstruct> DATA { get; set; }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator SendInstructCmd(string json)
        {
            return json.ToObject<SendInstructCmd>();
        }
    }

    public struct SingleInstruct
    {
        public int ID { get; set; }
        public string PACKAGE_BARCODE { get; set; }
        public string PRI { get; set; }

        public InstructTypeEnum TYPE { get; set; }

        public string SRC_ADDR { get; set; }

        public string DST_ADDR { get; set; }

        public int PackageQTY { get; set; }
        public bool IsEmptyTray { get; set; }

        public int TrayType { get; set; }
    }


    public enum InstructTypeEnum
    {
        [Description("入库指令")]
        InStorehouse = 0,
        [Description("出库指令")]
        OutStorehouse = 1,
        [Description("移库指令")]
        MoveStore = 2,
        [Description("盘点入库")]
        InventoryStore = 3,
        [Description("盘点出库")]
        InventoryOut=4,
        [Description("拣选入库")]
        PickIn=5,
        [Description("拣选出库")]
        PickOut=6,
        [Description("手动上架")]
        ManualPutaway=7,
        [Description("手动下架")]
        ManualDown=8,
        [Description("盘点异常")]
        Except=9,
        [Description("临时移位")]
        TempMove = 10,
        [Description("未知指令")]
        UnKnow,

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
