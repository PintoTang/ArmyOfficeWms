using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class NotifyScannerCardCmd
    {
        /// <summary>
        /// 位置地址 
        /// </summary>
        public int PostionNum { get; set; }
        /// <summary>
        ///  动作  1开、2关
        /// </summary>
        public int ActionNum { get; set; }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator NotifyScannerCardCmd(string json)
        {
            return json.ToObject<NotifyScannerCardCmd>();
        }
    }
}
