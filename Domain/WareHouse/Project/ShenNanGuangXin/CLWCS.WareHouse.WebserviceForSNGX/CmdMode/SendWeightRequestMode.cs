using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using Newtonsoft.Json;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{

    public class SendWeightRequestMode
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string INANDOUTPORT { get; set; }


        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator SendWeightRequestMode(string json)
        {

            return JsonConvert.DeserializeObject<SendWeightRequestMode>(json);
        }
    }

}