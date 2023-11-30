using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
    public class ReSendTaskCmd
    {
        /// <summary>
        /// 工作区域编码，01 入库区02 出库区 
        /// </summary>
        public string area_code { get; set; }

        private List<int> _node_id_list = new List<int>();
        /// <summary>
        /// 节点ID列表    出库 2001、2007  入库 1004  1007 1008  （出库重新扫码 发2001）
        /// </summary>
        public List<int> node_id_list
        {
            get { return _node_id_list; }

            set { _node_id_list = value; }
        }

        /// <summary>
        /// 定义Json字符串显示转换为Item对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static explicit operator ReSendTaskCmd(string json)
        {
            return json.ToObject<ReSendTaskCmd>();
        }
    }
}
