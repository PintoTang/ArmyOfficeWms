using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class WcsOrderInfoForABB
    {
        //任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量

        ////虚拟托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量|机器人完成次数------------待删除
        ////B01|1|13|1|2|0|0	

        /// <summary>
        /// 1入库、2出库、3库存整理入库、4库存整理出库 5、NG清理
        /// </summary>
        public string taskType { get; set; }
        /// <summary>
        /// 虚拟托盘号
        /// </summary>
        public string simTrayNo { get; set; }
        /// <summary>
        /// 指令ID
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 起始目标位置
        /// </summary>
        public string startAndEndPostion { get; set; }
        /// <summary>
        /// 当前位置
        /// </summary>
        public string curPostion { get; set; }
        /// <summary>
        /// 发送抓取数量
        /// </summary>
        public int sendCatchNum { get; set; }
        /// <summary>
        /// 实际抓取数量
        /// </summary>
        public int realityCatchNum { get; set; }
       
    }
}
