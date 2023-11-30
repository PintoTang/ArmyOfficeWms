using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    /// <summary>
    /// 指令优先级
    /// </summary>
    public class InstructPriorityCmd
    {
        /// <summary>
        /// 指令Id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 指令优先级
        /// </summary>
        public int PRI { get; set; }
    }
}
