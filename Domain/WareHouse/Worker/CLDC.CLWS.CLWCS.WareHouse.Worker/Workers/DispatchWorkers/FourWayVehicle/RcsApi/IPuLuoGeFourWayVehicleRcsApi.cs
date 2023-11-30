using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.RcsApi
{

    public interface IPuLuoGeFourWayVehicleRcsApi
    {
        /// <summary>
        /// 请求WCS货物通行
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult RequestWCSPermit(string cmd);
        /// <summary>
        /// 上报WCS货物通行完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult ReportWCSPermitFinish(string cmd);
        /// <summary>
        /// 上报搬运任务执行结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult ReportTaskResult(string cmd);
        /// <summary>
        /// 上报搬运任务异常
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult ReportTaskException(string cmd);
        /// <summary>
        /// 上报车辆状态信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult ReportAGVStatus(string cmd);
        /// <summary>
        /// 上报车辆运行参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult ReportAGVPara(string cmd);
        /// <summary>
        /// 上报物流轨迹信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        OperateResult ReportTrackInfo(string cmd);

    }
}
