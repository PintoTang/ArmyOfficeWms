using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class report_orderFinishCmd
    {
        public report_orderFinishCmd()
        {
            this.customized_info = new List<customized_info>();
        }

        public string order_no { get; set; }
        public string area_code { get; set; }
        /// <summary>
        /// 起始容器
        /// </summary>
        public string src_cont_barcode { get; set; }
        /// <summary>
        /// 目标容器
        /// </summary>
        public string dest_cont_barcode { get; set; }
        public List<customized_info> customized_info { get; set; }
    }

    public class customized_info
    {
        /// <summary>
        /// 箱子条码
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// 起始位置
        /// </summary>
        public int src_position { get; set; }
        /// <summary>
        /// 目标位置
        /// </summary>
        public int dest_position { get; set; }
        
    }

}
