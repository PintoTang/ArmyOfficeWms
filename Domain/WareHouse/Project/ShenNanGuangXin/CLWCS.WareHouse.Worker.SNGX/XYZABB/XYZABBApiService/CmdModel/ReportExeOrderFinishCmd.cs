using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class ReportExeOrderFinishCmd
    {
       public ReportExeOrderFinishCmd()
        {
            this.customized_info = new List<customized_infos>();
        }
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 工作区域编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 起点是否已抓空
        /// </summary>
        public bool start_isempty { get; set; }
        /// <summary>
        /// 终点是否已码满
        /// </summary>
        public bool dest_isfull { get; set; }
        public List<customized_infos> customized_info { get; set; }

    }
    public class customized_infos
    {
        /// <summary>
        /// 序号,左边1，右边2
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 箱条码
        /// </summary>
        public string box_barcode { get; set; }
        /// <summary>
        /// 抓取位置号—只需要出库/库存整理上报上来
        /// </summary>
        public int start_position { get; set; }
        /// <summary>
        /// 放置位置号
        /// </summary>
        public int dest_position { get; set; }

    }
}
