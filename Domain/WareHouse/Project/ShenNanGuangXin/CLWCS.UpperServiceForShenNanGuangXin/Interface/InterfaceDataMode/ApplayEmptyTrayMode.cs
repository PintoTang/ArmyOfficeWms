using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    /// <summary>
    /// 空托盘申请
    /// </summary>
    public class ApplayEmptyTrayMode
    {
        /// <summary>
        /// 目标地址
        /// </summary>
        public string Addr { get; set; }
        /// <summary>
        /// 托盘编号
        /// </summary>
        public string PalletBarcode { get; set; }

    }
}
