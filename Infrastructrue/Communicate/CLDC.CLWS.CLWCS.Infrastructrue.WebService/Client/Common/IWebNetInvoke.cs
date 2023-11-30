using System.Collections;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common
{
    public interface IWebNetInvoke
    {

        int TimeOut { get; set; }

        /// <summary>
        /// 日志显示名字
        /// </summary>
        string LogDisplayName { get; set; }

        /// <summary>
        /// 通讯的模式
        /// </summary>
        CommunicationModeEnum CommunicationMode { get; set; }

        ///// <summary>
        ///// 初始化
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="logDisplayName"></param>
        ///// <param name="mode"></param>
        ///// <returns></returns>
        //OperateResult Initilize(string url,string logDisplayName,CommunicationModeEnum mode);

        /// <summary>
        /// 请求服务的接口
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="methodName">方法名 区分大小写</param>
        /// <param name="cmd">Json参数</param>
        /// <returns></returns>
        OperateResult<string> ServiceRequest<T>(string url, string methodName, params object[] cmd)
            where T : IResponse, new();



        /// <summary>
        /// 请求服务接口，可带请求头
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodName"></param>
        /// <param name="cmd"></param>
        /// <param name="hearder"></param>
        /// <returns></returns>
        OperateResult<string> ServiceRequest<T>(string url, string methodName, string cmd,Hashtable hearder = null) where T : IResponse,new();

        /// <summary>
        ///     请求服务的接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="methodName"></param>
        /// <param name="paramHashtable"></param>
        /// <returns></returns>
        OperateResult<string> ServiceRequest<T>(string url, string methodName, Hashtable paramHashtable) where  T:IResponse,new();

    }
}
