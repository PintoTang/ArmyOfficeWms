using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class AgvCarryFinishedModel
    {
        /// <summary>
		/// 当前任务号
		/// </summary>
		public string OrderNO
        {
            get;
            set;
        }

        /// <summary>
        /// 容器条码
        /// </summary>
        public string ContainerNO
        {
            get;
            set;
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get;
            set;
        }

        /// <summary>
        /// 地址1是否就绪：0-未就绪；1-已就绪
        /// </summary>
        public int? Ready
        {
            get;
            set;
        }

        /// <summary>
        /// 就绪时间
        /// </summary>
        public DateTime? ReadyTime
        {
            get;
            set;
        }
    }
}
