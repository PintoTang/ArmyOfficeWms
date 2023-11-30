using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.Infrastructrue.Xml
{
    /// <summary>
    /// XML文件的操作类
    /// </summary>
    public sealed class XmlOperator
    {

        private string m_fileName;
        private XmlDocument doc = new XmlDocument();
        /// <summary>
        /// 构建方法，如果文件不存在会自动创建
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="autoCreate">是否自动创建</param>
        public XmlOperator(string filename, bool autoCreate)
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

        public XmlOperator(string filename)
        {
            this.m_fileName = filename;
            //读入XML文件
            doc.Load(m_fileName);
        }

        public XmlNodeList GetXmlNode(string path)
        {
            XmlNodeList nodes = doc.SelectNodes(path);
            return nodes;
        }

        /// <summary>
        /// 保存的方法
        /// </summary>
        public OperateResult Save()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                doc.Save(m_fileName);
                result.IsSuccess = true;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult Save(string path)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                doc.Save(path);
                result.IsSuccess = true;

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
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
        public void AddNode(XmlNode node)
        {
            doc.DocumentElement.AppendChild(node);
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
                if (nlist[i].Attributes["DeviceId"].Value == id)
                {
                    return nlist.Item(i);
                }
            }
            return null;
        }

        /// <summary>
        /// 通过关键的NodeName及其值获取对应的XMLElement
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public XmlElement GetXmlElement(string nodeName, string attributeName, string tag)
        {
            XmlNodeList node = doc.GetElementsByTagName(nodeName);
            foreach (XmlElement xml in node)
            {
                if (xml.GetAttribute(attributeName) == tag)
                {
                    return xml;
                }
            }
            return null;
        }
        /// <summary>
        /// 通过TagName及属性名称和值获取XmlNode
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="attributeName"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public XmlNode GetXmlNode(string nodeName, string attributeName, string tag)
        {
            XmlNodeList node = doc.GetElementsByTagName(nodeName);
            return node.Cast<XmlNode>().FirstOrDefault(xml => xml.Attributes != null && xml.Attributes[attributeName].Value == tag);
        }

        /// <summary>
        /// 把节点导入到当前文档
        /// </summary>
        /// <param name="node"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        private XmlNode ImportNode(XmlNode node, bool deep)
        {
            return doc.ImportNode(node, deep);
        }

        /// <summary>
        /// 替换Node
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        public OperateResult ReplaceChild(XmlNode oldNode, XmlNode newNode)
        {
            OperateResult result=OperateResult.CreateFailedResult();
            try
            {
                XmlNode parentXmlNode = oldNode.ParentNode;
                XmlNode curNewNode = ImportNode(newNode, true);
                if (parentXmlNode != null)
                {
                    parentXmlNode.ReplaceChild(curNewNode, oldNode);
                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "根据旧节点获取不到父节点";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// 通过Node的名称和标识获取节点上的属性及值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public NameValueCollection GetAttributeNameValue(string nodeName, string tag)
        {
            NameValueCollection nameValue = new NameValueCollection();
            XmlNodeList node = doc.GetElementsByTagName(nodeName);
            foreach (XmlElement xml in node)
            {
                if (xml.GetAttribute(nodeName) == tag)
                {
                    foreach (XmlAttribute xmlAttribute in xml.Attributes)
                    {
                        nameValue.Add(xmlAttribute.Name, xmlAttribute.Value);
                    }
                }
            }
            return nameValue;
        }

        /// <summary>
        /// 通过字符串转换为XmlNode
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public XmlNode GetXmlNodeFromString(string xml)
        {
            XmlDocument newXmlDoc = new XmlDocument();
            newXmlDoc.LoadXml(xml);
            XmlNode newNode = newXmlDoc.DocumentElement;
            return newNode;
        }


    }
}
