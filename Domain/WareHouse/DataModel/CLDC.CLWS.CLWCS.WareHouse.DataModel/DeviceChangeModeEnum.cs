using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 设备属性变化的枚举
    /// </summary>
    public enum DeviceChangeModeEnum
    {
        /// <summary>
        /// 指令完成
        /// </summary>
        OrderFinish,
        /// <summary>
        /// 连接状态
        /// </summary>
        OnlineStatus,
        /// <summary>
        /// 异常状态
        /// </summary>
        ExcpetionStatus,
        /// <summary>
        /// 动作步骤
        /// </summary>
        MoveStep,

    }
}
