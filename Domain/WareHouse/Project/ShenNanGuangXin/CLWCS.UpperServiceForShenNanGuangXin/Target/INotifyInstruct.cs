using System.ComponentModel;

namespace CLWCS.UpperServiceForHeFei.Target
{
	/// <summary>
	/// WCS对接上层系统指令接口服务
	/// </summary>
	public interface INotifyInstruct
	{
		/// <summary>
		///  WCS通知上层系统指令搬运完成
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		[Description("指令完成")]
		string NotifyInstructFinish(string cmd);

		/// <summary>
		/// WCS 通知上层系统指令取消
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		[Description("指令取消")]
        string NotifyInstructCancel(string cmd);
        /// <summary>
        /// 上报指令异常
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
		[Description("指令异常")]
		string NotifyInstructException(string cmd);


        ///// <summary>
        ///// WCS请求出
        ///// </summary>
        ///// <param name="cmd"></param>
        ///// <returns></returns>
        //[Description("wcs请求出")]
        //string IsAllowOut(string cmd);
        ///// <summary>
        ///// wcs上报入完成
        ///// </summary>
        ///// <param name="cmd"></param>
        ///// <returns></returns>
        //[Description("wcs上报入完成")]
        //string IsFinished(string cmd);
        ///// <summary>
        ///// WCS请求入
        ///// </summary>
        ///// <param name="cmd"></param>
        ///// <returns></returns>
        //[Description("wcs请求入")]
        //string IsAllowIn(string cmd);
         /// <summary>
        /// WCS请求WMS1008出空托盘申请
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Description("请求WMS1008出空托盘申请")]
        string ApplyEmptyTrayOut(string cmd);

          /// <summary>
        /// WCS请求WMS空托盘入库申请
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [Description("WCS请求WMS空托盘入库申请")]
        string ApplyEmptyTrayIn(string cmd);

        [Description("WCS通知WMS出库完成")]
        string NotifyOutFinish(string cmd);
        


        /// <summary>
        /// CS上报包装条码给wms，wms进行验证
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [Description("CS上报包装条码给wms，wms进行验证")]
        string NotifyPackageBarcodeCheck(string cmd);


        /// <summary>
        /// CS上报箱子放货完成(单次完成) 入库
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [Description("CS上报箱子放货完成(单次完成) 入库")]
        string NotifyPackagePutFinish(string cmd);

        /// <summary>
        /// CS上报箱子放货完成(单次完成) 出库
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [Description("CS上报箱子放货完成(单次完成) 出库")]
        string NotifyPackagePutFinishForOut(string cmd);

        /// <summary>
        /// CS通知WMS叠盘完成（当前托完成）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [Description("CS通知WMS叠盘完成（当前托完成）")]
        string NotifyPalletizerFinish(string cmd);

        /// <summary>
        /// CS通知WMS盘点的结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [Description("CS通知WMS盘点的结果")]
        string NotifyStocktakingInstructResult(string cmd);

        /// <summary>
        /// 故障点上报
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>

        [Description("故障点上报")]
        string ReportTroubleStatus(string cmd);

        [Description("开关门上报")]
        string OpenOrCloseDoor(string cmd);

        [Description("扫描验证新接口")]
        string NotifyPackageBarcodeCheckNew(string cmd);

        [Description("放货完成新接口")]
        string NotifyPackagePutFinishNew(string cmd);

        /// <summary>
        /// 通知开关卷帘门（冷库内）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string NotifyCrossDoor(string cmd);

        [Description("呼叫AGV搬运")]
        string NotifyAgvCarry(string cmd);

        [Description("条码上报")]
        string NotifyBarcodeReqNewInstruct(string cmd);

        [Description("申请空容器")]
        string NotifyApplyContainer(string cmd);

        [Description("入库申请")]
        string NotifyApplyIn(string cmd);


        [Description("开/关门完成通知")]
        string NotifyDoorOpenOrCloseFinished(string cmd);


        [Description("关键节点状态上报")]
        string ReportNodeStatus(string cmd);
    }

}
