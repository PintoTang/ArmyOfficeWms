using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class ForceCloseTaskCmd
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区 
        /// </summary>
        public string area_code { get; set; }

        /// <summary>
        /// 任务类型：0-当前托盘结束；1-所有任务结束
        /// </summary>
        public int taskType { get; set; }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator ForceCloseTaskCmd(string json)
        {
            return json.ToObject<ForceCloseTaskCmd>();
        }
    }
}
