using System;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using System.Threading;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using Newtonsoft.Json;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using System.IO;
using System.Diagnostics;

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    /// 出库调度
    /// </summary>
    public class DispatchOutGoodsTransportPointStationBusiness : OrderWorkerBuinessAbstract
    {
        string wcsWareHouseInFullPath = "";
        string wcsWareHouseOutFullPath = "";
        protected override OperateResult ParticularInitlize()
        {
            if(string.IsNullOrEmpty(wcsWareHouseInFullPath))
            {
                string strAppPath = Directory.GetCurrentDirectory();
                wcsWareHouseInFullPath = Path.Combine(strAppPath, @"SerialFile\wcsWareHouseInFiles.ini");
                wcsWareHouseOutFullPath = Path.Combine(strAppPath, @"SerialFile\wcsWareHouseOutFiles.ini");
            }
           
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }
        private  void DispatchWmsStockBus()
        {
            Thread.Sleep(3 * 1000);//等待plc称重平稳

            try
            {
                DeviceBaseAbstract xDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                TransportPointStation xTransDevice2001 = xDevice2001 as TransportPointStation;
                RollerDeviceControl xRoller2001 = xTransDevice2001.DeviceControl as RollerDeviceControl;
                OperateResult<int> xOpWeight2001 = xRoller2001.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
                OperateResult<string> xOpReadBarCode2001 = xRoller2001.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                //NotifyStocktakingInstructResult
                NotifyStocktakingResultCmdMode cmdMode = new NotifyStocktakingResultCmdMode();
                cmdMode.DATA.DST_ADDR = xTransDevice2001.CurAddress.FullName;
                if (xOpWeight2001.IsSuccess)
                {
                    cmdMode.DATA.WEIGHT = xOpWeight2001.Content;
                }
                if (xOpReadBarCode2001.IsSuccess)
                {
                    cmdMode.DATA.PALLETBARCODE = xOpReadBarCode2001.Content;
                }

                string cmdPara = JsonConvert.SerializeObject(cmdMode);
                string interfaceName = "NotifyStocktakingInstructResult";
                NotifyElement element = new NotifyElement("", interfaceName, "盘点完成结果上报", null, cmdPara);
                OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                if (!result.IsSuccess)
                {
                    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                    LogMessage("上报盘点结果异常：" + msg, EnumLogLevel.Error, false);
                }
                else
                {
                    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                    LogMessage("上报盘点结果成功：" + msg, EnumLogLevel.Info, false);
                }
            }
            catch (Exception ex)
            {
                LogMessage("上报盘点结果异常，参数数据：" + ex.ToString(), EnumLogLevel.Error, false);
            }

        }

        private void DispatchCrossDoorBus(string areaCode,string id)
        {
            try
            {
                NotifyCrossDoorModel cmdMode = new NotifyCrossDoorModel();
                if(areaCode.Equals("01"))
                {
                    DeviceBaseAbstract xDevice1008 = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                    TransportPointStation xTransDevice1008 = xDevice1008 as TransportPointStation;
                    RollerDeviceControl xRoller1008 = xTransDevice1008.DeviceControl as RollerDeviceControl;
                    OperateResult<string> xOpReadBarCode1008 = xRoller1008.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    if(xOpReadBarCode1008.IsSuccess)
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

                Stopwatch sw = new Stopwatch();
                sw.Start();
                OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                sw.Stop();
                LogMessage("卷帘门关门 NotifyCrossDoor上报耗时：" + sw.ElapsedMilliseconds+"毫秒", EnumLogLevel.Info, false);

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

        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            LogMessage("开始进入 AfterFinishTransportMsgBusiness 业务！", EnumLogLevel.Info, false);

            if (transportMsg.DestDevice.Id == 1008)
            {
                //上报卷帘门1008
                var task = Task.Run(() =>
                {
                    DispatchCrossDoorBus("01", transportMsg.TransportOrder.UpperTaskNo);
                });
                //延时等PLC写-1000
                Thread.Sleep(1500);

                DeviceBaseAbstract curDevice3003 = DeviceManage.Instance.FindDeivceByDeviceId(3003);
                TransportPointStation curTransDevice3003 = curDevice3003 as TransportPointStation;
                RollerDeviceControl roller1111 = curTransDevice3003.DeviceControl as RollerDeviceControl;
                roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, false);

                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                TransportPointStation curTransDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller1008 = curTransDevice.DeviceControl as RollerDeviceControl;
                //如果是库存整理 类型 则记录
                if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.InventoryArrangeOut))
                {
                    //库存整理出库
                  
                    if (roller1008.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 18).IsSuccess)
                    {
                        LogMessage("AfterFinishTransportMsgBusiness 当前搬运 库存整理出库！", EnumLogLevel.Info, false);
                    }
                }
                else
                {
                    //非库存整理  
                    roller1008.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                    DeviceBaseAbstract curDevice3008 = DeviceManage.Instance.FindDeivceByDeviceId(3008);
                    TransportPointStation curTransDevice3008 = curDevice3008 as TransportPointStation;
                    RollerDeviceControl roller3008 = curTransDevice3008.DeviceControl as RollerDeviceControl;
                    roller3008.Communicate.Write(DataBlockNameEnum.OPCWeightDataBlock, 0);
                }

                var task3 = Task.Run(() =>
                {
                    Thread.Sleep(2000);
                    DeviceBaseAbstract curDevice3007 = DeviceManage.Instance.FindDeivceByDeviceId(3007);
                    TransportPointStation curTransDevice3007 = curDevice3007 as TransportPointStation;
                    RollerDeviceControl roller3007 = curTransDevice3007.DeviceControl as RollerDeviceControl;
                    OperateResult<int> opOrderIDResult = roller3007.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
                    if(opOrderIDResult.IsSuccess&& opOrderIDResult.Content!=99)
                    {
                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0);
                    }
                    bool isWriteOk = IniHelper.WriteIniData("3003", "Node1008IsReBack", "false", wcsWareHouseInFullPath);
                    roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, true);

                    LogMessage("AfterFinishTransportMsgBusiness 入库 出库托盘到达，等待入库！", EnumLogLevel.Info, false);

                });
                return OperateResult.CreateSuccessResult("");
            }

            if (transportMsg.DestDevice.Id != 2001) return OperateResult.CreateSuccessResult();

            //上报卷帘门 2001
            var tsk = Task.Run(() =>
            {
                DispatchCrossDoorBus("02", transportMsg.TransportOrder.UpperTaskNo);
            });

            DeviceBaseAbstract curDevice3011 = DeviceManage.Instance.FindDeivceByDeviceId(3011);
            TransportPointStation transDevice3011 = curDevice3011 as TransportPointStation;
            RollerDeviceControl roller3011 = transDevice3011.DeviceControl as RollerDeviceControl;

            //if (!transportMsg.DestDevice.DeviceName.ToString().Equals("PointStation#2001")) return OperateResult.CreateSuccessResult();
            if (transportMsg.TransportOrder.IsEmptyTray)
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是空托盘！", EnumLogLevel.Info, false);
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                return OperateResult.CreateSuccessResult("出空托盘已到达2001，等待人工搬走");
            }

            if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.QualityConfirmOut))
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【品质确认】出库货物！", EnumLogLevel.Info, false);
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，等待WMS下发指令");
            }
            else if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.ManualEmptyTrayOut))
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【人工空托盘】出库货物！", EnumLogLevel.Info, false);
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，等待WMS下发指令");
            }
            else if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.ManualOut))
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【人工出库】货物！", EnumLogLevel.Info, false);
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，等待WMS下发指令");
            }
            else if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.ReturnGoods))
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【退货出库】货物！", EnumLogLevel.Info, false);
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，等待WMS下发指令");
            }
            else if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.ScrapOut))
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【报废出库】货物！", EnumLogLevel.Info, false);
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，等待WMS下发指令");
            }
            else if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.InventoryOut))
            {
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【盘点出库】货物！", EnumLogLevel.Info, false);
                //var task = Task.Run(() =>
                //{
                //    Thread.Sleep(1000);
                //    DispatchWmsStockBus();
                //});
                //return OperateResult.CreateSuccessResult("托盘已到达2001，等待WMS下发盘点回库指令");
            }
            if (transportMsg.TransportOrder.IsEmptyTray)
            {
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【空托盘】货物！", EnumLogLevel.Info, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，空托盘任务，等待人工处理");
            }



            //DeviceBaseAbstract curDevice3011 = DeviceManage.Instance.FindDeivceByDeviceId(3011);
            //TransportPointStation transDevice3011 = curDevice3011 as TransportPointStation;
            //RollerDeviceControl roller3011 = transDevice3011.DeviceControl as RollerDeviceControl;

            if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.UnKnow))
            {
                roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                LogMessage("AfterFinishTransportMsgBusiness 当前搬运是【未知指令】货物！", EnumLogLevel.Info, false);
                return OperateResult.CreateSuccessResult("托盘已到达2001，未知任务，等待人工确认异常处理");
            }
            var task2 = Task.Run(() =>
            {
                Thread.Sleep(2000);
                OutGoodsBusiness(transportMsg, roller3011);
            });
          
            return OperateResult.CreateSuccessResult();
        }

        private void OutGoodsBusiness(TransportMessage transportMsg, RollerDeviceControl roller3011)
        {
            LogMessage("开始处理业务 全部出库 或 拣选出库 或 盘点出库 AfterFinishTransportMsgBusiness！", EnumLogLevel.Info, false);

            ////清理出库ini文件
            //IniHelper.WriteIniData("3011", "OPCBarcodeDataBlock2", "", wcsFullPath);
            //IniHelper.WriteIniData("3011", "OPCBarcodeDataBlock21", "", wcsFullPath);
            //IniHelper.WriteIniData("3011", "OPCBarcodeDataBlock22", "", wcsFullPath);
            //IniHelper.WriteIniData("3011", "IsForceOutTask", "false", wcsFullPath);
            //IniHelper.WriteIniData("3011", "Node2001IsReBack", "false", wcsFullPath);
            //IniHelper.WriteIniData("3011", "Node2001IsEmpty", "false", wcsFullPath);

            int pickNum = -1;
            if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.PickOut))
            {
                //拣选出库
                pickNum = transportMsg.TransportOrder.Qty;
            }
            else if (transportMsg.TransportOrder.SourceTaskType.Equals(SourceTaskEnum.InventoryOut))
            {
                pickNum = 0;
            }
            OperateResult writeWeightDataResult = roller3011.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, pickNum);
            if (writeWeightDataResult.IsSuccess)
            {
                LogMessage("写入 3011 OPCWeightDataBlock 拣选数量" + pickNum + " 成功", EnumLogLevel.Info, false);
            }
            else
            {
                LogMessage("写入 3011 OPCWeightDataBlock 拣选数量" + pickNum + " 失败！", EnumLogLevel.Error, false);
            }

            OperateResult writeCountDataResult = roller3011.Communicate.Write(DataBlockNameEnum.CountDataBlock, 0);
            if (writeCountDataResult.IsSuccess)
            {
                LogMessage("写入 3011 CountDataBlock 实际抓取数量清0" + pickNum + " 成功", EnumLogLevel.Info, false);
            }
            else
            {
                LogMessage("写入 3011 CountDataBlock 实际抓取数量清0" + pickNum + " 失败！", EnumLogLevel.Error, false);
            }

            roller3011.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
            roller3011.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0);

            DeviceBaseAbstract curDevice3010 = DeviceManage.Instance.FindDeivceByDeviceId(3010);
            TransportPointStation transDevice3010 = curDevice3010 as TransportPointStation;
            RollerDeviceControl roller3010 = transDevice3010.DeviceControl as RollerDeviceControl;
            roller3010.Communicate.Write(DataBlockNameEnum.IsLoaded, true);
            roller3011.Communicate.Write(DataBlockNameEnum.IsLoaded, true);
        }

        public override OperateResult ExcuteTransportMessage(TransportMessage transMsg)
        {
            //1.通过搬运信息获取到设备及地址
            //2.获取到设备后判断开始设备及目标设备的状态，判断是否能够执行该指令
            //2.通过开始设备及地址（涉及到开始设备对地址协议的转换及通讯协议）下发给目标设备
            OperateResult exceptionResult = new OperateResult();
            try
            {
                DeviceBaseAbstract destDevice = transMsg.DestDevice;
                TransportDeviceBaseAbstract needAddTaskDevice = destDevice as TransportDeviceBaseAbstract;
                if (needAddTaskDevice == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("目标设备：{0} 不是搬运设备，请核实配置信息", destDevice.Name));
                }

                TransportDeviceBaseAbstract transportDevice = transMsg.TransportDevice as TransportDeviceBaseAbstract;
                if (transportDevice == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("搬运设备：{0} 不是搬运设备，请核实配置信息", transMsg.TransportDevice.Name));
                }
                OperateResult transportResult = transportDevice.DoTransportJob(transMsg);
                if (!transportResult.IsSuccess)
                {
                    return
                        OperateResult.CreateFailedResult(
                            string.Format("通知搬运设备：{0} 失败,失败原因：{2} \r\n将会重新执行指令：{1}", transportDevice.Name, transMsg.TransportOrder.OrderId, transportResult.Message), 1);
                }

                needAddTaskDevice.AddUnfinishedTask(transMsg);
                return OperateResult.CreateSuccessResult(string.Format("指令号：{1} 成功通知搬运设备：{0} ", transportDevice.Name,
                    transMsg.TransportOrder.OrderId));

            }
            catch (Exception ex)
            {
                exceptionResult.IsSuccess = false;
                exceptionResult.ErrorCode = 1;
                exceptionResult.Message = OperateResult.ConvertException(ex);
            }
            return exceptionResult;
        }


        public override OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult CancleOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult BeforeTransportBusinessHandle(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            return OperateResult.CreateSuccessResult();
        }
    }
}
