using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class NoticeRobotStatusCmd
    {
        /// <summary>
        /// //工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// //系统状态  工作状态
        /// </summary>
        public string robot_status { get; set; }
        /// <summary>
        /// /机器人状态说明
        /// </summary>
        public string robot_message { get; set; }
    }
}
