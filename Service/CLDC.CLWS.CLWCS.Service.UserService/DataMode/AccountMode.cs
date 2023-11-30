using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.Service.UserService.DataMode
{
    /// <summary>
    /// 账号信息
    /// </summary>
    public sealed class AccountMode
    {
        /// <summary>
        /// 用户ID 唯一标识
        /// </summary>
        public int AccId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string AccCode { get; set; }

        /// <summary>
        /// 组编号
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public RoleLevelEnum RoleId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 启用时间
        /// </summary>
        public DateTime EnableTime { get; set; }

        /// <summary>
        /// 禁用时间
        /// </summary>
        public DateTime DisableTime { get; set; }

        /// <summary>
        /// 使用状态
        /// </summary>
        public AccountStatusEnum UseStatus { get; set; }
        /// <summary>
        /// 用户的活动状态
        /// </summary>
        public AccountLiveStatus OnlineStatus { get; set; }

        /// <summary>
        /// 创建用户的ID
        /// </summary>
        public int CreaterId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最近修改的用户ID
        /// </summary>
        public int ModifierId { get; set; }
        /// <summary>
        /// 最近修改的时间
        /// </summary>
        public DateTime ModifierTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
