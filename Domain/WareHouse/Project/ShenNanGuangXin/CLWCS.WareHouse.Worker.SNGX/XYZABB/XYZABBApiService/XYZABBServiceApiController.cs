using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Infrastructrue.Ioc.DependencyFactory;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel;
using CLWCS.WareHouse.Device.HeFei.Simulate3D.Model;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService
{

    public class XYZABBServiceApiController : ApiController, IXYZABBServiceApiTwo
    {
        //[HttpPost]
        //public string notice_system_is_ready(Notice_System_Is_ReadyCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.notice_system_is_ready(cmd);
        //}

        //[HttpPost]
        //  public string get_sku_info(Get_Sku_InfoCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.get_sku_info(cmd);
        //}

        //[HttpPost]
        //public string report_task_status(Report_Task_StatusCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.report_task_status(cmd);
        //}

        //[HttpPost]
        //public string report_action_status(Report_Action_StatusCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.report_action_status(cmd);
        //}

        //[HttpPost]
        //public string notice_place_ws_is_full(Notice_Place_Ws_Is_FullCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.notice_place_ws_is_full(cmd);
        //}

        //[HttpPost]
        //public string notice_system_status(Notice_System_StatusCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.notice_system_status(cmd);
        //}

        //[HttpPost]
        //public string report_exception(Report_ExceptionCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();
        //    return abbWebApi.report_exception(cmd);
        //}

        //[HttpPost]
        //public string report_orderFinish(report_orderFinishCmd cmd)
        //{
        //    IXYZABBServiceApi abbWebApi = DependencyHelper.GetService<IXYZABBServiceApi>();

        //    return abbWebApi.report_orderFinish(cmd);
        //}
        [HttpPost]
        public string NoticeRobotStatus(NoticeRobotStatusCmd cmd)
        {
            IXYZABBServiceApiTwo abbWebApi = DependencyHelper.GetService<IXYZABBServiceApiTwo>();
            return abbWebApi.NoticeRobotStatus(cmd);
        }
        [HttpPost]
        public string ReportExeOrderException(ReportExeOrderExceptionCmd cmd)
        {
            IXYZABBServiceApiTwo abbWebApi = DependencyHelper.GetService<IXYZABBServiceApiTwo>();
            return abbWebApi.ReportExeOrderException(cmd);
        }
        [HttpPost]
        public string ReportExeOrderFinish(ReportExeOrderFinishCmd cmd)
        {
            IXYZABBServiceApiTwo abbWebApi = DependencyHelper.GetService<IXYZABBServiceApiTwo>();
            return abbWebApi.ReportExeOrderFinish(cmd);
        }
        [HttpPost]
        public string ReportFaultInfo(ReportFaultInfoCmd cmd)
        {
            IXYZABBServiceApiTwo abbWebApi = DependencyHelper.GetService<IXYZABBServiceApiTwo>();
            return abbWebApi.ReportFaultInfo(cmd);
        }
        [HttpPost]
        public string ReportRobotWorkInfo(ReportRobotWorkInfoCmd cmd)
        {
            IXYZABBServiceApiTwo abbWebApi = DependencyHelper.GetService<IXYZABBServiceApiTwo>();
            return abbWebApi.ReportRobotWorkInfo(cmd);
        }
    }
}
