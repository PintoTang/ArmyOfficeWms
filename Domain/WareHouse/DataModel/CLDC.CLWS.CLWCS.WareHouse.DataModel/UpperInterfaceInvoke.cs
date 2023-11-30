using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 上层接口
    /// </summary>
    [SugarTable("T_BO_UPPERINTERFACEINVOKE", "")]
    public class UpperInterfaceInvoke
    {
        public UpperInterfaceInvoke()
        {
            this.WhCode= SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 库房编码
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "")]
        public string WhCode
        {
            get;set;
        }

        /// <summary>
        /// guid唯一编号
        /// </summary>
        [SugarColumn(ColumnName = "GUID_ID", Length = 50, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "")]
        public string GuidId { get; set; }

        /// <summary>
        /// 接口的条码
        /// </summary>
        [SugarColumn(ColumnName = "BARCODE", Length = 255, IsNullable = true, ColumnDescription = "")]
        public string Barcode { get; set; }

        /// <summary>
        /// 接口方法名称
        /// </summary>
        [SugarColumn(ColumnName = "METHODNAME", Length = 100, IsNullable = false, ColumnDescription = "")]
        public string MethodName { get; set; }
        /// <summary>
        /// 处理的业务名称
        /// </summary>
        [SugarColumn(ColumnName = "BUSINESSNAME", Length = 100, IsNullable = false, ColumnDescription = "")]
        public string BusinessName { get; set; }
        /// <summary>
        /// 接口调用后的回调函数
        /// </summary>
        [SugarColumn(ColumnName = "CALLBACKFUNC", Length = 100, IsNullable = true, ColumnDescription = "")]
        public string CallBackFuncName { get; set; }
        /// <summary>
        /// 调用次数
        /// </summary>
        [SugarColumn(ColumnName = "INVOKETIME", DefaultValue = "0", IsNullable = true, ColumnDescription = "")]
        public int? InvokeTime { get; set; }
        /// <summary>
        /// 失败重复调用的最大次数
        /// </summary>
        [SugarColumn(ColumnName = "MAXTIME", DefaultValue = "5", IsNullable = true, ColumnDescription = "")]
        public int? MaxTime { get; set; }
        /// <summary>
        /// 调用的最终结果
        /// </summary>
        [SugarColumn(ColumnName = "RESULT", IsNullable = true, ColumnDescription = "")]
        public int? Result { get; set; }
        /// <summary>
        /// 调用的超时时间
        /// </summary>
        [SugarColumn(ColumnName = "INVOKETIMEOUT", IsNullable = true, ColumnDescription = "")]
        public int? TimeOut { get; set; }
        /// <summary>
        /// 调用接口耗时
        /// </summary>
        [SugarColumn(ColumnName = "INVOKEDELAY", IsNullable = true, ColumnDescription = "")]
        public int? InvokeDelay { get; set; }
        /// <summary>
        /// 调用后的信息
        /// </summary>
        [SugarColumn(ColumnName = "MESSAGE", Length = 4000, IsNullable = true, ColumnDescription = "条码")]
        public string Message { get; set; }
        /// <summary>
        /// 调用状态
        /// </summary>
        [SugarColumn(ColumnName = "INVOKESTATUS", IsNullable = true, ColumnDescription = "")]
        public InvokeStatusMode? InvokeStatus { get; set; }
        /// <summary>
        /// 第一次调用接口的时间
        /// </summary>
        [SugarColumn(ColumnName = "FIRSTINVOKEDATETIME", IsNullable = true, ColumnDescription = "")]
        public DateTime? FirstInvokDateTime { get; set; }
        /// <summary>
        /// 最后一次调用接口的时间
        /// </summary>
        [SugarColumn(ColumnName = "INVOKEFINISHDATETIME", IsNullable = true, ColumnDescription = "")]
        public DateTime? InvokeFinishDateTime { get; set; }
        /// <summary>
        /// 调用接口的参数列表
        /// </summary>
        [SugarColumn(ColumnName = "INVOKEPARAMETERS", Length = 8000, IsNullable = true, ColumnDescription = "")]
        public string Parameters { get; set; }
        /// <summary>
        /// 接口信息的添加时间
        /// </summary>
        [SugarColumn(ColumnName = "ADDDATE", IsNullable = true, ColumnDescription = "")]
        public DateTime? AddDateTime { get; set; }


    }

    public static class UpperInterfaceInvokeEx
    {
        public static NotifyElement ConverToNotifyElement(this UpperInterfaceInvoke databaseMode)
        {
            NotifyElement notify = new NotifyElement(databaseMode.Barcode, databaseMode.MethodName, databaseMode.BusinessName, null, databaseMode.Parameters);
            notify.GuidId = databaseMode.GuidId;
            notify.InvokeTime = (int)databaseMode.InvokeTime;
            notify.MaxTime = (int)databaseMode.MaxTime;
            return notify;
        }
    }

    /// <summary>
    /// 上层接口 过滤对象类
    /// </summary>
    public struct UpperInterfaceInvokeFilter
    {
        /// <summary>
        /// 页索引
        /// </summary>
        public long PageIndex { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>
        public string HandleResult { get; set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public string HandleStatus { get; set; }

       
        /// <summary>
        /// 开始时间
        /// </summary>
        public string HandleFromTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string HandleToTime { get; set; }
    }
  
}
