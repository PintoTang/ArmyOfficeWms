using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class NotifyReportTroubleStatusCmdMode
    {
        public string MESSAGE { get; set; }
        public string HandlingSuggest { get; set; }
        /// <summary>
        /// WarnType=1是软件异常，WarnType=2是设备异常，WarnType=3异常是有货托盘实际出库是空
        /// </summary>
        public int WarnType { get; set; }

        [JsonIgnore]
        public int DeviceId { get; set; }

        [JsonIgnore]
        public string DeviceName { get; set; }

        [JsonIgnore]
        public string FaultCode { get; set; }
    }
}
