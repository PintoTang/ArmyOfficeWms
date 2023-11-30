using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class ApplyDoorOpenOrCloseModel
    {
        /// <summary>
        /// 当前地址
        /// </summary>
        public string CurrAddr
        {
            get; set;
        }

        /// <summary>
        /// true:开门;false:关门
        /// </summary>
        public bool IsOpen
        {
            get; set;
        }

        /// <summary>
        /// 是否进门:true-进门；false-出门
        /// </summary>
        public bool IsIn
        {
            get; set;
        }
    }
}
