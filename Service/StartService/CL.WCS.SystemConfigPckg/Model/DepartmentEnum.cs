using System.ComponentModel;

namespace CL.WCS.SystemConfigPckg.Model
{
    public enum DepartmentEnum
    {
        /// <summary>
        /// 深圳市维斯物流自动化
        /// </summary>
        [Description("深圳市维斯物流自动化")] 
        WEISS,
        /// <summary>
        /// 南网
        /// </summary>
        [Description("中国南方电网")]
        SCG,
        /// <summary>
        /// 国网
        /// </summary>
        [Description("中国国家电网")]
        StateGrid,
        /// <summary>
        /// 自定义风格
        /// </summary>
        [Description("自定义风格")]
        Random,
        
    }
}
