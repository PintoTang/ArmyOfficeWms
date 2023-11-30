using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model
{
    /// <summary>
    /// 异常model
    /// </summary>
    public class ReportTaskExceptionMode
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// 任务异常代码
        /// </summary>
        public int TASKEXCEPTION_CODE { get; set; }

        public static explicit operator ReportTaskExceptionMode(string json)
        {
            return JsonConvert.DeserializeObject<ReportTaskExceptionMode>(json);
        }

    }
}
