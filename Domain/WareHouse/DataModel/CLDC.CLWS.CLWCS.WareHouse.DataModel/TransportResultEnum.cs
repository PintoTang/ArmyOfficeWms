using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    public enum TransportResultEnum
    {
        /// <summary>
        /// <para>返回“执行成功”：会将“下一步地址”赋值给指令中的“当前地址”并抛回给分发器。</para>
        /// <para>使用场景：指令正常执行完成时使用。</para>
        /// </summary>
        [Description("搬运完成已上报")]
        Success = 0,
        /// <summary>
        /// <para>返回“丢弃指令”：会导致当前指令被丢弃。</para>
        /// <para>使用场景：判定指令为无效指令时使用。</para>
        /// </summary>
        [Description("丢弃指令")]
        Discard = 1,
        /// <summary>
        /// <para></para>
        /// </summary>
        [Description("查找可执行命令")]
        Find = 2,
        /// <summary>
        /// <para>返回“设备故障”：会在分发器里解注册该设备，并将所有未完成的指令抛回给分发器，最后杀死执行线程。</para>
        /// <para>使用场景：执行指令的过程中，确定设备故障时使用。</para>
        /// </summary>
        [Description("设备故障")]
        Fault = 3,
        /// <summary>
        /// 指定搬运设备
        /// </summary>
        [Description("指定设备")]
        AppointId = 4,
        /// <summary>
        /// 搬运设备搬运等待
        /// </summary>
        [Description("等待执行")]
        Wait = 5,
        /// <summary>
        /// 指令重置
        /// </summary>
        [Description("指令重置")]
        Reset = 6,
        /// <summary>
        /// 等待完成中
        /// </summary>
        [Description("等待设备搬运完成")]
        UnFinish = 7,
        /// <summary>
        /// 搬运完成业务未完成
        /// </summary>
        [Description("搬运完成未上报")]
        Transported = 8,
        /// <summary>
        /// 业务处理失败
        /// </summary>
        [Description("业务处理失败")]
        BusinessFailed = 9,
        /// <summary>
        /// 通知上层系统业务失败
        /// </summary>
        [Description("通知上层失败")]
        NotifyUpFailed = 10
    }
}

