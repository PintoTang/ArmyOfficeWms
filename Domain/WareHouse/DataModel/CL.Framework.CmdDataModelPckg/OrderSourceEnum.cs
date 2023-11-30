using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.CmdDataModelPckg
{
    /// <summary>
    /// 任务来源 枚举
    /// </summary>
    public enum TaskSourceEnum
    {
        /// <summary>
        /// 上层系统
        /// </summary>
        [Description("上层系统")]
        UPPER,
        /// <summary>
        /// WCS
        /// </summary>
        [Description("WCS")]
        SELF,
        /// <summary>
        /// 人工
        /// </summary>
        [Description("人工")]
        MANNUAL
    }
}
