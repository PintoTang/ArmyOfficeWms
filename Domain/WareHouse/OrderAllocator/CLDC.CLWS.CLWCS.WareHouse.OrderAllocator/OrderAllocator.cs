using System;
using System.Collections.Generic;
using System.Linq;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.OrderAllocator
{
    public class OrderAllocator : IOrderAllocator
    {

        private readonly object locker = new object();


        private readonly Dictionary<Addr, IOrderReceive> dicInReceiver
            = new Dictionary<Addr, IOrderReceive>();

        private readonly Dictionary<Addr, IOrderReceive> dicOutReceiver
            = new Dictionary<Addr, IOrderReceive>();

        private readonly Dictionary<Addr, IOrderReceive> dicMoveReceiver
            = new Dictionary<Addr, IOrderReceive>();


        private readonly ExOrderPool unAllocatedOrderPool = new ExOrderPool();

        private void RemoveOrderReceiver(IOrderReceive orderReceiver)
        {
            RemoveOrderReceiverDic(orderReceiver, dicInReceiver);
            RemoveOrderReceiverDic(orderReceiver, dicOutReceiver);
            RemoveOrderReceiverDic(orderReceiver, dicMoveReceiver);
        }

        private void RemoveOrderReceiverDic(IOrderReceive orderReceiver, Dictionary<Addr, IOrderReceive> dic)
        {
            lock (dic)
            {
                bool isFindReceiverInTheDeviceNo = true;

                while (isFindReceiverInTheDeviceNo)
                {
                    isFindReceiverInTheDeviceNo = false;
                    foreach (KeyValuePair<Addr, IOrderReceive> currAddrPrefixPair in dic)
                    {
                        if (currAddrPrefixPair.Value == orderReceiver)
                        {
                            isFindReceiverInTheDeviceNo = true;
                            dic.Remove(currAddrPrefixPair.Key);
                            break;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 把指令添加到分发器
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult Receive(Order order)
        {
            //1.把Order转换为ExOrder
            //2.把ExOrder添加到未分发的列表内
            //3.释放一个分发指令的信号量
            if (order == null)
            {
                return OperateResult.CreateFailedResult("传入空参数", 1);
            }
            ExOrder exOrder;
            if (!(order is ExOrder))
            {
                try
                {
                    exOrder = ExOrder.OrderToExOrder(order);
                }
                catch (Exception ex)
                {
                    return OperateResult.CreateFailedResult(string.Format("Order转换为ExOrder异常:{0}", OperateResult.ConvertException(ex)), 1);
                }
            }
            else
            {
                exOrder = (ExOrder)order;
            }
            if (IsInvalidOrder(exOrder))
            {
                return OperateResult.CreateFailedResult(string.Format("指令：{0} 为不合法指令", exOrder), 1);
            }
            OperateResult repeatOrder = IsRepeatOrder(exOrder);
            if (repeatOrder.IsSuccess)
            {
                return repeatOrder;
            }

            OperateResult addResult = unAllocatedOrderPool.AddPool(exOrder);

            return addResult;

        }

        private OperateResult IsRepeatOrder(ExOrder exOrder)
        {
            int orderId = exOrder.OrderId;

            //int temp = unAllocatedOrderPool.Container.Count(
            //    it => it.BottomBoxBarcode == exOrder.BottomBoxBarcode
            //          && it.DestAddr == exOrder.DestAddr
            //          && it.OrderType == exOrder.OrderType
            //          && it.OrderId == exOrder.OrderId
            //          && it.OrderPriority == exOrder.OrderPriority
            //          && it.PileNo == exOrder.PileNo
            //          && it.StartAddr == exOrder.StartAddr
            //    );
            int temp = unAllocatedOrderPool.Container.Count(it => it.DocumentCode==exOrder.DocumentCode);

            if (temp > 0)
            {
                string msg = @"在某指令完成之前，MS又下发了一条相同指令Id的命令"
                             + "\r\n该指令Id为："
                             + orderId
                             + "\r\n该指令希望搬运的垛号为为："
                             + exOrder.PileNo
                             + "\r\n该指令希望到达的目的地址为："
                             + exOrder.DestAddr
                             + "\r\n原来的指令希望搬运的垛号为为："
                             + exOrder.PileNo
                             + "\r\n原来的指令希望到达的目的地址为："
                             + exOrder.DestAddr;
                return OperateResult.CreateSuccessResult(msg);
            }
            return OperateResult.CreateFailedResult();
        }


        private bool IsInvalidOrder(ExOrder exOrder)
        {
            return false;
        }

        private bool RepeatCheckRegisterSrcAddrPrefix(
            IOrderReceive orderReceiver,
            Dictionary<Addr, IOrderReceive> dic)
        {
            foreach (KeyValuePair<Addr, IOrderReceive> addrPair in dic)
            {
                if (orderReceiver.OrderReceiverName == addrPair.Value.OrderReceiverName)
                {
                    return false;
                }
            }

            return true;
        }

        private void AddOrderReceiver(
            IOrderReceive orderReceiver,
            List<string> srcAddrPrefixList,
            Dictionary<Addr, IOrderReceive> dic)
        {
            foreach (string srcAddrPrefix in srcAddrPrefixList)
            {
                Addr tempAddr = new Addr(srcAddrPrefix);
                if(dic.ContainsKey(tempAddr))
                {
                    throw new Exception("指令前缀重复注册处理者：" +"处理者："+ orderReceiver.OrderReceiverName.FullName+"地址："+tempAddr);
                }
                dic.Add(new Addr(srcAddrPrefix), orderReceiver);
            }
        }

        /// <summary>
        /// 注册命令接收者
        /// </summary>
        /// <param name="orderReceiver"></param>
        public void RegisterOrderReceiver(IOrderReceive orderReceiver)
        {
            lock (locker)
            {
                if (!RepeatCheckRegisterSrcAddrPrefix(orderReceiver, dicInReceiver)
                    || !RepeatCheckRegisterSrcAddrPrefix(orderReceiver, dicOutReceiver)
                    || !RepeatCheckRegisterSrcAddrPrefix(orderReceiver, dicMoveReceiver))
                {
                    throw new Exception("设备名重复注册：" + orderReceiver.OrderReceiverName.FullName);
                }

                if (null != orderReceiver.InSrcAddrPrefixList)
                {
                    AddOrderReceiver(orderReceiver, orderReceiver.InSrcAddrPrefixList, dicInReceiver);
                }
                if (null != orderReceiver.OutSrcAddrPrefixList)
                {
                    AddOrderReceiver(orderReceiver, orderReceiver.OutSrcAddrPrefixList, dicOutReceiver);
                }
                if (null != orderReceiver.MoveSrcAddrPrefixList)
                {
                    AddOrderReceiver(orderReceiver, orderReceiver.MoveSrcAddrPrefixList, dicMoveReceiver);
                }
                orderReceiver.OrderUpdateStatusEvent += UpdateOrderStatus;
            }
        }

        /// <summary>
        /// 解除命令接收者
        /// </summary>
        /// <param name="orderReceiver"></param>
        public void UnregisterOrderReceiver(IOrderReceive orderReceiver)
        {
            RemoveOrderReceiver(orderReceiver);
        }

        private ExOrder ObtainOrder()
        {
            //1.根据优先级
            //2.根据时间
            //3.根据分发失败的次数

            lock (unAllocatedOrderPool)
            {
                IEnumerable<ExOrder> orderlyOrder = unAllocatedOrderPool.Container.OrderBy(o => o.AllocateTime);
                return orderlyOrder.FirstOrDefault();
            }
        }


        public OperateResult<IOrderReceive> Alloc(ExOrder order)
        {

            switch (order.OrderType)
            {
                case OrderTypeEnum.In:
                    return _AllocValidOrder(order, dicInReceiver);
                case OrderTypeEnum.Out:
                    return _AllocValidOrder(order, dicOutReceiver);
                case OrderTypeEnum.Move:
                    return _AllocValidOrder(order, dicMoveReceiver);
                default:
                    return OperateResult.CreateFailedResult<IOrderReceive>(string.Format("根据指令：{0} 的指令方式：{1} 找不到合适的指令处理者", order, order.OrderType), 1);
            }
        }

        private OperateResult<List<IOrderReceive>> FindPrefixAccordantReceiverList(ExOrder exOrder, Dictionary<Addr, IOrderReceive> addrPrefixDic)
        {
            List<IOrderReceive> prefixAccordantReceiverList = new List<IOrderReceive>();
            OperateResult<List<IOrderReceive>> result = OperateResult.CreateFailedResult(prefixAccordantReceiverList, "无数据");
            foreach (KeyValuePair<Addr, IOrderReceive> child in addrPrefixDic)
            {
                try
                {
                    Addr addrPrefix = child.Key;
                    if (addrPrefix.IsContain(exOrder.CurrAddr))
                    {
                        prefixAccordantReceiverList.Add(child.Value);
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            result.IsSuccess = true;
            result.Content = prefixAccordantReceiverList;
            return result;
        }
        private static object allocLock = new object();
        private OperateResult<IOrderReceive> _AllocValidOrder(ExOrder exOrder, Dictionary<Addr, IOrderReceive> addrPrefixDic)
        {
            lock (allocLock)
            {
                OperateResult<List<IOrderReceive>> getOrderReceive = FindPrefixAccordantReceiverList(exOrder, addrPrefixDic);
                if (!getOrderReceive.IsSuccess)
                {
                    return OperateResult.CreateFailedResult<IOrderReceive>(string.Format("获取指令接收者失败：{0}", getOrderReceive.Message), 1);
                }
                List<IOrderReceive> prefixAccordantReceiverList = getOrderReceive.Content;

                //没有找到前缀匹配的设备，要么是设备被禁用了，要么是MS发送了一条错误的order
                if (prefixAccordantReceiverList.Count == 0)
                {
                    return OperateResult.CreateFailedResult<IOrderReceive>(string.Format("根据指令：{0} 找不到合适的指令处理者", exOrder), 1);
                }
                foreach (IOrderReceive receiver in prefixAccordantReceiverList)
                {
                    OperateResult isCanRecieve = receiver.IsCanRecieveOrder(exOrder);
                    if (!isCanRecieve.IsSuccess)
                    {
                        LogMessage(string.Format("工作者:{0} 当前不能接收指令：{1} 原因：{2}", receiver.OrderReceiverName.FullName, exOrder.OrderId, isCanRecieve.Message), EnumLogLevel.Info, false);
                    }
                }
                if (!prefixAccordantReceiverList.Exists(o => o.IsCanRecieveOrder(exOrder).IsSuccess))
                {
                    return OperateResult.CreateFailedResult<IOrderReceive>(string.Format("根据指令：{0} 找不到正在工作的指令处理者", exOrder), 1);
                }

                //MS没有建议使用的设备
                if (exOrder.PriorDeviceList == null || exOrder.PriorDeviceList.Count == 0)
                {
                    IOrderReceive firstOrderhandle = prefixAccordantReceiverList.FirstOrDefault(o => o.IsCanRecieveOrder(exOrder).IsSuccess);
                    if (firstOrderhandle == null)
                    {
                        return OperateResult.CreateFailedResult<IOrderReceive>(string.Format("根据指令：{0} 找不到正在工作并且空闲的指令处理者", exOrder), 1);
                    }
                    return _AllocateOrderToOrderHandle(exOrder, firstOrderhandle);
                }
                IOrderReceive orderCanReceiver = null;
                for (int i = 0; i < exOrder.PriorDeviceList.Count; i++)
                {
                    for (int j = 0; j < prefixAccordantReceiverList.Count; j++)
                    {
                        if (exOrder.PriorDeviceList[i].FullName == prefixAccordantReceiverList[j].OrderReceiverName.FullName)
                        {
                            orderCanReceiver = prefixAccordantReceiverList[j];
                            break;
                        }
                    }
                    if (orderCanReceiver != null)
                        break;
                }

                if (orderCanReceiver != null)
                {
                    return _AllocateOrderToOrderHandle(exOrder, orderCanReceiver);
                }
                return OperateResult.CreateFailedResult<IOrderReceive>("可接收指令的接收者为空");
            }
        }

        private OperateResult<IOrderReceive> _AllocateOrderToOrderHandle(ExOrder order, IOrderReceive orderHandle)
        {
            OperateResult<IOrderReceive> allocateResult = new OperateResult<IOrderReceive>();
            OperateResult receiveResult = orderHandle.ReceiveOrder(order);
            if (!receiveResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<IOrderReceive>(string.Format("把指令：{0} 分发给：{1} 失败，将会重新分发", order, orderHandle.OrderReceiverName.FullName), 1);
            }
            string msg = string.Format("指令：{0} 成功分发给：{1}", order, orderHandle.OrderReceiverName.FullName);
            LogMessage(msg, EnumLogLevel.Info, false);
            allocateResult.IsSuccess = true;
            allocateResult.Content = orderHandle;
            return allocateResult;
        }



        private string name = "指令分发器";

        /// <summary>
        /// 指令分发器的名字
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set { name = value; }
        }


        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            LogHelper.WriteLog(this.Name, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }

        public OperateResult UpdateOrderStatus(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type)
        {
            if (UpdateOrderStatusEvent != null)
            {
                return UpdateOrderStatusEvent(deviceName, order, type);
            }
            return OperateResult.CreateFailedResult("尚未注册:NotifyOrderFinishEvent 的方法", 1);
        }


        public event OrderUpdateStatusDelegate UpdateOrderStatusEvent;
    }
}