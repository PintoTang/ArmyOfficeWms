using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Services.Description;
using System.Xml;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Microsoft.CSharp;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
    public class WebServiceHelper
    {

        //public static object InvokeWebServiceProxy(string url, string methodname, object[] param, Hashtable soapHeader=null,
        //    string soapHeadClassName = "", int timeOut = 30*1000)
        //{

        //    return null;
        //}

        public static object InvokeWebServicePost(string url, string methodname, Hashtable paramHashtable,string soapAction="",int timeOut=30)
        {
            OperateResult<XmlDocument> resposeResult = DynamicCallWebService.InvokeWebServiceMethod(url, methodname, paramHashtable,soapAction,timeOut);
            if (resposeResult.IsSuccess)
            {
               return resposeResult.Content.InnerText;
            }
            else
            {
                return resposeResult.Message;
            }
        }

        //动态调用web服务
        public static object InvokeWebServiceDynamic(string url, string methodname, object[] args)
        {
            return WebServiceHelper.InvokeWebService(url, string.Empty, methodname, args);
        }

        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = WebServiceHelper.GetWsClassName(url);
            }
            try
            {
                InstanceInfo instance = GetInstanceInfo(@namespace, classname, methodname);
                if (instance != null)
                {
                    return instance.InstanceMethod.Invoke(instance.Instance, args);
                }
                //获取WSDL
                WebClient wc = new WebClient();
                if (!url.ToLower().Contains("wsdl"))
                {
                    url = url + "?WSDL";
                }
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                wc.Dispose();
                stream.Close();
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();
                csc.Dispose();
                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                InstanceInfo newInstance = new InstanceInfo()
                {
                    ClassName = classname,
                    MethodName = methodname,
                    NameSpace = @namespace,
                    InstanceType = t,
                    InstanceMethod = mi,
                    Instance = obj
                };
                _curInstanceInfo.Add(newInstance);

                return mi.Invoke(obj, args);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        private static List<InstanceInfo> _curInstanceInfo = new List<InstanceInfo>();

        private static InstanceInfo GetInstanceInfo(string nameSpace, string className, string methodName)
        {
            if (_curInstanceInfo.Exists(i => i.ClassName.Equals(className) && i.NameSpace.Equals(nameSpace) && i.MethodName.Equals(methodName)))
            {
                return
                    _curInstanceInfo.FirstOrDefault(
                        i =>
                            i.ClassName.Equals(className) && i.NameSpace.Equals(nameSpace) &&
                            i.MethodName.Equals(methodName));
            }
            else
            {
                return null;
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }
    }

    public class InstanceInfo
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
        /// 实例类型
        /// </summary>
        public Type InstanceType { get; set; }
        /// <summary>
        /// 需要调用的方法
        /// </summary>
        public System.Reflection.MethodInfo InstanceMethod { get; set; }

        public object Instance { get; set; }
    }

}
