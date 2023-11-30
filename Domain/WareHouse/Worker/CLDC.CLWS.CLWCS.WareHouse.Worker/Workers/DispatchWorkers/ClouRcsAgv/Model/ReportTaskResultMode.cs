using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model
{
    public class ReportTaskResultMode
    {
        /// <summary>
        /// :任务编号，
        /// </summary>
        public string TASK_NO { get; set; }
        /// <summary>
        /// :任务状态，
        /// </summary>
        public string TASK_STATUS { get; set; }
        /// <summary>
        /// :设备编号
        /// </summary>
        public string AGV_NO { get; set; }


        public static explicit operator ReportTaskResultMode(string json)
        {
            return JsonConvert.DeserializeObject<ReportTaskResultMode>(json);
        }

    }

}
