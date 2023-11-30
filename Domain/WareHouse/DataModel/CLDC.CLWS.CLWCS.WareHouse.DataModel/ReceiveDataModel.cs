using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    public enum ReceiveDataHandleStatus
    {
        [Description("未处理")]
        UnHandle = 1,
        [Description("已成功处理")]
        HandledSuccess = 4,
        [Description("处理异常")]
        HandleFailed = 5

    }
    /// <summary>
    /// 接收数据Model
    /// </summary>
    [SugarTable("T_BO_RECEIVEDATA", "")]
    public class ReceiveDataModel
    {
        public  ReceiveDataModel()
        {
            this.WhCode = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="methodParaValue">参数</param>
        /// <param name="source">source</param>
        public ReceiveDataModel(string methodName, string methodParaValue, string source,List<int> instructIds=null)
        {
            GuidId = Guid.NewGuid().ToString("N");
            HandleStatus = ReceiveDataHandleStatus.UnHandle;
            MethodName = methodName;
            MethodParamValue = methodParaValue;
            ReceiveDateTime = DateTime.Now;
            WhCode = SystemConfig.Instance.WhCode;
            Source = source;
            if (instructIds != null && instructIds.Count > 0)
            {
                InstructIds = string.Join(",", instructIds) + ",";
            }
        }
        /// <summary>
        /// guid
        /// </summary>
        [SugarColumn(ColumnName = "Rd_guid", Length = 32, IsPrimaryKey = true, DefaultValue = "NEWID()", IsNullable = false, ColumnDescription = "")]
        public string GuidId { get; set; }
        /// <summary>
        /// 源
        /// </summary>
        [SugarColumn(ColumnName = "Rd_source", Length = 255, IsNullable = true, ColumnDescription = "信息来源")]
        public string Source { get; set; }
        /// <summary>
        /// 库房编码
        /// </summary>
        [SugarColumn(ColumnName = "Wh_code", Length = 20, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "")]
        public string WhCode { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        [SugarColumn(ColumnName = "Dhstatus_id", DefaultValue = "1", IsNullable = false, ColumnDescription = "")]
        public ReceiveDataHandleStatus? HandleStatus { get; set; }
        /// <summary>
        /// wms指令Id
        /// </summary>
        [SugarColumn(ColumnName = "rd_instructIds", IsNullable = false, ColumnDescription = "wms指令Id")]
        public string InstructIds { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        [SugarColumn(ColumnName = "Rd_methodname", Length = 128, IsNullable = false, ColumnDescription = "")]
        public string MethodName { get; set; }
        /// <summary>
        /// 方法参数
        /// </summary>
        [SugarColumn(ColumnName = "Rd_paramvalue", Length = 8000, IsNullable = true, ColumnDescription = "")]
        public string MethodParamValue { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        [SugarColumn(ColumnName = "Rd_receivedate", DefaultValue = "getdate()", IsNullable = false, ColumnDescription = "")]
        public DateTime? ReceiveDateTime { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        [SugarColumn(ColumnName = "Rd_handlerdate", IsNullable = true, ColumnDescription = "")]
        public DateTime? HandleDateTime { get; set; }
        /// <summary>
        /// 处理消息
        /// </summary>
        [SugarColumn(ColumnName = "Rd_handlermessage", IsNullable = true, Length = 8000, ColumnDescription = "处理结果信息")]
        public string HandleMessage { get; set; }

        [SugarColumn(ColumnName = "Rd_note", Length = 128, IsNullable = true, ColumnDescription = "")]
        public string Note { get; set; }

    }
    /// <summary>
    /// 接收数据过滤
    /// </summary>
    public struct ReceiveDataFilter
    {
        /// <summary>
        /// 首页
        /// </summary>
        public long PageIndex { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public string HandleStatus { get; set; }
        /// <summary>
        /// 接收开始时间
        /// </summary>
        public string ReceiveFromTime { get; set; }
        /// <summary>
        /// 接收记录时间
        /// </summary>
        public string ReceiveToTime { get; set; }
        /// <summary>
        /// 处理开始时间
        /// </summary>
        public string HandleFromTime { get; set; }
        /// <summary>
        /// 处理结束时间
        /// </summary>
        public string HandleToTime { get; set; }
    }
}
