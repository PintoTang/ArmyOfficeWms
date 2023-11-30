using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CommonTaskCore
{
    /// <summary>
    /// 单任务处理
    /// </summary>
    public class TaskProcess
    {
        /// <summary>
        /// 任务号
        /// </summary>
        public string TaskSerialNo { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescription { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public int TaskType { get; set; }
        /// <summary>
        /// 当前步骤
        /// </summary>
        public int CurStep { get; set; }
        /// <summary>
        /// 请求
        /// </summary>
        public object Request { get; set; }
        /// <summary>
        /// 返回
        /// </summary>
        public object Response { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public TaskContext Context { get; set; }
        /// <summary>
        /// 通过步骤编号获取对应的步骤信息
        /// </summary>
        /// <param name="stepId"></param>
        /// <returns>OperateResult<FunctionStep></returns>
        public OperateResult<FunctionStep> GetMethodInfoByStepId(int stepId)
        {
            OperateResult<FunctionStep> getResult = OperateResult.CreateFailedResult<FunctionStep>("无数据");
            try
            {
                if (Context != null)
                {
                    if (Context.FunctionList != null)
                    {
                        FunctionStep singleResult = Context.FunctionList.Single(f => f.StepId.Equals(stepId));
                        getResult.Content = singleResult;
                        getResult.IsSuccess = true;
                        getResult.Message = "获取成功";
                        return getResult;
                    }
                    getResult.Message = "任务上下文的方法列表为空";
                    return getResult;
                }
                getResult.Message = "当前任务上下文为空";
                return getResult;
            }
            catch (InvalidOperationException signeEx)
            {
                getResult.Message = string.Format("获取失败：{0}", signeEx.Message);
            }
            return getResult;
        }
        /// <summary>
        /// 通过当前方法编号及其返回的结果获取下一个方法信息
        /// </summary>
        /// <param name="stepId"></param>
        /// <param name="methodResult"></param>
        /// <returns></returns>
        public OperateResult<FunctionStep> GetNextMethodInfo(int stepId, int methodResult)
        {
            OperateResult<FunctionStep> getResult = OperateResult.CreateFailedResult<FunctionStep>("无数据");
            try
            {
                OperateResult<FunctionStep> getCurMethodResult = GetMethodInfoByStepId(stepId);
                if (!getCurMethodResult.IsSuccess)
                {
                    getResult.Message = string.Format("根据当前步骤：{0} 获取当前方法失败", stepId);
                    return getResult;
                }
                FunctionStep curMethod = getCurMethodResult.Content;
                ResultInfo resultInfo = curMethod.ResultProcessList.Single(i => i.ResultValue.Equals(methodResult));
                OperateResult<FunctionStep> getNextMethodResult = GetMethodInfoByStepId(resultInfo.NextStep);
                if (!getNextMethodResult.IsSuccess)
                {
                    getResult.Message = getNextMethodResult.Message;
                    return getResult;
                }
                return getNextMethodResult;
            }
            catch (InvalidOperationException invalidEx)
            {
                getResult.Message = invalidEx.Message;
            }
            return getResult;
        }
        /// <summary>
        /// 调用步骤
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public OperateResult<int> InvokeMethod(FunctionStep function)
        {
            OperateResult<int> invokeResult = OperateResult.CreateFailedResult(-1, "初始化");
            try
            {
                var type = Type.GetType(function.Method.NameSpace + "." + function.Method.ClassName);
                if (type == null)
                {
                    invokeResult.Message = string.Format("根据：{0} {1} 获取类型失败", function.Method.NameSpace, function.Method.ClassName);
                    return invokeResult;
                }
                object instance = Activator.CreateInstance(type);
                System.Reflection.MethodInfo method = type.GetMethod(function.Method.MethodName);
                if (method == null)
                {
                    invokeResult.Message = string.Format("根据方法名：{0} 获取方法失败", function.Method.MethodName);
                    return invokeResult;
                }
                object methodReturn = method.Invoke(instance, new object[] { function.Method.Param });
                if (methodReturn is OperateResult<int>)
                {
                    invokeResult = methodReturn as OperateResult<int>;
                    return invokeResult;
                }
                if (methodReturn is int)
                {
                    invokeResult.Content = int.Parse(methodReturn.ToString());
                    invokeResult.IsSuccess = true;
                    return invokeResult;
                }

                invokeResult.IsSuccess = false;
                invokeResult.Message = string.Format("方法：{0}.{1}.{2} 返回的参数：{3} 格式不正确", function.Method.NameSpace, function.Method.ClassName, function.Method.MethodName, methodReturn);
                return invokeResult;
            }
            catch (Exception ex)
            {
                invokeResult.Message = OperateResult.ConvertException(ex);
                invokeResult.IsSuccess = false;
                invokeResult.Content = -1;
            }
            return invokeResult;

        }
    }
}
