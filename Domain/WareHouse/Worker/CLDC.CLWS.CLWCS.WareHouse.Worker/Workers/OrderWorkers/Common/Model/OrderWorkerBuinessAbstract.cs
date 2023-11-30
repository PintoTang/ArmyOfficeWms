using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model
{
    /// <summary>
    /// 工作者业务处理基类
    /// </summary>
    public abstract class OrderWorkerBuinessAbstract : WorkerBusinessAbstract
    {



        /// <summary>
        /// 获取开始最优的可执行装备
        /// </summary>
        /// <param name="Device"></param>
        /// <returns></returns>
        public virtual OperateResult<DeviceBaseAbstract> ChooseBestStartDevice(List<AssistantDevice> Device, ExOrder order)
        {
            //1.根据协助者的状态是否可用
            //2.根据协助者的工作负荷

            //获取开始设备的算法
            OperateResult<DeviceBaseAbstract> operateResult = new OperateResult<DeviceBaseAbstract>();
            Addr curAddr = order.CurrAddr;
            List<AssistantDevice> startDevice = Device.FindAll(w => w.Device.Accessible(curAddr));
            if (startDevice.Count <= 0)
            {
                operateResult.IsSuccess = false;
                operateResult.Message = string.Format("指令：{0} 找不到起点地址相匹配的可用设备", order.OrderId);
                //打印日志找不到起点处理的设备
                return operateResult;
            }

            List<DeviceBaseAbstract> availableDevice = new List<DeviceBaseAbstract>();
            List<DeviceBaseAbstract> disableDevice = new List<DeviceBaseAbstract>();
            StringBuilder disableMsg = new StringBuilder();

            foreach (AssistantDevice assistantDevice in startDevice)
            {
                OperateResult isAvailableResult = assistantDevice.Device.Availabe();
                if (isAvailableResult.IsSuccess)
                {
                    availableDevice.Add(assistantDevice.Device);
                }
                else
                {
                    disableDevice.Add(assistantDevice.Device);
                    disableMsg.Append(string.Format("设备：{0} 此时不可用，原因：{1}", assistantDevice.Device.Name,
                        isAvailableResult.Message));
                }
            }

            if (availableDevice.Count<=0)
            {
                operateResult.IsSuccess = false;
                operateResult.Content = default(DeviceBaseAbstract);
                operateResult.Message = string.Format("指令：{1} 此时不存在可用的开始设备，原因：{0}", disableMsg.ToString(), order.OrderId);
                return operateResult;
            }

            DeviceBaseAbstract bestAssistant = availableDevice.FirstOrDefault();

            operateResult.IsSuccess = true;
            operateResult.Content = bestAssistant;
            operateResult.Message = string.Format("根据指令：{0} 获取到最优的开始设备：{1}", order.OrderId, bestAssistant.Name);

            return operateResult;

        }

        public virtual OperateResult<DeviceBaseAbstract> ChooseBestDestDevice(
            List<AssistantDevice> assistants, List<Addr> destAddrLst)
        {
            OperateResult<DeviceBaseAbstract> operateResult = new OperateResult<DeviceBaseAbstract>();
            //获取搬运设备的算法
            List<AssistantDevice> destDevices = new List<AssistantDevice>();

            foreach (AssistantDevice assistant in assistants)
            {
                foreach(var addr in destAddrLst)
                {
                    if (assistant.Device.Accessible(addr))
                    {
                        destDevices.Add(assistant);
                    }
                }
                //destDevices.AddRange(from addr in destAddrLst where assistant.Device.Accessible(addr) select assistant);
            }

            if (destDevices.Count <= 0)
            {
                operateResult.IsSuccess = false;
                operateResult.Message = string.Format("根据目标地址集合：{0} 找不到目标地址相匹配的可用设备", string.Join("_", destAddrLst));
                return operateResult;
            }

            List<DeviceBaseAbstract> availableDevice = new List<DeviceBaseAbstract>();
            List<DeviceBaseAbstract> disableDevice = new List<DeviceBaseAbstract>();
            StringBuilder disableMsg = new StringBuilder();
            foreach (AssistantDevice assistantDevice in destDevices)
            {
                OperateResult isAvailableResult = assistantDevice.Device.Availabe();
                if (isAvailableResult.IsSuccess)
                {
                    availableDevice.Add(assistantDevice.Device);
                }
                else
                {
                    disableDevice.Add(assistantDevice.Device);
                    disableMsg.Append(string.Format("设备：{0} 此时不可用，原因：{1}", assistantDevice.Device.Name,
                        isAvailableResult.Message));
                }
            }

            if (availableDevice.Count <= 0)
            {
                operateResult.IsSuccess = false;
                operateResult.Content = default(DeviceBaseAbstract);
                operateResult.Message = string.Format("根据目标地址集合：{1} 此时找不到可用的设备，原因：{0}", disableMsg, string.Join("_", destAddrLst));
                return operateResult;
            }

            DeviceBaseAbstract bestAssistant = availableDevice.FirstOrDefault();

            operateResult.IsSuccess = true;
            operateResult.Content = bestAssistant;
            operateResult.Message = string.Format("根据目标地址集合：{0} 获取到最优的目标装备：{1}", string.Join("_", destAddrLst), bestAssistant.Name);

            return operateResult;

        }

        /// <summary>
        /// 获取目标最优的可执行装备
        /// </summary>
        /// <param name="assistants"></param>
        /// <param name="startAddr"></param>
        /// <param name="destAddr"></param>
        /// <returns></returns>
        public virtual OperateResult<DeviceBaseAbstract> ChooseBestTransportDevice(List<AssistantDevice> assistants, Addr startAddr, Addr destAddr)
        {
            OperateResult<DeviceBaseAbstract> operateResult = new OperateResult<DeviceBaseAbstract>();
            //获取目标设备的算法
            List<DeviceBaseAbstract> transportDevices = new List<DeviceBaseAbstract>();

            //此处添加一个计算出下步地址的集合 的业务逻辑

            foreach (AssistantDevice assistant in assistants)
            {
                if (assistant.AssistantType.Equals(AssistantType.Transport))
                {
                    TransportDeviceBaseAbstract transportDevice = assistant.Device as TransportDeviceBaseAbstract;
                    if (transportDevice == null)
                    {
                        continue;
                    }
                    if (transportDevice.Accessible(startAddr, destAddr))
                    {
                        transportDevices.Add(transportDevice);
                    }
                }
            }

            if (transportDevices.Count <= 0)
            {
                operateResult.IsSuccess = false;
                operateResult.Message = string.Format("找不到从：{0} 搬运到：{1} 的搬运设备", startAddr, destAddr);
                return operateResult;
            }


            List<DeviceBaseAbstract> availableDevice = new List<DeviceBaseAbstract>();
            List<DeviceBaseAbstract> disableDevice = new List<DeviceBaseAbstract>();
            StringBuilder disableMsg = new StringBuilder();
            foreach (DeviceBaseAbstract assistantDevice in transportDevices)
            {
                OperateResult isAvailableResult = assistantDevice.Availabe();
                if (isAvailableResult.IsSuccess)
                {
                    availableDevice.Add(assistantDevice);
                }
                else
                {
                    disableDevice.Add(assistantDevice);
                    disableMsg.Append(string.Format("设备：{0} 此时不可用，原因：{1}", assistantDevice.Name,
                        isAvailableResult.Message));
                }
            }

            if (availableDevice.Count <= 0)
            {
                operateResult.IsSuccess = false;
                operateResult.Content = default(DeviceBaseAbstract);
                operateResult.Message = string.Format("此时找不到从：{1} 搬运到：{2} 的搬运设备，原因：{0}", disableMsg, startAddr, destAddr);
                return operateResult;
            }

            DeviceBaseAbstract bestAssistant = availableDevice.FirstOrDefault();

            operateResult.IsSuccess = true;
            operateResult.Content = bestAssistant;
            operateResult.Message = string.Format("根据开始地址：{0} 目的地址：{1} 获取到最优的搬运设备：{2}", startAddr, destAddr, bestAssistant.Name);

            return operateResult;

        }

        public OperateResult<TransportMessage> ComputeTransportMessage(List<AssistantDevice> workerAssistants, ExOrder order)
        {
            //1.根据指令的目标地址，通过协助者的当前地址和能到达的目标地址进行匹配
            //2.返回协助者的当前地址当作指令的下一步地址
            //3.针对目标地址为模糊匹配的目标地址判断，比如仓位、站台组等
            lock (workerAssistants)
            {
                TransportMessage transportMsg = new TransportMessage(Guid.NewGuid().ToString("N"));
                OperateResult<TransportMessage> operateResult = OperateResult.CreateSuccessResult(transportMsg);

                OperateResult<DeviceBaseAbstract> getStartDevice = ChooseBestStartDevice(workerAssistants, order);
                if (!getStartDevice.IsSuccess)
                {
                    operateResult.IsSuccess = false;
                    operateResult.Message = getStartDevice.Message;
                    return operateResult;
                }

                DeviceBaseAbstract bestStartDevice = getStartDevice.Content;
                transportMsg.StartDevice = bestStartDevice;
                transportMsg.StartAddr = order.CurrAddr.Clone();

                ///在此更具开始设备找到下一个地址的信息
                OperateResult<List<Addr>> getNextAddrLstResult = bestStartDevice.ComputeNextAddr(order.DestAddr);

                if (!getNextAddrLstResult.IsSuccess)
                {
                    operateResult.IsSuccess = false;
                    operateResult.Message = getNextAddrLstResult.Message;
                    return operateResult;
                }
                List<Addr> nextAddrLst = getNextAddrLstResult.Content;

                OperateResult<DeviceBaseAbstract> getDestDevice = ChooseBestDestDevice(workerAssistants, nextAddrLst);
                if (!getDestDevice.IsSuccess)
                {
                    operateResult.IsSuccess = false;
                    operateResult.Message = getDestDevice.Message;
                    return operateResult;
                }


                DeviceBaseAbstract bestDestDevice = getDestDevice.Content;

                if (bestDestDevice.IsEndDevice())
                {
                    transportMsg.DestAddr = order.DestAddr.Clone();
                }
                else
                {
                    transportMsg.DestAddr = bestDestDevice.CurAddress.Clone();
                }

                transportMsg.DestDevice = bestDestDevice;

                OperateResult<DeviceBaseAbstract> getTransportDevice = ChooseBestTransportDevice(workerAssistants,
                    bestStartDevice.CurAddress, bestDestDevice.CurAddress);
                if (!getTransportDevice.IsSuccess)
                {
                    operateResult.IsSuccess = false;
                    operateResult.Message = getTransportDevice.Message;
                    return operateResult;
                }
                transportMsg.TransportDevice = getTransportDevice.Content;
                transportMsg.CurAddr = transportMsg.TransportDevice.CurAddress.Clone();
                transportMsg.OwnerId = transportMsg.TransportDevice.Id;
                transportMsg.TransportOrder = order;
                transportMsg.UpdateDateTime = DateTime.Now;
                transportMsg.TrayType = order.TrayType.GetValueOrDefault();
                return operateResult;
            }
        }

        /// <summary>
        /// 指令下发
        /// </summary>
        /// <param name="transMsg"></param>
        /// <returns></returns>
        public virtual OperateResult ExcuteTransportMessage(TransportMessage transMsg)
        {

            //1.通过搬运信息获取到设备及地址
            //2.获取到设备后判断开始设备及目标设备的状态，判断是否能够执行该指令
            //2.通过开始设备及地址（涉及到开始设备对地址协议的转换及通讯协议）下发给目标设备
            OperateResult exceptionResult = new OperateResult();
            try
            {
                DeviceBaseAbstract startDevice = transMsg.StartDevice;
                DeviceBaseAbstract destDevice = transMsg.DestDevice;
                TransportDeviceBaseAbstract transportDevice = (TransportDeviceBaseAbstract)transMsg.TransportDevice;
                OperateResult transportResult = transportDevice.DoTransportJob(transMsg);
                if (!transportResult.IsSuccess)
                {
                    return
                        OperateResult.CreateFailedResult(
                            string.Format("通知搬运设备：{0} 失败,失败原因：{2} \r\n将会重新执行指令：{1}", transportDevice.Name, transMsg.TransportOrder.OrderId, transportResult.Message), 1);
                }
                else
                {
                    return OperateResult.CreateSuccessResult(string.Format("指令号：{1} 成功通知搬运设备：{0} ", transportDevice.Name,
                        transMsg.TransportOrder.OrderId));
                }
            }
            catch (Exception ex)
            {
                exceptionResult.IsSuccess = false;
                exceptionResult.ErrorCode = 1;
                exceptionResult.Message = OperateResult.ConvertException(ex);
            }
            return exceptionResult;
        }

        /// <summary>
        /// 指令下发前的业务处理
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult BeforeTransportBusinessHandle(TransportMessage transport);


        /// <summary>
        /// 指令下发后的业务处理
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        public virtual OperateResult AfterTransportBusinessHandle(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 处理强制完成的业务
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public abstract OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order);

        /// <summary>
        /// 取消指令业务
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public abstract OperateResult CancleOrder(DeviceName deviceName, ExOrder order);

        /// <summary>
        /// 搬运信息完成的业务
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="transport"></param>
        /// <returns></returns>
        public OperateResult FinishTransport(DeviceName deviceName, TransportMessage transport)
        {
            //1.搬运信息完成前的业务
            //2.搬运信息完成业务
            //3.搬运信息完成后的业务
            OperateResult beforeFinishResult = BeforeFinishTransportMsgBusiness(deviceName, transport);
            if (!beforeFinishResult.IsSuccess)
            {
                return beforeFinishResult;
            }

            OperateResult finishResult = FinishTransportMsgBusiness(deviceName, transport);
            if (!finishResult.IsSuccess)
            {
                return finishResult;
            }
            OperateResult afterFinishResult = AfterFinishTransportMsgBusiness(deviceName, transport);
            if (!afterFinishResult.IsSuccess)
            {
                return afterFinishResult;
            }
            return OperateResult.CreateSuccessResult();
        }


        public abstract OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg);


        public abstract OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg);


        public abstract OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg);

        public abstract OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg);

    }
}
