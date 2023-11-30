using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Service.Authorize;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.Service.OperateLog.Model
{
    /// <summary>
    /// 操作日志的数据模型
    /// </summary>
    [SugarTable("T_AC_OPERATE_LOG", "")]
    public class OperateLogModel
    {
        public OperateLogModel()
        {
            this.WhCode = SystemConfig.Instance.WhCode;
        }
        public OperateLogModel(string content)
        {
            this.WhCode = SystemConfig.Instance.WhCode;
            this.LogContent = content;
            LogRecordDate=DateTime.Now;
            LogUserAccount = CookieService.CurSession != null ? CookieService.CurSession.UserInfo.Account.AccCode : "游客";
            LogUserName = CookieService.CurSession != null ? CookieService.CurSession.UserInfo.Person.PersonName : "游客";
        }
        /// <summary>
        /// 日志内容
        /// </summary>
        [SugarColumn(ColumnName = "LOG_CONTENT", Length = 1024, IsNullable = false, ColumnDescription = "日志内容")]
        public string LogContent { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        [SugarColumn(ColumnName = "LOG_USER_NAME", Length = 255, IsNullable = false, ColumnDescription = "操作者的名字")]
        public string LogUserName { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>
        [SugarColumn(ColumnName = "LOG_USER_ACCOUNT", Length = 255, IsNullable = false, ColumnDescription = "操作者的用户编号")]
        public string LogUserAccount { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        [SugarColumn(ColumnName = "LOG_RECORD_DATE", IsNullable = false, ColumnDescription = "记录时间")]
        public DateTime? LogRecordDate { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        [SugarColumn(ColumnName = "LOG_WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string WhCode
        {
            get;set;
        }
    }

    public struct OperateLogFilter
    {
        public long PageIndex { get; set; }
        public long PageSize { get; set; }
        public string LogContent { get; set; }

        public string LogUserName { get; set; }

        public string RecordFromTime { get; set; }

        public string RecordToTime { get; set; }

        public string LogUserAccount { get; set; }
    }

}
