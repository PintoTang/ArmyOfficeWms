using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Stackingcrane;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.stackingcrane.OpcStackingcrane;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ProtocolAnalysisForOpcStackingcrane : StackingcraneProtocolAbstract
    {
        /// <summary>
        /// 是否是双深位
        /// </summary>
        public bool IsDoubleCell { get; set; }


        private Dictionary<int, int> rowChangeDictionary = new Dictionary<int, int>();

        private Dictionary<int, TaskExcuteStatusType> faultCodeDictionary = new Dictionary<int, TaskExcuteStatusType>();
        public int ColumnChangeToDoubleRow(Addr sourceAddr, bool isDoubleCell)
        {
            int destRang = 0;
            int sourceRow= sourceAddr.Range;
            int depth = sourceAddr.Depth;
            if (isDoubleCell == false)
            {
                destRang = (1 == sourceRow % 2) ? 2 : 3;
            }
            else
            {
                if (depth == 0)
                {
                    destRang = (1 == sourceRow % 2) ? 2 : 3;
                }
                else
                {
                    destRang = (1 == sourceRow % 2) ? 1 : 4;
                }
            }
            return destRang;
        }

        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            rowChangeDictionary.Clear();
            faultCodeDictionary.Clear();

            OperateResult result = OperateResult.CreateFailedResult();
            if (xmlNode == null || !xmlNode.HasChildNodes)
            {
                result.IsSuccess = false;
                result.Message = "xmlNode 节点为NULL";
                return result;
            }
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("IsDoubleCell"))
                {
                    string content = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        IsDoubleCell = Boolean.Parse(node.InnerText.Trim());
                    }
                    continue;
                }
                if (node.Name.Equals("RowChange"))
                {
                    string content = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        string[] key_value = content.Split(';');
                        foreach (string keyValue in key_value)
                        {
                            if (string.IsNullOrEmpty(keyValue))
                            {
                                continue;
                            }
                            string[] from_to = keyValue.Split('_');
                            if (from_to.Length >= 2)
                            {
                                int from = int.Parse(from_to[0]);
                                int to = int.Parse(from_to[1]);
                                rowChangeDictionary.Add(from, to);
                            }
                        }
                    }
                    continue;
                }
                if (node.Name.Equals("FaultCode"))
                {
                    string content = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        string[] key_value = content.Split(';');
                        foreach (string keyValue in key_value)
                        {
                            if (string.IsNullOrEmpty(keyValue))
                            {
                                continue;
                            }
                            string[] from_to = keyValue.Split('_');
                            if (from_to.Length >= 2)
                            {
                                int code = int.Parse(from_to[1]);
                                TaskExcuteStatusType type = (TaskExcuteStatusType)Enum.Parse(typeof(TaskExcuteStatusType), from_to[0]);
                                faultCodeDictionary.Add(code, type);
                            }
                        }
                    }
                    continue;
                }
            }
            result.IsSuccess = true;
            return result;
        }

        public override OperateResult ParticularInitConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "ControlHandle/ProtocolTranslation/Config";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", DeviceId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", DeviceId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                return InitalizeConfig(xmlNode);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult<StackingcraneCmdElement> TransportChangeToCmdElement(TransportMessage transportMsg)
        {
            StackingcraneCmdElement cmd = new StackingcraneCmdElement();
            OperateResult<StackingcraneCmdElement> result = OperateResult.CreateFailedResult<StackingcraneCmdElement>(cmd, "无数据");
            try
            {
                ExOrder order = transportMsg.TransportOrder;
                string startAddrType = transportMsg.StartAddr.Type;
                string destAddrType = transportMsg.DestAddr.Type;
                if (startAddrType.Equals("FloorStation") && destAddrType.Equals("Cell"))
                {
                    cmd.CmdType = CmdTypeMode.In;
                    cmd.SourceStationRow = (transportMsg.StartAddr.Range % 2 == 1) ? 1 : 2;
                    cmd.SourceStationLayer = transportMsg.StartAddr.Row;
                    cmd.SourceStationColumn = transportMsg.StartAddr.Column;
                    cmd.SourceCellColumn = 0;
                    cmd.SourceCellLayer = 0;
                    cmd.SourceCellRow = 0;
                    cmd.DestStationColumn = 0;
                    cmd.DestStationLayer = 0;
                    cmd.DestStationRow = 0;
                    cmd.DestCellRow = ColumnChangeToDoubleRow(transportMsg.DestAddr, IsDoubleCell);
                    cmd.DestCellLayer = transportMsg.DestAddr.Row;
                    cmd.DestCellColumn = transportMsg.DestAddr.Column;
                }
                else if (startAddrType.Equals("FloorStation") && destAddrType.Equals("FloorStation"))
                {
                    cmd.CmdType = CmdTypeMode.StationToStation;
                }
                else if (startAddrType.Equals("Cell") && destAddrType.Equals("FloorStation"))
                {
                    cmd.CmdType = CmdTypeMode.Out;
                    cmd.SourceStationColumn = 0;
                    cmd.SourceStationLayer = 0;
                    cmd.SourceStationRow = 0;
                    cmd.DestCellColumn = 0;
                    cmd.DestCellLayer = 0;
                    cmd.DestCellRow = 0;
                    cmd.SourceCellColumn = transportMsg.StartAddr.Column;
                    cmd.SourceCellRow = ColumnChangeToDoubleRow(transportMsg.StartAddr, IsDoubleCell);
                    cmd.SourceCellLayer = transportMsg.StartAddr.Row;
                    cmd.DestStationColumn = transportMsg.DestAddr.Column;
                    cmd.DestStationLayer = transportMsg.DestAddr.Row;
                    cmd.DestStationRow = (transportMsg.DestAddr.Range % 2 == 1) ? 1 : 2;
                }
                else if (startAddrType.Equals("DefaultRange") && destAddrType.Equals("Cell"))
                {
                    cmd.CmdType = CmdTypeMode.Pick;
                    cmd.SourceStationColumn = 0;
                    cmd.SourceStationLayer = 0;
                    cmd.SourceStationRow = 0;
                    cmd.SourceCellColumn = 0;
                    cmd.SourceCellLayer = 0;
                    cmd.SourceCellRow = 0;
                    cmd.DestStationColumn = 0;
                    cmd.DestStationLayer = 0;
                    cmd.DestStationRow = 0;
                    cmd.DestCellColumn = transportMsg.DestAddr.Column;
                    cmd.DestCellRow = ColumnChangeToDoubleRow(transportMsg.DestAddr, IsDoubleCell);
                    cmd.DestCellLayer = transportMsg.DestAddr.Row;
                }
                else if (startAddrType.Equals("Cell") && destAddrType.Equals("Cell"))
                {
                    cmd.CmdType= CmdTypeMode.Move;
                    cmd.SourceStationColumn = 0;
                    cmd.SourceStationLayer = 0;
                    cmd.SourceStationRow = 0;
                    cmd.DestStationColumn = 0;
                    cmd.DestStationLayer = 0;
                    cmd.DestStationRow = 0;
                    cmd.SourceCellColumn = transportMsg.StartAddr.Column;
                    cmd.SourceCellRow = ColumnChangeToDoubleRow(transportMsg.StartAddr, IsDoubleCell);
                    cmd.SourceCellLayer = transportMsg.StartAddr.Row;
                    cmd.DestCellColumn = transportMsg.DestAddr.Column;
                    cmd.DestCellRow = ColumnChangeToDoubleRow(transportMsg.DestAddr, IsDoubleCell);
                    cmd.DestCellLayer = transportMsg.DestAddr.Row;
                }
                else
                {
                    cmd.CmdType = CmdTypeMode.None;
                }
                cmd.CmdNum = order.OrderId;
                cmd.VerificationCode = (int)cmd.CmdType + cmd.DestCellColumn + cmd.DestCellLayer + cmd.DestCellRow + cmd.DestStationColumn + cmd.DestStationLayer + cmd.DestStationRow + cmd.SourceCellColumn + cmd.SourceCellLayer + cmd.SourceCellRow + cmd.SourceStationColumn + cmd.SourceStationLayer + cmd.SourceStationRow;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;

            //else if (cmd.CmdType.Equals(CmdTypeMode.Change)) //类型为更改仓位 代码暂时未用到 
            //{
            //    cmd.SourceStationRow = (transportMsg.StartAddr.Range % 2 == 1) ? 1 : 2;
            //    cmd.SourceStationLayer = transportMsg.StartAddr.Row;
            //    cmd.SourceStationColumn = transportMsg.StartAddr.Column;
            //    cmd.SourceCellColumn = 0;
            //    cmd.SourceCellLayer = 0;
            //    cmd.SourceCellRow = 0;
            //    cmd.DestStationColumn = 0;
            //    cmd.DestStationLayer = 0;
            //    cmd.DestStationRow = 0;
            //    cmd.DestCellColumn = ColumnChangeToDoubleColumn(transportMsg.DestAddr, isDoubleCell);
            //    cmd.DestCellRow = (transportMsg.DestAddr.Range % 2 == 1) ? 1 : 2;
            //    cmd.DestCellLayer = transportMsg.DestAddr.Row;
            //}
        }

        public override OperateResult ParticularInitialize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override TaskExcuteStatusType FaultCodeChangeToExceptionType(int faultCode)
        {
            if (faultCodeDictionary.ContainsKey(faultCode))
            {
                return faultCodeDictionary[faultCode];
            }
            else
            {
                return TaskExcuteStatusType.UnknowException;
            }
        }
    }
}
