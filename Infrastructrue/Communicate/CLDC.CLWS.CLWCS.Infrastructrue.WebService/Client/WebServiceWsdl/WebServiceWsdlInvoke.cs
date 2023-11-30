using System;
using System.Collections;
using System.Text;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
    public sealed class WebServiceWsdlInvoke : IWebNetInvoke
    {
        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        private string _logDisplayName="WebService调用接口";
        private CommunicationModeEnum _communicationMode= CommunicationModeEnum.Production;

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

        public OperateResult<string> ServiceRequest<T>(string url, string methodName, params object[] cmd) where T : IResponse, new()
        {
            StringBuilder cmdString = new StringBuilder();
            if (cmd != null && cmd.Length > 0)
            {
                foreach (object para in cmd)
                {
                    cmdString.Append(para.ToString());
                }
            }
            OperateResult<string> reponseResult = new OperateResult<string>();
            if (CommunicationMode== CommunicationModeEnum.Production)
            {
                try
                {
                    object reponse = WebServiceHelper.InvokeWebServiceDynamic(url, methodName, cmd);
                    reponseResult.Content = reponse.ToString();
                    reponseResult.IsSuccess = true;
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 成功,返回参数：{3}", url, methodName, cmdString.ToString(), reponseResult.Content);
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    cmdString.Clear();
                    return reponseResult;
                }
                catch (Exception ex)
                {
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 发生异常：{3}", url, methodName, cmdString.ToString(), OperateResult.ConvertException(ex));
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    reponseResult.Message = OperateResult.ConvertExMessage(ex);
                    reponseResult.IsSuccess = false;
                }
                return reponseResult;
            }
            if (CommunicationMode == CommunicationModeEnum.Automatic)
            {
                T response = new T();
                string msg = string.Format("[自动返回] 请求信息：Url：{0} 方法名：{1} 参数：{2} 返回结果：{3}", url, methodName, cmd,response.ToSuccessMsg());
                LogMessage(msg, EnumLogLevel.Info, true);
                reponseResult.IsSuccess = true;
                reponseResult.Message = msg;
                reponseResult.Content = response.ToSuccessMsg();
                return reponseResult;
            }
            LogMessage("模式不正确", EnumLogLevel.Error, true);
            return reponseResult;
           
        }

        public OperateResult<string> ServiceRequest<T>(string url, string methodName, string cmd, Hashtable hearder = null) where T : IResponse, new()
        {
            OperateResult<string> reponseResult = new OperateResult<string>();
            if (CommunicationMode == CommunicationModeEnum.Production)
            {
                try
                {
                    object reponse = WebServiceHelper.InvokeWebServiceDynamic(url, methodName, new object[] { cmd });
                    reponseResult.Content = reponse.ToString();
                    reponseResult.IsSuccess = true;
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 成功,返回参数：{3}", url, methodName, cmd, reponseResult.Content);
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    return reponseResult;
                }
                catch (Exception ex)
                {
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 发生异常：{3}", url, methodName, cmd, OperateResult.ConvertException(ex));
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    reponseResult.Message = OperateResult.ConvertExMessage(ex);
                    reponseResult.IsSuccess = false;
                }
                return reponseResult;
            }
            if (CommunicationMode == CommunicationModeEnum.Automatic)
            {
                T response = new T();
                string msg = string.Format("[自动返回] 请求信息：Url：{0} 方法名：{1} 参数：{2} 返回结果：{3}", url, methodName, cmd,response.ToSuccessMsg());
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
            StringBuilder cmdString = new StringBuilder();
            if (paramHashtable != null && paramHashtable.Count > 0)
            {
                foreach (object para in paramHashtable.Values)
                {
                    cmdString.Append(para);
                }
            }
            OperateResult<string> reponseResult = new OperateResult<string>();
            if (CommunicationMode == CommunicationModeEnum.Production)
            {
                try
                {
                    object reponse = WebServiceHelper.InvokeWebServiceDynamic(url, methodName, new object[] { cmdString.ToString() });
                    reponseResult.Content = reponse.ToString();
                    reponseResult.IsSuccess = true;
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 成功,返回参数：{3}", url, methodName, HashtableConvertToString(paramHashtable), reponseResult.Content);
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    if (paramHashtable != null) paramHashtable.Clear();
                    return reponseResult;
                }
                catch (Exception ex)
                {
                    string logMessange = string.Format("调用：{0} 的方法：{1} 参数：{2} 发生异常：{3}", url, methodName, HashtableConvertToString(paramHashtable), OperateResult.ConvertException(ex));
                    LogMessage(logMessange, EnumLogLevel.Info, true);
                    reponseResult.Message = OperateResult.ConvertExMessage(ex);
                    reponseResult.IsSuccess = false;
                }
                return reponseResult;
            }
            if (CommunicationMode == CommunicationModeEnum.Automatic)
            {
                T response=new T();
                string msg = string.Format("[自动返回] 请求信息：Url：{0} 方法名：{1} 参数：{2} 返回结果：{3}", url, methodName, HashtableConvertToString(paramHashtable),response.ToSuccessMsg());
                LogMessage(msg, EnumLogLevel.Info, true);
                reponseResult.IsSuccess = true;
                reponseResult.Message = msg;
                reponseResult.Content = response.ToSuccessMsg();
                return reponseResult;
            }
            LogMessage("模式不正确", EnumLogLevel.Error, true);
            return reponseResult;
        }
        private string HashtableConvertToString(Hashtable hashTable)
        {
            StringBuilder message=new StringBuilder();
            foreach (object key in hashTable.Keys)
            {
                string temp = string.Format("{0}:{1}", key, hashTable[key]);
                message.Append(temp);
            }
            return message.ToString();
        }
    }
}
