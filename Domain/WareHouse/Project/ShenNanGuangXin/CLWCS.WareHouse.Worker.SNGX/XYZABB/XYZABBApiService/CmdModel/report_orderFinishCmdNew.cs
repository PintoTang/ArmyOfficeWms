using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    /// <summary>
    /// 机器人上报完成指令
    /// </summary>
    public class report_orderFinishCmdNew
    {
        public report_orderFinishCmdNew()
        {
            this.customized_info = new List<customized_infoNew>();
        }
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 区域编号 01入库 02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 是否已经抓空
        /// </summary>
        public bool start_isempty { get; set; }
        /// <summary>
        /// 终点是否已码满
        /// </summary>
        public bool dest_isfull { get; set; }

        public List<customized_infoNew> customized_info { get; set; }
    }

    public class customized_infoNew
    {
        /// <summary>
        /// 序号 左边1，右边2
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 箱子条码
        /// </summary>
        public string box_barcode { get; set; }
        /// <summary>
        /// 起始位置
        /// </summary>
        public int start_position { get; set; }
        /// <summary>
        /// 放置位置
        /// </summary>
        public int dest_position { get; set; }

    }

}
