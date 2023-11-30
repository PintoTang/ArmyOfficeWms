using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class is_pallet_emptyMode
    {
        /// <summary>
        /// 放置工作空间号，与XYZ工作空间一致（入库为2，出库为0）
        /// </summary>
        public string ws_id { get; set; }
    }

    public class XYZABB_is_pallet_empty_Response
    {
        /// <summary>
        /// 返回结果代码 (0: 成功   1: 异常 (无该任务)  2: 资料格式错误)
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string error_message { get; set; }
        public int result { get; set; }

        public static explicit operator XYZABB_is_pallet_empty_Response(string json)
        {
            return JsonConvert.DeserializeObject<XYZABB_is_pallet_empty_Response>(json);
        }
    }

}
