using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.WareHouse.Device.HeFei.Simulate3D.Model
{
    public class Notice_System_StatusCmd
    {
        /// <summary>
        /// 区域 01 入库  02出库
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 0成功，1失败
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 异常补充信息
        /// </summary>
        public string error_message { get; set; }

        private returnData _data = new returnData();
        public returnData return_data
        {
            get { return _data; }
            set { _data = value; }
        }
    }

    public class returnData
    {
        /// <summary>
        /// 工作状态，详情参考状态表值
        /// </summary>
        public string work_status { get; set; }
        /// <summary>
        /// 状态标识，start 开始动作，finish动作完成
        /// </summary>
        public string work_flag { get; set; }
        /// <summary>
        /// 动作状态描述
        /// </summary>
        public string work_status_desc { get; set; }

    }
}
