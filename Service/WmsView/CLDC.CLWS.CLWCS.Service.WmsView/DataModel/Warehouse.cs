using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
    /// <summary>
    /// 库房
    /// </summary>
    [SugarTable("t_Warehouse")]
	public class Warehouse
	{

		/// <summary>
		/// 库房编码
		/// </summary>
		[SugarColumn]
		public string Code { get; set; }

		/// <summary>
		/// 库房名称
		/// </summary>
		[SugarColumn]
		public string Name { get; set; }

		/// <summary>
		/// 库房类型：1-立库；2-平库
		/// </summary>
		[SugarColumn]
		public int? WarehouseType { get; set; }

		/// <summary>
		/// 对应ERP系统的库房编码
		/// </summary>
		[SugarColumn]
		public string UpperSysCode { get; set; }

		/// <summary>
		/// 状态：0-禁用；1-正常
		/// </summary>
		[SugarColumn]
		public int? Status { get; set; }

		/// <summary>
		/// 联系人
		/// </summary>
		[SugarColumn]
		public string Contact { get; set; }

		/// <summary>
		/// 联系电话
		/// </summary>
		[SugarColumn]
		public string MobilePhone { get; set; }

		/// <summary>
		/// 地址
		/// </summary>
		[SugarColumn]
		public string AddrDetail { get; set; }

		/// <summary>
		/// 库房面积
		/// </summary>
		[SugarColumn]
		public decimal? BuiltArea { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		[SugarColumn]
		public string Remark { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? AreaCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? StationCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? RoutewayCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public int? ShelfCount { get; set; }

		/// <summary>
		/// 创建者Id
		/// </summary>
		[SugarColumn]
		public long? CreatedUserId { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		[SugarColumn]
		public string CreatedUserName { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[SugarColumn]
		public DateTime? CreatedTime { get; set; }

		/// <summary>
		/// 修改者Id
		/// </summary>
		[SugarColumn]
		public long? ModifiedUserId { get; set; }

		/// <summary>
		/// 修改人
		/// </summary>
		[SugarColumn]
		public string ModifiedUserName { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		[SugarColumn]
		public DateTime? ModifiedTime { get; set; }

		/// <summary>
		/// 是否删除
		/// </summary>
		[SugarColumn]
		public bool? IsDeleted { get; set; }

		/// <summary>
		/// 租户Id
		/// </summary>
		[SugarColumn]
		public long? TenantId { get; set; }
	}
}
