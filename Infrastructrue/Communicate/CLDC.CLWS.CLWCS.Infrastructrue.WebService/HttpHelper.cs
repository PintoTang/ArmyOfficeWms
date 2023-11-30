using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
    public class HttpHelper
    {

        /// <summary>
        /// 以Post 形式提交数据到 uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="files"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] Post(string uri, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            return Post(new Uri(uri), files, values);
        }
        /// <summary>
        /// 以Post 形式提交数据到 uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="files"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] Post(Uri uri, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            byte[] line = Encoding.ASCII.GetBytes("\r\n--" + boundary);
            var lineLength = line.Length;
            using (var stream = new MemoryStream())
            {
                //提交文本字段
                if (values != null)
                {
                    string format = "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in values.Keys)
                    {
                        stream.Write(line, 0, lineLength);
                        string s = string.Format(format, key, values[key]);
                        byte[] data = Encoding.UTF8.GetBytes(s);
                        stream.Write(data, 0, data.Length);
                    }
                }

                //提交文件
                if (files != null)
                {
                    string fformat = "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    foreach (UploadFile file in files)
                    {
                        stream.Write(line, 0, lineLength);
                        string s = string.Format(fformat, file.Name, file.Filename, file.ContentType);
                        byte[] data = Encoding.UTF8.GetBytes(s);
                        stream.Write(data, 0, data.Length);
                        stream.Write(file.Data, 0, file.Data.Length);
                    }
                }
                if (values != null || files != null)
                {
                    stream.Write(line, 0, lineLength);
                    stream.Write(Encoding.ASCII.GetBytes("--\r\n"), 0, 4);/*后面必须跟上换行，否则python接收文件报错*/
                    //                    stream.Write(Encoding.ASCII.GetBytes("--"), 0, 2);
                }
                request.ContentLength = stream.Length;
                stream.Position = 0L;
                using (var requestStream = request.GetRequestStream())
                {
                    stream.CopyTo(requestStream);
                }
            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var mstream = new MemoryStream())
            {
                responseStream.CopyTo(mstream);
                return mstream.ToArray();
            }
        }



        ///// <summary>
        /////  Post表单 UTF8  (方法已验证，可用 json)
        ///// </summary>
        ///// <param name="url">url地址</param>
        ///// <param name="jsonData"></param>
        ///// <returns>jsonData  result: true取Content  False 取message</returns>
        //public static OperateResult<string> PostForm(string url, string jsonData)
        //{
        //    OperateResult<string> result = new OperateResult<string>();
        //    try
        //    {
        //        WebClient myWebClient = new WebClient();
        //        myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        //        myWebClient.Encoding = System.Text.Encoding.UTF8;
        //        byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);
        //        byte[] responseArray = myWebClient.UploadData(url, "POST", byteArray);
        //        myWebClient.Dispose();
        //        string sRemoteInfo = Encoding.UTF8.GetString(responseArray);
        //        result.IsSuccess = true;
        //        result.Content = sRemoteInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.IsSuccess = false;
        //        result.Message = OperateResult.ConvertException(ex);
        //    }
        //    return result;
        //}


        /// <summary>
        ///  Post表单 UTF8  (方法已验证，可用 json)
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="requestData"></param>
        /// <param name="hearders"></param>
        /// <returns>jsonData  result: true取Content  False 取message</returns>
        public static OperateResult<string> Post(string url, string requestData,Hashtable hearders=null)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                WebClient myWebClient = new WebClient();
                if (hearders != null)
                {
                    foreach (object key in hearders.Keys)
                    {
                        myWebClient.Headers.Add(key.ToString(),hearders[key].ToString());
                    }
                }
                else
                {
                    myWebClient.Headers.Add("Content-Type", "application/json");
                    myWebClient.Headers.Add("Accept-Language", "zh-CN");
                    myWebClient.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                }

                myWebClient.Encoding = System.Text.Encoding.UTF8;
                byte[] byteArray = Encoding.UTF8.GetBytes(requestData);
                byte[] responseArray = myWebClient.UploadData(url, "POST", byteArray);
                myWebClient.Dispose();
                string sRemoteInfo = Encoding.UTF8.GetString(responseArray);
                result.IsSuccess = true;
                result.Content = sRemoteInfo;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public static OperateResult<string> CreateHttpResponse(string url, bool isPost, string postData = null, string authHeaderData = "", int type = 1)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("url is empty!");
            }

            OperateResult<string> result = new OperateResult<string>();

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = isPost ? "POST" : "GET";
            request.Timeout = 30000;
            
            request.ContentType = "application/json";
            request.Headers.Add("Accept-Language", "zh-CN");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.AutomaticDecompression = DecompressionMethods.GZip;

            if (!string.IsNullOrEmpty(authHeaderData))
            {
                if (type == 1)
                {
                    request.Headers.Add("Authorization", authHeaderData);
                }
                else
                {
                    request.Headers.Add("access_token", authHeaderData);
                }
            }

            request.Accept = "*/*";
            request.KeepAlive = false;

            request.CookieContainer = new CookieContainer();

            if (isPost)
            {
                Encoding encoding = Encoding.GetEncoding("utf-8");
                byte[] data = null;
                if (string.IsNullOrEmpty(postData))
                {
                    request.ContentLength = 0;
                }
                else
                {
                    data = encoding.GetBytes(postData);
                    request.ContentLength = data.Length;
                    request.GetRequestStream().Write(data, 0, data.Length);
                }
            }

            string sReturnString = string.Empty;
            HttpWebResponse res;
            try
            {
                using (res = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = res.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            //读取服务器端返回的消息 
                            sReturnString = sr.ReadToEnd();
                        }
                    }
                }
                result.IsSuccess = true;
                result.Content = sReturnString;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        public static OperateResult<XmlDocument> InvokeWebMethod(string url, string action, Hashtable hashtableCmd)
        {
            OperateResult<XmlDocument> result = new OperateResult<XmlDocument>();
            
            string body = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.kernelextension.icow.com/\">";
            body += "<soapenv:Header/>";
            body += "<soapenv:Body>";
            body += "<web:" + action + ">";
            foreach (var key in hashtableCmd.Keys)
            {
                body += "<" + key+ ">";
                body += hashtableCmd[key].ToString();
                body += "</" + key + ">";
            }
            body += "</web:" + action + ">";
            body += "</soapenv:Body>";
            body += "</soapenv:Envelope>";
            HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml;charset=UTF-8";
            //request.ContentType = "application/json;charset=UTF-8";
           
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(body);
            request.ContentLength = payload.Length;
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(payload, 0, payload.Length);
            }
            string sReturnString = "";
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    //读取服务器端返回的消息 
                    sReturnString = sr.ReadLine();
                }
            }
            string retXml = Regex.Replace(sReturnString, @"^\{"".+[^\}]\}<[^>]+?>", "");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);

            result.IsSuccess = true;
            result.Content = doc;
            return result;
        }


        //public static OperateResult<string> PostForm(string url, Hashtable formData)
        //{
        //    OperateResult<string> result=new OperateResult<string>();
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            if (formData != null)
        //            {
        //                foreach (var formKey in formData.Keys)
        //                    client.DefaultRequestHeaders.Add(formKey.ToString(), formData[formKey].ToString());
        //            }
        //            using (HttpContent httpContent = new StringContent(formData, Encoding.UTF8))
        //            {
        //                if (contentType != null)
        //                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        //                HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
        //                return response.Content.ReadAsStringAsync().Result;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
                
        //        throw;
        //    }
        //}

        /// <summary>
        /// 上传文件
        /// </summary>
        public class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public byte[] Data { get; set; }
        }

    }
}
