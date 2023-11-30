using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CommonTaskCore
{
    /// <summary>
    /// 任务
    /// </summary>
    public sealed class TaskContext
    {
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescription { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public int TaskType { get; set; }
        /// <summary>
        /// 步骤列表
        /// </summary>
        public List<FunctionStep> FunctionList { get; set; }

    }
    /// <summary>
    /// 步骤
    /// </summary>
    public sealed class FunctionStep
    {
        /// <summary>
        /// 步骤ID
        /// </summary>
        public int StepId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// 结果列表
        /// </summary>
        public List<ResultInfo> ResultProcessList { get; set; }

    }
    /// <summary>
    /// 结果信息
    /// </summary>
    public sealed class ResultInfo
    {
        /// <summary>
        /// 结果
        /// </summary>
        public int ResultValue { get; set; }
        /// <summary>
        /// 结果描述
        /// </summary>
        public string ResultDescription { get; set; }
        /// <summary>
        /// 下一个步骤
        /// </summary>
        public int NextStep { get; set; }
        /// <summary>
        /// 下一个参数对象
        /// </summary>

        public object NextParam { get; set; }

    }
    /// <summary>
    /// 方法
    /// </summary>
    public sealed class MethodInfo
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Param { get; set; }
    }
    /// <summary>
    /// 执行状态枚举
    /// </summary>
    public enum ExecuteState
    {
        
    }

}
