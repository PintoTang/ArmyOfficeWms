using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DataModel
{
    /// <summary>
    /// 角色等级
    /// </summary>
    public enum RoleLevelEnum
    {
        /// <summary>
        /// 游客
        /// </summary>
        [Description("游客")]
        游客 = 0,
        /// <summary>
        /// 运维人员
        /// </summary>
        [Description("运维人员")]
        运维人员 = 1,
        /// <summary>
        /// 超级运维人员
        /// </summary>
        [Description("超级运维人员")]
        超级运维人员 = 2,
        /// <summary>
        /// 开发者
        /// </summary>
        [Description("开发者")]
        开发者 = 3,
        /// <summary>
        /// 管理员
        /// </summary>
        [Description("管理员")]
        管理员 = 4,
        /// <summary>
        /// 超级管理员
        /// </summary>
        [Description("超级管理员")]
        超级管理员 = 5
    }
}
