using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class Notice_Place_Ws_Is_FullCmd
    {
        /// <summary>
        /// 拆垛任务的id，具有唯一性
        /// </summary>
        public string task_id { get; set; }

        /// <summary>
        /// 工作区域编码，01 入库区02 出库区
        /// </summary>
        public string area_code { get; set; }

        /// <summary>
        /// 码垛工作空间号（会由XYZ定义每个空间的名称，入库为2，出库为1）
        /// </summary>
        public string ws_id { get; set; }

        /// <summary>
        /// 码垛数量，默认值: 入库=40出库=16
        /// </summary>
        public int pallet_count { get; set; }

        /// <summary>
        /// 托盘id-------用作校验
        /// </summary>
        public string pallet_id { get; set; }
    }
}
