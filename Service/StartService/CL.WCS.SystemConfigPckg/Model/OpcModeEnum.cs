using System.ComponentModel;

namespace CL.WCS.SystemConfigPckg.Model
{
    /// <summary>
    /// Opc的启动模式
    /// </summary>
   public enum  OpcModeEnum
    {
       /// <summary>
       /// 生产环境
       /// </summary>
       [Description("生产环境")]
       Production,
       /// <summary>
       /// 自动化模拟环境
       /// </summary>
       [Description("自动化模拟")]
       Automatic,
       /// <summary>
       /// 手动测试环境
       /// </summary>
       [Description("手动测试")]
       Manual,
    }
}
