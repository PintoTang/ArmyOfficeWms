using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class AgvCallModel
    {
        /// <summary>
        /// 任务类型：1-收货区-->理货区；
        /// </summary>
        public int TaskType
        {
            get; set;
        }

        /// <summary>
        /// 起始地址
        /// </summary>
        public string SrcAddr
        {
            get; set;
        }
    }
}
