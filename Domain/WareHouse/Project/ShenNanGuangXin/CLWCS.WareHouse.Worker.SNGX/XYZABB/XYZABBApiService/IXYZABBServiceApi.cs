using CLWCS.WareHouse.Device.HeFei.Simulate3D.Model;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService
{
    public interface IXYZABBServiceApi
    {
        /// <summary>
        /// 1、告知机器人状态是否OK接口 (仿真 不用)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string notice_system_is_ready(Notice_System_Is_ReadyCmd cmd);
        /// <summary>
        /// 2、机器人请求WCS扫码接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string get_sku_info(Get_Sku_InfoCmd cmd);
        /// <summary>
        /// 3、机器人请求WCS来料托盘退库接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string report_task_status(Report_Task_StatusCmd cmd);
        /// <summary>
        /// 4、回报WCS单次码垛结果接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string report_action_status(Report_Action_StatusCmd cmd);
        /// <summary>
        /// 5、通知WCS码垛箱已满接口（请求wcs入库码垛托盘）
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string notice_place_ws_is_full(Notice_Place_Ws_Is_FullCmd cmd);
        /// <summary>
        /// 6、上报WCS机器人工作状态接口(仿真)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string notice_system_status(Notice_System_StatusCmd cmd);
        /// <summary>
        /// 7、上报WCS机器人工作状态接口 (仿真)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string report_exception(Report_ExceptionCmd cmd);


        string report_orderFinish(report_orderFinishCmd cmd);
    }
}
