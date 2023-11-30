using System.Collections;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.WebApi
{
    public class WebApiInvokeCmd
    {
        public WebApiInvokeCmd(string httpUrl,string methodName,string invokeCmd)
        {
            HttpUrl = httpUrl;
            MethodName = methodName;
            InvokeCmd = invokeCmd;

        }
        /// <summary>
        /// Http的连接地址
        /// </summary>
        public string HttpUrl { get; set; }

        /// <summary>
        /// 调用的方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 调用的参数集合
        /// </summary>
        public string InvokeCmd { get; set; }

    }
}
