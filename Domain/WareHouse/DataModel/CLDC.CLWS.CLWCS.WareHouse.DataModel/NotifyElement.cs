using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    /// <summary>
    /// 接口调用的最终结果
    /// </summary>
    public enum InvokeStatusMode
    {
        /// <summary>
        /// 还没调用
        /// </summary>
        [Description("未调用完成")]
        UnInvoke = 0,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Failed = 2,
        /// <summary>
        /// 丢弃
        /// </summary>
        [Description("丢弃")]
        Abord = 3,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Exception = 4,
        /// <summary>
        /// 处理中
        /// </summary>
        [Description("处理中")]
        Process = 5
    }

    public enum InvokeResultMode
    {
        [Description("调用失败")]
        Failed = 0,
        [Description("调用成功")]
        Success = 1
    }


    /// <summary>
    /// 通知接口服务的返回委托
    /// </summary>
    /// <param name="invokeResult"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate OperateResult NotifyCallBack(OperateResult invokeResult, object parameters);


    /// <summary>
    /// 上报接口的要素
    /// </summary>
    public class NotifyElement
    {
        private int maxTime = 1;
        private long timeOut = 20 * 60 * 1000;

        public NotifyElement(string barcode, string methodeName, string businessName, NotifyCallBack callbackFunc, params object[] parameters)
        {
            this.Barcode = barcode;
            GuidId = Guid.NewGuid().ToString("N");
            this.MethodName = methodeName;
            this.BusinessName = businessName;
            this.CallBackFunc = callbackFunc;
            this.Parameters = parameters;
            this.FirstInvokeDatetime = DateTime.Now;
            this.InvokeFinishDatetime = DateTime.Now;
            AddTime = DateTime.Now;
        }

        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 接口调用唯一标识
        /// </summary>
        public string GuidId { get; set; }

        /// <summary>
        /// 接口调用的参数
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        /// 上报接口方法名称
        /// </summary>
        [Description("上报接口方法名称")]
        public string MethodName { get; set; }

        /// <summary>
        /// 接口业务名称
        /// </summary>
        [Description("接口业务名称")]
        public string BusinessName { get; set; }

        /// <summary>
        /// 接口调用成功后的回调方法
        /// </summary>
        [Description("接口调用后的回调方法")]
        public NotifyCallBack CallBackFunc { get; set; }

        /// <summary>
        /// 方法调用的次数
        /// </summary>
        [Description("接口调用的次数")]
        public int InvokeTime { get; set; }

        /// <summary>
        /// 方法调用的最大次数，超过此次数不再自动调用
        /// </summary>
        [Description("方法调用的最大次数")]
        public int MaxTime
        {
            get { return maxTime; }
            set { maxTime = value; }
        }

        /// <summary>
        /// 接口执行结果
        /// </summary>
        [Description("接口执行结果")]
        public InvokeResultMode Result { get; set; }

        /// <summary>
        /// 方法超时时间 单位毫秒
        /// </summary>
        [Description("超时时间")]
        public long TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        /// <summary>
        /// 调用耗时 单位毫秒
        /// </summary>
        [Description("调用接口耗时")]
        public long InvokeDelay { get; set; }

        /// <summary>
        /// 接口调用结果的信息提示
        /// </summary>
        [Description("接口调用结果的信息提示")]
        public string Message { get; set; }

        /// <summary>
        /// 接口调用的最终结果
        /// </summary>
        [Description("接口调用的最终结果")]
        public InvokeStatusMode InovkeResult { get; set; }

        /// <summary>
        /// 首次调用时间
        /// </summary>
        public DateTime FirstInvokeDatetime { get; set; }

        /// <summary>
        /// 调用结束时间
        /// </summary>
        public DateTime InvokeFinishDatetime { get; set; }

        public string ParameterToString()
        {
            StringBuilder msg = new StringBuilder();
            if (Parameters==null)
            {
                return string.Empty;
            }
            foreach (var parameter in Parameters)
            {
                msg.Append(parameter.ToString());
            }
            return msg.ToString();
        }

        public override string ToString()
        {
            return string.Format("接口名称：{0} 接口参数：{1}",MethodName,ParameterToString());
        }

        public DateTime AddTime { get; set; }

    }

    public static class NotifyElementEx
    {
        public static UpperInterfaceInvoke ConverToDatabaseMode(this NotifyElement notifyElement)
        {
            UpperInterfaceInvoke invoke = new UpperInterfaceInvoke();
            invoke.Message = notifyElement.Message;
            invoke.Barcode = notifyElement.Barcode;
            invoke.AddDateTime = notifyElement.AddTime;
            invoke.FirstInvokDateTime = notifyElement.FirstInvokeDatetime;
            invoke.InvokeDelay = (int)notifyElement.InvokeDelay;
            invoke.InvokeFinishDateTime = notifyElement.InvokeFinishDatetime;
            invoke.InvokeStatus = notifyElement.InovkeResult;
            invoke.Parameters = notifyElement.ParameterToString();
            invoke.Result = (int)notifyElement.Result;
            invoke.TimeOut = (int)notifyElement.TimeOut;
            invoke.BusinessName = notifyElement.BusinessName;
            invoke.CallBackFuncName = notifyElement.CallBackFunc == null ? "无回调" : notifyElement.CallBackFunc.Method.Name;
            invoke.MaxTime = notifyElement.MaxTime;
            invoke.InvokeTime = notifyElement.InvokeTime;
            invoke.GuidId = notifyElement.GuidId;
            invoke.MethodName = notifyElement.MethodName;
            return invoke;
        }
    }
}
