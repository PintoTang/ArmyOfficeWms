using SqlSugar;

namespace CLDC.CLWS.CLWCS.Service.Authorize.DataMode
{
    /// <summary>
    /// 组数据结构
    /// </summary>
    [SugarTable("T_ST_GROUP", "用户组管理")]
    public class GroupMode
    {
        /// <summary>
        /// 组编号
        /// </summary>
        [SugarColumn(ColumnName = "GROUP_ID", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "组织id")]
        public int GroupId { get; set; }
        
        /// <summary>
        /// 组织名称
        /// </summary>
        [SugarColumn(ColumnName = "GROUP_NAME", Length = 50, IsNullable = true, ColumnDescription = "组织名称")]
        public string GroupName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "DEPT", Length = 255, IsNullable = true, ColumnDescription = "组描述")]
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "REMARK", Length = 100, IsNullable = true, ColumnDescription = "remark")]
        public string Remark { get; set; }

    }
}
