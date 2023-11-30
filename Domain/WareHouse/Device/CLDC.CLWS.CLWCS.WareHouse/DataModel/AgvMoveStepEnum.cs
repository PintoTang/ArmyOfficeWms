using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    /// <summary>
    /// Agv动作步骤枚举
    /// </summary>
    public enum AgvMoveStepEnum
    {
        [Description("空闲")]
        Free=0,
        [Description("开始")]
        MoveStart=1,
        [Description("完成")]
        Finish=2,
        [Description("开始取货")]
        PickStart=3,
        [Description("取货完成")]
        PickFinish=4,
        [Description("开始放货")]
        PutStart=5,
        [Description("放货完成")]
        PutFinish=6,
        [Description("取消")]
        Cancel=7,
        [Description("空叉取消")]
        EmptyForkFinish=8

    }
}
