using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.WmsView.DataModel
{
    public enum TaskTypeEnum
    {
        /// <summary>
        /// 指挥作业
        /// </summary>
        [Description("指挥作业")]
        指挥作业 = 1,
        /// <summary>
        /// 指挥通信
        /// </summary>
        [Description("指挥通信")]
        指挥通信 = 2,
        /// <summary>
        /// 工程防护
        /// </summary>
        [Description("工程防护")]
        工程防护 = 3,
        /// <summary>
        /// 野营保障
        /// </summary>
        [Description("野营保障")]
        野营保障 = 4,
        /// <summary>
        /// 军需给养
        /// </summary>
        [Description("军需给养")]
        军需给养 = 5,
        /// <summary>
        /// 抗洪防汛
        /// </summary>
        [Description("抗洪防汛")]
        抗洪防汛 = 6,
        /// <summary>
        /// 森林灭火
        /// </summary>
        [Description("森林灭火")]
        森林灭火 = 7,
        /// <summary>
        /// 反恐维稳
        /// </summary>
        [Description("反恐维稳")]
        反恐维稳 = 8
    }
}
