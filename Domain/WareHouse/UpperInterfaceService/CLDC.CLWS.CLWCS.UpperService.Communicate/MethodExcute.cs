using System;
using System.Reflection;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.UpperService.Communicate
{
    /// <summary>
    /// 通讯
    /// </summary>
    public abstract class MethodExcute
    {
        private CommunicationModeEnum _communicationMode= CommunicationModeEnum.Production;

        /// <summary>
        /// 对接协议的调用接口
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public OperateResult<object> Invoke(NotifyElement element)
        {
            OperateResult<object> invokeResult = new OperateResult<object>();
            Type type = GetType();
            try
            {
                MethodInfo method = type.GetMethod(element.MethodName);
                if (method == null)
                {
                    invokeResult.IsSuccess = false;
                    invokeResult.Message = string.Format("在类型：{0} 中获取方法：{1} 失败", type.FullName, element.MethodName);
                    return invokeResult;
                }
                object result = method.Invoke(this, element.Parameters);
                if (result == null)
                {
                    invokeResult.IsSuccess = false;
                    invokeResult.Message = string.Format("调用方法：{0} 失败，参数:  {1} ", element.MethodName, "返回参数为null");
                    return invokeResult;
                }
                invokeResult.IsSuccess = true;
                invokeResult.Message = string.Format("调用方法：{0} 成功", element.MethodName);
                invokeResult.Content = result;
            }
            catch (Exception ex)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = string.Format("调用方法：{0} 发生异常  异常详情：{1}", element.MethodName, OperateResult.ConvertExMessage(ex));
            }
            return invokeResult;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public OperateResult Initialize(string url,int timeOut,CommunicationModeEnum communicationMode)
        {
            this.WebserviceUrl = url;
            this.TimeOut = timeOut;
            this.CommunicationMode = communicationMode;
            OperateResult initConfigResult= InitConfig();
            if (!initConfigResult.IsSuccess)
            {
                return initConfigResult;
            }
            OperateResult particularResult = ParticularInitilize();
            if (!particularResult.IsSuccess)
            {
                return particularResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 特有初始化
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult ParticularInitilize();

        private OperateResult InitConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// webserviceUrl地址
        /// </summary>
        public string WebserviceUrl { get; set; }

        public string NameSpace { get; set; }
        public string ClassName { get; set; }

        public string Name { get; set; }

        public CommunicationModeEnum CommunicationMode
        {
            get { return _communicationMode; }
            set { _communicationMode = value; }
        }
    }
}
