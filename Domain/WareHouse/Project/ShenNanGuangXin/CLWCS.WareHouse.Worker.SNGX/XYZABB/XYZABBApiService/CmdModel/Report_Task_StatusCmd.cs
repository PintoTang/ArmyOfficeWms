using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class Report_Task_StatusCmd
    {
        //拆垛任务的id，具有唯一性
        public string task_id { get; set; }
      
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }
        //任务状态，0为正常完成，99为异常台数量大于6回退，98为扫码条码不匹配回退
        public int task_status { get; set; }
        //任务实际总抓取数量
        public int pick_num { get; set; }
        //目标抓取数量，-1为清空托盘
        public int target_num { get; set; }
        //补充信息，task_status为99时，message为“error_station_full” task_status为98时，message为”mismatch_barcode”
        public string message { get; set; }
        //定制化需求，预设为空dict
        public ccResult customized_result { get; set; }
    }
    public class  ccResult
    {

    }
}
