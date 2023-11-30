using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Target
{
    public enum WmsServiceForHeFeiEnum
    {
        [Description("包装条码-货物绑定-WMS进行校验")]
        NotifyPackageBarcodeCheck,
        [Description("码盘机码盘完成")]
        NotifyPalletizerFinish,
        [Description("上报指令完成")]
        NotifyInstructFinish,
        [Description("上报盘点结果")]
        NotifyStocktakingInstructResult,
        [Description("上报指令取消")]
        NotifyInstructCancel,
        [Description("上报指令异常")]
        NotifyInstructException,
        [Description("通知拆盘动作完成")]
        NotifyUnstackFinish,
        //[Description("wcs请求wms出")]
        //IsAllowOut,
        //[Description("wcs上报入库完成")]
        //IsFinished,
        //[Description("wcs请求wms入库")]
        //IsAllowIn,
        [Description("wcs请求wms空托盘出库")]
        ApplyEmptyTrayOut,
        [Description("wcs请求wms空托盘入库")]
        ApplyEmptyTrayIn,

        [Description("CS上报箱子放货完成 入库")]
        NotifyPackagePutFinish,

        [Description("CS上报箱子放货完成 出库")]
        NotifyPackagePutFinishForOut,

        [Description("设备故障上报")]
        ReportTroubleStatus,
        [Description("开关门上报")]
        OpenOrCloseDoor,
        [Description("扫描验证新接口")]
        NotifyPackageBarcodeCheckNew,
        [Description("放货完成新接口")]
        NotifyPackagePutFinishNew,
        [Description("卷帘门开关门")]
        NotifyCrossDoor,
        [Description("呼叫AGV")]
        NotifyAgvCarry,
        [Description("上报条码")]
        NotifyBarcodeReqNewInstruct,
        [Description("申请空容器")]
        NotifyApplyContainer,
        [Description("入库申请")]
        NotifyApplyIn,

        [Description("上报开门/关门完成")]
        NotifyDoorOpenOrCloseFinished,
        [Description("关键节点状态上报")]
        ReportNodeStatus,

    }
}
