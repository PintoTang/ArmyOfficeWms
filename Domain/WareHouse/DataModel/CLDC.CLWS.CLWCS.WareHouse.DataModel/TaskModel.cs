using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 任务Model
    /// </summary>
    [SugarTable("T_AC_TASK", "")]
    public class TaskModel
    {
        public TaskModel()
        {
            this.WhCode = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 任务编码
        /// </summary>
        [SugarColumn(ColumnName = "TASK_CODE", Length = 255, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "")]
        public string TaskCode { get; set; }
        /// <summary>
        /// 任务执行状态
        /// </summary>
        [SugarColumn(ColumnName = "TASK_PROCESS_STATUS", IsNullable = false, ColumnDescription = "")]
        public TaskRunStatusEnum TaskProcessStatus { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        [SugarColumn(ColumnName = "TASK_TYPE", IsNullable = false, ColumnDescription = "")]
        public TaskTypeEnum TaskType { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        [SugarColumn(ColumnName = "ADDTIME", IsNullable = true, ColumnDescription = "")]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 库房编码
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = true, ColumnDescription = "仓库编号")]
        public string WhCode { get; set; }
    }

    /// <summary>
    /// 任务类型 枚举
    /// </summary>
    public enum TaskTypeEnum
    {
        [Description("入库任务")]
        InStorehouse = 1,
        [Description("出库任务")]
        OutStorehouse = 2,
        [Description("盘点任务")]
        InventoryStore = 3
    }

    /// <summary>
    /// 任务运行状态枚举
    /// </summary>
    public enum TaskRunStatusEnum
    {
        [Description("任务进行中")]
        Doing = 1,
        [Description("任务已结束")]
        Finished = 2,
    }

   



}
