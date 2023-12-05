using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
	/// <summary>
	/// 物料
	/// </summary>
	[SugarTable("t_Material")]
	public class Material
	{

		/// <summary>
		/// 物料号
		/// </summary>
		[SugarColumn]
		public string MaterialCode { get; set; }

		/// <summary>
		/// 物料描述
		/// </summary>
		[SugarColumn]
		public string MaterialDesc { get; set; }

		/// <summary>
		/// 规格型号
		/// </summary>
		[SugarColumn]
		public string MaterialSpec { get; set; }

		/// <summary>
		/// 物料类别：1-容器；2-成品；3-半成品；4-原材料
		/// </summary>
		[SugarColumn]
		public int? MaterialCategory { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public long? MaterialGroupId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public string MaterialGroupCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn]
		public string MaterialGroupName { get; set; }

		/// <summary>
		/// 计量单位Id
		/// </summary>
		[SugarColumn]
		public long? UnitId { get; set; }

		/// <summary>
		/// 计量单位
		/// </summary>
		[SugarColumn]
		public string UnitName { get; set; }

		/// <summary>
		/// 最小安全库存
		/// </summary>
		[SugarColumn]
		public int? MinSafetyStock { get; set; }

		/// <summary>
		/// 最大安全库存
		/// </summary>
		[SugarColumn]
		public int? MaxSafetyStock { get; set; }

		/// <summary>
		/// 物料图片
		/// </summary>
		[SugarColumn]
		public string Img { get; set; }

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
