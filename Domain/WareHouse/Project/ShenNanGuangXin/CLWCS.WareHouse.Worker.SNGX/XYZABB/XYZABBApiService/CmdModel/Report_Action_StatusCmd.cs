using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class Report_Action_StatusCmd
    {
        /// <summary>
        /// 拆垛任务的id，具有唯一性
        /// </summary>
        public string task_id { get; set; }

        public string area_code { get; set; }

        /// <summary>
        /// 抓取数量，单次
        /// </summary>
        public int pick_num { get; set; }

        /// <summary>
        /// 托盘id，用于wcs校验
        /// </summary>
        public string pallet_id { get; set; }

        /// <summary>
        /// 动作结果，0: 成功，非0则异常
        /// </summary>
        public int action_status { get; set; }

        /// <summary>
        /// 补充信息，可为空
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// //定制化需求，此时为{"barcode":“123456”,"place_position":0}，barcode为码箱的条码，place_position为放置位置（数字形式表示）dict类型
        /// </summary>
        public customizedResult customized_result { get; set; }
    }

    public class customizedResult
    {
        //public string barcode { get; set; }
        private List<string> _barcodeList = new List<string>();
        public List<string> barcode
        {
            get { return _barcodeList; }

            set { _barcodeList = value; }
        }


        private List<int> _place_position = new List<int>();
        public List<int> place_position
        {
            get { return _place_position; }

            set { _place_position = value; }
        }

        private List<int> _pick_position = new List<int>();
        public List<int> pick_position
        {
            get { return _pick_position; }

            set { _pick_position = value; }
        }
    }
}
