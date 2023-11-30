using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 任务执行结果 枚举
    /// </summary>
   public enum TaskHandleResultEnum
    {
        /// <summary>
        /// 自动完成
        /// </summary>
        [Description("自动完成")]
        Finish,
        /// <summary>
        /// 丢弃
        /// </summary>
        [Description("丢弃")]
        Discard,
        /// <summary>
        /// 强制完成
        /// </summary>
        [Description("强制完成")]
        ForceFinish,
        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancle,
        /// <summary>
        /// 更新
        /// </summary>
        [Description("更新")]
        Update
    }
}
