using System.ComponentModel;

namespace CL.WCS.SystemConfigPckg.Model
{
    public enum DepartmentEnum
    {
        /// <summary>
        /// 科陆
        /// </summary>
        [Description("科陆")]
        CLOU,
        /// <summary>
        /// 南网
        /// </summary>
        [Description("南网")]
        SCG,
        /// <summary>
        /// 国网
        /// </summary>
        [Description("国网")]
        StateGrid,

        [Description("自定义风格")]
        Random,
        
    }
}
