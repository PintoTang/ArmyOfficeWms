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
        /// 物资编码
        /// </summary>
        [SugarColumn]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物资名称
        /// </summary>
        [SugarColumn]
        public string MaterialDesc { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [SugarColumn]
        public string MaterialSpec { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn]
        public string AreaCode { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        [SugarColumn]
        public string UnitName { get; set; }

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
