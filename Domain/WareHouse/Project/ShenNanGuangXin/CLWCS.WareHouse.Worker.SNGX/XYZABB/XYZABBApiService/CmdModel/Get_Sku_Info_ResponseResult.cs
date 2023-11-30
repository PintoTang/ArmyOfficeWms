using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;

namespace CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel
{
    public class Get_Sku_Info_ResponseResult
    {
        private skuInfo _data = new skuInfo();

        public skuInfo sku_info
        {
            get { return _data; }
            set { _data = value; }
        }
        public int error { get; set; }

        public string message { get; set; }


        public static explicit operator Get_Sku_Info_ResponseResult(string json)
        {
            return json.ToObject<Get_Sku_Info_ResponseResult>();
        }
        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class skuInfo
    {
        /// <summary>
        /// 商品编号(预设为空字符串)
        /// </summary>
        public string sku_id { get; set; }

        /// <summary>
        /// 商品长度 (单位: 公尺)
        /// </summary>
        public float length { get; set; }

        /// <summary>
        /// 商品宽]度 (单位: 公尺)
        /// </summary>
        public float width { get; set; }

        /// <summary>
        /// 商品高度 (单位: 公尺）
        /// </summary>
        public float height { get; set; }

        /// <summary>
        /// 商品重量 (单位: 公斤)
        /// </summary>
        public float weight { get; set; }
    }
}
