using System;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class TransportPointStationBusiness : OrderWorkerBuinessAbstract
    {
        protected override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }
        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
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
                    return OperateResult.CreateFailedResult(string.Format("目标设备：{0} 不是搬运设备，请核实配置信息",destDevice.Name));
                }

                TransportDeviceBaseAbstract transportDevice = transMsg.TransportDevice as TransportDeviceBaseAbstract;
                if (transportDevice==null)
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
