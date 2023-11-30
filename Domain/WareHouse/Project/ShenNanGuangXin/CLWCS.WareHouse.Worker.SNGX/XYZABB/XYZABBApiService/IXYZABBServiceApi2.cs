using CLWCS.WareHouse.Device.HeFei.Simulate3D.Model;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService
{
    public interface IXYZABBServiceApiTwo
    {
        /// <summary>
        /// 7.1	通知机器人作业状态
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string NoticeRobotStatus(NoticeRobotStatusCmd cmd);
        /// <summary>
        /// 7.2	通知指令执行异常
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string ReportExeOrderException(ReportExeOrderExceptionCmd cmd);


        /// <summary>
        /// 7.3	通知指令执行完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string ReportExeOrderFinish(ReportExeOrderFinishCmd cmd);


        /// <summary>
        /// 7.4	上报机器人作业信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string ReportRobotWorkInfo(ReportRobotWorkInfoCmd cmd);

        /// <summary>
        /// 7.5	上报机器人故障
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string ReportFaultInfo(ReportFaultInfoCmd cmd);
    }
}
