using System;
using System.Threading;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.OrderHandle
{
    public sealed class OrderNotifyHandle : IOrderNotifyCentre
    {
        private readonly Pool<IHandleOrderExcuteStatus> _orderChangeListener = new Pool<IHandleOrderExcuteStatus>();

        public Pool<IHandleOrderExcuteStatus> OrderStatusListenerList
        {
            get { return _orderChangeListener; }
        }

        public OperateResult RegisterOrderStatusListener(IHandleOrderExcuteStatus listener)
        {
            lock (_orderChangeListener)
            {
                return _orderChangeListener.AddPool(listener);
            }
        }

        public OperateResult UnRegisterOrderStatusListener(IHandleOrderExcuteStatus listener)
        {
            lock (_orderChangeListener)
            {
                return _orderChangeListener.RemovePool(listener);
            }
        }

        public void NotifyOrderChange(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type)
        {
            new Thread(() =>
            {
                lock (_orderChangeListener)
                {
                    foreach (IHandleOrderExcuteStatus listener in _orderChangeListener.Container)
                    {
                        try
                        {
                            listener.HandleOrderChange(deviceName, order, type);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
            }).Start();
        }

    }
}
