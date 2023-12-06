using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.Xml;
using RFIDReaderAPI;
using RFIDReaderAPI.Interface;
using RFIDReaderAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Xml;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.ClouRFID.Model
{
    /// <summary>
    /// clou RFID扫描设备 
    /// </summary>
    public class RFIDForClou : IdentityDeviceAbstract<List<string>>, IAsynchronousMessage
    {
        object locker = new object();
        bool isClear = false;
        ClouRfidDeviceProperty DeviceProperty;
        #region 内部接口
        public override bool IsRepeated(List<string> toCompare)
        {
            return false;
        }

        public override OperateResult SaveContentToDatabase(List<string> content, HandleStatusEnum status)
        {
            throw new NotImplementedException();
        }

        public override OperateResult SetCurrentContent(List<string> content)
        {
            throw new NotImplementedException();
        }

        public override OperateResult DeleteLiveDataContent(LiveData content)
        {
            throw new NotImplementedException();
        }

        public override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                XmlNode xmlNode = doc.GetXmlNode("Device", "DeviceId", Id.ToString());


                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string stationPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(stationPropertyXml))
                {
                    try
                    {
                        DeviceProperty = (ClouRfidDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(ClouRfidDeviceProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：StationProperty", stationPropertyXml));
                }

                this.ExposedUnId = DeviceProperty.Config.ExposedUnId;

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult GetDeviceRealStatus()
        {
            throw new NotImplementedException();
        }

        public override OperateResult GetDeviceRealData()
        {
            throw new NotImplementedException();
        }

        public override OperateResult Availabe()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool Accessible(Addr destAddr)
        {
            throw new NotImplementedException();
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            throw new NotImplementedException();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            throw new NotImplementedException();
        }

        protected override OperateResult<ViewAbstract> CreateView()
        {
            throw new NotImplementedException();
        }

        protected override System.Windows.Window CreateAssistantView()
        {
            throw new NotImplementedException();
        }

        protected override System.Windows.Window CreateConfigView()
        {
            throw new NotImplementedException();
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            throw new NotImplementedException();
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            throw new NotImplementedException();
        }

        public override OperateResult UpdateProperty()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 公共属性
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 天线编号:indicates which antenna will be configuredIt starts at 0. So, 0 means first antenna and 3 means fourth antenna.
        /// </summary>
        public byte AntIdx { get; set; }
        #endregion

        public delegate void BarcodeEventHandler(string barcode);

        public event BarcodeEventHandler BarcodeChangedEvent;
        /// <summary>
        /// 条码发生改变时触发事件
        /// </summary>
        private string barcode;
        public string Barcode
        {
            get => barcode;
            set
            {
                barcode = value;
                BarcodeChangedEvent(value);
            }
        }

        public delegate void BarcodeCallback(List<string> barcodeList, params object[] para);
        /// <summary>
        /// 上报条码事件：上报设备名字、扫到的条码
        /// </summary>
        public event BarcodeCallback ReceiveBarcodeHandler;

        private string stationName = "CLOU-RFID";

        RFIDForClou tempRFIDForClou = null;
        eAntennaNo antennaNo;
        private List<string> rfidList = new List<string>();
        /// <summary>
        /// 连接标识 即对应的协议 ConnID
        /// </summary>
        private string ipAndPort = string.Empty;
        private ConvertMode convertMode = ConvertMode.Hex;
        //private List<string> barcodeList = new List<string>();
        private Dictionary<int, int> dicPowerParam = new Dictionary<int, int>();
        private int MaxSize = 0;
        private int RunMode = 1;
        private int SearchTime = 4;
        private int ScanTime = 10;//设置读取扫描时间
        private int ChangeRangeTime = 8;
        private int PowerValue = 33;
        private int ReadMaxCount = 65;
        /// <summary>
        /// 科陆RFID
        /// </summary>
        /// <param name="deviceName">备名称</param>
        /// <param name="rfidInfo">RFID设备配置信息</param>
        /// <param name="convertMode">PLC上报条码转换</param>
        public RFIDForClou(string rfidInfo, ConvertMode convertMode)
            : base()
        {
            this.convertMode = convertMode;
            if (tempRFIDForClou == null)
            {
                tempRFIDForClou = this;
            }
            ipAndPort = rfidInfo;
            Connection();
        }

        private bool Connection()
        {

            bool isConn = false;
            try
            {
                isConn = RFIDReader.CheckConnect(ipAndPort);
                if (!isConn)
                {
                    isConn = RFIDReader.CreateTcpConn(ipAndPort, this);
                    if (!isConn)
                    {
                        LogHelper.WriteLog(stationName, "RFID连接失败：CreateTcpConn() = false");
                    }
                }
                isConn = true;
            }
            catch (Exception ex)
            {
                isConn = false;
                LogHelper.WriteLog(stationName, "CLOU RFID 连接异常。/r/n" + ex.StackTrace);
            }
            return isConn;
        }
        private void Close()
        {

        }

        /// <summary>
        /// 开始读取标签
        /// </summary>
        /// <param name="readCount">读取次数</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public bool BeginRead(int readCount, out string error)
        {
            bool result = true;
            error = string.Empty;
            try
            {
                rfidList = new List<string>();
                MaxSize = 0;
                if (RunMode == 0)
                {
                    return result;
                }
                if (!Connection())
                {
                    result = false;
                    error = "与读写器创建连接失败";
                    return result;
                }
                if (!isReadAntenna)
                {
                    GetAntennaNo();
                }
                bool isFinish = false;



                int errCode = RFIDReader._Tag6C.GetEPC(ipAndPort, antennaNo, eReadType.Inventory);
                if (errCode != 0)
                {
                    result = false;
                    error = PaseError(errCode);
                }

                //for (int i = 0; i < readCount; i++)
                //{
                //    int sleepTime = SearchTime;
                //    int scanTime = ScanTime;
                //    int changeRangeTime = ChangeRangeTime;
                //    GetSleepTime(ref scanTime, ref sleepTime);
                //    int nextSleepTime = changeRangeTime - sleepTime;
                //    result = ChannelSwitchScan(eRF_Range.FCC_902_to_928MHz, true, sleepTime, 0, ref isFinish, out error);
                //    if (scanTime <= 0 || nextSleepTime <= 0)
                //    {
                //        break;
                //    }
                //    GetSleepTime(ref scanTime, ref nextSleepTime);
                //    result = ChannelSwitchScan(eRF_Range.FCC_902_to_928MHz, false, nextSleepTime, 1, ref isFinish, out error);
                //    if (isFinish || !result || scanTime <= 0)
                //    {
                //        break;
                //    }
                //    GetSleepTime(ref scanTime, ref sleepTime);
                //    result = ChannelSwitchScan(eRF_Range.ETSI_866_to_868MHz, true, sleepTime, 0, ref isFinish, out error);
                //    if (scanTime <= 0 || nextSleepTime <= 0)
                //    {
                //        break;
                //    }
                //    GetSleepTime(ref scanTime, ref nextSleepTime);
                //    result = ChannelSwitchScan(eRF_Range.ETSI_866_to_868MHz, false, nextSleepTime, 1, ref isFinish, out error);
                //    if (isFinish || !result || scanTime <= 0)
                //    {
                //        break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                result = false;
                error = ex.Message;
            }
            return result;
        }

        public List<string> EndRead()
        {
            if (RunMode == 1)
            {
                RFIDReader._Tag6C.Stop(ipAndPort);
                Thread.Sleep(100);
            }
            return rfidList;
        }

        /// <summary>
        /// 切换频道与基带参数的扫描模式
        /// </summary>
        /// <param name="range">频段</param>
        /// <param name="changeRange">是否切换频段</param>
        /// <param name="count">扫描时常(s)</param>
        /// <param name="searchType">盘存标志参数（0仅用Flag A盘存，1仅用Flag B盘存，2轮流使用Flag A和Flag B双面盘存）</param>
        /// <param name="isFinish">是否扫描完成</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        private bool ChannelSwitchScan(eRF_Range range, bool changeRange, int count, int searchType, ref bool isFinish, out string error)
        {
            bool result = true;
            error = string.Empty;
            int errCode = ChannelSwitchScan(range, changeRange, count, searchType, ref isFinish);
            if (isFinish)
            {
                return result;
            }
            else if (errCode != 0)
            {
                error = PaseError(errCode);
                result = false;
            }
            return result;
        }

        private string PaseError(int errCode)
        {
            string error = string.Empty;
            switch (errCode)
            {
                case 1:
                    error = "天线端口参数错误";
                    break;
                case 2:
                    error = "选择读取参数错误";
                    break;
                case 3:
                    error = "TID读取参数错误";
                    break;
                case 4:
                    error = "用户数据区读取参数错误";
                    break;
                case 5:
                    error = "保留区读取参数错误";
                    break;
                case 6:
                    error = "其他参数错误";
                    break;
                default:
                    error = "未知错误";
                    break;
            }
            return error;
        }

        private int ChannelSwitchScan(eRF_Range range, bool changeRange, int count, int searchType, ref bool isFinish)
        {
            int errCode = RFIDReader._RFIDConfig.SetEPCBaseBandParam(ipAndPort, 1, 7, 2, searchType);
            if (errCode != 0)
            {
                return errCode;
            }
            if (changeRange)
            {
                errCode = RFIDReader._RFIDConfig.SetReaderRF(ipAndPort, range);
                if (errCode != 0)
                {
                    return errCode;
                }
                if (dicPowerParam.Count > 0)
                {
                    errCode = RFIDReader._RFIDConfig.SetANTPowerParam(ipAndPort, dicPowerParam);
                    if (errCode != 0)
                    {
                        return errCode;
                    }
                }
            }
            errCode = RFIDReader._Tag6C.GetEPC(ipAndPort, antennaNo, eReadType.Inventory);
            if (errCode == 0)
            {
                isFinish = Sleep(count);
                RFIDReader._Tag6C.Stop(ipAndPort);
            }
            return errCode;
        }

        private bool Sleep(int count)
        {
            bool result = false;
            for (int i = 0; i < count; i++)
            {
                if (MaxSize > 0 && rfidList.Count >= MaxSize)
                {
                    result = true;
                    break;
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
            return result;
        }

        private void GetSleepTime(ref int scanTime, ref int sleepTime)
        {
            if (scanTime < sleepTime)
            {
                sleepTime = scanTime;
            }
            scanTime -= sleepTime;
        }

        bool isReadAntenna = false;
        void GetAntennaNo()
        {
            List<int> antList = new List<int> { 1, 2 };
            int count = 0;
            for (int i = 0; i < antList.Count; i++)
            {
                if (count == 0)
                {
                    antennaNo = RFIDReader._Tag6C.GeteAntennaNo(antList[i].ToString());
                }
                else
                {
                    antennaNo = antennaNo | RFIDReader._Tag6C.GeteAntennaNo(antList[i].ToString());
                }
                if (PowerValue > 0)
                {
                    dicPowerParam[i] = PowerValue;
                }
                else
                {
                    dicPowerParam = new Dictionary<int, int>();
                }
                count++;
            }


            //foreach (var item in antList.OrderBy(x => x))
            //{
            //    if (count == 0)
            //    {
            //        antennaNo = RFIDReader._Tag6C.GeteAntennaNo(item.ToString());
            //    }
            //    else
            //    {
            //        antennaNo = antennaNo | RFIDReader._Tag6C.GeteAntennaNo(item.ToString());
            //    }
            //    if (PowerValue > 0)
            //    {
            //        dicPowerParam[item] = PowerValue;
            //    }
            //    else
            //    {
            //        dicPowerParam = new Dictionary<int, int>();
            //    }
            //    count++;
            //}
            isReadAntenna = true;
        }

        /// <summary>
        /// 读取条码
        /// </summary>
        public void GetBarcode()
        {
            string error = string.Empty;
            if (!Connection())
            {
                error = "与读写器创建连接失败";
            }
            if (!isReadAntenna)
            {
                GetAntennaNo();
            }
            string err = "";
            BeginRead(ReadMaxCount, out err);
            Thread.Sleep(300);
            List<string> recBarCodeList = EndRead();

            List<string> result = new List<string>();
            if (recBarCodeList.Count == 0)
            {
                LogHelper.WriteLog(stationName, "RFID  recBarCodeList 没有读取到数据！");
            }
            if (rfidList.Count == 0)
            {
                LogHelper.WriteLog(stationName, "RFID  rfidList 没有读取到数据！");
            }
            if (rfidList != null && rfidList.Count > 0)
            {
                string str = string.Format("RFID 读取到的条码：未转换前 条码数量:{0}  条码信息:{1}", rfidList.Count.ToString(), string.Join(",", rfidList.ToArray()));
                LogHelper.WriteLog(stationName, str);
            }
            if (rfidList.Any())
            {
                foreach (string itemBarcode in rfidList)
                {
                    string formatBarcode = itemBarcode.Trim();
                    if (SystemConfig.Instance.WhCode.Equals("FS1"))
                    {
                        formatBarcode = ToConvertBarCode(itemBarcode.Trim());
                    }
                    if (!result.Contains(formatBarcode))
                    {
                        result.Add(formatBarcode);
                    }
                }
                result = result.Distinct().ToList();
            }
            rfidList.Clear();
            OnReceiveBarcode(result);
        }

        //处理箱表条码信息
        private string ToConvertBarCode(string barCode)
        {
            string nexBarCode = "";
            //单相表箱子	5	FD	WD
            //单相表条码	5	49	DY
            //三相表箱子	5	FC	WS
            //三相表条码	5	86	SF
            //互感器箱子	5	FA	WH
            string barCodeType = barCode.Substring(5, 2);
            switch (barCodeType)
            {
                case "FD":
                    nexBarCode = barCode.Substring(0, 5) + "WD" + barCode.Substring(7);
                    break;
                case "49":
                    nexBarCode = barCode.Substring(0, 5) + "DY" + barCode.Substring(7);
                    break;
                case "FC":
                    nexBarCode = barCode.Substring(0, 5) + "WS" + barCode.Substring(7);
                    break;
                case "86":
                    nexBarCode = barCode.Substring(0, 5) + "SF" + barCode.Substring(7);
                    break;
                case "FA":
                    nexBarCode = barCode.Substring(0, 5) + "WH" + barCode.Substring(7);
                    break;
                case "06":
                    nexBarCode = barCode.Substring(0, 5) + "ZF" + barCode.Substring(7);
                    break;
                case "FB":
                    nexBarCode = barCode.Substring(0, 5) + "WZ" + barCode.Substring(7);
                    break;
                case "01":
                    nexBarCode = barCode.Substring(0, 5) + "ZP" + barCode.Substring(7);
                    break;
                case "87":
                    nexBarCode = barCode.Substring(0, 5) + "SG" + barCode.Substring(7);
                    break;
            }
            return nexBarCode;
        }

        /// <summary>
        /// 条码上报处理
        /// </summary>
        /// <param name="barcodeList">上报的条码</param>
        /// <param name="para">para</param>
        protected void OnReceiveBarcode(List<string> barcodeList, params object[] para)
        {
            string logMessage = "上报条码：" + string.Join(",", barcodeList.ToArray());
            LogHelper.WriteLog(stationName, logMessage);
            if (ReceiveBarcodeHandler != null)
            {
                try
                {
                    ReceiveBarcodeHandler(barcodeList, para);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(stationName, ex.Message);
                }
            }
        }

        private string ConvertHexToDecimal(string formatBarcode)
        {
            List<string> strList = new List<string>();
            for (int i = 0; i < formatBarcode.Length / 2; i++)
            {
                strList.Add(formatBarcode.Substring(2 * i, 2));
            }
            var strArr = strList.ToArray();
            string newBarcode = string.Empty;
            for (int i = 0; i < strArr.Length; i++)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] array = new byte[] { (byte)System.Convert.ToInt32(strArr[i], 16) };
                newBarcode += asciiEncoding.GetString(array);
            }
            return newBarcode;
        }

        public void GPIControlMsg(GPI_Model gpi_model)
        {
            throw new NotImplementedException();
        }

        public void OutPutTags(Tag_Model tag)
        {
            lock (locker)
            {
                string code = tag.EPC;
                code = CodeConvert(code);
                Barcode = code;
                if (!rfidList.Contains(code))
                {
                    code = CodeMatch(code);
                    if (!string.IsNullOrEmpty(code))
                    {
                        rfidList.Add(code);
                        Barcode = code;
                    }
                }
            }
        }

        public void PortConnecting(string connID)
        {

        }

        private string CodeConvert(string code)
        {
            string result = code;
            return result;
        }
        private string CodeMatch(string code)
        {
            return code;
        }

        public void OutPutTagsOver()
        {

        }

        public void PortClosing(string connID)
        {

        }

        public void WriteDebugMsg(string msg)
        {

        }

        public void WriteLog(string msg)
        {

        }
    }
}
