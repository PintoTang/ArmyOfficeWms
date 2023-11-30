using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
    public class DynamicCallWebService
    {
        #region Tip:使用说明
        //webServices 应该支持Get和Post调用，在web.config应该增加以下代码
        //<webServices>
        // <protocols>
        //  <add name="HttpGet"/>
        //  <add name="HttpPost"/>
        // </protocols>
        //</webServices>

        //调用示例：
        //Hashtable ht = new Hashtable(); //Hashtable 为webservice所需要的参数集
        //ht.Add("str", "test");
        //ht.Add("b", "true");
        //XmlDocument xx = WebSvcCaller.QuerySoapWebService("http://localhost:81/service.asmx", "HelloWorld", ht);
        //MessageBox.Show(xx.OuterXml);
        #endregion

        /// <summary>
        /// 需要WebService支持Post调用
        /// </summary>
        public static OperateResult<XmlDocument> CallWebServicePost(string url, string methodeName, Hashtable hashParam)
        {
            OperateResult<XmlDocument> result = new OperateResult<XmlDocument>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url + "/" + methodeName);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                SetWebRequest(request);
                byte[] data = EncodePars(hashParam);
                WriteRequestData(request, data);
                XmlDocument responseResult = ReadXmlResponse(request.GetResponse());
                result.IsSuccess = true;
                result.Content = responseResult;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        /// <summary>
        /// 需要WebService支持Get调用
        /// </summary>
        public static OperateResult<XmlDocument> CallWebServiceGet(string url, string methodeName, Hashtable hashParam)
        {
            OperateResult<XmlDocument> result = new OperateResult<XmlDocument>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url + "/" + methodeName + "?" + ParsToString(hashParam));
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                SetWebRequest(request);
                XmlDocument responseResult = ReadXmlResponse(request.GetResponse());
                result.IsSuccess = true;
                result.Content = responseResult;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// 通用WebService调用(Soap),参数Pars为String类型的参数名、参数值
        /// </summary>
        public static OperateResult<XmlDocument> InvokeWebServiceMethod(string url, string methodeName, Hashtable hashParam, string soapAction = "",int timeOut=30)
        {
            OperateResult<XmlDocument> result = new OperateResult<XmlDocument>();
            Timeout = timeOut;
            url = url.Split('?')[0];
            XmlDocument responseResult = null;
            if (XmlNamespaces.ContainsKey(url))
            {
                responseResult = RequestWebServiceMethod(url, methodeName, hashParam, XmlNamespaces[url].ToString(), soapAction);
            }
            else
            {
                responseResult = RequestWebServiceMethod(url, methodeName, hashParam, GetNamespace(url), soapAction);
            }
            result.IsSuccess = true;
            result.Content = responseResult;

            return result;

        }


        private static XmlDocument RequestWebServiceMethod(string url, string methodName, Hashtable hashParam, string xmlNs, string soapAction = "")
        {
            XmlNamespaces[url] = xmlNs;//加入缓存，提高效率
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            SetWebRequest(request);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            string scriptSoapAction;
            if (string.IsNullOrEmpty(soapAction))
            {
                if (SoapActionTable.ContainsKey(url + methodName))
                {
                    scriptSoapAction = SoapActionTable[url + methodName].ToString();
                }
                else
                {
                    scriptSoapAction = GetActionName(url, methodName);
                }
            }
            else
            {
                scriptSoapAction = soapAction;
            }
            request.Headers.Add("SOAPAction", scriptSoapAction);
            SetWebRequest(request);
            byte[] data = EncodeParsToSoap(hashParam, xmlNs, methodName);
            WriteRequestData(request, data);
            XmlDocument doc = new XmlDocument(), doc2 = new XmlDocument();
            doc = ReadXmlResponse(request.GetResponse());

            XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
            mgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            String retXml = doc.SelectSingleNode("//soap:Body/*/*", mgr).InnerXml;
            doc2.LoadXml("<root>" + retXml + "</root>");
            AddDelaration(doc2);
            return doc2;
        }
        private static string GetNamespace(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?WSDL");
            SetWebRequest(request);
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sr.ReadToEnd());
                sr.Close();
                return doc.SelectSingleNode("//@targetNamespace").Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message+"请检查服务或网络问题");
            }
          
        }

        private static string GetActionName(string url, string methodName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?WSDL");
            SetWebRequest(request);
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            //@"binding.+?name=""{0}""\>.+?Action=""([^""]+)"""
            string regexString = string.Format(@"name=""{0}""\>.+?Action=""([^""]+)""", methodName);
            Regex re = new Regex(regexString, RegexOptions.Multiline);
            var match = re.Match(doc.InnerXml);
            if (match.Success)
            {
                string action = match.Groups[1].Value;
                SoapActionTable[url + methodName] = action;
                return action;
            }
            return string.Empty;
        }

        private static byte[] EncodeParsToSoap(Hashtable hashPara, string xmlNs, string methodName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"  xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"></soap:Envelope>");
            AddDelaration(doc);
            XmlElement soapBody = doc.CreateElement("soap", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlElement soapMethod = doc.CreateElement(methodName);
            soapMethod.SetAttribute("xmlns", xmlNs);
            foreach (var k in hashPara.Keys)
            {
                XmlElement soapPar = doc.CreateElement(k.ToString());
                soapPar.InnerXml = ObjectToSoapXml(hashPara[k]);
                soapMethod.AppendChild(soapPar);
            }
            soapBody.AppendChild(soapMethod);
            doc.DocumentElement.AppendChild(soapBody);
            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }
        private static string ObjectToSoapXml(object o)
        {
            XmlSerializer mySerializer = new XmlSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            mySerializer.Serialize(ms, o);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Encoding.UTF8.GetString(ms.ToArray()));
            if (doc.DocumentElement != null)
            {
                return doc.DocumentElement.InnerXml;
            }
            else
            {
                return o.ToString();
            }
        }

        /// <summary>
        /// 设置凭证与超时时间
        /// </summary>
        /// <param name="request"></param>
        private static void SetWebRequest(HttpWebRequest request)
        {
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = Timeout*1000;
            request.ReadWriteTimeout = Timeout*1000;
        }

        public static int Timeout = 30;
        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

        private static byte[] EncodePars(Hashtable hashPara)
        {
            return Encoding.UTF8.GetBytes(ParsToString(hashPara));
        }

        private static String ParsToString(Hashtable hashPara)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string k in hashPara.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }

        private static XmlDocument ReadXmlResponse(WebResponse response)
        {
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String retXml = sr.ReadToEnd();
            Console.WriteLine("retXml:" + retXml);
            retXml = Regex.Replace(retXml, @"^\{"".+[^\}]\}<[^>]+?>", "");
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }

        private static void AddDelaration(XmlDocument doc)
        {
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.InsertBefore(decl, doc.DocumentElement);
        }

        private static readonly Hashtable XmlNamespaces = new Hashtable();//缓存xmlNamespace，避免重复调用GetNamespace

        private static readonly Hashtable SoapActionTable = new Hashtable();
    }
}
