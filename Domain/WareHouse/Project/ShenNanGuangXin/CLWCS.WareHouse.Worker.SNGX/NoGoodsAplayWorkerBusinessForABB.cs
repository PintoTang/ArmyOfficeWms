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

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    ///2007 无货 向ABB发送可以码垛任务 
    /// </summary>
    public class NoGoodsAplayWorkerBusinessForABB : SwitchingWorkerBusinessAbstract
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

            if (device.Id == 200701)
            {
                DeviceBaseAbstract curDevice2015 = DeviceManage.Instance.FindDeivceByDeviceId(2015);
                TransportPointStation transDevice2015 = curDevice2015 as TransportPointStation;
                RollerDeviceControl roller2015 = transDevice2015.DeviceControl as RollerDeviceControl;
                OperateResult<int> opcResult2015 = roller2015.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                if (opcResult2015.IsSuccess)
                {
                    if (opcResult2015.Content == 888)
                    {
                        return OperateResult.CreateSuccessResult("2015 指令ID 为888，不处理" + device.Id+ "按钮业务");
                    }
                }
                else
                {
                    LogMessage("读取2015 指令ID OPC  失败！+"+opcResult2015.Message, EnumLogLevel.Error, true);
                }

            }
            else if (device.Id == 201501)
            {
                //读取2019
                return CreatABBNotice_ABB_Is_ReadTask(2019, "02", 3002);
            }
            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(2019);
            TransportPointStation transDevice = curDevice as TransportPointStation;
            RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;

            OperateResult opWriteOrderIdResult = roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -1000);
            if (opWriteOrderIdResult.IsSuccess)
            {
                LogMessage("写2019 包号 OPC -1000 成功，待处理 给ABB下发码垛任务", EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage("写2019 包号 OPC -1000 失败，待处理 给ABB下发码垛任务", EnumLogLevel.Error, true);
            }
            return opWriteOrderIdResult;

        }
        private OperateResult CreatABBNotice_ABB_Is_ReadTask(int startDeviceID, string areaCode, int destABBDeviceID)
        {
            //复位OPC 就绪
            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(startDeviceID);
            TransportPointStation transDevice = curDevice as TransportPointStation;
            RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
            //生成任务号  写入到条码
            int strTaskID = _orderManage.GetGlobalOrderID();


            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(destABBDeviceID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;

            notice_place_ws_readyMode readyMode = new notice_place_ws_readyMode
            {
                area_code = areaCode,
                //pallet_id = opReadBarCodeResult.Content,
                pallet_id = "",
                task_id = strTaskID.ToString(),
                ws_id = "1"
            };
            DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(2020);
            TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
            RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
            OperateResult<string> opReadBarCode2020 = roller2020.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

            OperateResult writeBarCode2019 = roller.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, opReadBarCode2020.Content);
            if (writeBarCode2019.IsSuccess)
            {

                OperateResult response = abbControl.notice_place_ws_ready(readyMode);
                if (!response.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(response.Message, 1);
                }

                OperateResult resetReadyR = roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, strTaskID);
                if (!resetReadyR.IsSuccess)
                {
                    return OperateResult.CreateFailedResult("复位2019 OPC 任务号 失败,当前ID:" + startDeviceID.ToString());
                }
            }
            else
            {
                return OperateResult.CreateFailedResult("写入2019 条码失败 :" + opReadBarCode2020.Content, 1);
            }

            return OperateResult.CreateSuccessResult();
        }
    }
}
