using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 任务状态内容
    /// </summary>
    public class TaskStatusContent
    {
        /// <summary>
        /// 执行状态
        /// </summary>
        public TaskExcuteStepStatusEnum ExcuteStatus { get; set; }

        /// <summary>
        /// 执行代码
        /// </summary>
        public int TaskExcuteCode { get; set; }

        /// <summary>
        /// 任务执行信息
        /// </summary>
        public string TaskExcuteMessage { get; set; }

    }
}
