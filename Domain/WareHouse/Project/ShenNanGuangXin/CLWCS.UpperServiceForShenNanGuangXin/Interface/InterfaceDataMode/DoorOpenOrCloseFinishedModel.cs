using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class DoorOpenOrCloseFinishedModel
    {
        /// <summary>
        /// true-开门完成；false-关门完成
        /// </summary>
        public bool IsOpen
        {
            get; set;
        }

        /// <summary>
        /// true-进门；false-出门
        /// </summary>
        public bool IsIn
        {
            get; set;
        }
    }
}
