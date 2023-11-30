using CL.WCS.SystemConfigPckg.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Model
{
    [SugarTable("T_AC_STRINGCHARTASK", "")]
    public class StringCharTaskModel
    {
        public StringCharTaskModel()
        {
            this.WH_CODE = SystemConfig.Instance.WhCode;
        }
        public StringCharTaskModel(string taskCode)
        {
            UniqueCode = taskCode;
            this.WH_CODE = SystemConfig.Instance.WhCode;
        }
        /// <summary>
        /// 任务来源
        /// </summary>
        [SugarColumn(ColumnName = "TASK_SOURCE", IsNullable = false, ColumnDescription = "任务来源")]
        public int? TaskSource { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        /// <summary>
        /// 上层系统任务编号
        /// </summary>
        [SugarColumn(ColumnName = "UPPER_TASK_CODE", Length = 255, IsNullable = false, ColumnDescription = "上层系统关联任务编号")]
        public string UpperTaskCode { get; set; }

        [SugarColumn(ColumnName = "TASK_CODE", Length = 255, IsNullable = false, ColumnDescription = "任务编号唯一标识")]
        public string UniqueCode { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        [SugarColumn(ColumnName = "TASK_TYPE", IsNullable = false, ColumnDescription = "任务类型")]
        public int? TaskType { get; set; }

        /// <summary>
        /// 任务信息的Json值
        /// </summary>
        [SugarColumn(ColumnName = "TASK_VALUE", Length = 2024, IsNullable = false, ColumnDescription = "任务内容")]
        public string TaskValue { get; set; }

        /// <summary>
        /// 任务处理情况
        /// </summary>
        [SugarColumn(ColumnName = "TASK_PROCESS_STATUS", IsNullable = false, ColumnDescription = "任务处理状态")]
        public int? ProcessStatus { get; set; }

        /// <summary>
        /// 任务归属的设备Id
        /// </summary>
        [SugarColumn(ColumnName = "BELONG_DEVICE_ID", IsNullable = true, ColumnDescription = "任务归属的当前设备编号")]
        public int? DeviceId { get; set; }
        /// <summary>
        /// 下层系统任务号
        /// </summary>
        [SugarColumn(ColumnName = "LOWER_TASK_CODE", Length = 255, IsNullable = false, ColumnDescription = "下层系统关联任务编号")]
        public string LowerTaskCode { get; set; }


        /// <summary>
        /// 库房编号
        /// </summary>
        [SugarColumn(ColumnName = "WH_CODE", Length = 255, IsNullable = false, ColumnDescription = "库房编号")]
        public string WH_CODE { get; set; }
    }
}
