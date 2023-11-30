using System;
using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Framework.Log.Helper;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Roller;

namespace CLWCS.WareHouse.Device.HeFei
{
    public sealed class RollerProtocolForHeavyOut : RollerProtocolAnalysisAbstract
    {
        public override OperateResult ParticularInitialize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 通过wcs下发的任务 转换成任务OPC需要的任务类型
        /// </summary>
        /// <param name="emSourceTask">源任务类型</param>
        /// <returns>PLC 协议对应的任务类型</returns>
        private int GetTaskTypeValueByEmSourceTask(SourceTaskEnum emSourceTask)
        {
            int value = 0;
            switch (emSourceTask)
            {
                case SourceTaskEnum.In:
                    value = 1;
                    break;
                case SourceTaskEnum.Out:
                    value = 2;
                    break;
                case SourceTaskEnum.Move:
                    break;
                case SourceTaskEnum.InventoryIn:
                    value = 3;
                    break;
                case SourceTaskEnum.InventoryOut:
                    value = 4;
                    break;
                case SourceTaskEnum.PickIn:
                    value = 5;
                    break;
                case SourceTaskEnum.PickOut:
                    value = 6;
                    break;
                case SourceTaskEnum.QualityConfirmOut:
                    value = 12;
                    break;
                case SourceTaskEnum.UnKnow:
                    break;
                default:
                    break;
            }
            return value;
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

                int taskTypeValue = GetTaskTypeValueByEmSourceTask(transportMsg.TransportOrder.SourceTaskType.GetValueOrDefault());//加入任务类型 给PLC使用
                string msg = "收到任务类型:" + transportMsg.TransportOrder.SourceTaskType.ToString() + "转换给plc值:" + taskTypeValue.ToString();
                LogHelper.WriteLog("taskTypePlc", msg, EnumLogLevel.Info);
                CmdDic.Add(DataBlockNameEnum.OrderTypeDataBlock, taskTypeValue);

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
