using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 任务指令DataModel
    /// </summary>
    [SugarTable("T_AC_TASKORDER", "")]
    public class TaskOrderDataModel
    {
        public TaskOrderDataModel()
        {
            this.WhCode = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 任务编号
        /// </summary>
        [SugarColumn(ColumnName = "TASK_CODE", Length = 255, IsNullable = false, ColumnDescription = "上层系统的任务编号")]
        public string TaskCode { get; set; }

        /// <summary>
        /// 指令编号
        /// </summary>
        [SugarColumn(ColumnName = "ORDER_ID", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "指令编号")]
        public int OrderId { get; set; }

        /// <summary>
        /// 设备任务号
        /// </summary>
        [SugarColumn(ColumnName = "DEVICE_TASK_CODE", IsNullable = true, ColumnDescription = "设备任务号")]
        public string DeviceTaskCode { get; set; }

        /// <summary>
        /// 库房编码
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "仓库编号")]
        public string WhCode
        {
            get;
            set;
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        [SugarColumn(ColumnName = "ADDTIME", IsNullable = false, ColumnDescription = "添加时间")]
        public DateTime AddTime { get; set; }
    

    }
}
