using System;
using System.Collections;
using System.Text;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
    public sealed class WebApiInvoke : IWebNetInvoke
    {

        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        private string _logDisplayName = "WebApi服务接口";
        private CommunicationModeEnum _communicationMode = CommunicationModeEnum.Production;


        public int TimeOut { get; set; }

        public string LogDisplayName
        {
            get { return _logDisplayName; }
            set { _logDisplayName = value; }
        }

        public CommunicationModeEnum CommunicationMode
        {
            get { return _communicationMode; }
            set { _communicationMode = value; }
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        private void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            LogHelper.WriteLog(this.LogDisplayName, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }

        public OperateResult<string> ServiceRequest<T>(string url, string methodName, params object[] cmd) where T : IResponse, new()
        {
            OperateResult<string> reponseResult = new OperateResult<string>();
            StringBuilder cmdString = new StringBuilder();
            if (cmd != null && cmd.Length > 0)
            {
                foreach (object para in cmd)
                {
                    cmdString.Append(para);
                }
            }
            if (CommunicationMode == CommunicationModeEnum.Production)
            {
                try
                {

                    string urlMethod = url + "/" + methodName;
                    reponseResult = HttpHelper.Post(urlMethod, cmdString.ToString());
                    //reponseResult = HttpHelper.CreateHttpResponse(urlMethod,true, cmdString.ToString());
                    string logMessange;
                    if (reponseResult.IsSuccess)
                    {
                        logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 成功,返回参数：{3}", url, methodName, cmdString.ToString(), reponseResult.Content);
                    }
                    else
                    {
                        logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 失败,返回参数：{3}", url, methodName, cmdString.ToString(), reponseResult.Message);
                    }
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    return reponseResult;
                }
                catch (Exception ex)
                {
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 失败,异常信息：{3}", url, methodName, cmdString, OperateResult.ConvertException(ex));
                    reponseResult.Message = OperateResult.ConvertExMessage(ex);
                    reponseResult.IsSuccess = false;
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                }
                return reponseResult;
            }
            if (CommunicationMode == CommunicationModeEnum.Automatic)
            {
                T response = new T();
                string msg = string.Format("[自动返回] 请求信息：Url：{0} 方法名：{1} 参数：{2} 返回结果：{3}", url, methodName, cmdString, response.ToSuccessMsg());
                LogMessage(msg, EnumLogLevel.Info, true);
                reponseResult.IsSuccess = true;
                reponseResult.Message = msg;
                reponseResult.Content = response.ToSuccessMsg();
                return reponseResult;
            }
            LogMessage("模式不正确", EnumLogLevel.Error, true);
            return reponseResult;

        }

        public OperateResult<string> ServiceRequest<T>(string url, string methodName, Hashtable paramHashtable) where T : IResponse, new()
        {
            OperateResult<string> reponseResult = new OperateResult<string>();
            StringBuilder cmdString = new StringBuilder();

            foreach (object para in paramHashtable.Keys)
            {
                string param = string.Format("{0}={1}", para, paramHashtable[para]);
                cmdString.Append(param + "&");
            }
            if (CommunicationMode == CommunicationModeEnum.Production)
            {
                try
                {
                    Hashtable hearders = new Hashtable();
                    hearders.Add("Content-Type", "application/x-www-form-urlencoded");
                    string urlMethod = url + "/" + methodName;
                    reponseResult = HttpHelper.Post(urlMethod, cmdString.ToString(), hearders);
                    string logMessange;
                    if (reponseResult.IsSuccess)
                    {
                        logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 成功,返回参数：{3}", url, methodName,
                            cmdString.ToString(), reponseResult.Content);
                        reponseResult.IsSuccess = true;
                    }
                    else
                    {
                        logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 失败,返回参数：{3}", url, methodName,
                            cmdString.ToString(), reponseResult.Message);
                    }
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    return reponseResult;
                }
                catch (Exception ex)
                {
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 失败,异常信息：{3}", url, methodName, cmdString, OperateResult.ConvertException(ex));
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    reponseResult.Message = OperateResult.ConvertExMessage(ex);
                    reponseResult.IsSuccess = false;
                }
                return reponseResult;
            }
            if (CommunicationMode == CommunicationModeEnum.Automatic)
            {
                T response = new T();
                string msg = string.Format("[自动返回] 请求信息：Url：{0} 方法名：{1} 参数：{2}  返回结果：{3}", url, methodName, cmdString, response.ToSuccessMsg());
                LogMessage(msg, EnumLogLevel.Info, true);
                reponseResult.IsSuccess = true;
                reponseResult.Message = msg;
                reponseResult.Content = response.ToSuccessMsg();
                return reponseResult;
            }
            LogMessage("模式不正确", EnumLogLevel.Error, true);
            return reponseResult;
        }


        public OperateResult<string> ServiceRequest<TResponse>(string url, string methodName, string cmd, Hashtable hearder = null) where TResponse : IResponse, new()
        {
            OperateResult<string> reponseResult = new OperateResult<string>();
            if (CommunicationMode == CommunicationModeEnum.Production)
            {
                try
                {
                    string urlMethod = url + "/" + methodName;
                    if (SystemConfig.Instance.WhCode.Equals("SNDL1"))
                    {
                        reponseResult = HttpHelper.CreateHttpResponse(urlMethod, true, cmd);
                    }
                    else
                    {
                        reponseResult = HttpHelper.Post(urlMethod, cmd, hearder);
                    }
                 

                    string logMessange;
                    if (reponseResult.IsSuccess)
                    {
                        logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 成功,返回参数：{3}", url, methodName, cmd,
                            reponseResult.Content);
                    }
                    else
                    {
                        logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 失败,返回参数：{3}", url, methodName, cmd,
                            reponseResult.Message);
                    }
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    return reponseResult;
                }
                catch (Exception ex)
                {
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 失败,异常信息：{3}", url, methodName, cmd, OperateResult.ConvertException(ex));
                    reponseResult.Message = OperateResult.ConvertExMessage(ex);
                    reponseResult.IsSuccess = false;
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                }
                return reponseResult;
            }
            if (CommunicationMode == CommunicationModeEnum.Automatic)
            {
                TResponse response = new TResponse();
                string msg = string.Format("[自动返回] 请求信息：Url：{0} 方法名：{1} 参数：{2} 返回信息：{3}", url, methodName, cmd, response.ToSuccessMsg());
                LogMessage(msg, EnumLogLevel.Info, true);
                reponseResult.IsSuccess = true;
                reponseResult.Message = msg;
                reponseResult.Content = response.ToSuccessMsg();

                if (SystemConfig.Instance.WhCode.Equals("TianJing01"))
                {
                    reponseResult.Content = "true";
                }

                return reponseResult;
            }
            LogMessage("模式不正确", EnumLogLevel.Error, true);
            return reponseResult;
        }

    }
}
