
using System;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{

    /// <summary>
    /// XML操作类, 必需用XPATH表达式来获取相应节点
    /// </summary>
    public sealed class Xml
    {
        private string m_fileName;
        private XmlDocument doc = new XmlDocument();
        /// <summary>
        /// 构建方法，如果文件不存在会自动创建
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="autoCreate">是否自动创建</param>
        public Xml(string filename, bool autoCreate)
        {
            this.m_fileName = filename;

            if (autoCreate == true)
            {
                if (!File.Exists(filename))
                {
                    using (StreamWriter sw = File.CreateText(filename))
                    {
                    }
                    //输出
                    using (StreamWriter sw = File.AppendText(filename))
                    {
                        sw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                        sw.WriteLine("<root>");
                        sw.WriteLine("</root>");
                    }
                }
            }
            //读入XML文件
            doc.Load(m_fileName);
        }
        /// <summary>
        /// 保存的方法
        /// </summary>
        public void Save()
        {
            doc.Save(m_fileName);
        }
        /// <summary>
        /// 获取XML文件节点下的子节点的键值列表的集合
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns>名称值对集合</returns>
        public IList<NameValueCollection> GetNodeValuesList(string nodeName)
        {
            XmlNodeList xnl = doc.SelectNodes(nodeName);
            
            IList<NameValueCollection> list = new List<NameValueCollection>();

            foreach (XmlNode node in xnl)
            {
                NameValueCollection nvc = new NameValueCollection();
                foreach (XmlAttribute x in node.Attributes)
                {
                    nvc.Add(x.Name, x.Value);
                }
                list.Add(nvc);
            }
            return list;
        }


        /// <summary>
        /// 获取XML文件节点下的子节点的键值列表的集合(key,InnerText)
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns>名称值对集合</returns>
        public IList<NameValueCollection> GetNodeInnerTextDataList(string nodeName)
        {
            XmlNodeList xnl = doc.SelectNodes(nodeName);
            IList<NameValueCollection> list = new List<NameValueCollection>();
            foreach (XmlNode node in xnl)
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add(node.Name, node.InnerText);
                list.Add(nvc);
            }
            return list;
        }
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="value">值</param>
        private void AddNode(string nodeName, string value)
        {
            XmlElement newChild = doc.CreateElement(nodeName);
            newChild.InnerText = value;
            doc.DocumentElement.AppendChild(newChild);           
        }
        /// <summary>
        /// 写入节点属性
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">节点名称</param>
        /// <param name="value">值</param>
        private void WriteNode(string id, string name, string value)
        {
            XmlNode node = this.GetNodeById(id);

            XmlElement newChild = doc.CreateElement(name);
            newChild.SetAttribute("id", id);
            newChild.InnerText = value;
            doc.DocumentElement.InsertAfter(newChild, node);
            if (node != null)
            {
                doc.DocumentElement.RemoveChild(node);
            }
        }


        /// <summary>
        /// 获取XML文件的值列表 (未测试 不能使用)
        /// </summary>
        /// <param name="XmlFile">XML文件</param>
        /// <returns>名称值对集合</returns>
        private NameValueCollection GetXMLValues(string xmlFile)
        {
            //建立一个节点。
            XmlNodeList ChildList = doc.SelectNodes(xmlFile);
            NameValueCollection result = new NameValueCollection();
        
            foreach (var nvc in ChildList)
            {
                XmlElement xlEment = (XmlElement)nvc;
                string name = xlEment.Name;
                string strValue = xlEment.InnerText;
                result.Add(name, strValue);
            }
          
            return result;
        }

        /// <summary>
        /// 获取节点的值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点值数组</returns>
        private string[] GetNodeValues(string nodeName)
        {
            //建立一个节点。
            XmlNodeList ChildList = doc.DocumentElement.ChildNodes;
            string re = "";
            for (int i = 0; i < ChildList.Count; i++)
            {
                if (ChildList[i].Name == nodeName)
                    re += ChildList[i].InnerText + ",";
            }
            if (re == "")
                return null;
            re = re.TrimEnd(',');
            return re.Split(',');
        }

        /// <summary>
        /// 获取节点值
        /// </summary>
        /// <param name="nodeName">节点名字</param>
        /// <returns>节点值</returns>
        private string GetValue(string nodeName)
        {
            XmlNode node = doc.DocumentElement.SelectSingleNode(nodeName);
            if (node != null)
                return node.InnerText;
            else
                return "";
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="id">属性id</param>
        /// <returns>属性值</returns>
        private string GetValueById(string id)
        {
            XmlNodeList nlist = doc.DocumentElement.ChildNodes;
            for (int i = 0; i < nlist.Count; i++)
            {
                if (nlist[i].Attributes["id"].Value == id)
                {
                    return nlist[i].InnerText;
                }
            }
            return "";
        }
        /// <summary>
        /// 获取指定属性ID的节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private XmlNode GetNodeById(string id)
        {
            XmlNodeList nlist = doc.DocumentElement.ChildNodes;
            for (int i = 0; i < nlist.Count; i++)
            {
                if (nlist[i].Attributes["id"].Value == id)
                {
                    return nlist.Item(i);
                }
            }
            return null;
        }
    }
}
