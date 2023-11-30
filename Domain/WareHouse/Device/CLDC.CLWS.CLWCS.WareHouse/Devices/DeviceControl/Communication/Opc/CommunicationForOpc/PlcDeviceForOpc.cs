using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.Framework.OPCClientAbsPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Communication.Opc.CommunicationForOpc;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{

    /// <summary>
    /// OPC协议通讯接口实现
    /// </summary>
    public class PlcDeviceForOpc : IPlcDeviceCom
    {
        public PlcDeviceForOpc()
        {
            this.OpcClient = DependencyHelper.GetService<OPCClientAbstract>();
            this.OpcMoinitor = DependencyHelper.GetService<OPCMonitorAbstract>();
            OpcElement = new OpcElement();
        }

        private int writeOpcTimes = 2;
        private int sleepTimes = 20;//毫秒
        public OpcElement OpcElement { get; set; }

        /// <summary>
        /// OPC监控
        /// </summary>
        private OPCMonitorAbstract OpcMoinitor { get; set; }

        /// <summary>
        /// OPC的客户端
        /// </summary>
        private OPCClientAbstract OpcClient { get; set; }

        /// <summary>
        /// 地址计算器
        /// </summary>
        private IPlcItemCalculate AddressCalculator { get; set; }


        private OperateResult InitializeCalculator(XmlNode xmlNode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            string classValue = string.Empty;
            string nameSpace = string.Empty;
            int OpcId = 0;
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("Calculate"))
                {
                    classValue = node.Attributes["Class"].Value.Trim();
                    nameSpace = node.Attributes["NameSpace"].Value.Trim();
                    if (string.IsNullOrEmpty(classValue) || string.IsNullOrEmpty(nameSpace))
                    {
                        return OperateResult.CreateSuccessResult();
                    }

                    AddressCalculator = (IPlcItemCalculate)Assembly.Load(nameSpace).CreateInstance(nameSpace + "." + classValue);
                    if (AddressCalculator == null)
                    {
                        return OperateResult.CreateFailedResult(string.Format("Plc地址计算器初始化出错，类名：{0} 命名空间：{1}", classValue, nameSpace), 1);
                    }
                    return OperateResult.CreateSuccessResult();
                }
            }
            return OperateResult.CreateSuccessResult();
        }



        /// <summary>
        /// 根据固定的XML配置结构读取参数
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            string connection = string.Empty;
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("Connection"))
                {
                    connection = node.InnerText.Trim();
                    continue;
                }
                if (node.Name.Equals("DataBlockItems"))
                {
                    foreach (XmlNode dbNode in node.ChildNodes)
                    {
                        if (dbNode.Attributes != null)
                        {
                            Datablock dbDatablock = new Datablock();
                            string name = dbNode.Attributes["Name"].Value.Trim();
                            int opcId = int.Parse(dbNode.Attributes["OpcId"].Value.Trim());
                            string dataBlockName = dbNode.Attributes["DataBlockName"].Value.Trim();
                            DataBlockNameEnum datablockEnum = (DataBlockNameEnum)Enum.Parse(typeof(DataBlockNameEnum), dbNode.Attributes["DataBlockName"].Value);
                            string realDataBlockAddr = dbNode.Attributes["realDataBlockAddr"].Value.Trim();
                            if (string.IsNullOrEmpty(realDataBlockAddr))//如果没有配置则进行计算
                            {
                                if (AddressCalculator != null)
                                {
                                    realDataBlockAddr = AddressCalculator.CaculatePlcAddress(opcId, datablockEnum);
                                }
                            }
                            if (!string.IsNullOrEmpty(connection))
                            {
                                dbDatablock.Connection = connection;
                            }
                            dbDatablock.OpcId = opcId;
                            dbDatablock.Name = name;
                            dbDatablock.DataBlockName = dataBlockName;
                            dbDatablock.RealDataBlockAddr = realDataBlockAddr;
                            dbDatablock.DatablockEnum = datablockEnum;
                            OpcElement.AddDatablock(dbDatablock);
                        }
                    }
                    continue;
                }
            }
            result.IsSuccess = true;
            return result;
        }

        private OperateResult InitConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/Communication/Config";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                OperateResult initializeResult = InitializeCalculator(xmlNode);
                if (!initializeResult.IsSuccess)
                {
                    return initializeResult;
                }
                return InitalizeConfig(xmlNode);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
                //result = OperateResult.CreateFailedResult();//此行代码 待删除 屏蔽了具体错误
            }
            return result;
        }

        public int DeviceId { get; set; }

        public DeviceName DeviceName { get; set; }

        public OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            OperateResult initConfig = InitConfig();
            if (!initConfig.IsSuccess)
            {
                return initConfig;
            }
            return OperateResult.CreateSuccessResult();
        }


        public OperateResult Write(DataBlockNameEnum datablockNameEnum, string value)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
              
                //增加读取校验 3次，如果写失败则重新写入，写成功则无需再次写入
                for (int i = 0; i < writeOpcTimes; i++)
                {
                    this.OpcClient.Write(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr, value);
                    Thread.Sleep(sleepTimes);
                    OperateResult<string> readWriteOpcValue = this.ReadString(datablockNameEnum);
                    if (readWriteOpcValue.IsSuccess &&readWriteOpcValue.Content.Equals(value)) break;
                }
                getDbResult.Content.RealValue = value;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }
        public OperateResult Write(DataBlockNameEnum datablockNameEnum, object value)
        {
            if (value == null)
            {
                value = "";//处理报错信息
            }
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                for (int i = 0; i < writeOpcTimes; i++)
                {
                    this.OpcClient.Write(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr, value);
                    Thread.Sleep(sleepTimes);
                    //增加读取校验 1次，如果写失败则重新写入，写成功则无需再次写入
                    OperateResult<string> readWriteOpcValue = this.ReadString(datablockNameEnum);
                    if (readWriteOpcValue.IsSuccess && readWriteOpcValue.Content.Equals(value.ToString())) break;
                }
              
                getDbResult.Content.RealValue = value.ToString();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        public OperateResult Write(DataBlockNameEnum datablockNameEnum, float value)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
               
                //增加读取校验 1次，如果写失败则重新写入，写成功则无需再次写入
                for (int i = 0; i < writeOpcTimes; i++)
                {
                    this.OpcClient.Write(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr, value);
                    Thread.Sleep(sleepTimes);
                    OperateResult<float> readWriteOpcValue = this.ReadFloat(datablockNameEnum);
                    if (readWriteOpcValue.IsSuccess && readWriteOpcValue.Content.Equals(value)) break;
                }
                getDbResult.Content.RealValue = value.ToString(CultureInfo.InvariantCulture);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult Write(DataBlockNameEnum datablockNameEnum, bool value)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                for (int i = 0; i < writeOpcTimes; i++)
                {
                    this.OpcClient.Write(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr, value);
                    Thread.Sleep(sleepTimes);
                    OperateResult<bool> readWriteOpcValue = this.ReadBool(datablockNameEnum);
                    if (readWriteOpcValue.IsSuccess && readWriteOpcValue.Content.Equals(value)) break;
                }
                getDbResult.Content.RealValue = value.ToString(CultureInfo.InvariantCulture);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult Write(DataBlockNameEnum datablockNameEnum, int value)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                for (int i = 0; i < writeOpcTimes; i++)
                {
                    this.OpcClient.Write(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr, value);
                    Thread.Sleep(sleepTimes);
                    OperateResult<int> readWriteOpcValue = this.ReadInt(datablockNameEnum);
                    if (readWriteOpcValue.IsSuccess && readWriteOpcValue.Content.Equals(value)) break;
                }
                getDbResult.Content.RealValue = value.ToString(CultureInfo.InvariantCulture);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        public OperateResult<bool> ReadBool(DataBlockNameEnum datablockNameEnum)
        {
            OperateResult<bool> result = new OperateResult<bool>();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                bool readValue = this.OpcClient.ReadBool(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr);
                result.Content = readValue;
                getDbResult.Content.RealValue = readValue.ToString(CultureInfo.InvariantCulture);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }
        public OperateResult<string> ReadString(DataBlockNameEnum datablockNameEnum)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                string readValue = this.OpcClient.ReadString(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr);
                result.Content = readValue;
                getDbResult.Content.RealValue = readValue;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<float> ReadFloat(DataBlockNameEnum datablockNameEnum)
        {
            OperateResult<float> result = new OperateResult<float>();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                float readValue = this.OpcClient.ReadFloat(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr);
                result.Content = readValue;
                getDbResult.Content.RealValue = readValue.ToString(CultureInfo.InvariantCulture);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<int> ReadInt(DataBlockNameEnum datablockNameEnum)
        {
            OperateResult<int> result = new OperateResult<int>();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                int readValue = this.OpcClient.Read(this.DeviceName.FullName, getDbResult.Content.RealDataBlockAddr);
                result.Content = readValue;
                getDbResult.Content.RealValue = readValue.ToString(CultureInfo.InvariantCulture);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        public OperateResult RegisterValueChange(DataBlockNameEnum datablockNameEnum, CallbackContainOpcValue monitervaluechange)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                Datablock db = getDbResult.Content;
                OpcMoinitor.RegisterValueChange(DeviceName.FullName, db, monitervaluechange);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult RegisterNotEqualStartValue(DataBlockNameEnum datablockNameEnum, int startValue, CallbackContainOpcValue monitervaluechange)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                Datablock db = getDbResult.Content;
                OpcMoinitor.RegisterNotEqualStartValue(DeviceName.FullName, db, startValue, monitervaluechange);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        public string Name { get; set; }




        public OperateResult RegisterValueChange(DataBlockNameEnum datablockNameEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                Datablock db = getDbResult.Content;
                OpcMoinitor.RegisterValueChange(DeviceName.FullName, db, monitervaluechange);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult Write(Dictionary<DataBlockNameEnum, int> dictionary)
        {
            OperateResult writeResult = OperateResult.CreateFailedResult();
            try
            {
                if (dictionary == null)
                {
                    return OperateResult.CreateFailedResult("传入空参数", 1);
                }

                Dictionary<string, int> dictionaryValue = new Dictionary<string, int>();
                Dictionary<string, int> dictionaryDestValue = new Dictionary<string, int>();

                foreach (KeyValuePair<DataBlockNameEnum, int> item in dictionary)
                {
                    OperateResult<Datablock> result = OpcElement.CheckDatablockEnum(item.Key);
                    if (!result.IsSuccess)
                    {
                        writeResult.IsSuccess = false;
                        writeResult.Message = string.Format("查找不到：{0} 的对应配置项", item.Key);
                        return writeResult;
                    }

                    if (item.Key.Equals(DataBlockNameEnum.DestinationDataBlock)
                        || item.Key.Equals(DataBlockNameEnum.WriteDirectionDataBlock))
                    {
                        dictionaryDestValue.Add(result.Content.RealDataBlockAddr, item.Value);
                        result.Content.RealValue = item.Value.ToString();
                    }
                    else
                    {
                        dictionaryValue.Add(result.Content.RealDataBlockAddr, item.Value);
                        result.Content.RealValue = item.Value.ToString();
                    }
                }
                //1、先处理非目标地址的OPC地址
                if (dictionaryValue.Count >= 1)
                {
                    OpcClient.Write(DeviceName.FullName, dictionaryValue);
                    Thread.Sleep(20);
                    //2、验证非目标地址 写入的值是否正确，不正确则重新写入一次
                    foreach (KeyValuePair<string, int> NdicItem in dictionaryValue)
                    {
                        for (int i = 0; i < writeOpcTimes; i++)
                        {
                            int rdValue = OpcClient.Read(DeviceName.FullName, NdicItem.Key);
                            if (!rdValue.Equals(NdicItem.Value))
                            {
                                this.OpcClient.Write(DeviceName.FullName, NdicItem.Key, NdicItem.Value);
                                Thread.Sleep(sleepTimes);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                //3、处理目标地址的OPC地址
                if (dictionaryDestValue.Count >= 1)
                {
                    OpcClient.Write(DeviceName.FullName, dictionaryDestValue);
                    Thread.Sleep(20);
                    Thread.Sleep(sleepTimes);
                    //4、验证目标地址 写入的值是否正确，不正确则重新写入一次
                    foreach (KeyValuePair<string, int> NdicDestItem in dictionaryDestValue)
                    {
                        for (int i = 0; i < writeOpcTimes; i++)
                        {
                            int rdValue = OpcClient.Read(DeviceName.FullName, NdicDestItem.Key);
                            if (!rdValue.Equals(NdicDestItem.Value))
                            {
                                this.OpcClient.Write(DeviceName.FullName, NdicDestItem.Key, NdicDestItem.Value);
                                Thread.Sleep(sleepTimes);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                writeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                writeResult.IsSuccess = false;
                writeResult.Message = OperateResult.ConvertException(ex);
            }
            return writeResult;
        }

        public OperateResult RegisterFromStartToEndStatus(DataBlockNameEnum datablockNameEnum, int startValue, int endValue, MonitorSpecifiedOpcValueCallback callback)
        {
            OperateResult result = new OperateResult();
            try
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockNameEnum);
                if (!getDbResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 的没有配置:{1} 地址块", this.DeviceName.FullName, datablockNameEnum);
                    return result;
                }
                Datablock db = getDbResult.Content;
                OpcMoinitor.RegisterFromStartToEndStatus(DeviceName.FullName, db, startValue, endValue, callback);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public  void RefreshAllData()
        {
            foreach (Datablock datablock in OpcElement.Datablocks)
            {
                ReadString(datablock.DatablockEnum);
            }
        }

        public OperateResult Write(Dictionary<DataBlockNameEnum, object> dictionary)
        {
            OperateResult writeResult = OperateResult.CreateFailedResult();
            try
            {
                if (dictionary == null)
                {
                    return OperateResult.CreateFailedResult("传入空参数", 1);
                }

                Dictionary<string, object> dictionaryValue = new Dictionary<string, object>();
                Dictionary<string, object> dictionaryDestValue = new Dictionary<string, object>();
                foreach (KeyValuePair<DataBlockNameEnum, object> item in dictionary)
                {
                    OperateResult<Datablock> result = OpcElement.CheckDatablockEnum(item.Key);
                    if (!result.IsSuccess)
                    {
                        writeResult.IsSuccess = false;
                        writeResult.Message = string.Format("查找不到：{0} 的对应配置项", item.Key);
                        return writeResult;
                    }
                    if (item.Key.Equals(DataBlockNameEnum.DestinationDataBlock)
                      || item.Key.Equals(DataBlockNameEnum.WriteDirectionDataBlock))
                    {
                        dictionaryDestValue.Add(result.Content.RealDataBlockAddr, item.Value);
                        result.Content.RealValue = Convert.ToString(item.Value);
                    }
                    else
                    {
                        dictionaryValue.Add(result.Content.RealDataBlockAddr, item.Value);
                        result.Content.RealValue = Convert.ToString(item.Value);
                    }
                }
                //1、先处理非目标地址的OPC地址
                if (dictionaryValue.Count >= 1)
                {
                    OpcClient.Write(DeviceName.FullName, dictionaryValue);
                    Thread.Sleep(20);
                    //2、验证非目标地址 写入的值是否正确，不正确则重新写入一次
                    foreach (KeyValuePair<string, object> NdicItem in dictionaryValue)
                    {
                        for (int i = 0; i < writeOpcTimes; i++)
                        {
                            string rdValue = OpcClient.ReadString(DeviceName.FullName, NdicItem.Key);
                            if (!rdValue.Equals(NdicItem.Value.ToString()))
                            {
                                this.OpcClient.Write(DeviceName.FullName, NdicItem.Key, NdicItem.Value);
                                Thread.Sleep(sleepTimes);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                //3、处理目标地址的OPC地址
                if (dictionaryDestValue.Count >= 1)
                {
                    OpcClient.Write(DeviceName.FullName, dictionaryDestValue);
                    Thread.Sleep(20);
                    //4、验证目标地址 写入的值是否正确，不正确则重新写入一次
                    foreach (KeyValuePair<string, object> NdicDestItem in dictionaryDestValue)
                    {
                        for (int i = 0; i < writeOpcTimes; i++)
                        {
                            string rdValue = OpcClient.ReadString(DeviceName.FullName, NdicDestItem.Key);
                            if (!rdValue.Equals(NdicDestItem.Value.ToString()))
                            {
                                this.OpcClient.Write(DeviceName.FullName, NdicDestItem.Key, NdicDestItem.Value);
                                Thread.Sleep(sleepTimes);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                writeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                writeResult.IsSuccess = false;
                writeResult.Message = OperateResult.ConvertException(ex);
            }
            return writeResult;
        }

        public List<bool> ReadBoolByBlockEnums(int opcId,List<DataBlockNameEnum> datablockEnums)
        {
            List<DeviceAddressInfo> infos = new List<DeviceAddressInfo>();
            if(OpcElement.Datablocks==null|| OpcElement.Datablocks.Count == 0)
            {
                return null;
            }
            foreach(var datablockEnum in datablockEnums)
            {
                OperateResult<Datablock> getDbResult = OpcElement.CheckDatablockEnum(datablockEnum);
                var realDataBlockAddr = "";
                if (getDbResult.IsSuccess)
                {
                    realDataBlockAddr = getDbResult.Content.RealDataBlockAddr;
                }
                else
                {
                    realDataBlockAddr = AddressCalculator.CaculatePlcAddress(opcId, datablockEnum);
                }

                if (string.IsNullOrEmpty(realDataBlockAddr))
                {
                    return null;
                }
                infos.Add(new DeviceAddressInfo
                {
                    deviceName=this.DeviceName.FullName,
                    Datablock=new Datablock
                    {
                        DatablockEnum=datablockEnum,
                        RealDataBlockAddr= realDataBlockAddr,
                        OpcId=opcId,
                        Name= datablockEnum.GetDescription(),
                        DataBlockName=datablockEnum.ToString(),
                        Connection=OpcElement.Datablocks[0].Connection,
                    },
                });
            }
            
            List<bool> list=this.OpcClient.ReadBoolList(infos);
            return list;
        }
    }
}
