using System;
using System.Collections.Generic;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.Framework.OPCClientAbsPckg;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common
{
    public class DeviceDataBase : NotifyObject
    {
        public string XmlNodeName { get; set; }
        public int DeviceId { get; set; }

        #region 属性

     
        private List<Datablock> datablocks = new List<Datablock>();
        public List<Datablock> Datablocks
        {
            get { return datablocks; }
            set
            {
                datablocks = value;
                RaisePropertyChanged("Datablocks");
            }
        }

        private DeviceBusinessHandle deviceBusHandle = new DeviceBusinessHandle();
        public DeviceBusinessHandle DeviceBusHandle
        {
            get { return deviceBusHandle; }
            set
            {
                deviceBusHandle = value;
                RaisePropertyChanged("DeviceBusHandle");
            }
        }
        private DeviceControlHandle deviceControlHandle = new DeviceControlHandle();
        public DeviceControlHandle DeviceControlHandle
        {
            get { return deviceControlHandle; }
            set
            {
                deviceControlHandle = value;
                RaisePropertyChanged("DeviceControlHandle");
            }
        }
        private DeviceControlHandleConn deviceHandleConn = new DeviceControlHandleConn();
        public DeviceControlHandleConn DeviceHandleConn
        {
            get { return deviceHandleConn; }
            set
            {
                deviceHandleConn = value;
                RaisePropertyChanged("DeviceHandleConn");
            }
        }
        #endregion

     

        /// <summary>
        /// 加载xml文件通过设备ID加载  只加载 bus和Control
        /// </summary>
        /// <param name="id"></param>
        public void LoadXml_templateZero()
        {
            try
            {

                string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
                XmlNodeList xmlNodelist = DeviceConfigManage.Instance.xmlDoc.SelectNodes(strPath);

                XmlNode businessHandleXmlNode = xmlNodelist[0].SelectSingleNode("BusinessHandle");
                DeviceBusHandle.Name = businessHandleXmlNode.Attributes["Name"].Value;
                DeviceBusHandle.Class = businessHandleXmlNode.Attributes["Class"].Value;
                DeviceBusHandle.NameSpace = businessHandleXmlNode.Attributes["NameSpace"].Value;

                XmlNode controlHandleXmlNode = xmlNodelist[0].SelectSingleNode("ControlHandle");
                DeviceControlHandle.Name = controlHandleXmlNode.Attributes["Name"].Value;
                DeviceControlHandle.Class = controlHandleXmlNode.Attributes["Class"].Value;
                DeviceControlHandle.NameSpace = controlHandleXmlNode.Attributes["NameSpace"].Value;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 加载xml文件通过设备ID加载  bus、Control ——controlConn——ConfigDataBlock
        /// </summary>
        public void LoadData_templateOne()
        {
            try
            {
                string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
                XmlNodeList xmlNodelist = DeviceConfigManage.Instance.xmlDoc.SelectNodes(strPath);

                XmlNode businessHandleXmlNode = xmlNodelist[0].SelectSingleNode("BusinessHandle");
                DeviceBusHandle.Name = businessHandleXmlNode.Attributes["Name"].Value;
                DeviceBusHandle.Class = businessHandleXmlNode.Attributes["Class"].Value;
                DeviceBusHandle.NameSpace = businessHandleXmlNode.Attributes["NameSpace"].Value;

                XmlNode controlHandleXmlNode = xmlNodelist[0].SelectSingleNode("ControlHandle");
                DeviceControlHandle.Name = controlHandleXmlNode.Attributes["Name"].Value;
                DeviceControlHandle.Class = controlHandleXmlNode.Attributes["Class"].Value;
                DeviceControlHandle.NameSpace = controlHandleXmlNode.Attributes["NameSpace"].Value;


                XmlNode controlConnXmlNode = controlHandleXmlNode.SelectSingleNode("Communication");
                DeviceHandleConn.Name = controlConnXmlNode.Attributes["Name"].Value;
                DeviceHandleConn.Class = controlConnXmlNode.Attributes["Class"].Value;
                DeviceHandleConn.NameSpace = controlConnXmlNode.Attributes["NameSpace"].Value;


                XmlNode protocolXmlNode = controlConnXmlNode.SelectSingleNode("Config");
                DeviceHandleConn.ConnConfig.ConfigType = controlConnXmlNode.Attributes["Type"].Value;

                XmlNode ConfigCalculateXmlNode = protocolXmlNode.SelectSingleNode("Calculate");
                DeviceHandleConn.ConnConfig.CalculateConfig.Class = ConfigCalculateXmlNode.Attributes["Class"].Value;
                DeviceHandleConn.ConnConfig.CalculateConfig.NameSpace = ConfigCalculateXmlNode.Attributes["NameSpace"].Value;

                XmlNode ConfigConnectionXmlNode = protocolXmlNode.SelectSingleNode("Connection");
                DeviceHandleConn.ConnConfig.Connection = ConfigConnectionXmlNode.FirstChild.Value;


                XmlNode ConfigDataBlockItemsXmlNode = protocolXmlNode.SelectSingleNode("DataBlockItems");
                DeviceHandleConn.ConnConfig.DataBlockItems.Template = ConfigDataBlockItemsXmlNode.Attributes["Template"].Value;
                foreach (XmlNode dataBlockNode in ConfigDataBlockItemsXmlNode.ChildNodes)
                {
                    DeviceHandleConn.ConnConfig.DataBlockItems.DBlockItemXmlList.Add(new DataBlockItemsItemConfig
                    {
                        OpcId = int.Parse(dataBlockNode.Attributes["OpcId"].Value),
                        Name = dataBlockNode.Attributes["Name"].Value,
                        DataBlockName = dataBlockNode.Attributes["DataBlockName"].Value,
                        realDataBlockAddr = dataBlockNode.Attributes["realDataBlockAddr"].Value,
                    });
                }
            }
            catch (Exception ex)
            {


            }
        }


        /// <summary>
        /// 加载xml文件通过设备ID加载  只加载 bus和Control（无 DataBlockItems） 承载货架
        /// </summary>
        /// <param name="id"></param>
        public void LoadXml_templateTwo()
        {
            try
            {
                string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
                XmlNodeList xmlNodelist = DeviceConfigManage.Instance.xmlDoc.SelectNodes(strPath);

                XmlNode businessHandleXmlNode = xmlNodelist[0].SelectSingleNode("BusinessHandle");
                DeviceBusHandle.Name = businessHandleXmlNode.Attributes["Name"].Value;
                DeviceBusHandle.Class = businessHandleXmlNode.Attributes["Class"].Value;
                DeviceBusHandle.NameSpace = businessHandleXmlNode.Attributes["NameSpace"].Value;

                XmlNode controlHandleXmlNode = xmlNodelist[0].SelectSingleNode("ControlHandle");
                DeviceControlHandle.Name = controlHandleXmlNode.Attributes["Name"].Value;
                DeviceControlHandle.Class = controlHandleXmlNode.Attributes["Class"].Value;
                DeviceControlHandle.NameSpace = controlHandleXmlNode.Attributes["NameSpace"].Value;

                //Control  Communication
                XmlNode controlConnXmlNode = controlHandleXmlNode.SelectSingleNode("Communication");
                DeviceHandleConn.Name = controlConnXmlNode.Attributes["Name"].Value;
                DeviceHandleConn.Class = controlConnXmlNode.Attributes["Class"].Value;
                DeviceHandleConn.NameSpace = controlConnXmlNode.Attributes["NameSpace"].Value;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// 保存xml文件
        /// </summary>
        public void SaveXml_templateOne()
        {
            try
            {
                XmlElement xe = DeviceConfigManage.Instance.xmlDoc.DocumentElement;
                string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
                XmlNode parentxmlNode = xe.SelectSingleNode(strPath);

                //Bus
                XmlNode busXmlNode = parentxmlNode.SelectSingleNode("BusinessHandle");
                XmlElement busSelectXe = (XmlElement)busXmlNode;
                busSelectXe.SetAttribute("Name", DeviceBusHandle.Name);
                busSelectXe.SetAttribute("Class", DeviceBusHandle.Class);
                busSelectXe.SetAttribute("NameSpace", DeviceBusHandle.NameSpace);

                //Control
                XmlNode controlXmlNode = parentxmlNode.SelectSingleNode("ControlHandle");
                XmlElement controlSelectXe = (XmlElement)controlXmlNode;
                controlSelectXe.SetAttribute("Name", DeviceControlHandle.Name);
                controlSelectXe.SetAttribute("Class", DeviceControlHandle.Class);
                controlSelectXe.SetAttribute("NameSpace", DeviceControlHandle.NameSpace);

                //Control  Communication
                XmlNode controlConnXmlNode = controlXmlNode.SelectSingleNode("Communication");
                XmlElement controlConfigSelectXe = (XmlElement)controlConnXmlNode;
                controlConfigSelectXe.SetAttribute("Name", DeviceHandleConn.Name);
                controlConfigSelectXe.SetAttribute("Class", DeviceHandleConn.Class);
                controlConfigSelectXe.SetAttribute("NameSpace", DeviceHandleConn.NameSpace);

                //Control  Communication Config
                XmlNode controlConnConfigXmlNode = controlConnXmlNode.SelectSingleNode("Config");
                XmlElement controlConnConfigSelectXe = (XmlElement)controlConnConfigXmlNode;
                controlConnConfigSelectXe.SetAttribute("Type", DeviceHandleConn.ConnConfig.ConfigType);

                //Control Communication Calculate
                XmlNode controlConnCalculateXmlNode = controlConnConfigXmlNode.SelectSingleNode("Calculate");
                XmlElement controlConfigCalculateSelectXe = (XmlElement)controlConnCalculateXmlNode;
                controlConfigCalculateSelectXe.SetAttribute("Class", DeviceHandleConn.ConnConfig.CalculateConfig.Class);
                controlConfigCalculateSelectXe.SetAttribute("NameSpace", DeviceHandleConn.ConnConfig.CalculateConfig.NameSpace);

                //Control Communication Connection
                XmlNode controlConnConnectionXmlNode = controlConnConfigXmlNode.SelectSingleNode("Connection");
                XmlElement controlConnConnectionSelectXe = (XmlElement)controlConnConnectionXmlNode;
                controlConnConnectionSelectXe.InnerText = DeviceHandleConn.ConnConfig.Connection;


                //Control Communication  DataBlockItems
                XmlNode controlConnDataBlockItemsXmlNode = controlConnConfigXmlNode.SelectSingleNode("DataBlockItems");
                XmlElement controlConnDataBlockItemsSelectXe = (XmlElement)controlConnDataBlockItemsXmlNode;
                controlConnDataBlockItemsSelectXe.SetAttribute("Template", DeviceHandleConn.ConnConfig.DataBlockItems.Template);


                //xml保存 OPC项 采用全删全增 （无唯一标识）************

                //转换成xml格式的对象 进行保存
                string conn = DeviceHandleConn.ConnConfig.Connection;
                //给原始对象赋值
                List<Datablock> datablockList = DeviceHandleConn.ConnConfig.DataBlockItems.DatablockList;
                if (datablockList != null && datablockList.Count > 0)
                {
                    //1、移除OPC全部项
                    for (int i = controlConnDataBlockItemsXmlNode.ChildNodes.Count; i > 0; i--)
                    {
                        if (controlConnDataBlockItemsXmlNode.FirstChild != null)
                        {
                            controlConnDataBlockItemsXmlNode.RemoveChild(controlConnDataBlockItemsXmlNode.FirstChild);
                        }
                    }

                    XmlElement node = (XmlElement)controlConnDataBlockItemsXmlNode;
                    XmlDocument xmlDoc = DeviceConfigManage.Instance.xmlDoc;

                    string strOpcItemPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]/ControlHandle/Communication/Config/DataBlockItems[@Template=\"{2}\"]", XmlNodeName, DeviceId, DeviceHandleConn.ConnConfig.DataBlockItems.Template);

                    List<DataBlockItemsItemConfig> dataBlockItems = DeviceHandleConn.ConnConfig.DataBlockItems.DBlockItemXmlList;
                    dataBlockItems.Clear();
                    XmlNode memberlist = xmlDoc.SelectSingleNode(strOpcItemPath);
                    foreach (Datablock dblock in datablockList)
                    {
                        int opcId = dblock.OpcId;
                        string name = dblock.Name;
                        string dataBlockName = dblock.DataBlockName;
                        string realdblockAddr = dblock.RealDataBlockAddr;

                        //创建子项节点 并添加属性和Value
                        XmlElement member = xmlDoc.CreateElement("Item");
                        member.SetAttribute("OpcId", opcId.ToString());
                        member.SetAttribute("Name", name);
                        member.SetAttribute("DataBlockName", dataBlockName);

                        object readAddr = realdblockAddr.Clone();

                        string noRealDataBlockAddr = "";
                        if (readAddr.ToString().Contains(conn))
                        {
                            noRealDataBlockAddr = readAddr.ToString().Replace(conn, "");//还原最原始的Addr
                        }
                        member.SetAttribute("realDataBlockAddr", noRealDataBlockAddr);
                        memberlist.AppendChild(member);

                        dataBlockItems.Add(new DataBlockItemsItemConfig
                        {
                            OpcId = opcId,
                            Name = name,
                            DataBlockName = dataBlockName,
                            realDataBlockAddr = noRealDataBlockAddr
                        });
                    }
                    DeviceHandleConn.ConnConfig.DataBlockItems.DBlockItemXmlList = dataBlockItems;//更新本地
                }
                DeviceConfigManage.Instance.xmlDoc.Save(DeviceConfigManage.Instance.xmlFullPath);//保存的路径 新路径 To Do
            }
            catch (Exception ex)
            {


            }
        }

        /// <summary>
        /// 保存xml文件
        /// </summary>
        public void SaveXml_templateZero()
        {
            try
            {
                XmlElement xe = DeviceConfigManage.Instance.xmlDoc.DocumentElement;
                string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
                XmlNode parentxmlNode = xe.SelectSingleNode(strPath);

                //Bus
                XmlNode busXmlNode = parentxmlNode.SelectSingleNode("BusinessHandle");
                XmlElement busSelectXe = (XmlElement)busXmlNode;
                busSelectXe.SetAttribute("Name", DeviceBusHandle.Name);
                busSelectXe.SetAttribute("Class", DeviceBusHandle.Class);
                busSelectXe.SetAttribute("NameSpace", DeviceBusHandle.NameSpace);

                //Control
                XmlNode controlXmlNode = parentxmlNode.SelectSingleNode("ControlHandle");
                XmlElement controlSelectXe = (XmlElement)controlXmlNode;
                controlSelectXe.SetAttribute("Name", DeviceControlHandle.Name);
                controlSelectXe.SetAttribute("Class", DeviceControlHandle.Class);
                controlSelectXe.SetAttribute("NameSpace", DeviceControlHandle.NameSpace);

                DeviceConfigManage.Instance.xmlDoc.Save(DeviceConfigManage.Instance.xmlFullPath);//保存的路径 新路径 To Do
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 保存xml文件
        /// </summary>
        public void SaveXml_templateTwo()
        {
            try
            {
                XmlElement xe = DeviceConfigManage.Instance.xmlDoc.DocumentElement;
                string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
                XmlNode parentxmlNode = xe.SelectSingleNode(strPath);

                //Bus
                XmlNode busXmlNode = parentxmlNode.SelectSingleNode("BusinessHandle");
                XmlElement busSelectXe = (XmlElement)busXmlNode;
                busSelectXe.SetAttribute("Name", DeviceBusHandle.Name);
                busSelectXe.SetAttribute("Class", DeviceBusHandle.Class);
                busSelectXe.SetAttribute("NameSpace", DeviceBusHandle.NameSpace);

                //Control
                XmlNode controlXmlNode = parentxmlNode.SelectSingleNode("ControlHandle");
                XmlElement controlSelectXe = (XmlElement)controlXmlNode;
                controlSelectXe.SetAttribute("Name", DeviceControlHandle.Name);
                controlSelectXe.SetAttribute("Class", DeviceControlHandle.Class);
                controlSelectXe.SetAttribute("NameSpace", DeviceControlHandle.NameSpace);

                //Control  Communication
                XmlNode controlConnXmlNode = controlXmlNode.SelectSingleNode("Communication");
                XmlElement controlConfigSelectXe = (XmlElement)controlConnXmlNode;
                controlConfigSelectXe.SetAttribute("Name", DeviceHandleConn.Name);
                controlConfigSelectXe.SetAttribute("Class", DeviceHandleConn.Class);
                controlConfigSelectXe.SetAttribute("NameSpace", DeviceHandleConn.NameSpace);

                DeviceConfigManage.Instance.xmlDoc.Save(DeviceConfigManage.Instance.xmlFullPath);//保存的路径 新路径 To Do
            }
            catch (Exception ex)
            {

            }
        }
    }
    public class DeviceBusinessHandle
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string NameSpace { get; set; }

    }
    public class DeviceControlHandle
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string NameSpace { get; set; }

    }
    public class DeviceControlHandleConn
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string NameSpace { get; set; }

        private DeviceControlHandleConnConfig connConfig = new DeviceControlHandleConnConfig();
        public DeviceControlHandleConnConfig ConnConfig
        {
            get { return connConfig; }
            set
            {
                connConfig = value;
            }
        }
    }
    public class DeviceControlHandleConnConfig
    {
        public string ConfigType { get; set; }
        private CalculateConfig calculateConfig = new CalculateConfig();
        public CalculateConfig CalculateConfig
        {
            get { return calculateConfig; }
            set
            {
                calculateConfig = value;
            }
        }
        public string Connection { get; set; }
        private ConfigDataBlockItems dataBlockItems = new ConfigDataBlockItems();
        public ConfigDataBlockItems DataBlockItems
        {
            get { return dataBlockItems; }
            set
            {
                dataBlockItems = value;
            }
        }
    }
    public class CalculateConfig
    {
        public string Class { get; set; }
        public string NameSpace { get; set; }
    }
    public class ConfigDataBlockItems
    {
        public string Template { get; set; }

        private List<Datablock> datablockList;
        /// <summary>
        /// 转换过的数据(真的地址)
        /// </summary>
        public List<Datablock> DatablockList
        {
            get { return datablockList; }
            set
            {
                datablockList = value;
            }
        }

        private List<DataBlockItemsItemConfig> dBlockItemXmlList = new List<DataBlockItemsItemConfig>();
        /// <summary>
        /// 用于保存XML格式用的
        /// </summary>
        public List<DataBlockItemsItemConfig> DBlockItemXmlList
        {
            get { return dBlockItemXmlList; }
            set
            {
                dBlockItemXmlList = value;
            }
        }
    }
    public class DataBlockItemsItemConfig
    {
        public int OpcId { get; set; }
        public string Name { get; set; }
        public string DataBlockName { get; set; }
        public string realDataBlockAddr { get; set; }

        public string OpcValue { get; set; }
    }

}
