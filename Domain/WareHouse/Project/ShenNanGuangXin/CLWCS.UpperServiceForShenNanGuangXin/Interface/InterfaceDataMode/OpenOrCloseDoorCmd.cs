using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class OpenOrCloseDoorCmd
    {
        /// <summary>
        /// 位置地址 
        /// </summary>
        public int PostionNum { get; set; }
        /// <summary>
        ///  动作  1开、2关
        /// </summary>
        public int ActionNum { get; set; }

    }
}
