using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 任务执行步骤状态 枚举
    /// </summary>
    public enum TaskExcuteStepStatusEnum
    {
        [Description("已接收")]
        Received = 0,
        [Description("处理中")]
        Processing = 1,
        [Description("取货中")]
        Picking = 2,
        [Description("运输中")]
        Transport = 3,
        [Description("放货中")]
        Putting = 4,
        [Description("已完成")]
        Finished = 5,
        [Description("异常")]
        Exception = 6
    }
}
