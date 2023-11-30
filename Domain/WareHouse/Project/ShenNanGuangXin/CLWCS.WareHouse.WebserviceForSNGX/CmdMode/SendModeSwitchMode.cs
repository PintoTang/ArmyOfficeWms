using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using Newtonsoft.Json;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class SendModeSwitchMode
    {
        public List<SendModeData> DATA { get; set; }
       

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator SendModeSwitchMode(string json)
        {

            return JsonConvert.DeserializeObject<SendModeSwitchMode>(json);
        }
    }

    public class SendModeData
    {
        /// <summary>
        /// 设备模式 0禁用 1 入库 2 出库 
        /// </summary>
        public int MODE { get; set; }
        /// <summary>
        /// 库口地址
        /// </summary>
        public string INANDOUTPORT { get; set; }
    }
}

