using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class CmdNodeStatusModel
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string CurrAddr { get; set; }

        /// <summary>
        /// 是否报警
        /// </summary>
        public bool IsAlarm { get; set; }
    }
}
