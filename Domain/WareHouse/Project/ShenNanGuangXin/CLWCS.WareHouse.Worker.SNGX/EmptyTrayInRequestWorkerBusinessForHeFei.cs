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
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    ///空托盘入库请求上架——>WMS
    /// </summary>
   public class EmptyTrayInRequestWorkerBusinessForHeFei : SwitchingWorkerBusinessAbstract
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
            OperateResult<string> wmsAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(device.CurAddress.FullName);
            if (!wmsAddress.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("Wcs地址：{0} 转换Wms的地址失败，失败原因：\r\n{1}", device.CurAddress.ToString(), wmsAddress.Message), 1);
            }

            DeviceBaseAbstract curDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
            TransportPointStation transDevice2001 = curDevice2001 as TransportPointStation;
            RollerDeviceControl roller2001 = transDevice2001.DeviceControl as RollerDeviceControl;
            OperateResult<string> opReadBarCode2001 = roller2001.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

            if (opReadBarCode2001.IsSuccess)
            {
                if (string.IsNullOrEmpty(opReadBarCode2001.Content) || string.IsNullOrEmpty(opReadBarCode2001.Content.Trim()) || opReadBarCode2001.Content.Equals("ERROR"))
                {
                    return OperateResult.CreateFailedResult("读取2001条码 OPC 为空!", 1);
                }
            }
            else
            {
                return OperateResult.CreateFailedResult("读取2001条码 OPC失败!" + opReadBarCode2001.Message, 1);
            }
            //调用机器人 摄像头 是否有货， 无货则调用 TO DO
            OperateResult<int> opOrderTypeOpResult = roller2001.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
            if (!opOrderTypeOpResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult("读取2001 类型 OPC 错误!", 1);
            }
            bool isNeedChecked = true;
            if (opOrderTypeOpResult.Content == 11 || opOrderTypeOpResult.Content == 12)
            {
                isNeedChecked = false;
            }
            if (isNeedChecked)
            {
                bool isHaveGoods = IsHaveGoodsForABBVision("02", 2001);
                if (isHaveGoods)
                {
                    //有货回去
                    OperateResult op = NotifyWmsReportTroubleStatus("当前有货，非空托盘，请人工干预！！！");
                    if (!op.IsSuccess)
                    {
                        string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", "NotifyWmsReportTroubleStatus", op.Message);
                        return OperateResult.CreateFailedResult(msg, 1);
                    }

                    return OperateResult.CreateSuccessResult("NotifyWmsReportTroubleStatus 接口调用成功");
                }
            }

            ApplayEmptyTrayMode applayEmptyTrayMode = new ApplayEmptyTrayMode { Addr = wmsAddress.Content,PalletBarcode=opReadBarCode2001.Content };
            string cmdPara = JsonConvert.SerializeObject(applayEmptyTrayMode);
            string interFaceName = "ApplyEmptyTrayIn";
            NotifyElement element = new NotifyElement("", interFaceName, "空托盘入库请求", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}",interFaceName, result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            else
            {
                try
                {
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult("调用接口 "+interFaceName+"成功，并且WMS返回成功信息");
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口  " + interFaceName + " 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            return result;
        }

        private bool IsHaveGoodsForABBVision(string areaCode, int nodeID)
        {
            int abbNodeID = 0;
            string addr = "";
            if (areaCode.Equals("01"))
            {
                abbNodeID = 3001;
                if (nodeID == 1004)
                {
                    addr = "1";
                }
                else if (nodeID == 1007)
                {
                    addr = "2";
                }
                else if (nodeID == 1008)
                {
                    addr = "4";
                }
            }
            else if (areaCode.Equals("02"))
            {
                abbNodeID = 3002;
                if (nodeID == 2001)
                {
                    addr = "1";
                }
            }

            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(abbNodeID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
            OperateResult<InvokRobotVision_Response> op = abbControl.InvokRobotVision(new InvokRobotVisionMode { area_code = areaCode, visi_addr = addr });
            if (op.IsSuccess && op.Content != null)
            {
                //{"result": "1", "message": "success", "data": {"is_empty": 0, "count": 14}}
                return (op.Content.data != null && op.Content.data.is_empty == 0);
            }
            return false;
        }

        /// <summary>
        /// 设备异常信息上报
        /// </summary>
        /// <param name="deviceErrMsg"></param>
        /// <returns></returns>
        private OperateResult NotifyWmsReportTroubleStatus(string deviceErrMsg)
        {
            NotifyReportTroubleStatusCmdMode mode = new NotifyReportTroubleStatusCmdMode
            {
                MESSAGE = deviceErrMsg
            };

            string cmdPara = JsonConvert.SerializeObject(mode);
            string interFaceName = "ReportTroubleStatus";
            NotifyElement element = new NotifyElement("", interFaceName, "上报设备异常信息WMS", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            else
            {
                try
                {
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult("调用接口 " + interFaceName + "成功，并且WMS返回成功信息");
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口  " + interFaceName + " 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            return result;
        }
    }
}
