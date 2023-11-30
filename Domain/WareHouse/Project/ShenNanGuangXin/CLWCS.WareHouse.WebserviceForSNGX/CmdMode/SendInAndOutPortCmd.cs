using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class SendInAndOutPortCmd
    {
        public int ID { get; set; }
        public string InAndOutPort { get; set; }
        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator SendInAndOutPortCmd(string json)
        {
            return json.ToObject<SendInAndOutPortCmd>();
        }
    }
}
