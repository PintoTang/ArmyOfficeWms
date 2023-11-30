using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    [SugarTable("T_ST_PERSON_INFO", "人员信息")]
    public class PersonMode
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [SugarColumn(ColumnName = "ACC_CODE", Length = 255, IsNullable = false, ColumnDescription = "账号")]
        public string AccCode { get; set; }

        /// <summary>
        /// 组编号
        /// </summary>
        [SugarColumn(ColumnName = "GROUP_ID", IsNullable = true, ColumnDescription = "组的编号")]
        public int? GroupId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnName = "PERSON_NAME", Length = 50, IsNullable = true, ColumnDescription = "人员姓名")]
        public string PersonName { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [SugarColumn(ColumnName = "WORK_ID", Length = 20, IsNullable = true, ColumnDescription = "身份证号")]
        public string WorkId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "DEPT", Length = 50, IsNullable = true, ColumnDescription = "部门")]
        public string Description { get; set; }

        /// <summary>
        /// 技能
        /// </summary>
        [SugarColumn(ColumnName = "SKILLS", Length = 15, IsNullable = true, ColumnDescription = "专业技能")]
        public string Skills { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        [SugarColumn(ColumnName = "ADDRESS", Length = 100, IsNullable = true, ColumnDescription = "地址")]
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [SugarColumn(ColumnName = "TELEPHONE", Length = 15, IsNullable = true, ColumnDescription = "联系电话")]
        public string TelephoneNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(ColumnName = "REMARK", Length = 100, IsNullable = true, ColumnDescription = "remark")]
        public string Remark { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        [SugarColumn(ColumnName = "E_MAIL", Length = 30, IsNullable = true, ColumnDescription = "电子邮箱")]
        public string Email { get; set; }

    }
}
