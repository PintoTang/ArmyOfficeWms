using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class ReportRobotWorkInfoCmd
    {
        /// <summary>
        /// 工作区域编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 开始地址
        /// </summary>
        public string start_addr { get; set; }

        /// <summary>
        /// 目标地址
        /// </summary>
        public string dest_addr { get; set; }
        /// <summary>
        /// 工作状态标识
        /// </summary>
        public string work_flag { get; set; }
        /// <summary>
        /// 工作状态描述
        /// </summary>
        public string work_status_desc { get; set; }
    }
}
