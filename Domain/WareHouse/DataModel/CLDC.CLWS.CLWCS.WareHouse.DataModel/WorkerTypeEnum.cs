using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 工作类型
    /// </summary>
    public enum WorkerTypeEnum
    {
        /// <summary>
        /// 指令处理工作者
        /// </summary>
        [Description("指令分配")]
        OrderWorker,
        /// <summary>
        /// 信息验证工作者
        /// </summary>
        [Description("信息验证")]
        IdentityWorker,
        /// <summary>
        /// 拣选工作者
        /// </summary>
        [Description("拣选分配")]
        PickingWorker,
        /// <summary>
        /// 按钮工作者
        /// </summary>
        [Description("按钮分配")]
        SwitchingWorker,
        /// <summary>
        /// 拆码工作者
        /// </summary>
        [Description("拆码分配")]
        PalletierWorker,
        /// <summary>
        /// 出入库工作者
        /// </summary>
        [Description("出入库分配")]
        InAndOutWorker,

        /// <summary>
        /// 调度工作者
        /// </summary>
        [Description("调度分配")]
        DispatchWorker,

        /// <summary>
        /// 服务工作者
        /// </summary>
        [Description("服务调度")]
        ServiceWorker

    }
}
