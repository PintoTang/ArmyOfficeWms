using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using System;
using System.Collections.Generic;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Stackingcrane;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.stackingcrane.OpcStackingcrane;
using CLDC.Infrastructrue.Xml;

namespace CLWCS.WareHouse.Device.HeFei
{
    /// <summary>
    /// SNDL堆垛机协议
    /// </summary>
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
            int sourceRow = sourceAddr.Range;
            int depth = sourceAddr.Depth;
            if (isDoubleCell == false)
            {
                destRang = (1 == sourceRow % 2) ? 1 : 2;
            }
            else
            {
//Cell:6_1_1_1_A01 3

//Cell:6_1_1_0_A01  2
//Cell:5_1_1_0_A01  (3号左边) 1
//Cell:4_1_1_0_A01 (2号右边) 3

//Cell:3_1_1_0_A01    2
//Cell:3_1_1_1_A01  1
                if (depth == 0)
                {
                    if (sourceRow == 3)
                    {
                        destRang = 2;
                    }
                    else if (sourceRow == 4)
                    {
                        destRang = 1;
                    }
                    else if (sourceRow == 5)
                    {
                        destRang = 2;
                    }
                    else
                    {
                          destRang = (1 == sourceRow % 2) ? 2 : 3;
                    }
                }
                else
                {
                      if (sourceRow == 3)
                        {
                            destRang = 1;
                        }
                        else if (sourceRow == 6)
                        {
                            destRang = 3;
                        }
                        else
                        {
                              destRang = (1 == sourceRow % 2) ? 1 : 4;
                        }
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
                            string[] from_to = keyValue.Split('_');//OutButEmpty_1009;InButExist_1006;InDepthButShallowExist_1007;OutDepthButShallowExist_1008
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
                    if (transportMsg.StartAddr.IsContain("FloorStation:1_1_1"))
                    {
                        cmd.SourceStationNum = 1001;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:2_1_1"))
                    {
                        cmd.SourceStationNum = 1004;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:3_1_1"))
                    {
                        cmd.SourceStationNum = 1020;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:4_1_1"))
                    {
                        cmd.SourceStationNum = 1040;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:5_1_1"))
                    {
                        cmd.SourceStationNum = 1042;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:1_2_1"))
                    {
                        cmd.SourceStationNum = 2001;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:2_2_1"))
                    {
                        cmd.SourceStationNum = 2004;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:3_2_1"))
                    {
                        cmd.SourceStationNum = 2020;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:4_2_1"))
                    {
                        cmd.SourceStationNum = 2034;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:5_2_1"))
                    {
                        cmd.SourceStationNum = 2036;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:1_3_1"))
                    {
                        cmd.SourceStationNum = 3001;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:2_3_1"))
                    {
                        cmd.SourceStationNum = 3004;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:3_3_1"))
                    {
                        cmd.SourceStationNum = 3023;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:4_3_1"))
                    {
                        cmd.SourceStationNum = 3039;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:5_3_1"))
                    {
                        cmd.SourceStationNum = 3040;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:1_4_1"))
                    {
                        cmd.SourceStationNum = 4001;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:2_4_1"))
                    {
                        cmd.SourceStationNum = 4004;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:3_4_1"))
                    {
                        cmd.SourceStationNum = 4021;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:4_4_1"))
                    {
                        cmd.SourceStationNum = 4039;
                    }
                    else if (transportMsg.StartAddr.IsContain("FloorStation:5_4_1"))
                    {
                        cmd.SourceStationNum = 4041;
                    }
                    cmd.SourceStationRow = 0;
                    cmd.SourceStationLayer = 0;
                    cmd.SourceStationColumn = 0;

                    //目的
                    cmd.SourceCellRow = ColumnChangeToDoubleRow(transportMsg.DestAddr, IsDoubleCell);
                    cmd.SourceCellLayer = transportMsg.DestAddr.Row;
                    cmd.SourceCellColumn = transportMsg.DestAddr.Column;

                    //if (transportMsg.TransportDevice.Id == 9002)
                    //{
                    //    cmd.SourceCellColumn = transportMsg.DestAddr.Column - 2;
                    //}
                    cmd.DestStationNum = 0;

                 
                }
                else if (startAddrType.Equals("FloorStation") && destAddrType.Equals("FloorStation"))
                {
                    cmd.CmdType = CmdTypeMode.StationToStation;
                }
                else if (startAddrType.Equals("Cell") && destAddrType.Equals("FloorStation"))
                {
                    cmd.CmdType = CmdTypeMode.Out;

                    cmd.SourceStationNum = 0;//源 站台编号
                    cmd.SourceStationRow = ColumnChangeToDoubleRow(transportMsg.StartAddr, IsDoubleCell); ;//源 排
                    cmd.SourceStationLayer = transportMsg.StartAddr.Row;//源 层
                    cmd.SourceStationColumn = transportMsg.StartAddr.Column; ;//源 列


                    //目的
                    cmd.SourceCellRow = 0;
                    cmd.SourceCellLayer = 0;
                    cmd.SourceCellColumn = 0;

                    if (transportMsg.DestAddr.IsContain("FloorStation:1_1_1"))
                    {
                        cmd.DestStationNum = 1001;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:2_1_1"))
                    {
                        cmd.DestStationNum = 1004;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:3_1_1"))
                    {
                        cmd.DestStationNum = 1020;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:4_2_1"))
                    {
                        cmd.DestStationNum = 2034;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:4_1_1"))
                    {
                        cmd.DestStationNum = 1040;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:5_1_1"))
                    {
                        cmd.DestStationNum = 1042;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:5_2_1"))
                    {
                        cmd.DestStationNum = 2036;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:1_3_1"))
                    {
                        cmd.DestStationNum = 3001;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:2_3_1"))
                    {
                        cmd.DestStationNum = 3004;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:3_3_1"))
                    {
                        cmd.DestStationNum = 3023;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:4_3_1"))
                    {
                        cmd.DestStationNum = 3039;
                    }
                    else if (transportMsg.DestAddr.IsContain("FloorStation:5_3_1"))
                    {
                        cmd.DestStationNum = 3040;
                    }
                }
                else if (startAddrType.Equals("DefaultRange") && destAddrType.Equals("Cell"))
                {
                    cmd.CmdType = CmdTypeMode.Pick;
                    cmd.SourceStationNum = 0;//源 站台编号
                    cmd.SourceStationRow = ColumnChangeToDoubleRow(transportMsg.StartAddr, IsDoubleCell); ;//源 排
                    cmd.SourceStationLayer = transportMsg.StartAddr.Row;//源 层
                    cmd.SourceStationColumn = transportMsg.StartAddr.Column; ;//源 列

                    //目的
                    cmd.SourceCellRow = 0;
                    cmd.SourceCellLayer = 0;
                    cmd.SourceCellColumn = 0;
                    //cmd.DestStationNum = StackIngcraneRelevanceStationManage.Instance.GetStackingcraneStationInfo(this.DeviceId).OutStationNum;//获取堆垛机对应的站台编号
                }
                else if (startAddrType.Equals("Cell") && destAddrType.Equals("Cell"))
                {
                    cmd.CmdType = CmdTypeMode.Move;
                    cmd.SourceStationNum = 0;//源 站台编号
                    cmd.SourceStationRow = ColumnChangeToDoubleRow(transportMsg.StartAddr, IsDoubleCell); //源 排
                    cmd.SourceStationLayer = transportMsg.StartAddr.Row;//源 层
                                                                        //cmd.SourceStationColumn = transportMsg.StartAddr.Column; //源 列

                    cmd.SourceStationColumn = transportMsg.StartAddr.Column;
                    cmd.SourceCellColumn = transportMsg.DestAddr.Column;

                    //目的
                    cmd.SourceCellRow = ColumnChangeToDoubleRow(transportMsg.DestAddr, IsDoubleCell); ;
                    cmd.SourceCellLayer = transportMsg.DestAddr.Row;
                    //cmd.SourceCellColumn = transportMsg.DestAddr.Column; 
                    cmd.DestStationNum = 0;//获取堆垛机对应的站台编号

                }
                else
                {
                    cmd.CmdType = CmdTypeMode.None;
                }
                cmd.SendCmdCode = 1;
                cmd.CmdNum = order.OrderId;
                cmd.TrayType = order.TrayType.GetValueOrDefault();
                cmd.VerificationCode = (int)cmd.CmdType  + cmd.SourceCellColumn + cmd.SourceCellLayer + cmd.SourceCellRow + cmd.SourceStationColumn + cmd.SourceStationLayer + cmd.SourceStationRow + cmd.SourceStationNum + cmd.DestStationNum+ cmd.TrayType;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
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
