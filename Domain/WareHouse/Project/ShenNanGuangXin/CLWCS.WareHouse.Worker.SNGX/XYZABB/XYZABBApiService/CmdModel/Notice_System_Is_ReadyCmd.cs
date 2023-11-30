using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    /// <summary>
    /// 1、告知机器人状态是否OK接口 参数
    /// </summary>
    public class Notice_System_Is_ReadyCmd
    {
        /// <summary>
        /// //工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// //系统状态，0表示ok，-1表示未准备好
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// /补充系统状态信息
        /// </summary>
        public string message { get; set; }
      
    }
}
