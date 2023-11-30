using System;
using System.ServiceModel;
using System.Web.Services.Protocols;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.PlletizerWithControl.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLDC.Framework.Log.Helper;
using CLWCS.WareHouse.WebserviceForHeFei.CmdMode;
using Infrastructrue.Ioc.DependencyFactory;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using System.IO;
using System.Linq;
using System.Globalization;

namespace CLWCS.WareHouse.WebserviceForHeFei
{
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    [ServiceBehavior(Namespace = "http://www.szclou.com", InstanceContextMode = InstanceContextMode.Single)]
    public class WebserviceForHeFei : IWebserviceForHeFei, IService
    {
        string wcsWareHouseInFullPath = "";
        string wcsWareHouseOutFullPath = "";
        public WebserviceForHeFei()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            _receiveDataHandle = DependencyHelper.GetService<ReceiveDataAbstract>();
            _orderManage = DependencyHelper.GetService<OrderManage>();
            _robotReadyRecordDataHandle = DependencyHelper.GetService<RobotReadyRecordDataAbstract>();
            if (string.IsNullOrEmpty(wcsWareHouseInFullPath))
            {
                string strAppPath = Directory.GetCurrentDirectory();
                wcsWareHouseInFullPath = Path.Combine(strAppPath, @"SerialFile\wcsWareHouseInFiles.ini");
                wcsWareHouseOutFullPath = Path.Combine(strAppPath, @"SerialFile\wcsWareHouseOutFiles.ini");
            }
        }
        private string _serviceName = "提供Wms接口服务";
        private readonly IWmsWcsArchitecture _wmsWcsDataArchitecture;
        private readonly ReceiveDataAbstract _receiveDataHandle;
        private readonly OrderManage _orderManage;
        private readonly RobotReadyRecordDataAbstract _robotReadyRecordDataHandle;

        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            LogHelper.WriteLog(this.ServiceName, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }

        private OperateResult VerificateInstruct(SingleInstruct instruct)
        {
            if (string.IsNullOrEmpty(instruct.PACKAGE_BARCODE))
            {
                string msg = "上层系统下发的PACKAGE_BARCODE 垛号为空";
                return OperateResult.CreateFailedResult(msg, 1);
            }

            ///此处需要通过对接WMS的基础数据转换
            OperateResult<string> endAddr = _wmsWcsDataArchitecture.WmsToWcsAddr(instruct.DST_ADDR);
            if (!endAddr.IsSuccess)
            {
                string msg = string.Format("上层系统下发目标地址：{0} 转换成WCS系统的地址失败,失败原因：\r\n{1}，请检查基础参数", instruct.DST_ADDR,
                    endAddr.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            OperateResult<string> startAddr = _wmsWcsDataArchitecture.WmsToWcsAddr(instruct.SRC_ADDR);
            if (!startAddr.IsSuccess)
            {
                string msg = string.Format("上层系统下发开始地址：{0} 转换成WCS系统的地址失败，,失败原因：\r\n{1}, 请检查基础参数", instruct.SRC_ADDR,
                    startAddr.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 创建序列号
        /// </summary>
        /// <returns></returns>
        private static string GenerateSerialNoByTime()
        {
            TimeSpan ts1 = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 30, 0);
            double ts = Math.Round(ts1.TotalMilliseconds);
            return ts.ToString(CultureInfo.InvariantCulture);
        }

        private OperateResult<Order> ChangeInstructToOrder(SingleInstruct instruct)
        {
            Order order = new Order();
            OperateResult<Order> result = OperateResult.CreateFailedResult(order, "无数据");
            try
            {

                ///此处需要通过对接WMS的基础数据转换
                OperateResult<string> endAddr = _wmsWcsDataArchitecture.WmsToWcsAddr(instruct.DST_ADDR);
                if (!endAddr.IsSuccess)
                {
                    string msg = string.Format("上层系统下发目标地址：{0} 转换成WCS系统的地址失败，,失败原因：\r\n{1} 请检查基础参数", instruct.DST_ADDR,
                        endAddr.Message);
                    result.Message = msg;
                    result.IsSuccess = false;
                    return result;
                }

                OperateResult<string> startAddr = _wmsWcsDataArchitecture.WmsToWcsAddr(instruct.SRC_ADDR);

                if (!startAddr.IsSuccess)
                {
                    string msg = string.Format("上层系统下发开始地址：{0} 转换成WCS系统的地址失败，,失败原因：\r\n{1} 请检查基础参数", instruct.SRC_ADDR,
                        startAddr.Message);
                    result.Message = msg;
                    result.IsSuccess = false;
                    return result;
                }

                Addr destAddr = new Addr(endAddr.Content);
                string palletBarcode = instruct.PACKAGE_BARCODE;
                int priority = Convert.ToInt32(instruct.PRI);
                order.CurrAddr = new Addr(startAddr.Content);
                order.DestAddr = destAddr;
                order.IsReport = true;
                order.UpperTaskNo = instruct.ID.ToString();
                order.DocumentCode = instruct.ID.ToString();
                order.Source = TaskSourceEnum.UPPER;
                order.PileNo = palletBarcode;
                order.OrderPriority = priority;
                order.DeviceTaskNo = GenerateSerialNoByTime();
                order.Qty = instruct.PackageQTY;
                order.IsEmptyTray = instruct.IsEmptyTray;
                order.TrayType = instruct.TrayType;
                result.IsSuccess = true;

                switch (instruct.TYPE)
                {
                    case InstructTypeEnum.InStorehouse:
                        order.SourceTaskType = SourceTaskEnum.In;
                        order.OrderType = OrderTypeEnum.In;
                        break;
                    case InstructTypeEnum.ManualPutaway:
                        order.SourceTaskType = SourceTaskEnum.HandUpLoad;
                        order.OrderType = OrderTypeEnum.In;
                        break;
                    case InstructTypeEnum.OutStorehouse:
                        order.SourceTaskType = SourceTaskEnum.Out;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.ManualDown:
                        order.SourceTaskType = SourceTaskEnum.HandDownLoad;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.MoveStore:
                    case InstructTypeEnum.TempMove:
                        order.SourceTaskType = SourceTaskEnum.Move;
                        order.OrderType = OrderTypeEnum.Move;
                        break;
                    case InstructTypeEnum.InventoryStore:
                        order.SourceTaskType = SourceTaskEnum.InventoryIn;
                        order.OrderType = OrderTypeEnum.In;
                        break;
                    case InstructTypeEnum.InventoryOut:
                        order.SourceTaskType = SourceTaskEnum.InventoryOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.PickOut:
                        order.SourceTaskType = SourceTaskEnum.PickOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.PickIn:
                        order.SourceTaskType = SourceTaskEnum.PickIn;
                        order.OrderType = OrderTypeEnum.In;
                        break;
                    case InstructTypeEnum.Except:
                        order.SourceTaskType = SourceTaskEnum.UnKnow;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.QualityConfirmIn://品质确认入库
                        order.SourceTaskType = SourceTaskEnum.QualityConfirmIn;
                        order.OrderType = OrderTypeEnum.In;
                        break;
                    case InstructTypeEnum.QualityConfirmOut://品质确认出库
                        order.SourceTaskType = SourceTaskEnum.QualityConfirmOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.ManualEmptyTrayOut://人工空托盘出库
                        order.SourceTaskType = SourceTaskEnum.ManualEmptyTrayOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.ManualOut://人工出库
                        order.SourceTaskType = SourceTaskEnum.ManualOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.ReturnGoods://退货出库
                        order.SourceTaskType = SourceTaskEnum.ReturnGoods;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.ScrapOut://报废出库
                        order.SourceTaskType = SourceTaskEnum.ScrapOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.InventoryArrangeOut://库存整理出库
                        order.SourceTaskType = SourceTaskEnum.InventoryArrangeOut;
                        order.OrderType = OrderTypeEnum.Out;
                        break;
                    case InstructTypeEnum.InventoryArrangeIn://库存整理入库
                        order.SourceTaskType = SourceTaskEnum.InventoryArrangeIn;
                        order.OrderType = OrderTypeEnum.In;
                        break;
                    default:
                        order.SourceTaskType = SourceTaskEnum.UnKnow;
                        order.OrderType = OrderTypeEnum.UnKnow;
                        break;
                }

                result.Content = order;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        /// <summary>
        /// GCP给CS下发搬运指令
        /// </summary>
        private OperateResult HandleSendInstruct(string cmd)
        {
            //1.校验指令参数
            //2.判断指令的特殊处理
            //3.获取指令的目标地址（涉及到地址的转换）
            //4.判断路径是否可用
            //5.验证仓跺关系
            //6.正常指令生成指令
            //7.回复握手
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                SendInstructCmd sendInstructParameters = (SendInstructCmd) cmd;
                foreach (SingleInstruct instruct in sendInstructParameters.DATA)
                {
                    OperateResult verificateInstructResult = VerificateInstruct(instruct);
                    if (!verificateInstructResult.IsSuccess)
                    {
                        LogMessage(verificateInstructResult.Message, EnumLogLevel.Error, true);
                        handleResult.IsSuccess = false;
                        handleResult.Message = verificateInstructResult.Message;
                        return handleResult;
                    }

                    OperateResult<Order> changeToOrderResult = ChangeInstructToOrder(instruct);
                    if (!changeToOrderResult.IsSuccess)
                    {
                        LogMessage(changeToOrderResult.Message, EnumLogLevel.Error, true);
                        handleResult.IsSuccess = false;
                        handleResult.Message = changeToOrderResult.Message;
                        return handleResult;
                    }
                    Order currentOrder = changeToOrderResult.Content;

                    OperateResult<Order> generateResult = _orderManage.GenerateOrder(currentOrder);
                    if (generateResult.IsSuccess)
                    {
                        Order order = generateResult.Content;
                        //if (order.CurrAddr.IsContain("PutGoodsPort:1_1_1") && order.DestAddr.FullName.Contains("Cell:"))
                        //{
                        //    //上报卷帘门1008
                        //    var task = Task.Run(() =>
                        //    {
                        //        //1008 有指令入库
                        //        Random rd = new Random();
                        //        int sleepTimes = rd.Next(800, 3000);
                        //        Thread.Sleep(sleepTimes);
                        //        DispatchCrossDoorBus("01", order.UpperTaskNo);
                        //    });
                        //}
                        //if (order.CurrAddr.IsContain("GetGoodsPort:3_1_1") && order.DestAddr.FullName.Contains("Cell:"))
                        //{
                        //    //上报卷帘门2001
                        //    var task = Task.Run(() =>
                        //    {
                        //        //2001 有指令入库
                        //        Random rd = new Random();
                        //        int sleepTimes = rd.Next(800, 3000);
                        //        Thread.Sleep(sleepTimes);
                        //        DispatchCrossDoorBus("02", order.UpperTaskNo);
                        //    });
                        //}

                        LogMessage(string.Format("指令生成成功，指令：{0}", generateResult.Content), EnumLogLevel.Info, true);
                    }
                    else
                    {
                        string msg = string.Format("指令生成失败，指令信息：指令类型：{0}  开始地址：{1} 目标地址：{2} 垛号：{3} 优先级：{4}",
                            currentOrder.OrderType, currentOrder.CurrAddr, currentOrder.DestAddr, currentOrder.PileNo,
                            currentOrder.OrderPriority);
                        LogMessage(msg, EnumLogLevel.Error, true);
                        handleResult.IsSuccess = false;
                        handleResult.Message = msg;
                        return handleResult;
                    }
                }
                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;

            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;

        }

        private void DispatchCrossDoorBus(string areaCode, string id)
        {
            try
            {
                NotifyCrossDoorModel cmdMode = new NotifyCrossDoorModel();
                if (areaCode.Equals("01"))
                {
                    DeviceBaseAbstract xDevice1008 = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                    TransportPointStation xTransDevice1008 = xDevice1008 as TransportPointStation;
                    RollerDeviceControl xRoller1008 = xTransDevice1008.DeviceControl as RollerDeviceControl;
                    OperateResult<string> xOpReadBarCode1008 = xRoller1008.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    if (xOpReadBarCode1008.IsSuccess)
                    {
                        cmdMode.DATA.PalletBarcode = xOpReadBarCode1008.Content;
                    }
                }
                else if (areaCode.Equals("02"))
                {
                    DeviceBaseAbstract xDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                    TransportPointStation xTransDevice2001 = xDevice2001 as TransportPointStation;
                    RollerDeviceControl xRoller2001 = xTransDevice2001.DeviceControl as RollerDeviceControl;
                    OperateResult<string> xOpReadBarCode2001 = xRoller2001.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    if (xOpReadBarCode2001.IsSuccess)
                    {
                        cmdMode.DATA.PalletBarcode = xOpReadBarCode2001.Content;
                    }
                }
                cmdMode.DATA.AreaCode = areaCode;
                cmdMode.DATA.Id = int.Parse(id);
                string cmdPara = JsonConvert.SerializeObject(cmdMode);
                string interfaceName = "NotifyCrossDoor";
                NotifyElement element = new NotifyElement("", interfaceName, "卷帘门关门上报", null, cmdPara);
                OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                if (!result.IsSuccess)
                {
                    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                    LogMessage("卷帘门关门上报结果异常：" + msg, EnumLogLevel.Error, false);
                }
                else
                {
                    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                    LogMessage("卷帘门关门上报成功：" + msg, EnumLogLevel.Info, false);
                }
            }
            catch (Exception ex)
            {
                LogMessage("卷帘门关门上报结果异常，参数数据：" + ex.ToString(), EnumLogLevel.Error, false);
            }
        }


        private object _lockObj = new object();
        public string SendInstruct(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "SendInstruct", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();

            //SendInstructCmd sendInstructParameters = cmd.ToObject<SendInstructCmd>();
            //if (sendInstructParameters == null || sendInstructParameters.DATA == null || sendInstructParameters.DATA.Count == 0)
            //{
            //    string repeatMsg = string.Format("参数有误", "SendInstruct", cmd);
            //    handleResult.RESULT = 0;
            //    handleResult.ERR_MSG = repeatMsg;
            //    LogMessage(repeatMsg, EnumLogLevel.Error, true);
            //    return handleResult.ToString();
            //}
            //var instructIds = sendInstructParameters.DATA.Where(t => t.ID > 0).Select(t => t.ID).Distinct().ToList();
            //if (instructIds == null || instructIds.Count == 0)
            //{
            //    string repeatMsg = string.Format("参数有误,指令Id为空", "SendInstruct", cmd);
            //    handleResult.RESULT = 0;
            //    handleResult.ERR_MSG = repeatMsg;
            //    LogMessage(repeatMsg, EnumLogLevel.Error, true);
            //    return handleResult.ToString();
            //}
            ReceiveDataModel dataModel = new ReceiveDataModel("SendInstruct", cmd, ServiceName);

            lock (_lockObj)
            {
                try
                {
                    bool isRepeatInvoke = _receiveDataHandle.IsExistDataBase(dataModel);
                    if (isRepeatInvoke)
                    {
                        string repeatMsg = string.Format("重复调用接口：{0} 参数：{1}", "SendInstruct", cmd);
                        handleResult.RESULT = 0;
                        handleResult.ERR_MSG = repeatMsg;
                        LogMessage(repeatMsg, EnumLogLevel.Error, true);
                        return handleResult.ToString();
                    }
                    _receiveDataHandle.Insert(dataModel);
                    OperateResult handleInstructResult = HandleSendInstruct(cmd);
                    if (!handleInstructResult.IsSuccess)
                    {
                        handleResult.RESULT = 0;
                        handleResult.ERR_MSG = handleInstructResult.Message;
                        LogMessage(handleInstructResult.Message, EnumLogLevel.Error, true);
                        dataModel.HandleDateTime = DateTime.Now;
                        dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                        dataModel.HandleMessage = handleInstructResult.Message;
                        _receiveDataHandle.Update(dataModel);
                        return handleResult.ToString();
                    }
                    handleResult.RESULT = CLDC.CLWS.CLWCS.WareHouse.DataModel.HandleResult.Success;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleMessage = "处理成功";
                    _receiveDataHandle.Update(dataModel);
                    string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "SendInstruct", cmd);
                    LogMessage(handleLog, EnumLogLevel.Info, true);
                    handleResult.ERR_MSG = "成功";
                    return handleResult.ToString();

                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("处理下发搬运指令接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "SendInstruct"),
                        EnumLogLevel.Error, true);
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = ex.Message;
                    _receiveDataHandle.Update(dataModel);
                }
                return handleResult.ToString();
            }
        }

        public string SendUnstackCmd(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "SendUnstackCmd", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            //处理拆垛指令
            SendUnstackCmd unStackCmd = cmd.ToObject<SendUnstackCmd>();
            if (unStackCmd == null || unStackCmd.DATA == null)
            {
                string repeatMsg = string.Format("参数有误", "SendInstruct", cmd);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = repeatMsg;
                LogMessage(repeatMsg, EnumLogLevel.Error, true);
                return handleResult.ToString();
            }
            var instructIds =new List<int> { unStackCmd.DATA.ID };
            if (instructIds == null || instructIds.Count == 0)
            {
                string repeatMsg = string.Format("参数有误,指令Id为空", "SendInstruct", cmd);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = repeatMsg;
                LogMessage(repeatMsg, EnumLogLevel.Error, true);
                return handleResult.ToString();
            }
            ReceiveDataModel dataModel = new ReceiveDataModel("SendUnstackCmd", cmd, ServiceName,instructIds);
            try
            {
                bool isRepeatInvoke = _receiveDataHandle.IsExistDataBase(dataModel);
                if (isRepeatInvoke)
                {
                    string repeatMsg = string.Format("重复调用接口：{0} 参数：{1}", "SendUnstackCmd", cmd);
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = repeatMsg;
                    LogMessage(repeatMsg, EnumLogLevel.Error, true);
                    return handleResult.ToString();
                }
                _receiveDataHandle.SaveAsync(dataModel);


                DeviceBaseAbstract device = DeviceManage.Instance.FindDeviceByDeviceName(unStackCmd.DATA.PALLETIZER_NAME);
                if (device == null)
                {
                    string msg = string.Format("查找不到设备名称：{0} 的设备，请核实设备信息", unStackCmd.DATA.PALLETIZER_NAME);
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = msg;
                    LogMessage(msg, EnumLogLevel.Error, true);
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleMessage = "处理失败";
                    _receiveDataHandle.SaveAsync(dataModel);

                    return handleResult.ToString();
                }
                IPalletize palletizerDevice = device as IPalletize;
                if (palletizerDevice == null)
                {
                    string msg = string.Format("设备编号：{0} 设备名：{1} 不具备拆垛功能，请核实设备信息", device.Id, device.DeviceName);
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = msg;
                    LogMessage(msg, EnumLogLevel.Error, true);
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleMessage = "处理失败";
                    _receiveDataHandle.SaveAsync(dataModel);

                    return handleResult.ToString();
                }

                DePalletizeCmd dePalletizeCmd = new DePalletizeCmd()
                {
                    ContainerType = unStackCmd.DATA.CONTAINER_TYPE,
                    DePalletizeCount = unStackCmd.DATA.QTY,
                    PileNum = 5,
                    UpperTaskCode = unStackCmd.DATA.ID.ToString(),
                    DeviceTaskCode = unStackCmd.DATA.ID.ToString()
                };
                OperateResult sendTaskResult = palletizerDevice.DePalletize(dePalletizeCmd.ToString());
                if (!sendTaskResult.IsSuccess)
                {
                    string msg = string.Format("拆盘机下发拆盘任务失败 参数:{0},原因：{1}", dePalletizeCmd, sendTaskResult.Message);
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = msg;
                    LogMessage(msg, EnumLogLevel.Error, true);
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleMessage = "处理失败";
                    _receiveDataHandle.SaveAsync(dataModel);

                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                handleResult.ERR_MSG = "成功";
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "SendUnstackCmd", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理下发拆盘指令接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "SendUnstackCmd"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        public string SendDeviceMode(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "SendDeviceMode", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncNewResReMsg handleResult = new SyncNewResReMsg();
            //返回集合的对象
            handleResult.SyncNewResReMsgDataList = new List<SyncNewResReMsgData>();
            SyncNewResReMsgData resReMsgData = new SyncNewResReMsgData();
            ReceiveDataModel dataModel = new ReceiveDataModel("SendDeviceMode", cmd, ServiceName);

            try
            {
                List<SendModeData> temp = Json.ToList<SendModeData>(cmd);
                if (temp == null || temp.Count == 0)
                {
                    string msg = string.Format("模式切换失败:{0}", "传入的参数为空不正确");
                    HandCmd(handleResult, dataModel, msg);
                    resReMsgData.RESULT = 0;
                    resReMsgData.MESSAGE = msg;
                    handleResult.SyncNewResReMsgDataList.Add(resReMsgData);
                    return handleResult.ToString();
                }

                foreach (var sendModeData in temp)
                {
                    try
                    {
                        resReMsgData = new SyncNewResReMsgData();
                        int wmsMode = sendModeData.MODE;
                        string curAddr = sendModeData.INANDOUTPORT.Trim();
                        resReMsgData.INANDOUTPORT = curAddr;
                        if (!wmsMode.Equals(0) && !wmsMode.Equals(1) && !wmsMode.Equals(2))
                        {
                            string msg = string.Format("下发的设备模式值：{0} 不在协议范围内", wmsMode);
                            HandCmd(handleResult, dataModel, msg);
                            resReMsgData.RESULT = 0;
                            resReMsgData.MESSAGE = msg;
                            handleResult.SyncNewResReMsgDataList.Add(resReMsgData);
                            continue;
                        }
                        DeviceModeEnum wcsMode = DeviceModeEnum.Default;
                        if (wmsMode.Equals(1))
                        {
                            wcsMode = DeviceModeEnum.In;
                        }
                        else if (wmsMode.Equals(2))
                        {
                            wcsMode = DeviceModeEnum.Out;
                        }
                        else if (wmsMode.Equals(0))
                        {
                            wcsMode = DeviceModeEnum.Default;
                        }
                        int deviceId = 0;
                        //根据地址 获取到设备的OPC地址
                        if (curAddr.Equals("InAndOutPort:1_1_1")) //1001  1号口
                        {
                            deviceId = 1001001;
                        }
                        else if (curAddr.Equals("InAndOutPort:2_1_1")) //1113  2号口
                        {
                            deviceId = 1113001;
                        }
                        DeviceBaseAbstract swithDeviceAbstract = DeviceManage.Instance.FindDeivceByDeviceId(deviceId);
                        if (swithDeviceAbstract == null)
                        {
                            string msg = string.Format("根据设备ID：{0} 获取设备不到对应设备，请检查配置信息", deviceId);
                            HandCmd(handleResult, dataModel, msg);
                            resReMsgData.RESULT = 0;
                            resReMsgData.MESSAGE = msg;
                            handleResult.SyncNewResReMsgDataList.Add(resReMsgData);
                            continue;
                        }
                        if (swithDeviceAbstract is IChangeMode)
                        {
                            IChangeMode modeChangeDevice = swithDeviceAbstract as IChangeMode;
                            OperateResult changeMode = modeChangeDevice.ChangeMode(wcsMode);
                            string msg = string.Format("设备ID：{0} 切换模式：{1} {2}", deviceId, wcsMode,
                                changeMode.IsSuccess ? "成功" : "失败 " + changeMode.Message);
                            if (changeMode.IsSuccess)
                            {
                                resReMsgData.RESULT = HandleResult.Success;
                            }
                            else
                            {
                                resReMsgData.RESULT = HandleResult.Failed;
                            }
                            LogMessage(msg, EnumLogLevel.Info, true);
                            resReMsgData.MESSAGE = msg;
                        }

                        dataModel.HandleDateTime = DateTime.Now;
                        dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                        dataModel.HandleMessage = resReMsgData.MESSAGE;
                        handleResult.SyncNewResReMsgDataList.Add(resReMsgData);
                        _receiveDataHandle.SaveAsync(dataModel);
                    }
                    catch (Exception ex)
                    {
                        resReMsgData.RESULT = 0;
                        resReMsgData.MESSAGE = OperateResult.ConvertExMessage(ex);
                        handleResult.SyncNewResReMsgDataList.Add(resReMsgData);
                        LogMessage(string.Format("设备模式：{0} 转换发生异常:{1}", cmd, resReMsgData.MESSAGE), EnumLogLevel.Error,
                            true);
                    }
                }
                return handleResult.ToString();

            }
            catch (Exception ex)
            {
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = OperateResult.ConvertExMessage(ex);

                resReMsgData.RESULT = 0;
                resReMsgData.MESSAGE = OperateResult.ConvertExMessage(ex);
                handleResult.SyncNewResReMsgDataList.Add(resReMsgData);

                _receiveDataHandle.SaveAsync(dataModel);
                LogMessage(string.Format("设备模式：{0} 转换发生异常:{1}", cmd, OperateResult.ConvertException(ex)),
                    EnumLogLevel.Error, true);
            }
            return handleResult.ToString();
        }

        private void HandCmd(SyncNewResReMsg handleResult, ReceiveDataModel dataModel, string msg)
        {
            LogMessage(msg, EnumLogLevel.Error, true);
            //handleResult.RESULT = HandleResult.Failed;
            //handleResult.MESSAGE = msg;
            dataModel.HandleMessage = msg;
            dataModel.HandleDateTime = DateTime.Now;
            dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
            _receiveDataHandle.SaveAsync(dataModel);
        }

        /// <summary>
        /// 称重请求
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string SendWeightRequest(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "SendWeightRequest", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("SendWeightRequest", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);
                //OperateResult handleSendWeightRequestResult = HandleSendWeightRequest(cmd);
                OperateResult handleSendWeightRequestResult = OperateResult.CreateFailedResult("需求不明确，功能暂未开发，未读取");
                if (!handleSendWeightRequestResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleSendWeightRequestResult.Message;
                    LogMessage(handleSendWeightRequestResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleSendWeightRequestResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "SendWeightRequest", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理称重请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "SendWeightRequest"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        /// <summary>
        /// 刷卡通知
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyScannerCard(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyScannerCard", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("NotifyScannerCard", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);
             
                OperateResult handleNotifyScannerCardResult = HandleNotifyScannerCard(cmd);
                if (!handleNotifyScannerCardResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "NotifyScannerCard", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理刷卡请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "NotifyScannerCard"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }
        /// <summary>
        ///处理刷卡 开始和超时
        /// </summary>
        private OperateResult HandleNotifyScannerCard(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                NotifyScannerCardCmd scannerCardCmd = cmd.ToObject<NotifyScannerCardCmd>();
                if (scannerCardCmd.ActionNum == 1 || scannerCardCmd.ActionNum == 2)
                {

                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的动作类型错误! 错误值:" + scannerCardCmd.ActionNum.ToString();
                    return handleResult;
                }
                if (scannerCardCmd.PostionNum <= 0 || scannerCardCmd.PostionNum > 6)
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的位置地址类型错误! 错误值:" + scannerCardCmd.PostionNum.ToString();
                    return handleResult;
                }
                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(3010);
                TransportPointStation transDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
               
                int writeValue = scannerCardCmd.ActionNum == 1 ? 888 : 0;
                OperateResult opcResult3010 = OperateResult.CreateSuccessResult();
                switch (scannerCardCmd.PostionNum)
                {
                    case 1://入库口卷帘门
                        break;
                    case 2://出库口卷帘门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.OpcDoorIsAllowed2, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入 3010 失败";
                            return handleResult;
                        }
                        break;
                    case 3://出库口进机器人门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.OpcDoorIsAllowed3, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入 3010 失败";
                            return handleResult;
                        }
                        break;
                    case 4://进1号堆垛机门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.OpcDoorIsAllowed4, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入3010 失败";
                            return handleResult;
                        }
                        break;
                    case 5://进2号堆垛机门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.OpcDoorIsAllowed5, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入 3010 失败";
                            return handleResult;
                        }
                        break;
                    case 6://进入库 维修门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.OpcDoorIsAllowed6, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入 3010 失败";
                            return handleResult;
                        }
                        break;
                    default:
                        break;
                }
                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }

        public string OpenOrCloseDoorRequest(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "OpenOrCloseDoorRequest", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("OpenOrCloseDoorRequest", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleNotifyScannerCardResult = HandleOpenOrCloseDoorRequest(cmd);
                if (!handleNotifyScannerCardResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "OpenOrCloseDoorRequest", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("开关门请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "OpenOrCloseDoorRequest"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        /// <summary>
        /// 处理请求开关门  入库 
        /// </summary>
        private OperateResult HandleOpenOrCloseDoorRequest(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                NotifyScannerCardCmd scannerCardCmd = cmd.ToObject<NotifyScannerCardCmd>();
                if( scannerCardCmd.ActionNum==1 || scannerCardCmd.ActionNum==2)
                {

                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的动作类型错误! 错误值:" + scannerCardCmd.ActionNum.ToString();
                    return handleResult;
                }
                if (scannerCardCmd.PostionNum <= 0 || scannerCardCmd.PostionNum > 6)
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的位置地址类型错误!  错误值:" + scannerCardCmd.PostionNum.ToString();
                    return handleResult;
                }
                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(3010);
                TransportPointStation transDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                int writeValue = scannerCardCmd.ActionNum == 1 ? 1 : 2;
                OperateResult opcResult3010 = OperateResult.CreateSuccessResult();
                switch (scannerCardCmd.PostionNum)
                {
                    case 1://入库口卷帘门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.OpcDoorIsAllowed1, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入 3010 失败";
                            return handleResult;
                        }
                        break;
                    case 2://出库口卷帘门
                      
                        break;
                    case 3://出库口进机器人门
                       
                        break;
                    case 4://进1号堆垛机门
                      
                        break;
                    case 5://进2号堆垛机门
                       
                        break;
                    case 6://进入库 维修门
                       
                        break;
                    default:
                        break;
                }
                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }

        /// <summary>
        /// 通知报警 开始和暂停
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyOrClearWarning(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyOrClearWarning", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("NotifyOrClearWarning", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleNotifyScannerCardResult = HandleNotifyOrClearWarning(cmd);
                if (!handleNotifyScannerCardResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "NotifyOrClearWarning", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理通知报警请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "NotifyOrClearWarning"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }


        private OperateResult HandleNotifyOrClearWarning(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                NotifyScannerCardCmd scannerCardCmd = cmd.ToObject<NotifyScannerCardCmd>();
                if (scannerCardCmd.ActionNum == 1 || scannerCardCmd.ActionNum == 2)
                {

                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的动作类型错误！ 错误值:" + scannerCardCmd.ActionNum.ToString();
                    return handleResult;
                }
                if (scannerCardCmd.PostionNum <= 0 || scannerCardCmd.PostionNum > 6)
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的位置地址类型错误! 错误值：" + scannerCardCmd.PostionNum.ToString();
                    return handleResult;
                }
                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(3010);
                TransportPointStation transDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;

                int writeValue = scannerCardCmd.ActionNum == 1 ? 888 : 0;
                OperateResult opcResult3010 = OperateResult.CreateSuccessResult();
                switch (scannerCardCmd.PostionNum)
                {
                    case 1://入库口卷帘门

                        break;
                    case 2://出库口卷帘门

                        break;
                    case 3://出库口进机器人门
                    case 4://进1号堆垛机门
                    case 5://进2号堆垛机门
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.NotifyOrClearWarningOut, writeValue);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入 3010 失败";
                            return handleResult;
                        }
                        break;
                    case 6://进入库 维修门

                        break;
                    default:
                        break;
                }
                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }

        /// <summary>
        /// 强制结束拆码垛任务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ForceCloseTask(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "ForceCloseTask", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, false);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("ForceCloseTask", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleNotifyScannerCardResult = HandleForceCloseTask(cmd);
                if (!handleNotifyScannerCardResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "ForceCloseTask", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理通知报警请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "ForceCloseTask"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        private OperateResult HandleForceCloseTask(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                ForceCloseTaskCmd forceCloseTaskCmd = cmd.ToObject<ForceCloseTaskCmd>();
                if (string.IsNullOrEmpty(forceCloseTaskCmd.area_code))
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的area_code 不能为空";
                    return handleResult;
                }

                if (forceCloseTaskCmd.area_code.Equals("01") || forceCloseTaskCmd.area_code.Equals("02"))
                {

                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的area_code 错误！ 传入值为:" + forceCloseTaskCmd.area_code;
                    return handleResult;
                }

                if (forceCloseTaskCmd.area_code.Equals("01"))
                {
                    if(forceCloseTaskCmd.taskType==0|| forceCloseTaskCmd.taskType == 1)
                    {
                        bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", forceCloseTaskCmd.taskType.ToString(), wcsWareHouseInFullPath);
                        string strMsg = string.Format("处理wms结束任务 IniHelper 3003 IsForceInTask 写值:{0} 结果: {1}", forceCloseTaskCmd.taskType.ToString(), isWriteOk ? "成功" : "失败");
                        LogMessage(strMsg, EnumLogLevel.Info, false);

                        bool isWriteOk2 = IniHelper.WriteIniData("3003", "IsStartTask", "false", wcsWareHouseInFullPath);
                        string strResult = isWriteOk2 ? "成功" : "失败";
                        LogMessage("处理wms结束任务 3003  IsStartTask 写  " + "false" + strResult, EnumLogLevel.Info, false);
                    }


                    ////调用wms叠盘完成
                    //int weight = 0;
                    //DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                    //TransportPointStation transDevice = curDevice as TransportPointStation;
                    //RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                    //OperateResult<string> opcResult1008 = roller.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                    //OperateResult<int> goodsWeightResult = roller.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
                    //if (goodsWeightResult.IsSuccess)
                    //{
                    //    weight = goodsWeightResult.Content;
                    //}
                    //PalletizerFinishParaMode parmData = new PalletizerFinishParaMode
                    //{
                    //    ADDR = "PutGoodsPort:1_1_1",
                    //    PALLETBARCODE = "",
                    //    Weight = weight
                    //};
                    //if (opcResult1008.IsSuccess && !string.IsNullOrEmpty(opcResult1008.Content))
                    //{
                    //    parmData.PALLETBARCODE = opcResult1008.Content;
                    //}

                    //NotifyPalletizerFinishCmdMode mode = new NotifyPalletizerFinishCmdMode
                    //{
                    //    DATA = parmData
                    //};

                    //string cmdPara = JsonConvert.SerializeObject(mode);
                    //string interfaceName = "NotifyPalletizerFinish";
                    //NotifyElement element = new NotifyElement("", interfaceName, "入库码盘完成上报", null, cmdPara);
                    //OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                    //if (!result.IsSuccess)
                    //{
                    //    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                    //    handleResult.ErrorCode = 2;
                    //    handleResult.IsSuccess = false;
                    //    handleResult.Message = result.Message;
                    //}
                    //else
                    //{
                    //    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                    //    handleResult.IsSuccess = true;
                    //    handleResult.Message = "上报码垛箱已满接口成功!";
                    //}



                    //CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(3001) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
                    //XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
                    ////terminate_pallet 强制结束码垛  1008
                    ////terminate_task  强制结束拆垛  1004、1007
                    //DeviceBaseAbstract curDevice1004 = DeviceManage.Instance.FindDeivceByDeviceId(1004);
                    //TransportPointStation transDevice1004 = curDevice1004 as TransportPointStation;
                    //RollerDeviceControl roller1004 = transDevice1004.DeviceControl as RollerDeviceControl;
                    //OperateResult<int> opOrderIdResult1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    //OperateResult<int> opDirectionResult1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.WriteDirectionDataBlock);

                    //if (opOrderIdResult1004.IsSuccess && opDirectionResult1004.IsSuccess)
                    //{
                    //    if (opOrderIdResult1004.Content > 0 && opDirectionResult1004.Content == 1004)
                    //    {
                    //        if (opOrderIdResult1004.Content != 1004)
                    //        {
                    //            OperateResult op = abbControl.terminate_task(new terminate_taskMode
                    //            {
                    //                area_code = forceCloseTaskCmd.area_code,
                    //                task_id = opOrderIdResult1004.Content.ToString()
                    //            });
                    //            LogMessage("通知结束 1004 任务 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
                    //            if (!op.IsSuccess)
                    //            {
                    //                handleResult.IsSuccess = false;
                    //                handleResult.Message = "机器人返回错误信息:" + op.Message;
                    //                return handleResult;
                    //            }
                    //        }


                    //    }
                    //}
                    //DeviceBaseAbstract curDevice1007 = DeviceManage.Instance.FindDeivceByDeviceId(1007);
                    //TransportPointStation transDevice1007 = curDevice1007 as TransportPointStation;
                    //RollerDeviceControl roller1007 = transDevice1007.DeviceControl as RollerDeviceControl;
                    //OperateResult<int> opOrderIdResult1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    //OperateResult<int> opDirectionResult1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.WriteDirectionDataBlock);

                    //if (opOrderIdResult1007.IsSuccess&& opDirectionResult1007.IsSuccess)
                    //{
                    //    if (opOrderIdResult1007.Content > 0 && opDirectionResult1007.Content==1007)
                    //    {
                    //        if (opOrderIdResult1007.Content != 1007)
                    //        {
                    //            OperateResult op = abbControl.terminate_task(new terminate_taskMode
                    //            {
                    //                area_code = forceCloseTaskCmd.area_code,
                    //                task_id = opOrderIdResult1007.Content.ToString()
                    //            });
                    //            LogMessage("通知结束 1007 任务 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
                    //            if (!op.IsSuccess)
                    //            {
                    //                handleResult.IsSuccess = false;
                    //                handleResult.Message = "机器人返回错误信息:" + op.Message;
                    //                return handleResult;
                    //            }
                    //        }
                    //    }
                    //}

                    //DeviceBaseAbstract curDevice1008 = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                    //TransportPointStation transDevice1008 = curDevice1008 as TransportPointStation;
                    //RollerDeviceControl roller1008 = transDevice1008.DeviceControl as RollerDeviceControl;
                    //OperateResult<string> opBarcodeResult1008 = roller1008.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    //OperateResult<int> opOrderIdResult1008 = roller1008.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    //if (opOrderIdResult1008.IsSuccess)
                    //{
                    //    if (!string.IsNullOrEmpty(opBarcodeResult1008.Content))
                    //    {
                    //        if(opOrderIdResult1008.IsSuccess&& opOrderIdResult1008.Content == -1000)
                    //        {
                    //            //说明
                    //            handleResult.IsSuccess = true;
                    //            handleResult.Message = "WCS无需调用机器人 自行处理托盘放行成功";
                    //            return handleResult;
                    //        }

                    //        OperateResult op = abbControl.terminate_pallet(new terminate_palletMode
                    //        {
                    //            area_code = forceCloseTaskCmd.area_code,
                    //            pallet_id = opBarcodeResult1008.Content,
                    //            ws_id = forceCloseTaskCmd.area_code.Equals("01") ? "2" : "0"
                    //        });
                    //        LogMessage("通知结束 1008 任务 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
                    //        if (!op.IsSuccess)
                    //        {
                    //            handleResult.IsSuccess = false;
                    //            handleResult.Message = "机器人返回错误信息:" + op.Message;
                    //            return handleResult;
                    //        }
                    //    }
                    //}

                }
                else if (forceCloseTaskCmd.area_code.Equals("02"))
                {
                    //CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(3002) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
                    //XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
                    ////terminate_pallet 强制结束码垛  2007
                    ////terminate_task  强制结束拆垛  2001
                    //DeviceBaseAbstract curDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                    //TransportPointStation transDevice2001 = curDevice2001 as TransportPointStation;
                    //RollerDeviceControl roller2001 = transDevice2001.DeviceControl as RollerDeviceControl;
                    //OperateResult<int> opOrderIdResult2001 = roller2001.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    //if (opOrderIdResult2001.IsSuccess)
                    //{
                    //    if (opOrderIdResult2001.Content > 0)
                    //    {
                    //        OperateResult op = abbControl.terminate_task(new terminate_taskMode
                    //        {
                    //            area_code = forceCloseTaskCmd.area_code,
                    //            task_id = opOrderIdResult2001.Content.ToString()
                    //        });
                    //        LogMessage("通知结束 2001 任务 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
                    //        if (!op.IsSuccess)
                    //        {
                    //            handleResult.IsSuccess = false;
                    //            handleResult.Message = "机器人返回错误信息:" + op.Message;
                    //            return handleResult;
                    //        }
                    //    }
                    //}

                    //DeviceBaseAbstract curDevice2007 = DeviceManage.Instance.FindDeivceByDeviceId(2007);
                    //TransportPointStation transDevice2007 = curDevice2007 as TransportPointStation;
                    //RollerDeviceControl roller2007 = transDevice2007.DeviceControl as RollerDeviceControl;
                    //OperateResult<string> opOrderIdResult2007 = roller2007.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    //if (opOrderIdResult2007.IsSuccess)
                    //{
                    //    if (!string.IsNullOrEmpty(opOrderIdResult2007.Content))
                    //    {
                    //        OperateResult op = abbControl.terminate_pallet(new terminate_palletMode
                    //        {
                    //            area_code = forceCloseTaskCmd.area_code,
                    //            pallet_id = "",//出库 码垛无托盘编号
                    //            ws_id = forceCloseTaskCmd.area_code.Equals("01") ? "2" : "0"
                    //        });
                    //        LogMessage("通知结束 2007 任务 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
                    //        if (!op.IsSuccess)
                    //        {
                    //            handleResult.IsSuccess = false;
                    //            handleResult.Message = "机器人返回错误信息:" + op.Message;
                    //            return handleResult;
                    //        }
                    //    }
                    //}
                    if (forceCloseTaskCmd.taskType == 0 || forceCloseTaskCmd.taskType == 1)
                    {
                        bool isWriteOk = IniHelper.WriteIniData("3011", "IsForceOutTask", forceCloseTaskCmd.taskType.ToString(), wcsWareHouseOutFullPath);
                        string strMsg = string.Format("IniHelper 3011 IsForceOutTask 写值:{0} 结果: {1}", forceCloseTaskCmd.taskType.ToString(), isWriteOk ? "成功" : "失败");
                        LogMessage(strMsg, EnumLogLevel.Error, false);
                    }
                }

                ////处理wms结束任务 
                //try
                //{
                //    DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                //    TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                //    RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                  
                //    OperateResult writeIsLoadedResult2019= roller2019.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                //    if(writeIsLoadedResult2019.IsSuccess)
                //    {
                //        LogMessage("处理wms 结束码垛 状态 2019 false 成功！", EnumLogLevel.Info, true);
                //    }
                //    else
                //    {
                //        string exMsg = string.Format("处理wms 结束任务，WCS 2019 载货写入失败，原因：{0}", writeIsLoadedResult2019.Message);
                //        LogMessage(exMsg, EnumLogLevel.Error, false);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    string exMsg = string.Format("2019 载货写入失败，原因：{0}", OperateResult.ConvertException(ex));
                //    LogMessage(exMsg, EnumLogLevel.Error, true);
                //}
              
                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }

        public string NotifyTrespassOpenDoor(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyTrespassOpenDoor", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("NotifyTrespassOpenDoor", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleNotifyTrespassOpenDoorResult = HandleNotifyTrespassOpenDoor(cmd);
                if (!handleNotifyTrespassOpenDoorResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyTrespassOpenDoorResult.Message;
                    LogMessage(handleNotifyTrespassOpenDoorResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyTrespassOpenDoorResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "NotifyTrespassOpenDoor", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理异常开关门接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "NotifyTrespassOpenDoor"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        private OperateResult HandleNotifyTrespassOpenDoor(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                TrespassOpenDoorCmd trespassOpenDoorCmd = cmd.ToObject<TrespassOpenDoorCmd>();
                if (!trespassOpenDoorCmd.PostionNum.Equals(3) && !trespassOpenDoorCmd.PostionNum.Equals(4)
                    && !trespassOpenDoorCmd.PostionNum.Equals(5))
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的PostionNum 不在范围内,实际传入值为:" + trespassOpenDoorCmd.PostionNum; ;
                    return handleResult;
                }

                if (!trespassOpenDoorCmd.ActionNum.Equals(1) && !trespassOpenDoorCmd.ActionNum.Equals(2))
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的ActionNum 错误！ 实际传入值为:" + trespassOpenDoorCmd.ActionNum;
                    return handleResult;
                }

                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(3010);
                TransportPointStation transDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;

                int writeValue = trespassOpenDoorCmd.ActionNum == 1 ? 888 : 0;
                OperateResult opcResult3010 = OperateResult.CreateSuccessResult();
                opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.NotifyTrespassOpenDoor, writeValue);
                if (!opcResult3010.IsSuccess)
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "处理失败，写入3010 失败，写入值:"+ writeValue.ToString();
                    return handleResult;
                }

                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }


        public string ReSendTask(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "ReSendTask", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("ReSendTask", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleReSendTaskResult = HandleReSendTask(cmd);
                if (!handleReSendTaskResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleReSendTaskResult.Message;
                    LogMessage(handleReSendTaskResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleReSendTaskResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "ReSendTask", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理重发机器人业务接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "ReSendTask"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }


        private OperateResult HandleReSendTask(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                ReSendTaskCmd reSendTaskCmd = cmd.ToObject<ReSendTaskCmd>();
                if (!reSendTaskCmd.area_code.Equals("01") && !reSendTaskCmd.area_code.Equals("02"))
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "area_code 错误！ 实际传入值为:" + reSendTaskCmd.area_code;
                    return handleResult;
                }

                if (reSendTaskCmd.area_code.Equals("02"))
                {
                    //处理出库 重新扫描问题
                    if (reSendTaskCmd.node_id_list != null
                        && reSendTaskCmd.node_id_list.Exists(x => x == 1004))
                    {
                        DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(3010);
                        TransportPointStation transDevice = curDevice as TransportPointStation;
                        RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;

                        OperateResult opcResult3010 = OperateResult.CreateSuccessResult();
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 0);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入3010 失败，写入值:" + 0;
                            return handleResult;
                        }
                        CancellationTokenSource cts = new CancellationTokenSource();
                        Task.Delay(2000, cts.Token);
                     
                        opcResult3010 = roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -1000);
                        if (!opcResult3010.IsSuccess)
                        {
                            handleResult.IsSuccess = false;
                            handleResult.Message = "处理失败，写入3010 失败，写入值:" + -1000;
                            return handleResult;
                        }
                    }
                }
              
                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }


        /// <summary>
        /// 通知 入库任务开始和停止
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyStartTask(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyStartTask", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("NotifyStartTask", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleNotifyScannerCardResult = HandleNotifyStartTask(cmd);
                if (!handleNotifyScannerCardResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "NotifyStartTask", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理通知开始任务请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "NotifyStartTask"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        private OperateResult HandleNotifyStartTask(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            try
            {
                NotifyStartTaskCmd startTaskCmd = cmd.ToObject<NotifyStartTaskCmd>();
                if (startTaskCmd.ActionNum == 1 || startTaskCmd.ActionNum == 2)
                {
                   
                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的动作类型错误！ 错误值:" + startTaskCmd.ActionNum.ToString();
                    return handleResult;
                }
               
                if (startTaskCmd.area_code.Equals("01") || startTaskCmd.area_code.Equals("02"))
                {
                   
                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的area_code位置地址类型错误! 错误值：" + startTaskCmd.area_code;
                    return handleResult;
                }
                if(startTaskCmd.ActionNum==1 || startTaskCmd.ActionNum == 2)
                {
                    //wms start
                    //处理wms结束任务 
                    try
                    {
                        string writeValue = startTaskCmd.ActionNum == 1 ? "true" : "false";

                        bool isWriteOk = IniHelper.WriteIniData("3003", "IsStartTask", writeValue, wcsWareHouseInFullPath);
                        string strResult = isWriteOk ? "成功" : "失败";
                        LogMessage("处理wms开始任务 3003  IsStartTask 写  "+ writeValue + strResult, EnumLogLevel.Info, false);
                        //处理 to do 开始结束任务 3003


                        string writeValue2 = startTaskCmd.ActionNum == 1 ? "" : "1";
                        bool isWriteOk2 = IniHelper.WriteIniData("3003", "IsForceInTask", writeValue2, wcsWareHouseInFullPath);
                        LogMessage("处理wms开始任务 3003  IsForceInTask 写  " + writeValue2 + strResult, EnumLogLevel.Info, false);

                        //DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                        //TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                        //RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                        //bool isLoaded = startTaskCmd.ActionNum == 1 ? true : false;

                        //OperateResult writeIsLoadedResult2019 = roller2019.Communicate.Write(DataBlockNameEnum.IsLoaded, isLoaded);
                        //if (writeIsLoadedResult2019.IsSuccess)
                        //{
                        //    LogMessage("处理wms开始任务 状态 2019  成功！", EnumLogLevel.Info, true);
                        //}
                        //else
                        //{
                        //    string exMsg = string.Format("处理wms开始任务，WCS 2019 载货写入失败，原因：{0}", writeIsLoadedResult2019.Message);
                        //    LogMessage(exMsg, EnumLogLevel.Error, false);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //string exMsg = string.Format("2019 载货写入失败，原因：{0}", OperateResult.ConvertException(ex));
                        //LogMessage(exMsg, EnumLogLevel.Error, true);
                    }
                }

                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }

        /// <summary>
        /// WMS通知wcs库存整理任务 开始
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NotifyInventoryArrangeTask(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyInventoryArrangeTask", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("NotifyInventoryArrangeTask", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                OperateResult handleNotifyScannerCardResult = HandleNotifyInventoryArrangeTask(cmd);
                if (!handleNotifyScannerCardResult.IsSuccess)
                {
                    handleResult.RESULT = 0;
                    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                    dataModel.HandleDateTime = DateTime.Now;
                    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                    _receiveDataHandle.SaveAsync(dataModel);
                    return handleResult.ToString();
                }
                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "NotifyInventoryArrangeTask", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理通知开始任务请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "NotifyInventoryArrangeTask"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        /// <summary>
        /// 处理库存整理业务
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private OperateResult HandleNotifyInventoryArrangeTask(string cmd)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            int taskType = 0;
            try
            {
                NotifyInventoryArrangeTaskCmd taskCmd = cmd.ToObject<NotifyInventoryArrangeTaskCmd>();
                if (taskCmd.area_code == "01" || taskCmd.area_code == "02")
                {

                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的 area_code 错误！ 错误值:" + taskCmd.area_code;
                    return handleResult;
                }
                if (taskCmd.taskType == 1)
                {
                    //1出 默认1008-xxx  4
                    //if (!string.IsNullOrEmpty(taskCmd.dest_addr))
                    //{
                    //    if (taskCmd.dest_addr.Equals("GetGoodsPort:1_1_1"))
                    //    {
                    //        //1004-1
                    //        taskType = 41;
                    //    }
                    //    else if (taskCmd.dest_addr.Equals("GetGoodsPort:2_1_1"))
                    //    {
                    //        //1007-2
                    //        taskType = 42;
                    //    }
                    //    else
                    //    {
                    //        handleResult.IsSuccess = false;
                    //        handleResult.Message = "传入的 dest_addr 错误！ 错误值:" + taskCmd.dest_addr;
                    //        return handleResult;
                    //    }
                    //}
                }
                else if (taskCmd.taskType == 2)
                {
                    // 2入， 默认xxx - 1008-4
                    //if (!string.IsNullOrEmpty(taskCmd.start_addr))
                    //{
                    //    if (taskCmd.start_addr.Equals("GetGoodsPort:1_1_1"))
                    //    {
                    //        //1004-1
                    //        taskType = 14;
                    //    }
                    //    else if (taskCmd.start_addr.Equals("GetGoodsPort:2_1_1"))
                    //    {
                    //        //1007-2
                    //        taskType = 24;
                    //    }
                    //    else
                    //    {
                    //        handleResult.IsSuccess = false;
                    //        handleResult.Message = "传入的 start_addr 错误！ 错误值:" + taskCmd.start_addr;
                    //        return handleResult;
                    //    }
                    //}
                    
                }
                else
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "传入的 taskType 错误！ 错误值:" + taskCmd.taskType;
                    return handleResult;
                }
                int newTaskType = int.Parse(taskCmd.taskType.ToString());

                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(3008);
                TransportPointStation curTransDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller3008 = curTransDevice.DeviceControl as RollerDeviceControl;
                if(!roller3008.Communicate.Write(DataBlockNameEnum.OPCWeightDataBlock, newTaskType).IsSuccess)
                {
                    handleResult.IsSuccess = false;
                    handleResult.Message = "WCS 写入节点3008 OPCWeightDataBlock  OPC 失败:" + newTaskType;
                    return handleResult;
                }

                handleResult.IsSuccess = true;
                handleResult.Message = "处理成功";
                return handleResult;
            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.IsSuccess = false;
                handleResult.Message = exMsg;
            }
            return handleResult;
        }

        public string ApplyCrossDoorRequest(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "ApplyCrossDoorRequest", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            ReceiveDataModel dataModel = new ReceiveDataModel("ApplyCrossDoorRequest", cmd, ServiceName);
            try
            {
                _receiveDataHandle.SaveAsync(dataModel);

                var doorOpenOrCloseCmd=cmd.ToObject<ApplyDoorOpenOrCloseModel>();
                if (doorOpenOrCloseCmd == null)
                {
                    string msg = string.Format("参数错误：{0}",cmd);
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);

                    return handleResult.ToJson();
                }
                //OperateResult handleNotifyScannerCardResult = HandleNotifyInventoryArrangeTask(cmd);
                //if (!handleNotifyScannerCardResult.IsSuccess)
                //{
                //    handleResult.RESULT = 0;
                //    handleResult.ERR_MSG = handleNotifyScannerCardResult.Message;
                //    LogMessage(handleNotifyScannerCardResult.Message, EnumLogLevel.Error, true);
                //    dataModel.HandleDateTime = DateTime.Now;
                //    dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                //    dataModel.HandleMessage = handleNotifyScannerCardResult.Message;
                //    _receiveDataHandle.SaveAsync(dataModel);
                //    return handleResult.ToString();
                //}

                DeviceBaseAbstract device = DeviceManage.Instance.FindDeivceByDeviceId(11901);
                if (device == null)
                {
                    string msg = string.Format("查找不到设备名称：11901 的设备，请核实设备信息");
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);

                    return handleResult.ToJson();
                }
                SwitchDevice switchDevice = device as SwitchDevice;
                if (switchDevice == null)
                {
                    string msg = string.Format("搬运设备转化失败");
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);

                    return handleResult.ToJson();
                }
                ClSwitchDeviceControl roller = switchDevice.DeviceControl as ClSwitchDeviceControl;
                //1.AGV请求进入，请求开门
                //2.允许AGV进入，已开门
                //3.AGV进入完成，请求关门
                //4.AGV进入完成，已关门

                //5.AGV请求离开，请求开门
                //6.允许AGV离开，已开门
                //7.AGV离开完成，请求关门
                //8.AGV离开完成，已关门
                int val = 0;
                if (doorOpenOrCloseCmd.IsIn)
                {
                    if (doorOpenOrCloseCmd.IsOpen)
                    {
                        val = 1;
                    }
                    else
                    {
                        val = 3;
                    }
                }
                else
                {
                    if (doorOpenOrCloseCmd.IsOpen)
                    {
                        val = 5;
                    }
                    else
                    {
                        val = 7;
                    }
                }
                OperateResult readOpcResult = roller.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, val);

                if (readOpcResult.IsSuccess == false)
                {
                    string msg = "PLC 写入 " + switchDevice.Id + " 失败!";
                    LogMessage(msg, EnumLogLevel.Error, true);
                    handleResult.ERR_MSG = "PLC 写入 " + switchDevice.Id + " 失败!";
                    handleResult.RESULT = HandleResult.Failed;
                    return handleResult.ToJson();
                }

                handleResult.RESULT = HandleResult.Success;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandledSuccess;
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleMessage = "处理成功";
                _receiveDataHandle.SaveAsync(dataModel);
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "ApplyCrossDoorRequest", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理通知开始任务请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "ApplyCrossDoorRequest"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
                dataModel.HandleDateTime = DateTime.Now;
                dataModel.HandleStatus = ReceiveDataHandleStatus.HandleFailed;
                dataModel.HandleMessage = ex.Message;
                _receiveDataHandle.SaveAsync(dataModel);
            }
            return handleResult.ToString();
        }

        private RollerDeviceControl GetTransportDeviceControlByDeviceId(int deviceId,out string addr,out string errorMsg)
        {
            errorMsg = "";
            addr = "";
            DeviceBaseAbstract device = DeviceManage.Instance.FindDeivceByDeviceId(deviceId);
            if (device == null)
            {
                errorMsg = string.Format("查找不到设备名称：{0} 的设备，请核实设备信息",deviceId.ToString());
                return null;
            }
            addr=device.CurAddress.ToString();
            TransportPointStation transDevice = device as TransportPointStation;
            if (transDevice == null)
            {
                errorMsg = string.Format("搬运设备转化失败");
                return null;
            }
            RollerDeviceControl rollerDeviceControl = transDevice.DeviceControl as RollerDeviceControl;
            if (rollerDeviceControl == null)
            {
                errorMsg= string.Format("搬运设备控制类失败");
                return null;
            }
            return rollerDeviceControl;
        }
        public string NotifyAgvCarryFinished(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyAgvCarryFinished", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            try
            {
                var data = cmd.ToObject<AgvCarryFinishedModel>();
                if (data == null)
                {
                    string msg = string.Format("参数错误：{0}", cmd);
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);

                    return handleResult.ToJson();
                }
                int count = _robotReadyRecordDataHandle.GetRobotReadyRecordsCount(new RobotReadyRecordQueryInput
                {
                    OrderNO=data.OrderNO,
                    WhereLambda=t=>t.Status!=2,
                });
                if (count > 0)
                {
                    string msg = string.Format("任务已经存在：{0}", data.OrderNO);
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                count = _robotReadyRecordDataHandle.GetRobotReadyRecordsCount(new RobotReadyRecordQueryInput
                {
                    WhereLambda=t=>t.Status!=2,
                    Addr1=data.Addr,
                });
                if (count > 0)
                {
                    string msg = $"地址{data.Addr}存在未完成的任务";
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                #region 左右上料位
                string errorMsg;
                string addr;
                RollerDeviceControl leftRollerDeviceControl = GetTransportDeviceControlByDeviceId(1021,out addr,out errorMsg);
                if (leftRollerDeviceControl == null)
                {
                    handleResult.ERR_MSG = errorMsg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(errorMsg, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                RollerDeviceControl rightRollerDeviceControl = GetTransportDeviceControlByDeviceId(1022, out addr, out errorMsg);
                if (rightRollerDeviceControl == null)
                {
                    handleResult.ERR_MSG = errorMsg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(errorMsg, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                #endregion
                OperateResult<bool> leftIsReadyContent=leftRollerDeviceControl.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                OperateResult<bool> rightIsReadyContent = rightRollerDeviceControl.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                var res =_robotReadyRecordDataHandle.Insert(new RobotReadyRecord
                {
                    OrderNO=data.OrderNO,
                    Addr1=data.Addr,
                    Ready1=data.Ready,
                    ReadyTime1=data.ReadyTime,
                    ContainerNO1=data.ContainerNO,
                    CreateDate=DateTime.Now,
                    Ready2=0,
                    Status=0,
                    StepIndex=0,
                });
                if (!res.IsSuccess)
                {
                    handleResult.ERR_MSG = res.Message;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(res.Message, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                handleResult.RESULT = HandleResult.Success;
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "NotifyAgvCarryFinished", cmd);
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理通知开始任务请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, cmd, "NotifyAgvCarryFinished"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
            }
            return handleResult.ToString();
        }


        public string RobotHasTrayRequest()
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "HasTrayRequest", "");
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            try
            {
                #region 左右上料位
                string errorMsg;
                string leftAddr;
                RollerDeviceControl leftRollerDeviceControl = GetTransportDeviceControlByDeviceId(1021, out leftAddr, out errorMsg);
                if (leftRollerDeviceControl == null)
                {
                    handleResult.ERR_MSG = errorMsg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(errorMsg, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                string rightAddr;
                RollerDeviceControl rightRollerDeviceControl = GetTransportDeviceControlByDeviceId(1022, out rightAddr, out errorMsg);
                if (rightRollerDeviceControl == null)
                {
                    handleResult.ERR_MSG = errorMsg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(errorMsg, EnumLogLevel.Error, true);
                    return handleResult.ToJson();
                }
                #endregion
                OperateResult<bool> leftIsReadyContent = leftRollerDeviceControl.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                OperateResult<bool> rightIsReadyContent = rightRollerDeviceControl.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                HasTrayResultCmdModel data = new HasTrayResultCmdModel();
                if (leftIsReadyContent.Content)
                {
                    var palletBarcodeContent=leftRollerDeviceControl.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    data.PointTrayData.Add(new PointTrayDataModel
                    {
                        Addr= leftAddr,
                        PalletBarcode= palletBarcodeContent.Content,
                        HasTray= leftIsReadyContent.Content,
                    });
                }
                if (rightIsReadyContent.Content)
                {
                    var palletBarcodeContent = rightRollerDeviceControl.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    data.PointTrayData.Add(new PointTrayDataModel
                    {
                        Addr = rightAddr,
                        PalletBarcode = palletBarcodeContent.Content,
                        HasTray = rightIsReadyContent.Content,
                    });
                }

                handleResult.DATA = data;
                handleResult.RESULT = HandleResult.Success;
                string handleLog = string.Format("成功处理WMS的调用接口：{0} 参数：{1}", "HasTrayRequest", "");
                LogMessage(handleLog, EnumLogLevel.Info, true);
                handleResult.ERR_MSG = "成功";
                return handleResult.ToString();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("处理是否有托盘请求接口:{2} 数据异常：{0} 参数：{1}", ex.Message, "", "HasTrayRequest"),
                    EnumLogLevel.Error, true);
                handleResult.RESULT = 0;
                handleResult.ERR_MSG = OperateResult.ConvertException(ex);
            }
            return handleResult.ToString();
        }

        public string NotifyPlcRobotIn(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyPlcRobotIn", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            try
            {
                RobotInCmd data = cmd.ToObject<RobotInCmd>();

                DeviceBaseAbstract device = DeviceManage.Instance.FindDeivceByDeviceId(13001);
                if (device == null)
                {
                    string msg = string.Format("查找不到设备Id：{0} 的设备，请核实设备信息", 13001);
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);

                    return handleResult.ToJson();
                }

                SwitchDevice switchDevice = device as SwitchDevice;
                if (switchDevice == null)
                {
                    string msg = string.Format("设备转化失败");
                    handleResult.ERR_MSG = msg;
                    handleResult.RESULT = HandleResult.Failed;
                    LogMessage(msg, EnumLogLevel.Error, true);

                    return handleResult.ToJson();
                }

                ClSwitchDeviceControl deviceControl = switchDevice.DeviceControl as ClSwitchDeviceControl;

                OperateResult writOpcResult = deviceControl.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, data.IsStart?1:0);
                
                if (writOpcResult.IsSuccess == false )
                {
                    string msg = "PLC 写入 " + 13001 + " 失败!";
                    LogMessage(msg, EnumLogLevel.Error, true);
                    handleResult.ERR_MSG = "PLC 写入 " + 13001 + " 失败!";
                    handleResult.RESULT = HandleResult.Failed;
                    return handleResult.ToJson();
                }
                handleResult.RESULT = HandleResult.Success;
                handleResult.ERR_MSG = "处理成功";
                return handleResult.ToJson();

            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.RESULT = HandleResult.Failed;
                handleResult.ERR_MSG = exMsg;
            }
            return handleResult.ToJson();
        }

        public string NotifyPriorityChange(string cmd)
        {
            string receiveLog = string.Format("接收到WMS调用接口：{0} 参数：{1}", "NotifyPriorityChange", cmd);
            LogMessage(receiveLog, EnumLogLevel.Info, true);
            SyncResReErr handleResult = SyncResReErr.CreateFailedResult();
            try
            {
                List<InstructPriorityCmd> list = cmd.ToObject <List<InstructPriorityCmd>>();
                if(list!=null && list.Count > 0)
                {
                    foreach(var item in list)
                    {
                        var res=_orderManage.UpdateOrderPriority(item.ID.ToString(), item.PRI);
                        if (!res.IsSuccess)
                        {
                            handleResult.RESULT = HandleResult.Failed;
                            handleResult.ERR_MSG = "调整优先级失败";
                            return handleResult.ToJson();
                        }
                    }
                }
                handleResult.RESULT = HandleResult.Success;
                handleResult.ERR_MSG = "处理成功";
                return handleResult.ToJson();

            }
            catch (Exception ex)
            {
                string exMsg = string.Format("解析指令信息失败，原因：{0}", OperateResult.ConvertException(ex));
                LogMessage(exMsg, EnumLogLevel.Error, true);
                handleResult.RESULT = HandleResult.Failed;
                handleResult.ERR_MSG = exMsg;
            }
            return handleResult.ToJson();
        }
    }
}
