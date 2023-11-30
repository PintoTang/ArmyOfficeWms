using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class ReportFaultInfoCmd
    {
        private string order_no { get; set; }
        /// <summary>
        /// 工作区域编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 故障代码
        /// </summary>
        public string fault_code { get; set; }
        /// <summary>
        /// 故障描述
        /// </summary>
        public string fault_desc { get; set; }
        /// <summary>
        /// 故障时间 （yyyy-MM-dd HH:mm:ss ）
        /// </summary>
        public string fault_time { get; set; }
        /// <summary>
        /// 故障附加信息
        /// </summary>
        public string fault_info { get; set; }
        public bool is_stop { get; set; }

    }
}
