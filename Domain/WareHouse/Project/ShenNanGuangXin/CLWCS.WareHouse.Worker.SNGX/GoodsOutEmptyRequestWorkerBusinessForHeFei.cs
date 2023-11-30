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
    ///货物入库1008 码垛就绪
    /// </summary>
    public class GoodsOutEmptyRequestWorkerBusinessForHeFei : SwitchingWorkerBusinessAbstract
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
            if (device.Id == 100801)
            {
                startDeviceID = 1008;
                destABBDeviceID = 3001;
                areaCode = "01";
            }
            //复位OPC 就绪
            DeviceBaseAbstract curDevice1004 = DeviceManage.Instance.FindDeivceByDeviceId(1004);
            TransportPointStation transDevice1004 = curDevice1004 as TransportPointStation;
            RollerDeviceControl roller1004 = transDevice1004.DeviceControl as RollerDeviceControl;
            OperateResult<int> opOrderId1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            OperateResult<string> opReadBarCode1004 = roller1004.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            if (opOrderId1004.IsSuccess && opReadBarCode1004.IsSuccess)
            {
                int barCodeTaskId = 0;
                if(!string.IsNullOrEmpty(opReadBarCode1004.Content))
                {
                     barCodeTaskId = int.Parse(opReadBarCode1004.Content.ToString());
                }
               
                if (opOrderId1004.Content > 0 && opOrderId1004.Content == barCodeTaskId)
                {
                    //判断wms是否开始任务
                    DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                    TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                    RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                    OperateResult<bool> isLoadedResult2019 = roller2019.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    if (isLoadedResult2019.IsSuccess && isLoadedResult2019.Content == true)
                    {
                        return CreatABBNotice_ABB_Is_ReadTask(startDeviceID, areaCode, destABBDeviceID);
                    }
                }

                DeviceBaseAbstract curDevice1007 = DeviceManage.Instance.FindDeivceByDeviceId(1007);
                TransportPointStation transDevice1007 = curDevice1007 as TransportPointStation;
                RollerDeviceControl roller1007 = transDevice1007.DeviceControl as RollerDeviceControl;
                OperateResult<int> opOrderId1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
                OperateResult<string> opReadBarCode1007 = roller1007.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                if (opOrderId1007.IsSuccess && opReadBarCode1007.IsSuccess)
                {
                    int barCodeTaskId1007 = 0;
                    if (!string.IsNullOrEmpty(opReadBarCode1007.Content))
                    {
                        barCodeTaskId1007 = int.Parse(opReadBarCode1007.Content.ToString());
                    }
                
                    if (opOrderId1007.Content > 0 && opOrderId1007.Content == barCodeTaskId1007)
                    {
                        //判断wms是否开始任务
                        DeviceBaseAbstract curDevice2019 = DeviceManage.Instance.FindDeivceByDeviceId(2019);
                        TransportPointStation transDevice2019 = curDevice2019 as TransportPointStation;
                        RollerDeviceControl roller2019 = transDevice2019.DeviceControl as RollerDeviceControl;
                        OperateResult<bool> isLoadedResult2019 = roller2019.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        if (isLoadedResult2019.IsSuccess && isLoadedResult2019.Content == true)
                        {
                            return CreatABBNotice_ABB_Is_ReadTask(startDeviceID, areaCode, destABBDeviceID);
                        }
                    }
                }
                else
                {
                    return OperateResult.CreateFailedResult("读写1007 OPC 失败", 1);
                }
            }
            else
            {
                return OperateResult.CreateFailedResult("读写1004 OPC 失败", 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult CreatABBNotice_ABB_Is_ReadTask(int startDeviceID, string areaCode, int destABBDeviceID)
        {
            //复位OPC 就绪
            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(startDeviceID);
            TransportPointStation transDevice = curDevice as TransportPointStation;
            RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
            //生成任务号  写入到条码
            int strTaskID = _orderManage.GetGlobalOrderID();
            OperateResult resetReadyR = roller.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, strTaskID);
            if (!resetReadyR.IsSuccess)
            {
                return OperateResult.CreateFailedResult("复位OPC失败,当前ID:" + startDeviceID.ToString());
            }

            OperateResult<string> opReadBarCodeResult = roller.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            if (!opReadBarCodeResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult("opc读取条码失败,开始设备ID:" + startDeviceID.ToString());
            }
            if (string.IsNullOrEmpty(opReadBarCodeResult.Content))
            {
                return OperateResult.CreateFailedResult("opc读取条码为空,开始设备ID:" + startDeviceID.ToString());
            }

            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(destABBDeviceID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;

            notice_place_ws_readyMode readyMode = new notice_place_ws_readyMode
            {
                area_code = areaCode,
                pallet_id = opReadBarCodeResult.Content,
                task_id = strTaskID.ToString(),
                ws_id = "2"
            };

            //

            OperateResult response = abbControl.notice_place_ws_ready(readyMode);
            if (!response.IsSuccess)
            {
                return OperateResult.CreateFailedResult(response.Message, 1);
            }
            return OperateResult.CreateSuccessResult();
        }
    }
}
