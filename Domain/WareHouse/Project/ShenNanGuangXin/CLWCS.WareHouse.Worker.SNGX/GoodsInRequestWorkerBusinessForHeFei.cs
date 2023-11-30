using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.Model;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using System.Threading;

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    ///货物入库请求上架—ABB
    /// </summary>
    public class GoodsInRequestWorkerBusinessForHeFei : SwitchingWorkerBusinessAbstract
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        private OrderManage _orderManage;
        protected override OperateResult ParticularInitlize()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            _orderManage = DependencyHelper.GetService<OrderManage>();
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<AssistantDevice> workerAssistants, string barcode)
        {
            return OperateResult.CreateSuccessResult();
        }
        private static int task_ID = 0;
        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device)
        {

            OperateResult result = OperateResult.CreateFailedResult();
            //入库货物达到  下发任务给ABB
            int startDeviceID = 0;
            string areaCode = "";
            int destDeviceID = 1008;
            int destABBDeviceID = 0;
            if (device.Id == 100401)
            {
                startDeviceID = 1004;
                destABBDeviceID = 3001;
                areaCode = "01";
            }
            else if (device.Id == 100701)
            {
                startDeviceID = 1007;
                destABBDeviceID = 3001;
                areaCode = "01";
            }
            //复位OPC 就绪
            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(startDeviceID);
            TransportPointStation transDevice = curDevice as TransportPointStation;
            RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
            OperateResult<string> opReadBarCode = roller.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            if (!opReadBarCode.IsSuccess)
            {
              return  OperateResult.CreateFailedResult("读取opc条码地址失败,开始设备ID:" + startDeviceID.ToString());
            }
            if (!string.IsNullOrEmpty(opReadBarCode.Content))
            {
                return OperateResult.CreateFailedResult("读取opc条码不为空，请检查是否已经发送给机器人，没有则清空,开始设备ID:" + startDeviceID.ToString());
            }

            //生成任务号  写入到条码
            int strTaskID = _orderManage.GetGlobalOrderID();
           

            //DeviceBaseAbstract destDevice = DeviceManage.Instance.FindDeivceByDeviceId(destABBDeviceID);
            //TransportPointStation destTransDevice = destDevice as TransportPointStation;
            //RollerDeviceControl destRoller = destTransDevice.DeviceControl as RollerDeviceControl;

            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(destABBDeviceID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;

            //notice_place_ws_readyMode readyMode = new notice_place_ws_readyMode
            //{
            //    area_code = areaCode,
            //    pallet_id = "",
            //    task_id = strTaskID.ToString(),
            //    ws_id = "2"
            //};


            sku_infoMode skuMode = new sku_infoMode
            {
                sku_id = "",
                length = 0,
                width = 0,
                height = 0,
                weight = 0,
                sku_num = -1,
                sku_type = "box"
            };

            XYZABBTaskMode task = new XYZABBTaskMode
            {
                task_id = strTaskID.ToString(),
                sku_info = skuMode,
                pallet_id = "",
                to_scanner = true,
            };
            switch (startDeviceID)
            {
                case 1007:
                    task.from_ws = "1";
                    task.to_ws = "2";
                    break;
                case 1004:
                    task.from_ws = "0";
                    task.to_ws = "2";
                    break;
                case 2001:
                    task.from_ws = "0";
                    task.to_ws = "1";
                    break;
                default:
                    break;
            }
            OperateResult response = abbControl.single_class_depal_task(areaCode, task);
            if (!response.IsSuccess)
            {
                return OperateResult.CreateFailedResult(response.Message, 1);
            }

            OperateResult opWriteBarCodeResult = roller.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, strTaskID.ToString());
            if (!opWriteBarCodeResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult("写opc条码地址失败,开始设备ID:" + startDeviceID.ToString());
            }

            //写任务号 
            OperateResult opWriteTaskResult = roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, strTaskID.ToString());
            if (!opWriteTaskResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult("写opc任务号地址失败,开始设备ID:" + startDeviceID.ToString());
            }

            //判断 1008 托盘是否给机器人发送 码垛托盘到位任务
            DeviceBaseAbstract curDevice1008 = DeviceManage.Instance.FindDeivceByDeviceId(1008);
            TransportPointStation transDevice1008 = curDevice1008 as TransportPointStation;
            RollerDeviceControl roller1008 = transDevice1008.DeviceControl as RollerDeviceControl;
            OperateResult<int> opReadOrderId = roller1008.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
            if (opReadOrderId.IsSuccess)
            {
                if (opReadOrderId.Content == -1000)
                {
                    OperateResult<string> opReadBarCodeResult1008 = roller1008.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    if (opReadBarCodeResult1008.IsSuccess && !string.IsNullOrEmpty(opReadBarCodeResult1008.Content))
                    {
                        strTaskID = _orderManage.GetGlobalOrderID();
                        //直接下发机器人码垛
                        notice_place_ws_readyMode readyMode = new notice_place_ws_readyMode
                        {
                            area_code = areaCode,
                            pallet_id = opReadBarCodeResult1008.Content,
                            task_id = strTaskID.ToString(),
                            ws_id = "2"
                        };

                        DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                        TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                        RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                        OperateResult<bool> isLoadedResult2019 = roller2019.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        if (isLoadedResult2019.IsSuccess && isLoadedResult2019.Content == true)
                        {
                            OperateResult readyResponse = abbControl.notice_place_ws_ready(readyMode);
                            if (!readyResponse.IsSuccess)
                            {
                                return OperateResult.CreateFailedResult(readyResponse.Message, 1);
                            }
                            //写入任务号 到1008
                            OperateResult resetReadyR = roller1008.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, strTaskID);
                            if (!resetReadyR.IsSuccess)
                            {
                                return OperateResult.CreateFailedResult("写入1008 指令ID OPC失败,任务ID:" + strTaskID.ToString());
                            }
                        }
                    }

                    ////有货 未发送给机器人
                    //OperateResult opcOrdrID = roller1008.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 1);
                    //Thread.Sleep(500);
                    //if (opcOrdrID.IsSuccess)
                    //{
                    //    opcOrdrID = roller1008.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -1000);
                    //}
                }
                else
                {
                    //已经下发码垛任务  不需要处理
                }
            }

            return OperateResult.CreateSuccessResult(); ;
            //Addr curAddr = DeviceManage.Instance.FindDeivceByDeviceId(startDeviceID).CurAddress;
            //Addr destAddr = DeviceManage.Instance.FindDeivceByDeviceId(destDeviceID).CurAddress;

            //DataObservablePool<ExOrder> unfinishOrder = _orderManage.GetAllUnAllocatedOrder();
            //if (unfinishOrder == null || unfinishOrder.Lenght == 0)
            //{
            //    return CreateOrderForABB(curAddr, destAddr);
            //}
            //else
            //{
            //    //存在未完成指令
            //    //判断是否存在已经下发的指令 不存在则生成，存在则不处理
            //    var nn = unfinishOrder.FindAllData(x => x.StartAddr.Equals(curAddr)
            //        && x.DestAddr.Equals(destAddr));
            //    if (nn != null && nn.Count() > 0)
            //    {
            //        //存在
            //        return OperateResult.CreateFailedResult("未完成指令已存在指令，不生成指令，", 1);
            //    }
            //    else
            //    {
            //        return CreateOrderForABB(curAddr, destAddr);
            //    }
            //}
        }
        private OperateResult CreateOrderForABB(Addr curAddr, Addr destAddr)
        {
            //当前无未完成指令,直接生成ABB指令 生成入库指令
            Order order = new Order();
            //1008  PutGoodsPort:1_1_1 放货口1
            order.CurrAddr = curAddr;
            order.DestAddr = destAddr;
            order.OrderType = OrderTypeEnum.In;
            order.Source = TaskSourceEnum.SELF;
            order.PileNo = "";
            order.IsReport = false;
            OperateResult<Order> generateResult = _orderManage.GenerateOrder(order);
            if (generateResult.IsSuccess)
            {

                //丢弃执行未完成的任务
                return OperateResult.CreateSuccessResult("生成入库指令成功!");
            }
            else
            {
                //未能生成入库指令
                return OperateResult.CreateFailedResult("未能生成入库指令", 1);
            }
        }
    }
}
