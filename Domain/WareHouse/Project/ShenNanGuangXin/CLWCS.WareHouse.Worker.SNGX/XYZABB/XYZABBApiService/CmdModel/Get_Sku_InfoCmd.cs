using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
     public class Get_Sku_InfoCmd
    {
         /// <summary>
        /// 工作区域编码，01 入库区02 出库区
         /// </summary>
         public string area_code { get; set; }
    }
}
