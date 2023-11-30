using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class ReportExeOrderExceptionCmd
    {
        /// <summary>
        /// 指令编号
        /// </summary>
        public string order_no { get; set; }
        /// <summary>
        /// 工作区域编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 异常时间
        /// </summary>
        public string exception_time { get; set; }
        /// <summary>
        /// 异常代码
        /// </summary>
        public string exception_code { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string exception_message { get; set; }
        ///// <summary>
        ///// 指令是否终止
        ///// </summary>
        //public bool is_stop { get; set; }

    }
}
