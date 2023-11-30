using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class NotifyStartTaskCmd
    {
        /// <summary>
        /// 工作区域编码，01 入库区 02 出库区 
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        ///  动作  1开始、2结束
        /// </summary>
        public int ActionNum { get; set; }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator NotifyStartTaskCmd(string json)
        {
            return json.ToObject<NotifyStartTaskCmd>();
        }
    }
}
