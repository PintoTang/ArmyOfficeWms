using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CLDC.Infrastructrue.Xml
{
    /// <summary>
    /// Xml的序列化
    /// </summary>
    public static class XmlSerializerHelper
    {
        public static void SaveToXml(string filePath, object sourceObj, Type type, string xmlRootName)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && sourceObj != null)
            {
                type = type ?? sourceObj.GetType();

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    XmlSerializer xmlSerializer = string.IsNullOrWhiteSpace(xmlRootName) ?
                        new XmlSerializer(type) :
                        new XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                    xmlSerializer.Serialize(writer, sourceObj);
                }
            }
        }

        public static object LoadFromXml(string filePath, Type type)
        {
            object result = null;

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(type);
                    result = xmlSerializer.Deserialize(reader);
                }
            }
            return result;
        }
        /// <summary>
        /// 通过字符串流反序列化为数据模型
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeFromTextReader(TextReader reader, Type type)
        {
            XmlSerializer serialzer = new XmlSerializer(type);
            object result = serialzer.Deserialize(reader);
            return result;
        }
        /// <summary>
        /// 通过数据模型的对象及类型序列化为字符串
        /// </summary>
        /// <param name="dataMode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Serialize(object dataMode, Type type)
        {
            XmlSerializer serialzer = new XmlSerializer(type);
            StringWriter xmlOut = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = "\r\n";
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true;  // 不生成声明头
            XmlWriter xmlWriter = XmlWriter.Create(xmlOut, settings);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            serialzer.Serialize(xmlWriter, dataMode, namespaces);
            string outString = xmlOut.ToString();

            xmlWriter.Close();
            xmlOut.Close();
            return outString;
        }


    }
}
