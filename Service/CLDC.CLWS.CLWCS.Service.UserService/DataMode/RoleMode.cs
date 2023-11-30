using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.Service.UserService.DataMode
{
    /// <summary>
    /// 角色数据结构
    /// </summary>
    public sealed class RoleMode
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public RoleLevelEnum RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string RoleDescription { get; set; }
        /// <summary>
        /// 创建的该角色的用户ID
        /// </summary>
        public int CreaterId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改的用户ID
        /// </summary>
        public int ModifierId { get; set; }
        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime ModifieTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
