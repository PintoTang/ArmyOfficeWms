using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    public enum ShowCardShowEnum
    {
        [Description("任务")]
        Task,
        [Description("状态")]
        State,
        [Description("日志")]
        Log
    }
}
