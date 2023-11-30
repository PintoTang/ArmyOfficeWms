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
    ///货物放货 2007 码垛就绪
    /// </summary>
    public class PutGoodsRequestWorkerBusiness : SwitchingWorkerBusinessAbstract
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
            OperateResult result = OperateResult.CreateFailedResult();
            //入库货物达到  下发任务给ABB
            int startDeviceID = 0;
            string areaCode = "";
            int destABBDeviceID = 0;
            if (device.Id == 201901)
            {
                startDeviceID = 2019;
                destABBDeviceID = 3002;
                areaCode = "02";
            }
            DeviceBaseAbstract curDevice2015 = DeviceManage.Instance.FindDeivceByDeviceId(2015);
            TransportPointStation transDevice2015 = curDevice2015 as TransportPointStation;
            RollerDeviceControl roller2015 = transDevice2015.DeviceControl as RollerDeviceControl;
            OperateResult<int> opReadOrderId2015 = roller2015.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
            if(opReadOrderId2015.IsSuccess)
            {
                if (opReadOrderId2015.Content != 0)
                {
                    return OperateResult.CreateFailedResult("读写2015 指令ID 不等于0，不得下发任务，失败！", 1);
                }
            }
            else
            {
                return OperateResult.CreateFailedResult("读写2015 指令ID，失败！", 1);
            }

            DeviceBaseAbstract curDevice2007 = DeviceManage.Instance.FindDeivceByDeviceId(2007);
            TransportPointStation transDevice2007 = curDevice2007 as TransportPointStation;
            RollerDeviceControl roller2007 = transDevice2007.DeviceControl as RollerDeviceControl;
            OperateResult<int> opReadOrderId2007 = roller2007.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
            OperateResult<string> opReadBarCode2007 = roller2007.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

            if (opReadBarCode2007.IsSuccess)
            {
                if (opReadOrderId2007.Content != 0)
                {
                    return OperateResult.CreateFailedResult("读写2007 指令ID 不等于0，不得下发任务，失败！", 1);
                }

                DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                OperateResult<string> opReadBarCode2019 = roller2019.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                if (string.IsNullOrEmpty(opReadBarCode2019.Content) || string.IsNullOrEmpty(opReadBarCode2019.Content.Trim()))
                {
                    //空条码
                    return CreatABBNotice_ABB_Is_ReadTask(startDeviceID, areaCode, destABBDeviceID);
                }
                else
                {
                    return OperateResult.CreateFailedResult("读写2019 条码不为空，失败，已下发，如需重新下发，清空条码OPC", 1);
                }
            }
            else
            {
                return OperateResult.CreateFailedResult("读写2007 条码失败 OPC 失败", 1);
            }
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
                pallet_id ="",
                task_id = strTaskID.ToString(),
                ws_id = "1"
            };
            DeviceBaseAbstract curDevice2020 = DeviceManage.Instance.FindDeivceByDeviceId(2020);
            TransportPointStation transDevice2020 = curDevice2020 as TransportPointStation;
            RollerDeviceControl roller2020 = transDevice2020.DeviceControl as RollerDeviceControl;
            OperateResult<string> opReadBarCode2020 = roller2020.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

            OperateResult writeBarCode2019= roller.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, opReadBarCode2020.Content);
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
