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
    ///出到2001货物业务处理
    /// </summary>
    public class GoodsOutWorkerBusiness : SwitchingWorkerBusinessAbstract
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

        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device)
        {
            OperateResult opReuslt = OperateResult.CreateFailedResult();
            DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(2020);
            TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
            RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
            OperateResult<int> opOrderId2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            OperateResult<int> opDirectionData2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.WriteDirectionDataBlock);
            OperateResult<string> opReadBarCode2020 = roller2020.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            OperateResult<int> opReadOrderType2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
            OperateResult<int> opReadWeight2020 = roller2020.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);

            DeviceBaseAbstract curDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
            TransportPointStation transDevice2001 = curDevice2001 as TransportPointStation;
            RollerDeviceControl roller2001 = transDevice2001.DeviceControl as RollerDeviceControl;
            OperateResult<int> opOrderId2001 = roller2001.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            OperateResult<string> opReadBarCode2001 = roller2001.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            OperateResult<int> opReadOrderType2001 = roller2001.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);


            if (opReadOrderType2020.IsSuccess && opReadOrderType2020.Content == 1)
            {
                if (!opReadBarCode2020.Content.Equals(opReadBarCode2001.Content)) return OperateResult.CreateFailedResult("2020 与2001 条码不一致！");
                if (opDirectionData2020.Content != opOrderId2001.Content) return OperateResult.CreateFailedResult("2020目标地址 与2001 包号 不一致！");
                if (string.IsNullOrEmpty(opReadBarCode2001.Content)) return OperateResult.CreateFailedResult("2001 条码为空，不得为空！");
                if (opOrderId2001.Content == 0) return OperateResult.CreateFailedResult("2001 包号为0 无货");
                //符合条件
                CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(3002) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
                XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
                string areaCode = "02";
                //生成任务号  写入到条码
                int strTaskID = _orderManage.GetGlobalOrderID();
                sku_infoMode skuMode = new sku_infoMode
                {
                    sku_id = "",
                    length = 0,
                    width = 0,
                    height = 0,
                    weight = 0,
                    sku_num = -1,//WCS 下发抓取数量
                    sku_type = "box"
                };
                if (opReadOrderType2001.IsSuccess && opReadOrderType2001.Content == 6 && opReadWeight2020.IsSuccess)
                {
                    if (opReadWeight2020.Content != 0)
                    {
                        skuMode.sku_num = opReadWeight2020.Content;
                    }
                    else
                    {
                        LogMessage("2020拣选数量为0 请检查wms下发的拣选任务 拣选数量！", EnumLogLevel.Error, false);
                        return OperateResult.CreateFailedResult("2020拣选数量为0 请检查wms下发的拣选任务 拣选数量！");
                    }
                }
                XYZABBTaskMode task = new XYZABBTaskMode
                {
                    task_id = strTaskID.ToString(),
                    sku_info = skuMode,
                    pallet_id = opReadBarCode2001.Content,
                    to_scanner = true,
                    from_ws = "0",
                    to_ws = "1",
                };
                OperateResult response = abbControl.single_class_depal_task(areaCode, task);
                if (!response.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(response.Message, 1);
                }

                OperateResult writeOrderIdTaskResult = roller2020.Communicate.Write(DataBlockNameEnum.OPCOrderIdDataBlock, strTaskID);
                if (!writeOrderIdTaskResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult("写入2020任务号失败 opc 指令ID 失败：" + strTaskID.ToString());
                }

               

                //判断 2007 托盘是否给机器人发送 码垛托盘到位任务
                DeviceBaseAbstract curDevice2007 = DeviceManage.Instance.FindDeivceByDeviceId(2007);
                TransportPointStation transDevice2007 = curDevice2007 as TransportPointStation;
                RollerDeviceControl roller2007 = transDevice2007.DeviceControl as RollerDeviceControl;
                OperateResult<int> opReadOrderId = roller2007.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                if (opReadOrderId.IsSuccess)
                {
                    if (opReadOrderId.Content == 0)
                    {
                        //无货 未发送给机器人
                        DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                        TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                        RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                        OperateResult<int> opReadOrderId2019 = roller2019.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                        if(opReadOrderId2019.IsSuccess)
                        {
                            if(opReadOrderId2019.Content==-1000)
                            {
                                OperateResult opWriteOrderIDResult2019 = roller2019.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, 0);
                                if (opWriteOrderIDResult2019.IsSuccess)
                                {
                                    LogMessage("写2019 包号 OPC  0 成功", EnumLogLevel.Info, false);
                                }
                                else
                                {
                                    LogMessage("写2019 包号 OPC  0 失败", EnumLogLevel.Error, false);
                                }
                            }
                        }
                        else
                        {
                            LogMessage("读取2019 包号 OPC  失败", EnumLogLevel.Error, false);
                        }


                        OperateResult opWriteOrderIDResult = roller2019.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -1000);
                        if (!opWriteOrderIDResult.IsSuccess) return OperateResult.CreateFailedResult("写入2019 opc 指令ID -1000  失败");
                        return OperateResult.CreateSuccessResult("写入2019 opc 指令ID -1000 成功");
                    }
                    return OperateResult.CreateSuccessResult("成功123");
                }
                else
                {
                    return OperateResult.CreateFailedResult("读取2007 opc 指令ID 失败");
                }
            }
            else
            {
                return OperateResult.CreateFailedResult("2020 opc 类型不为 1");
            }
        }
    }
}
