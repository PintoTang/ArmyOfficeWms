using System;
using System.Threading;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.StringCharTaskHandle
{
    public sealed class StringCharTaskNotifyHandle : IStringCharTaskNotifyCentre
    {
        private readonly Pool<IHandleStringCharTaskStatus> _taskChangeListener = new Pool<IHandleStringCharTaskStatus>();

        public Pool<IHandleStringCharTaskStatus> TaskStatusListenerList { get { return _taskChangeListener; } }
        public OperateResult RegisterTaskStatusListener(IHandleStringCharTaskStatus listener)
        {
            lock (_taskChangeListener)
            {
                return _taskChangeListener.AddPool(listener);
            }
        }

        public OperateResult UnRegisterTaskStatusListener(IHandleStringCharTaskStatus listener)
        {
            lock (_taskChangeListener)
            {
                return _taskChangeListener.RemovePool(listener);
            }
        }

        public void NotifyStringCharTaskFinish(DeviceBaseAbstract device, StringCharTask task)
        {
            new Thread(() =>
            {
                lock (_taskChangeListener)
                {
                    foreach (IHandleStringCharTaskStatus listener in _taskChangeListener.Container)
                    {
                        try
                        {
                            listener.FinishStringCharTask(device, task);
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
