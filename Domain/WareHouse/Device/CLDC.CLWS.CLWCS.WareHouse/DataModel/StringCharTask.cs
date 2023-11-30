using System.ComponentModel;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    public class StringCharTask : IDeviceTaskContent
    {
        public StringCharTask()
        {

        }
        public StringCharTask(string taskCode)
        {
            UniqueCode = taskCode;
        }
        /// <summary>
        /// 任务来源
        /// </summary>
        public TaskSourceEnum TaskSource { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        /// <summary>
        /// 上层系统任务编号
        /// </summary>
        public string UpperTaskCode { get; set; }

        public string UniqueCode { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public StringCharTaskTypeEnum TaskType { get; set; }

        /// <summary>
        /// 任务信息的Json值
        /// </summary>
        public string TaskValue { get; set; }

        /// <summary>
        /// 任务处理情况
        /// </summary>
        public TaskProcessStatus ProcessStatus { get; set; }

        /// <summary>
        /// 任务归属的设备Id
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// 下层系统任务号
        /// </summary>
        public string LowerTaskCode { get; set; }

        public override string ToString()
        {
            return this.ToJson();
        }
        /// <summary>
        /// 任务Model
        /// </summary>
        public StringCharTaskModel DatabaseMode
        {
            get
            {
                StringCharTaskModel mode = new StringCharTaskModel()
                {
                   DeviceId=this.DeviceId,
                   LowerTaskCode=this.LowerTaskCode,
                   ProcessStatus=(int)this.ProcessStatus,
                   TaskSource=(int)this.TaskSource,
                   TaskType=(int)this.TaskType,
                   TaskValue=this.TaskValue,
                   UniqueCode=this.UniqueCode,
                   UpperTaskCode=this.UpperTaskCode
                };
                return mode;
            }
        }

    }


    /// <summary>
    /// StringCharTaskType 枚举
    /// </summary>
    public enum StringCharTaskTypeEnum
    {
        [Description("叠垛")]
        Stack,
        [Description("拆垛")]
        UnStack,
        [Description("关门")]
        CloseDoor,
        [Description("开门")]
        OpenDoor,
        [Description("扫描")]
        Scan,
        [Description("显示信息")]
        ShowMessage,
        [Description("调度栈板车")]
        SendStackDispatch,
        [Description("查询栈板车缓存状态")]
        AskStackLoadedStatus,
        [Description("申请栈板车专机取/放")]
        AskStackAction,
        [Description("其它")]
        Other,
    }
    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum TaskProcessStatus
    {
        [Description("创建")]
        Create = 0,
        [Description("正在处理")]
        Process = 1,
        [Description("已下发设备")]
        Sended = 2,
        [Description("已完成")]
        Finished = 3,
        [Description("异常")]
        Exception = 4,
    }

}
