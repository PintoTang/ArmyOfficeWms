using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    public enum CommunicationModeEnum
    {
        /// <summary>
        /// 生产环境
        /// </summary>
        [Description("生产环境")]
        Production,
        /// <summary>
        /// 自动化模拟环境
        /// </summary>
        [Description("自动化模拟")]
        Automatic,
        /// <summary>
        /// 手动测试环境
        /// </summary>
        [Description("手动测试")]
        Manual,
    }
}
