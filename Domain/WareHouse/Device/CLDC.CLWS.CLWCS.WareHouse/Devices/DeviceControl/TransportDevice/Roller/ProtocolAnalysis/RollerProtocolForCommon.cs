using System;
using System.Collections.Generic;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Roller;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public sealed class RollerProtocolForCommon : RollerProtocolAnalysisAbstract
    {
        public override OperateResult ParticularInitialize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitConfig()
        {
            return OperateResult.CreateSuccessResult();
        }



        public override OperateResult<Dictionary<DataBlockNameEnum, object>> ComposeCmd(TransportMessage transportMsg)
        {
            Dictionary<DataBlockNameEnum, object> CmdDic = new Dictionary<DataBlockNameEnum, object>();
            OperateResult<Dictionary<DataBlockNameEnum, object>> result = OperateResult.CreateFailedResult(CmdDic, "无数据");
            try
            {
                CmdDic.Add(DataBlockNameEnum.WriteDirectionDataBlock, transportMsg.DestDevice.Id);
                CmdDic.Add(DataBlockNameEnum.OPCBarcodeDataBlock, transportMsg.TransportOrder.PileNo);
                CmdDic.Add(DataBlockNameEnum.WriteOrderIDDataBlock, transportMsg.TransportOrder.OrderId);

                #region WMS指令Id
                if (!string.IsNullOrEmpty(transportMsg.TransportOrder.DocumentCode))
                {
                    CmdDic.Add(DataBlockNameEnum.WMSOPCOrderIdDataBlock, transportMsg.TransportOrder.DocumentCode);
                }
                else
                {
                    CmdDic.Add(DataBlockNameEnum.WMSOPCOrderIdDataBlock, "");

                }
                #endregion

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
    }
}
