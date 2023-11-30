using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    /// <summary>
    /// 库存整理类
    /// </summary>
    public class NotifyInventoryArrangeTaskCmd
    {
        /// <summary>
        /// 工作区域编码，01 入库区 02 出库区 
        /// </summary>
        public string area_code { get; set; }

        /// <summary>
        /// 1入， 默认xxx-1008  2\出 默认1008-xxx  
        /// </summary>
        public int taskType { get; set; }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator NotifyInventoryArrangeTaskCmd(string json)
        {
            return json.ToObject<NotifyInventoryArrangeTaskCmd>();
        }
    }

    ///// <summary>
    ///// 是否需要扫描  (0默认不扫、1扫)
    ///// </summary>
    //public bool isScanner { get; set; }
    ///// <summary>
    ///// 开始地址  （wms不指定地址 则由wcs默认）
    ///// </summary>
    //public string start_addr { get; set; }

    ///// <summary>
    ///// 目标地址 （wms不指定地址 则由wcs默认）
    ///// </summary>
    //public string dest_addr { get; set; }
}
