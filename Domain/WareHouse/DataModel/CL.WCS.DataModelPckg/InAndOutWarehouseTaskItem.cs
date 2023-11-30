using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 任务信息
    /// </summary>
	public class InAndOutWarehouseTaskItem
	{
		/// <summary>
		/// 返回消息说明	
		/// </summary>
		public string message
		{
			get;
			set;
		}

		/// <summary>
		///下载说明
		/// </summary>
		public string status
		{
			get;
			set;
		}

		/// <summary>
		/// 行id
		/// </summary>	
		public string interface_id
		{
			get;
			set;
		}

		/// <summary>
		/// 主体id	 
		/// </summary>	
		public string entity_id
		{
			get;
			set;
		}

		/// <summary>
		/// 仓库id	
		/// </summary>	
		public string warehouse_id
		{
			get;
			set;
		}

		/// <summary>
		/// 任务类型
		/// </summary>
		public string task_type
		{
			get;
			set;
		}

		/// <summary>
		///打包号	
		/// </summary>
		public string mo_collect_number
		{
			get;
			set;
		}

		/// <summary>
		/// 来源单据号-班组任务号
		/// </summary>
		public string source_bill_number
		{
			get;
			set;
		}

		/// <summary>
		///来源头id 	 
		/// </summary>
		public string source_header_id
		{
			get;
			set;
		}

		/// <summary>
		/// 要求交付时间	
		/// </summary>
		public string required_delivery_date
		{
			get;
			set;
		}

		/// <summary>
		/// 来源行id task_line_id	 
		/// </summary>
		public string source_line_id
		{
			get;
			set;
		}

		/// <summary>
		/// 物料id	
		/// </summary>
		public string item_id
		{
			get;
			set;
		}

		/// <summary>
		/// 物料code	 
		/// </summary>
		public string item_code
		{
			get;
			set;
		}

		/// <summary>
		///组织	 
		/// </summary>	
		public string organization_code
		{
			get;
			set;
		}

		/// <summary>
		///版本
		/// </summary>	
		public string revision
		{
			get;
			set;
		}

		/// <summary>
		///单位
		/// </summary>	
		public string uom_code
		{
			get;
			set;
		}

		/// <summary>
		/// 箱id	
		/// </summary>
		public string lpn_id
		{
			get;
			set;
		}

		/// <summary>
		/// 称名	
		/// </summary>
		public string lpn_code
		{
			get;
			set;
		}

		/// <summary>
		/// 拣料数量	 
		/// </summary>	
		public string request_quantity
		{
			get;
			set;
		}

		/// <summary>
		/// 交付数量	
		/// </summary>
		public string deliver_quantity
		{
			get;
			set;
		}

		/// <summary>
		/// 来源货区	
		/// </summary>
		public string source_zone_code
		{
			get;
			set;
		}

		/// <summary>
		///来源货区id  	 
		/// </summary>	
		public string source_zone_id
		{
			get;
			set;
		}

		/// <summary>
		/// 拣料货位	 
		/// </summary>	
		public string source_locator_code
		{
			get;
			set;
		}

		/// <summary>
		/// 拣料货位id	
		/// </summary>	
		public string source_locator_id
		{
			get;
			set;
		}

		/// <summary>
		///来源erp子库
		/// </summary>	
		public string source_subinventory_code
		{
			get;
			set;
		}

		/// <summary>
		///来源erp货位	
		/// </summary>
		public string source_erp_locator_code
		{
			get;
			set;
		}

		/// <summary>
		/// 行备注	 
		/// </summary>
		public string item_remark
		{
			get;
			set;
		}

		/// <summary>
		/// 交付对象	 
		/// </summary>
		public string delivery_object
		{
			get;
			set;
		}

		/// <summary>
		/// 数据来源dcs,ptl,avg等	
		/// </summary>	
		public string source_code
		{
			get;
			set;
		}

		/// <summary>
		/// 处理批次
		/// </summary>	
		public string process_batch_number
		{
			get;
			set;
		}

		/// <summary>
		/// 接口表处理状态(2--成功写入接口表,4--写入正式表成功,5--写入正式表失败)	 /// </summary>
		public string process_status
		{
			get;
			set;
		}

		/// <summary>
		/// 错误代码	
		/// </summary>
		public string processing_error_code
		{
			get;
			set;
		}

		/// <summary>
		/// 错误描述	 
		/// </summary>
		public string processing_error_message
		{
			get;
			set;
		}

		/// <summary>
		///创建人	
		/// </summary>
		public string created_by
		{
			get;
			set;
		}

		/// <summary>
		/// 创建时间	 
		/// </summary>	
		public string creation_date
		{
			get;
			set;
		}

		/// <summary>
		/// 最后修改人	
		/// </summary>	
		public string last_updated_by
		{
			get;
			set;
		}

		/// <summary>
		/// 最后修改时间	
		/// </summary>	
		public string last_update_date
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段1	 
		/// </summary>
		public string attribute1
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段2	 
		/// </summary>	
		public string attribute2
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段3	
		/// </summary>
		public string attribute3
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段4	 
		/// </summary>
		public string attribute4
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段5	
		/// </summary>	
		public string attribute5
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段6	
		/// </summary>
		public string attribute6
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段7	 
		/// </summary>	
		public string attribute7
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段8	
		/// </summary>	
		public string attribute8
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段9	
		/// </summary>
		public string attribute9
		{
			get;
			set;
		}

		/// <summary>
		/// 保留字段10
		/// </summary>
		public string attribute10
		{
			get;
			set;
		}
        /// <summary>
        /// 描述信息
        /// </summary>
		public string description
		{
			get;
			set;
		}

		/// <summary>
		/// 开箱标示，0为需拣选拆箱、1为整箱
		/// </summary>	
		public string open_flag
		{
			get;
			set;
		}

		/// <summary>
		/// 使用source_bill_number+interface_id	 
		/// </summary>
		public string task_number
		{
			get;
			set;
		}

		/// <summary>
		/// 任务优先级，1-5 1最高	
		/// </summary>	
		public string task_priority
		{
			get;
			set;
		}

		/// <summary>
		/// 来源货区	
		/// </summary>	
		public string to_zone_code
		{
			get;
			set;
		}

		/// <summary>
		/// 来源货区id	
		/// </summary>
		public string to_zone_id
		{
			get;
			set;
		}

		/// <summary>
		///拣料货位，上架入库入库到哪个货位时使用
		///	 </summary>	
		public string to_locator_code
		{
			get;
			set;
		}

		/// <summary>
		///拣料货位id	
		/// </summary>
		public string to_locator_id
		{
			get;
			set;
		}

		/// <summary>
		///目的erp子库	
		/// </summary>
		public string to_subinventory_code
		{
			get;
			set;
		}

		/// <summary>
		/// 目的erp货位	
		/// </summary>
		public string to_erp_locator_code
		{
			get;
			set;
		}

		/// <summary>
		/// ptl/agv 区分接口表的数据分发给不同类型的设备.根据库区来分流到agv	 /// </summary>
		public string task_category
		{
			get;
			set;
		}

		/// <summary>
		/// 设备编号
		/// </summary>
		public string machine_num
		{
			get;
			set;
		}

		/// <summary>
		/// 重发标志
		/// </summary>
		public string resend_flag
		{
			get;
			set;
		}

		/// <summary>
		/// 原单据号
		/// </summary>
		public string source_task_number
		{
			get;
			set;
		}

		/// <summary>
		/// 入库或者出库	 
		/// </summary>
		public string stock_type
		{
			get;
			set;
		}

		/// <summary>
        /// 定义Json字符串显示转换为Item对象
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static explicit operator InAndOutWarehouseTaskItem(string json)
		{
			return JsonConvert.DeserializeObject<InAndOutWarehouseTaskItem>(json); 
		}
	}

}
