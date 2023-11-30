using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.ThreadHandle;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.Framework.Log.Helper;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.OrderHandle;
using Infrastructrue.Ioc.DependencyFactory;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Manage
{

    /// <summary>
    /// 整个WCS系统的指令管理模块
    /// </summary>
    public sealed class OrderManage : NotifyObject, IOrderManage, IStateControl, IRestore
    {

        private readonly IOrderAllocator _allocatorHandler;
        private readonly IOrderGenerate _orderGeneraterHandler;
        private readonly OrderDataAbstract _orderDatabaseHandler;
        private readonly TaskOrderDataAbstract _taskOrderDataHandler;
        private readonly IOrderNotifyCentre _orderNotifyHandle;
        public OrderManage(IOrderAllocator allocator, IOrderGenerate orderGenerate, OrderDataAbstract orderDatabase)
        {
            this._allocatorHandler = allocator;
            _allocatorHandler.UpdateOrderStatusEvent += UpdateOrderStatus;
            _orderGeneraterHandler = orderGenerate;
            _orderDatabaseHandler = orderDatabase;
            _taskOrderDataHandler = DependencyHelper.GetService<TaskOrderDataAbstract>();
            _orderNotifyHandle = DependencyHelper.GetService<IOrderNotifyCentre>();
            HandleRestoreData();
           
        }
        public int GetGlobalOrderID()
        {
            return _orderGeneraterHandler.GetGlobalNewTaskId();
        }



        /// <summary>
        /// 断点恢复
        /// </summary>
        /// <returns></returns>
        public OperateResult Restore()
        {
            if (_orderDatabaseHandler == null)
            {
                return OperateResult.CreateFailedResult("指令数据库尚未实例化", 1);
            }
            List<StatusEnum> statusLst = new List<StatusEnum>() { StatusEnum.NotifyOPC, StatusEnum.Processing, StatusEnum.Waiting, StatusEnum.TransportCompleted };
            List<ExOrder> unfinishOrder = _orderDatabaseHandler.GetDataByStatus(statusLst);
            if (unfinishOrder == null || unfinishOrder.Count <= 0)
            {
                return OperateResult.CreateSuccessResult("断点恢复，无未完成指令");
            }
            foreach (ExOrder exOrder in unfinishOrder)
            {
                RestoreExOrder(exOrder);
                _managedDataPool.AddPool(exOrder);
                LogMessage(string.Format("断点恢复获取到的指令：{0} ", exOrder.OrderId), EnumLogLevel.Info, true);

                ReleaseOrderAmountSem(string.Format("断点恢复获取到的指令：{0} ", exOrder.OrderId));

            }
            return OperateResult.CreateSuccessResult();
        }

        private void RestoreExOrder(ExOrder exOrder)
        {
            OperateResult<TaskOrderDataModel> getTaskOrderDataModel = _taskOrderDataHandler.GetTaskCodeByOrderId(exOrder.OrderId);
            if (getTaskOrderDataModel.IsSuccess)
            {
                exOrder.DeviceTaskNo = getTaskOrderDataModel.Content.DeviceTaskCode;
                exOrder.UpperTaskNo = getTaskOrderDataModel.Content.TaskCode;
            }
        }

        private void StopThread(string msg)
        {
            ReleaseOrderAmountSem("系统退出");
            _pauseEvent.Set();
        }

        private ThreadHandleProcess _orderThreadHander;
        public OperateResult Start()
        {
            this.CurRunState = RunStateMode.Run;
            _orderThreadHander = ThreadHandleManage.CreateNewThreadHandle(Name + OrderHandleThreadName, ThreadHandle);
            _orderThreadHander.ThreadStopAction += StopThread;
            return _orderThreadHander.Start();
        }

        private const string OrderHandleThreadName = "_处理线程";

        public string Name = "指令管理";

        private string _enName = "OrderManage";
        public OperateResult Run()
        {
            _pauseEvent.Set();
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }



        public DataObservablePool<ExOrder> ManagedDataPool
        {
            get { return _managedDataPool; }
        }

        private object objLock = new object();


        private readonly AutoResetEvent _pauseEvent = new AutoResetEvent(false);

        private readonly Semaphore _orderAmountSem = new Semaphore(0, 0x7FFFFFFF);

        private void ThreadHandle()
        {
            //1.判断业务处理的运行状态，运行、暂停、停止
            //2.运行时判断等待执行的指令集合情况
            //3.根据等待执行指令集合的情况和命令处理信号进行命令处理
            //4.根据指令处理的条件获取一个需要处理的指令，条件：分发的次数 分发的时间
            //5.根据指令计算下一步地址，更改指令计算下步地址的次数
            //6.把指令下发到下一个设备
            //7.指令从等待执行指令的集合中移除
            //8.添加到等待完成的指令中

            while (_orderThreadHander.IsContinuous)
            {
                try
                {

                    if (_managedDataPool.Count(o => o.Status.Equals(StatusEnum.Discard)) > 0)
                    {
                        OperateResult<ExOrder> getDiscardOrder = _managedDataPool.FindData(o => o.Status.Equals(StatusEnum.Discard));
                        if (getDiscardOrder.IsSuccess)
                        {
                            DiscardOrder(new DeviceName("OrderManage#1"), getDiscardOrder.Content);
                            ReleaseOrderAmountSem("丢弃指令释放信号");
                        }
                    }

                    if (_managedDataPool.Count(o => o.Status.Equals(StatusEnum.Cancle)) > 0)
                    {
                        OperateResult<ExOrder> getCancelOrder = _managedDataPool.FindData(o => o.Status.Equals(StatusEnum.Cancle));
                        if (getCancelOrder.IsSuccess)
                        {
                            CancelOrder(new DeviceName("OrderManage#1"), getCancelOrder.Content);
                            ReleaseOrderAmountSem("取消指令释放信号");
                        }
                    }

                    _orderAmountSem.WaitOne();

                    if (!SystemConfig.Instance.IsLicenseAvailable)
                    {
                        Thread.Sleep(5000);
                        ReleaseOrderAmountSem("注册码过期释放信号量");
                        continue;
                    }

                    if (CurRunState.Equals(RunStateMode.Stop))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    if (_managedDataPool.Count(o => o.Status.Equals(StatusEnum.Discard)) > 0)
                    {
                        OperateResult<ExOrder> getDiscardOrder = _managedDataPool.FindData(o => o.Status.Equals(StatusEnum.Discard));
                        if (getDiscardOrder.IsSuccess)
                        {
                            DiscardOrder(new DeviceName("OrderManage#1"), getDiscardOrder.Content);
                        }
                    }

                    if (_managedDataPool.Count(o => o.Status.Equals(StatusEnum.Cancle)) > 0)
                    {
                        OperateResult<ExOrder> getCancelOrder = _managedDataPool.FindData(o => o.Status.Equals(StatusEnum.Cancle));
                        if (getCancelOrder.IsSuccess)
                        {
                            CancelOrder(new DeviceName("OrderManage#1"), getCancelOrder.Content);
                        }
                    }

                    if (_managedDataPool.Count(o => o.Status.Equals(StatusEnum.Waiting)) <= 0)
                    {
                        continue;
                    }
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        _pauseEvent.WaitOne();
                    }
                    IEnumerable<ExOrder> allocateOrderList = ObtainAllocateOrder();
                    IEnumerable<ExOrder> orderList = allocateOrderList as IList<ExOrder> ?? allocateOrderList.ToList();
                    if (allocateOrderList == null || !orderList.Any())
                    {
                        continue;
                    }
                    List<ExOrder> tempOrderList = new List<ExOrder>();
                    tempOrderList.Add(orderList.First());//orderList
                    if (orderList.Count() >= 2)
                    {
                        var nn = orderList.Where(x => x.OrderId != tempOrderList.First().OrderId &&
                            (x.OrderType == OrderTypeEnum.In));

                        if (nn != null && nn.Count() >= 1)
                        {
                            tempOrderList.AddRange(nn);
                        }

                        var nn2 = orderList.GetEnumerator();
                        while (nn2.MoveNext())
                        {
                            ExOrder tempOrder = nn2.Current;
                            if (tempOrderList.Exists(x => x.OrderId != tempOrder.OrderId))
                            {
                                if (!tempOrder.StartAddrName.Equals(tempOrder.CurrAddrName))
                                {
                                    tempOrderList.Insert(0, tempOrder);
                                }
                            }
                        }
                    }

                    foreach (ExOrder order in tempOrderList)
                    {
                        if (CurRunState.Equals(RunStateMode.Pause))
                        {
                            _pauseEvent.WaitOne();
                        }

                        //业务调度处理  1008 和2001
                        if (SystemConfig.Instance.WhCode.Equals("SNDL1"))
                        {
                            lock (_managedDataPool)
                            {
                                if (((order.OrderType == OrderTypeEnum.Out || order.OrderType == OrderTypeEnum.PickOut))
                                    && (order.Status == StatusEnum.Waiting || order.Status == StatusEnum.NotifyOPC
                                    || order.Status == StatusEnum.Processing || order.Status == StatusEnum.TransportCompleted))
                                {
                                    //如果还有其它除本身以外的正在执行的出库 也不得执行
                                    var ortherOutOrder = _managedDataPool.DataPool.Where(x => (x.OrderType == OrderTypeEnum.Out || x.OrderType == OrderTypeEnum.PickOut)
                                        && x.DestAddr.IsContain(order.DestAddr) && (x.Status != StatusEnum.Waiting));
                                    if (ortherOutOrder != null && ortherOutOrder.Count() > 0)
                                    {
                                        if(order.DestAddr.IsContain("GetGoodsPort:3_1_1"))
                                        {
                                            LogMessage("已有其它执行的出库任务，暂时不得下发指令", EnumLogLevel.Error, true);
                                            order.AllocateFailTime++;
                                            order.AllocateTime++;
                                            UpdateOrder(order);
                                            Thread.Sleep(5000);
                                            continue;
                                        }

                                        //LogMessage("已有其它执行的出库任务，暂时不得下发指令", EnumLogLevel.Error, true);
                                        //order.AllocateFailTime++;
                                        //order.AllocateTime++;
                                        //UpdateOrder(order);
                                        //Thread.Sleep(200);
                                        //continue;

                                    }

                                    if (order.DestAddr.IsContain("PutGoodsPort:1_1_1"))
                                    {
                                        //1008
                                        DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                                        TransportPointStation transDevice = curDevice as TransportPointStation;
                                        RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                                        OperateResult<int> opcResult1008 = roller.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                                        if (opcResult1008.IsSuccess)
                                        {
                                            if (opcResult1008.Content != 0)
                                            {
                                                //判断另外一个巷道是否有货

                                                if (order.CurrAddr.IsContain("FloorStation:1_1_1")
                                                    || order.CurrAddr.IsContain("FloorStation:3_1_1"))
                                                {

                                                }
                                                else
                                                {

                                                    //有货
                                                    LogMessage("1008当前位置有货未入库，暂时不得下发指令", EnumLogLevel.Error, true);
                                                    order.AllocateFailTime++;
                                                    order.AllocateTime++;
                                                    UpdateOrder(order);
                                                    Thread.Sleep(5000);
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                //无货
                                                //查看 指令 起始
                                                lock (_managedDataPool)
                                                {
                                                    var nn = _managedDataPool.DataPool.Where(x => x.OrderType == OrderTypeEnum.In && x.StartAddr.IsContain(order.DestAddr));

                                                    if (nn != null && nn.Count() > 0)
                                                    {
                                                        //有已经存在的入库指令 
                                                        //再次筛选 排  是否是同巷道的排  1-2   3-4 
                                                        int outGoodsRangeNum = order.StartAddr.Range;
                                                        if (outGoodsRangeNum == 1 || outGoodsRangeNum == 2)
                                                        {
                                                            var selectNN = nn.Where(x => x.DestAddr.Range == 1 || x.DestAddr.Range == 2);
                                                            if (selectNN != null && selectNN.Count() > 0)
                                                            {
                                                                //有入库指令不得发
                                                                LogMessage("1008当前位置有货正在入库，暂时不得下发指令1", EnumLogLevel.Error, true);
                                                                order.AllocateFailTime++;
                                                                order.AllocateTime++;
                                                                UpdateOrder(order);
                                                                Thread.Sleep(5000);
                                                                continue;
                                                            }
                                                        }
                                                        else if (outGoodsRangeNum == 3 || outGoodsRangeNum == 4)
                                                        {
                                                            var selectNN = nn.Where(x => x.DestAddr.Range == 3 || x.DestAddr.Range == 4);
                                                            if (selectNN != null && selectNN.Count() > 0)
                                                            {
                                                                LogMessage("1008当前位置有货正在入库，暂时不得下发指令2", EnumLogLevel.Error, true);
                                                                order.AllocateFailTime++;
                                                                order.AllocateTime++;
                                                                UpdateOrder(order);
                                                                Thread.Sleep(5000);
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            LogMessage("读取1008 包号 OPC失败，暂时不得下发指令22", EnumLogLevel.Error, true);
                                            order.AllocateFailTime++;
                                            order.AllocateTime++;
                                            UpdateOrder(order);
                                            Thread.Sleep(5000);
                                            continue;
                                        }
                                    }
                                    else if (order.DestAddr.IsContain("GetGoodsPort:3_1_1"))
                                    {
                                        //2001
                                        DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                                        TransportPointStation transDevice = curDevice as TransportPointStation;
                                        RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                                        OperateResult<int> opcResult2001 = roller.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                                        if (opcResult2001.IsSuccess)
                                        {
                                            if (opcResult2001.Content != 0)
                                            {
                                                //有货
                                                LogMessage("2001当前位置有货未入库，暂时不得下发指令", EnumLogLevel.Error, true);
                                                order.AllocateFailTime++;
                                                order.AllocateTime++;
                                                UpdateOrder(order);
                                                Thread.Sleep(5000);
                                                continue;
                                            }
                                            else
                                            {
                                                //无货
                                                //查看 指令 起始

                                                var nn = _managedDataPool.DataPool.Where(x => x.OrderType == OrderTypeEnum.In && x.StartAddr.IsContain(order.DestAddr));

                                                if (nn != null && nn.Count() > 0)
                                                {
                                                    //有已经存在的入库指令 
                                                    //再次筛选 排  是否是同巷道的排  1-2   3-4 
                                                    int outGoodsRangeNum = order.StartAddr.Range;
                                                    if (outGoodsRangeNum == 1 || outGoodsRangeNum == 2)
                                                    {
                                                        var selectNN = nn.Where(x => x.DestAddr.Range == 1 || x.DestAddr.Range == 2);
                                                        if (selectNN != null && selectNN.Count() > 0)
                                                        {
                                                            //有入库指令不得发
                                                            LogMessage("2001当前位置有货正在入库，暂时不得下发指令1", EnumLogLevel.Error, true);
                                                            order.AllocateFailTime++;
                                                            order.AllocateTime++;
                                                            UpdateOrder(order);
                                                            Thread.Sleep(5000);
                                                            continue;
                                                        }
                                                    }
                                                    else if (outGoodsRangeNum == 3 || outGoodsRangeNum == 4)
                                                    {
                                                        var selectNN = nn.Where(x => x.DestAddr.Range == 3 || x.DestAddr.Range == 4);
                                                        if (selectNN != null && selectNN.Count() > 0)
                                                        {
                                                            LogMessage("2001当前位置有货正在入库，暂时不得下发指令2", EnumLogLevel.Error, true);
                                                            order.AllocateFailTime++;
                                                            order.AllocateTime++;
                                                            UpdateOrder(order);
                                                            Thread.Sleep(5000);
                                                            continue;
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
                                            LogMessage("读取2001 包号 OPC失败，暂时不得下发指令22", EnumLogLevel.Error, true);
                                            order.AllocateFailTime++;
                                            order.AllocateTime++;
                                            UpdateOrder(order);
                                            Thread.Sleep(5000);
                                            continue;
                                        }
                                    }
                                }
                            }
                        }

                        //添加到分发器
                        OperateResult<IOrderReceive> allocateResult = _allocatorHandler.Alloc(order);
                        if (!allocateResult.IsSuccess)
                        {
                            LogMessage(allocateResult.Message, EnumLogLevel.Error, true);
                            order.AllocateFailTime++;
                            order.AllocateTime++;
                            UpdateOrder(order);
                            Thread.Sleep(5000);
                            continue;
                        }

                        //

                        order.IsAllocated = true;
                        order.Status = StatusEnum.Processing;
                        order.CurHandlerId = allocateResult.Content.ReceiverId;
                        UpdateOrder(order);
                        Thread.Sleep(200);
                    }
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        _pauseEvent.WaitOne();
                    }
                    if (_managedDataPool.Count(o => o.Status.Equals(StatusEnum.Waiting)) > 0)
                    {
                        //还存在等待处理的指令，接着处理
                        ReleaseOrderAmountSem("还有需要被处理的指令 释放信号");
                    }
                }
                catch (ThreadAbortException abort)
                {
                    LogMessage(abort.StackTrace.ToString(), EnumLogLevel.Error, false);
                }
                catch (Exception ex)
                {
                    LogMessage(ex.StackTrace.ToString(), EnumLogLevel.Error, false);
                }
                finally
                {
                    Thread.Sleep(200);
                }
            }
        }

        private void ReleaseOrderAmountSem(string reason)
        {
            LogMessage(reason, EnumLogLevel.Info, false);
            _orderAmountSem.Release();
        }

        public Action<string, EnumLogLevel> NotifyMsgToUiEvent;
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="isNotifyUi"></param>
        public void LogMessage(string msg, EnumLogLevel level = EnumLogLevel.Info, bool isNotifyUi = true)
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



        private readonly DataObservablePool<ExOrder> _managedDataPool = new DataObservablePool<ExOrder>();

        /// <summary>
        /// 添加未完成指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult AddUnfinishedOrder(ExOrder order)
        {
            order.Status = StatusEnum.Waiting;
            _orderDatabaseHandler.SaveOrderAsync(order);
            return _managedDataPool.AddPool(order);
        }
        /// <summary>
        /// 移除未完成指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult RemoveUnfinishedOrder(ExOrder order)
        {
            return _managedDataPool.RemovePool(order);
        }
        /// <summary>
        /// 更新未完成指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult UpdateUnfinishedOrder(ExOrder order)
        {
            OperateResult result = _managedDataPool.UpdatePool(order);
            return result;
        }

        /// <summary>
        /// 根据指令ID获取未完成指令
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OperateResult<ExOrder> FindUnfinishedOrder(int orderId)
        {
            return _managedDataPool.FindData(o => o.OrderId.Equals(orderId));
        }

        public OperateResult UpdateOrderPriority(string documentCode,int priority)
        {
            var content= _managedDataPool.FindData(o => o.DocumentCode==documentCode && o.Status== StatusEnum.Waiting);
            if (!content.IsSuccess|| content.Content==null)
            {
                return OperateResult.CreateSuccessResult();
            }
            var order = content.Content;
            order.OrderPriority = priority;
            return UpdateOrder(order);
        }

        private IEnumerable<ExOrder> ObtainAllocateOrder()
        {
            //1.先按优先级进行分组
            //2.组中按分配次数进行排序
            //3.获取最前组的最前一个
            lock (_managedDataPool)
            {
                IEnumerable<ExOrder> orderlyOrder = _managedDataPool.DataPool.Where(
                    o => o.Status.Equals(StatusEnum.Waiting))
                    .OrderByDescending(o => o.OrderPriority)
                    .ThenBy(o => o.AllocateTime);
                return orderlyOrder;
            }
        }


        private OperateResult FinishOrder(DeviceName deviceName, ExOrder order)
        {
            LogMessage(string.Format("接收到完成的指令：{0}", order), EnumLogLevel.Debug, false);
            OperateResult<ExOrder> finishOrder = GetMemoryExOrderByOrderId(order.OrderId);
            if (!finishOrder.IsSuccess)
            {
                LogMessage(string.Format("指令管理 找不到指令编号为：{0} 的未完成指令", order), EnumLogLevel.Warning, true);
                return OperateResult.CreateFailedResult(string.Format("找不到指令编号为：{0} 的未完成指令", order), 1);
            }
            ///更新数据库
            ///指令完成
            if (order.CurrAddrName.Equals(order.DestAddrName))
            {
                if (order.IsReport)
                {
                    NotifyOrderChange(deviceName, order, TaskHandleResultEnum.Finish);
                }
                order.Status = StatusEnum.CmdSent;
                order.FinishType = FinishType.AutoFinish;
                order.CurHandlerId = 0;
                UpdateOrder(order);
                LogMessage(string.Format("指令：{0}  执行完成！", order), EnumLogLevel.Info, true);
                RemoveUnfinishedOrder(order);
            }
            else
            {
                order.IsAllocated = false;
                order.AllocateTime = 0;
                order.AllocateFailTime = 0;
                order.CurHandlerId = 0;
                order.NextAddr = null;
                order.Status = StatusEnum.Waiting;
                order.FinishType = FinishType.UnFinished;
                NotifyOrderChange(deviceName, order, TaskHandleResultEnum.Update);
                OperateResult updateResult = UpdateOrder(order);

                if (!updateResult.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("更新指令：{0} 出错", finishOrder.Content), 1);
                }
                ReleaseOrderAmountSem("指令部分完成");
                LogMessage(string.Format("指令：{0} 继续分发", finishOrder.Content), EnumLogLevel.Info, false);
            }
            return OperateResult.CreateSuccessResult();
        }

        private void NotifyOrderChange(DeviceName deviceName, ExOrder order, TaskHandleResultEnum finish)
        {
            _orderNotifyHandle.NotifyOrderChange(deviceName, order, finish);
        }

        public OperateResult UpdateOrderStatus(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type)
        {
            switch (type)
            {
                case TaskHandleResultEnum.Finish:
                    return FinishOrder(deviceName, order);
                case TaskHandleResultEnum.Discard:
                    return DiscardOrder(deviceName, order);
                case TaskHandleResultEnum.ForceFinish:
                    return ForceFinishOrder(deviceName, order);
                case TaskHandleResultEnum.Cancle:
                    return CancelOrder(deviceName, order);
                case TaskHandleResultEnum.Update:
                    break;
                default:
                    break;
            }
            return OperateResult.CreateFailedResult();
        }


        /// <summary>
        /// 丢弃指令
        /// 未完成列表移除
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="exOrder">订单</param>
        public OperateResult DiscardOrder(DeviceName deviceName, ExOrder exOrder)
        {
            OperateResult<ExOrder> discardOrder = GetMemoryExOrderByOrderId(exOrder.OrderId);
            if (!discardOrder.IsSuccess)
            {
                LogMessage(string.Format("指令管理 找不到指令编号为：{0} 的未完成指令", exOrder), EnumLogLevel.Warning, true);
                return OperateResult.CreateFailedResult(string.Format("找不到指令编号为：{0} 的未完成指令", exOrder), 1);
            }
            exOrder.Status = StatusEnum.Discard;
            exOrder.FinishType = FinishType.Discard;
            exOrder.CurHandlerId = 0;
            NotifyOrderChange(deviceName, exOrder, TaskHandleResultEnum.Discard);
            UpdateOrder(exOrder);
            RemoveUnfinishedOrder(exOrder);
            return OperateResult.CreateSuccessResult();
        }



        /// <summary>
        /// 根据指令信息取消指令
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult CancelOrder(DeviceName deviceName, ExOrder order)
        {
            OperateResult<ExOrder> cancelOrder = GetMemoryExOrderByOrderId(order.OrderId);
            if (!cancelOrder.IsSuccess)
            {
                LogMessage(string.Format("指令管理 找不到指令编号为：{0} 的未完成指令", order), EnumLogLevel.Warning, true);
                return OperateResult.CreateFailedResult(string.Format("找不到指令编号为：{0} 的未完成指令", order), 1);
            }
            order.Status = StatusEnum.Cancle;
            order.FinishType = FinishType.Cancle;
            NotifyOrderChange(deviceName, order, TaskHandleResultEnum.Cancle);
            UpdateOrder(order);
            RemoveUnfinishedOrder(order);
            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 强制完成指令
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <returns>返回CancelOrderResult</returns>
        public OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order)
        {
            OperateResult<ExOrder> finishOrder = GetMemoryExOrderByOrderId(order.OrderId);
            if (!finishOrder.IsSuccess)
            {
                LogMessage(string.Format("指令管理 找不到指令编号为：{0} 的未完成指令", order), EnumLogLevel.Warning, true);
                return OperateResult.CreateFailedResult(string.Format("找不到指令编号为：{0} 的未完成指令", order), 1);
            }
            ///更新数据库
            ///指令完成
            if (order.CurrAddrName.Equals(order.DestAddrName))
            {
                if (order.IsReport)
                {
                    NotifyOrderChange(deviceName, order, TaskHandleResultEnum.ForceFinish);
                }
                order.Status = StatusEnum.CmdSent;
                order.FinishType = FinishType.ForceFinish;
                order.CurHandlerId = 0;
                UpdateOrder(order);
                LogMessage(string.Format("指令：{0}  强制执行完成！", order), EnumLogLevel.Info, true);

                RemoveUnfinishedOrder(order);
            }
            else
            {
                order.NextAddr = null;
                order.Status = StatusEnum.Waiting;
                order.IsAllocated = false;
                order.AllocateTime = 0;
                order.AllocateFailTime = 0;
                order.CurHandlerId = 0;
                NotifyOrderChange(deviceName, order, TaskHandleResultEnum.Update);
                UpdateOrder(order);
                ReleaseOrderAmountSem("强制完成");
                LogMessage(string.Format("指令：{0} 继续分发", order), EnumLogLevel.Info, false);
            }
            return OperateResult.CreateSuccessResult();
        }



        /// <summary>
        /// 更新指定的指令
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult UpdateOrder(ExOrder order)
        {
            OperateResult updateResult = _managedDataPool.UpdatePool(order);
            if (!updateResult.IsSuccess)
            {
                string msg = string.Format("更新指令：{0} 出错", order);
                LogMessage(msg);
                return updateResult;
            }
            _orderDatabaseHandler.SaveOrder(order);
            return OperateResult.CreateSuccessResult();
        }

        public void UpdateOrderAsync(ExOrder order)
        {
            OperateResult updateResult = _managedDataPool.UpdatePool(order);
            if (!updateResult.IsSuccess)
            {
                string msg = string.Format("更新指令：{0} 出错", order);
                LogMessage(msg);
            }
            _orderDatabaseHandler.SaveOrderAsync(order);
        }

        public OperateResult<ExOrder> GetMemoryExOrderByOrderId(int orderId)
        {
            return _managedDataPool.FindData(o => o.Id.Equals(orderId));
        }

        public OperateResult<ExOrder> GetDataBaseExOrderByOrderId(int orderId)
        {
            OperateResult<ExOrder> result = new OperateResult<ExOrder>();
            ExOrder exorder = _orderDatabaseHandler.GetExOrderByOrderId(orderId);
            if (exorder == null)
            {
                result.IsSuccess = false;
                return result;
            }
            result.IsSuccess = true;
            result.Content = exorder;
            return result;
        }



        /// <summary>
        /// 接收新的指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult AcceptOrder(Order order)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                ExOrder exOrder;
                if (!(order is ExOrder))
                {
                    try
                    {
                        exOrder = ExOrder.OrderToExOrder(order);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(string.Format("指令：{0} 转换异常", order), EnumLogLevel.Error, true);
                        return OperateResult.CreateFailedResult(string.Format("Order转换为ExOrder异常:{0}", OperateResult.ConvertException(ex)), 1);
                    }
                }
                else
                {
                    exOrder = (ExOrder)order;
                }
                if (IsInvalidOrder(exOrder))
                {
                    LogMessage(string.Format("指令：{0} 为不合法指令", exOrder), EnumLogLevel.Warning, true);
                    return OperateResult.CreateFailedResult(string.Format("指令：{0} 为不合法指令", exOrder), 1);
                }
                OperateResult repeatOrder = IsRepeatOrder(exOrder);
                if (repeatOrder.IsSuccess)
                {
                    LogMessage(repeatOrder.Message, EnumLogLevel.Warning, true);
                    return repeatOrder;
                }

                OperateResult addResult = AddUnfinishedOrder(exOrder);
                if (!addResult.IsSuccess)
                {
                    LogMessage(string.Format("指令：{0} 接收失败: {1}", exOrder, addResult.Message), EnumLogLevel.Error, true);
                    return addResult;
                }
                LogMessage(string.Format("成功接收到指令：{0}", exOrder), EnumLogLevel.Info, false);
                _orderAmountSem.Release();
                result.IsSuccess = true;
                result.Message = "成功";
                return result;
            }
            catch (Exception ex)
            {
                string msg = string.Format("接收指令失败：{0}", OperateResult.ConvertException(ex));
                LogMessage(msg);
            }
            result.IsSuccess = false;
            return result;
        }

        private RunStateMode _curState = RunStateMode.Pause;
        private ControlStateMode _curControlState = ControlStateMode.Auto;

        public RunStateMode CurRunState
        {
            get { return _curState; }
            set
            {
                _curState = value;
                RaisePropertyChanged("CurRunState");
            }
        }

        public ControlStateMode CurControlMode
        {
            get { return _curControlState; }
            set { _curControlState = value; }
        }



        public OperateResult Pause()
        {
            CurRunState = RunStateMode.Pause;
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Reset()
        {
            CurRunState = RunStateMode.Run;
            _pauseEvent.Set();
            ReleaseOrderAmountSem("复位释放信号");
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Stop()
        {
            return OperateResult.CreateSuccessResult();
        }


        private Pool<INotifyAttributeChange> attributeListener = new Pool<INotifyAttributeChange>();

        /// <summary>
        /// 注册属性变化
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterAttributeListener(INotifyAttributeChange listener)
        {
            attributeListener.AddPool(listener);
        }

        /// <summary>
        /// 解注册属性变化
        /// </summary>
        /// <param name="listener"></param>
        public void UnRegisterAttributeListener(INotifyAttributeChange listener)
        {
            attributeListener.RemovePool(listener);
        }

        /// <summary>
        /// 通知属性变化
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="attributeName">属性名</param>
        /// <param name="NewValue">属性新值</param>
        public void NotifyAttributeChange(string attributeName, object NewValue)
        {
            lock (attributeListener)
            {
                foreach (INotifyAttributeChange listener in attributeListener.Container)
                {
                    listener.NotifyAttributeChange(attributeName, NewValue);
                }
            }
        }


        private bool IsInvalidOrder(ExOrder exOrder)
        {
            return false;
        }

        private OperateResult IsRepeatOrder(ExOrder exOrder)
        {
            int orderId = exOrder.OrderId;

            int temp = _managedDataPool.Count(it =>it.Status!=StatusEnum.Discard && it.Status != StatusEnum.Cancle && it.DocumentCode == exOrder.DocumentCode && it.PileNo == exOrder.PileNo);

            if (temp > 0)
            {
                string msg = @"在某指令完成之前，MS又下发了一条相同指令Id的命令" + "\r\n该指令Id为：" + orderId + "\r\n该指令希望搬运的垛号为为：" + exOrder.PileNo + "\r\n该指令希望到达的目的地址为：" + exOrder.DestAddr + "\r\n原来的指令希望搬运的垛号为为：" + exOrder.PileNo + "\r\n原来的指令希望到达的目的地址为：" + exOrder.DestAddr;
                return OperateResult.CreateSuccessResult(msg);
            }
            return OperateResult.CreateFailedResult();
        }


        /// <summary>
        /// 创建指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult<Order> GenerateOrder(Order destOrder)
        {
            OperateResult<Order> generateOrderResult = _orderGeneraterHandler.GenerateOrder(destOrder);
            if (!generateOrderResult.IsSuccess)
            {
                return generateOrderResult;
            }
            OperateResult acceptResult = AcceptOrder(generateOrderResult.Content);
            if (!acceptResult.IsSuccess)
            {
                return OperateResult.CreateFailedResult<Order>(null, acceptResult.Message);
            }
            return OperateResult.CreateSuccessResult(generateOrderResult.Content);
        }

        /// <summary>
        /// 获取历史数据
        /// </summary>
        /// <returns></returns>
        public List<ExOrder> GetHisData(int pageIndex,int pageSize,string S_OrderID, string S_PackNum, string S_CurAddress, string S_NextAddress, string S_OrderStatu, string S_OrderType, string s_OrderAddStartTime, string s_OrderAddEndTime,out int totalCount)
        {
            List<ExOrder> exOrderlst = _orderDatabaseHandler.GetHisData(pageIndex,pageSize,S_OrderID, S_PackNum, S_CurAddress, S_NextAddress, S_OrderStatu, S_OrderType, s_OrderAddStartTime, s_OrderAddEndTime,out totalCount);

            return exOrderlst;
        }


        /// <summary>
        /// 获取搬运指令 数据
        /// </summary>
        /// <returns></returns>
        public List<ExOrder> GetTransportData(string S_OrderID, string S_PackNum, string S_CurAddress, string S_NextAddress, string S_OrderStatu, string S_OrderType, string s_OrderAddStartTime, string s_OrderAddEndTime)
        {
            List<ExOrder> exOrderlst = _orderDatabaseHandler.GetTransportData(S_OrderID, S_PackNum, S_CurAddress, S_NextAddress, S_OrderStatu, S_OrderType, s_OrderAddStartTime, s_OrderAddEndTime);

            return exOrderlst;
        }


        public OperateResult HandleRestoreData()
        {
            OperateResult restoreResult = Restore();
            if (!restoreResult.IsSuccess)
            {
                return restoreResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult RemoveOrder(ExOrder order)
        {
            return _managedDataPool.RemovePool(order);
        }


        public DataObservablePool<ExOrder> GetAllUnAllocatedOrder()
        {
            DataObservablePool<ExOrder> dataPool = new DataObservablePool<ExOrder>();
            IEnumerable<ExOrder> dataPoolContainer = _managedDataPool.FindAllData(o => o.FinishType.Equals(FinishType.UnFinished));
            foreach (ExOrder exOrder in dataPoolContainer)
            {
                dataPool.DataPool.Add(exOrder);
            }
            return dataPool;
        }

        public DataObservablePool<ExOrder> GetAllUnFinishedOrderByOwnerId(int ownerId)
        {
            DataObservablePool<ExOrder> dataPool = new DataObservablePool<ExOrder>();
            IEnumerable<ExOrder> dataPoolContainer = _managedDataPool.FindAllData(o => o.CurHandlerId.Equals(ownerId));
            foreach (ExOrder exOrder in dataPoolContainer)
            {
                dataPool.DataPool.Add(exOrder);
            }
            return dataPool;
        }
    }
}

