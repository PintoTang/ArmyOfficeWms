using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    /// <summary>
    /// 账号信息
    /// </summary>
    [SugarTable("T_ST_ACCOUNT", "帐号管理")]
    public class AccountMode
    {
        /// <summary>
        /// 用户ID 唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "ACC_ID", IsPrimaryKey = true, IsIdentity = true, IsNullable = false, ColumnDescription = "帐号id")]
        public int AccId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [SugarColumn(ColumnName = "ACC_CODE", Length = 50, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "帐号名")]
        public string AccCode { get; set; }

        /// <summary>
        /// 组编号
        /// </summary>
        [SugarColumn(ColumnName = "GROUP_ID", IsNullable = true, ColumnDescription = "GROUP_ID")]
        public int? GroupId { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        [SugarColumn(ColumnName = "ROLE_ID", IsNullable = true, ColumnDescription = "ROLE_ID")]
        public RoleLevelEnum? RoleLevel { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(ColumnName = "PASSWORD", Length = 32, IsNullable = true, ColumnDescription = "密码")]
        public string Password { get; set; }
        /// <summary>
        /// 启用时间
        /// </summary>
        [SugarColumn(ColumnName = "ENABLE_TIME", IsNullable = true, ColumnDescription = "帐号生效时间")]
        public DateTime? EnableTime { get; set; }

        /// <summary>
        /// 禁用时间
        /// </summary>
        [SugarColumn(ColumnName = "DISABLE_TIME", IsNullable = true, ColumnDescription = "帐号失效时间")]
        public DateTime? DisableTime { get; set; }

        /// <summary>
        /// 使用状态
        /// </summary>
        [SugarColumn(ColumnName = "USE_STATUS", IsNullable = true, ColumnDescription = "帐号状态")]
        public AccountStatusEnum? UseStatus { get; set; }
        /// <summary>
        /// 用户的活动状态
        /// </summary>
        [SugarColumn(ColumnName = "ONLINE_STATUS", IsNullable = true, ColumnDescription = "账号在线状态")]
        public AccountLiveStatus? OnlineStatus { get; set; }

        /// <summary>
        /// 创建用户的ID
        /// </summary>
        [SugarColumn(ColumnName = "CREATER", IsNullable = true, ColumnDescription = "创建人")]
        public int? CreaterId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnName = "CREATE_DATE", IsNullable = true, ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 最近修改的用户ID
        /// </summary>
        [SugarColumn(ColumnName = "MODIFIER", IsNullable = true, ColumnDescription = "修改人")]
        public int? ModifierId { get; set; }
        /// <summary>
        /// 最近修改的时间
        /// </summary>
        [SugarColumn(ColumnName = "MOMODIFY_DATE", IsNullable = true, ColumnDescription = "修改时间")]
        public DateTime? ModifierTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "REMARK", Length = 100, IsNullable = true, ColumnDescription = "remark")]
        public string Remark { get; set; }

    }
}
