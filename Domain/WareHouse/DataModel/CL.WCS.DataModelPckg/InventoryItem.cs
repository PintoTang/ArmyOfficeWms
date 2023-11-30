using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 转换出入库类型为Json格式
    /// </summary>
	public class InventoryItem : Item
	{
		/// <summary>
		///  1.回库 2.不可回库 3.出库（货品出库、子母托盘出库） 4.收集器
		/// </summary>
        [JsonProperty("INOUT_FLG")]
		public int InOutFlg
		{
			set;
			get;
		}


		/// <summary>
		/// 定义Json字符串显示转换为InventoryItem对象
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static explicit operator InventoryItem(string json)
		{
			return JsonConvert.DeserializeObject<InventoryItem>(json);
		}
	}
}
