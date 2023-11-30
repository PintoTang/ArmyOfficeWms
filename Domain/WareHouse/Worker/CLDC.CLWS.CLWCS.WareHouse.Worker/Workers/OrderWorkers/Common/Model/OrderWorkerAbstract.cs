using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.ThreadHandle;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.TaskHandleCenter;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.Common.Model
{
    /// <summary>
    /// 工作者的基类 主要通过设备的集合完成一系列的指令动作
    /// </summary>
    public abstract class OrderWorkerAbstract<TWorkerBusiness> : WorkerBaseAbstract, IOrderReceive, IStateControl, INotifyTaskStatus<TransportMessage>, IHandleTaskExcuteStatus<TransportMessage> where TWorkerBusiness : OrderWorkerBuinessAbstract
    {
        #region 事件

        #endregion


        #region 属性
    

        public int ReceiverId
        {
            get
            {
                return this.Id;
            }
            set { ;}
        }

        private OrderWorkerProperty _workerProperty = new OrderWorkerProperty();

        public OrderWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }


        private WorkStateMode curWorkState = WorkStateMode.Free;

        /// <summary>
        /// 工作状态
        /// </summary>
        public WorkStateMode CurWorkState
        {
            get { return curWorkState; }
            set
            {
                if (curWorkState != value)
                {
                    curWorkState = value;
                    RaisePropertyChanged("CurWorkState");
                }
            }
        }



        /// <summary>
        /// 判断工作者是否满负荷
        /// </summary>
        public bool IsFull
        {
            get { return UnFinishedOrderPool.FindAllData(t=>t.Status!=StatusEnum.Discard && t.Status!=StatusEnum.Cancle).ToList().Count >= WorkSize; }
        }

        private ThreadHandleProcess _orderThreadHandle;


        private DataObservablePool<ExOrder> _unFinishedOrderPool = new DataObservablePool<ExOrder>();

        /// <summary>
        /// 等待完成的指令
        /// </summary>
        public DataObservablePool<ExOrder> UnFinishedOrderPool
        {
            get { return _unFinishedOrderPool; }
            set
            {
                _unFinishedOrderPool = value;
                RefreshWorkerState();
            }
        }

        protected LiveStatusData WorkerLiveData = new LiveStatusData();

        protected override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig; 
                XmlNode xmlNode = doc.GetXmlNode("Coordination", "Id", Id.ToString());

                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string workerConfigXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(workerConfigXml))
                {
                    try
                    {
                        WorkerProperty = (OrderWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(OrderWorkerProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：OrderWorkerConfigProperty", workerConfigXml));
                }

                foreach (PrefixsItem addrPrefix in WorkerProperty.Config.AddrPrefixs.AddrPrefixsList)
                {
                    if (addrPrefix.Type.Equals("In"))
                    {
                        string[] addrLst = addrPrefix.Value.Trim().Split('|');
                        for (int i = 0; i < addrLst.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(addrLst[i]))
                            {
                                continue;
                            }
                            AddInSrcAddrPrefixList(addrLst[i]);
                        }
                    }
                    else if (addrPrefix.Type.Equals("Out"))
                    {
                        string[] addrLst = addrPrefix.Value.Trim().Split('|');
                        for (int i = 0; i < addrLst.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(addrLst[i]))
                            {
                                continue;
                            }
                            AddOutSrcAddrPrefixList(addrLst[i]);
                        }
                    }
                    else
                    {
                        string[] addrLst = addrPrefix.Value.Trim().Split('|');
                        for (int i = 0; i < addrLst.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(addrLst[i]))
                            {
                                continue;
                            }
                            AddMoveSrcAddrPrefixList(addrLst[i].Trim());
                        }
                    }
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public abstract OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> type);


        public override OperateResult GetWorkerRealStatus()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult<LiveStatusData> workerData = WorkerLiveDataHandler.GetDeviceLiveDataByDeviceId(this.Id);
                if (workerData.IsSuccess)
                {
                    LiveStatusData liveData = workerData.Content;
                    WorkerLiveData = liveData;
                    CurRunState = (RunStateMode)liveData.RunState;
                    CurDispatchState = (DispatchState)liveData.DispatchState;
                    CurControlMode = (ControlStateMode)liveData.ControlState;
                    CurUseState = (UseStateMode)liveData.UseState;
                    if (CurRunState.Equals(RunStateMode.Run))
                    {
                        Run();
                    }
                    else if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        Pause();
                    }
                    result.IsSuccess = true;
                }
                else
                {
                    WorkerLiveData.Alias = Name;
                    WorkerLiveData.ControlState = (int)CurControlMode;
                    WorkerLiveData.DispatchState = (int)CurDispatchState;
                    WorkerLiveData.Id = Id;
                    WorkerLiveData.Name = WorkerName.FullName;
                    WorkerLiveData.RunState = (int)CurRunState;
                    WorkerLiveData.UseState = (int)CurUseState;
                    string msg = string.Format("数据库不存在：{0} {1} 的实时状态记录，将重新插入", Id, Name);
                    LogMessage(msg, EnumLogLevel.Info, false);
                    OperateResult insertResult = WorkerLiveDataHandler.Save(WorkerLiveData);
                    return insertResult;
                }
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public override OperateResult GetWorkerRealData()
        {

            #region 恢复本工作者的未完成指令
            UnFinishedOrderPool = OrderManageHandler.GetAllUnFinishedOrderByOwnerId(Id);
            foreach (ExOrder order in UnFinishedOrderPool.DataPool)
            {
                string msg = string.Format("断点恢复指令：{0}", order);
                LogMessage(msg, EnumLogLevel.Debug, false);
            }
            #endregion
            ReleaseOrderAmountSem("断点恢复");
            return OperateResult.CreateSuccessResult();
        }

        protected virtual ExOrder ObtainOrder()
        {
            lock (UnFinishedOrderPool)
            {
                IEnumerable<ExOrder> orderlyOrder = UnFinishedOrderPool.DataPool.Where(o => o.Status.Equals(StatusEnum.Processing))
                    .OrderByDescending(o => o.OrderPriority)
                    .ThenBy(o => o.AllocateTime);
                return orderlyOrder.FirstOrDefault();
            }
        }

        protected LiveStatusAbstract WorkerLiveDataHandler { get; set; }

        protected IOrderManage OrderManageHandler { get; set; }

        public OperateResult RemoveUnfinishedOrder(ExOrder order)
        {
            OperateResult result = UnFinishedOrderPool.RemovePool(o => o.OrderId.Equals(order.OrderId));
            RefreshWorkerState();
            if (!result.IsSuccess)
            {
                return result;
            }
            return result;
        }

        /// <summary>
        /// 接收入库指令前缀
        /// </summary>
        private List<string> _inSrcAddrPrefixList = new List<string>();
        /// <summary>
        /// 接收出库指令前缀
        /// </summary>
        private List<string> _outSrcAddrPrefixList = new List<string>();
        /// <summary>
        /// 接收移库指令前缀
        /// </summary>
        private List<string> _moveSrcAddrPrefixList = new List<string>();

        /// <summary>
        /// 添加入库指令前缀
        /// </summary>
        /// <param name="addrPrefix"></param>
        /// <returns></returns>
        public bool AddInSrcAddrPrefixList(string addrPrefix)
        {
            lock (InSrcAddrPrefixList)
            {
                if (InSrcAddrPrefixList != null)
                {

                    if (InSrcAddrPrefixList.Contains(addrPrefix))
                    {
                        return true;
                    }
                    InSrcAddrPrefixList.Add(addrPrefix);
                    return true;
                }
                InSrcAddrPrefixList = new List<string> { addrPrefix };
                return true;
            }
        }

        /// <summary>
        /// 移除入库指令前缀地址集合
        /// </summary>
        /// <param name="addrPrefix"></param>
        /// <returns></returns>
        public bool RemoveInSrcAddrPrefixList(string addrPrefix)
        {
            lock (InSrcAddrPrefixList)
            {
                if (InSrcAddrPrefixList == null)
                {
                    return false;
                }
                lock (InSrcAddrPrefixList)
                {
                    if (InSrcAddrPrefixList.Contains(addrPrefix))
                    {
                        return InSrcAddrPrefixList.Remove(addrPrefix);
                    }
                    return true;
                }
            }
        }


        /// <summary>
        /// 添加入库指令前缀
        /// </summary>
        /// <param name="addrPrefix"></param>
        /// <returns></returns>
        public bool AddOutSrcAddrPrefixList(string addrPrefix)
        {
            lock (OutSrcAddrPrefixList)
            {
                if (OutSrcAddrPrefixList != null)
                {

                    if (OutSrcAddrPrefixList.Contains(addrPrefix))
                    {
                        return true;
                    }
                    OutSrcAddrPrefixList.Add(addrPrefix);
                    return true;
                }
                OutSrcAddrPrefixList = new List<string> { addrPrefix };
                return true;
            }
        }

        /// <summary>
        /// 移除入库指令前缀地址集合
        /// </summary>
        /// <param name="addrPrefix"></param>
        /// <returns></returns>
        public bool RemoveOutSrcAddrPrefixList(string addrPrefix)
        {
            lock (OutSrcAddrPrefixList)
            {
                if (OutSrcAddrPrefixList == null)
                {
                    return false;
                }
                lock (OutSrcAddrPrefixList)
                {
                    if (OutSrcAddrPrefixList.Contains(addrPrefix))
                    {
                        return OutSrcAddrPrefixList.Remove(addrPrefix);
                    }
                    return true;
                }
            }
        }


        /// <summary>
        /// 添加入库指令前缀
        /// </summary>
        /// <param name="addrPrefix"></param>
        /// <returns></returns>
        public bool AddMoveSrcAddrPrefixList(string addrPrefix)
        {
            lock (MoveSrcAddrPrefixList)
            {
                if (MoveSrcAddrPrefixList != null)
                {

                    if (MoveSrcAddrPrefixList.Contains(addrPrefix))
                    {
                        return true;
                    }
                    MoveSrcAddrPrefixList.Add(addrPrefix);
                    return true;
                }
                MoveSrcAddrPrefixList = new List<string> { addrPrefix };
                return true;
            }
        }



        /// <summary>
        /// 移除入库指令前缀地址集合
        /// </summary>
        /// <param name="addrPrefix"></param>
        /// <returns></returns>
        public bool RemoveMoveSrcAddrPrefixList(string addrPrefix)
        {
            lock (MoveSrcAddrPrefixList)
            {
                if (MoveSrcAddrPrefixList == null)
                {
                    return false;
                }
                lock (MoveSrcAddrPrefixList)
                {
                    if (MoveSrcAddrPrefixList.Contains(addrPrefix))
                    {
                        return MoveSrcAddrPrefixList.Remove(addrPrefix);
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// 是否可以接收指令
        /// </summary>
        /// <returns></returns>
        public OperateResult IsCanRecieveOrder(ExOrder order)
        {
            if (!CurRunState.Equals(RunStateMode.Run))
            {
                return OperateResult.CreateFailedResult(string.Format("根据指令：{0} 指令处理者不处于运行状态", order), 1);
            }
            if (!CurControlMode.Equals(ControlStateMode.Auto))
            {
                return OperateResult.CreateFailedResult(string.Format("根据指令：{0} 指令处理者不处于自动运行状态", order), 1);
            }
            if (!CurDispatchState.Equals(DispatchState.On))
            {
                return OperateResult.CreateFailedResult(string.Format("根据指令：{0} 指令处理者不处于调度状态", order), 1);
            }
            if (IsFull)
            {
                return OperateResult.CreateFailedResult(string.Format("根据指令：{0} 指令处理者正在满负荷运行中", order), 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        protected TWorkerBusiness WorkerBusiness { get; set; }

        /// <summary>
        /// 释放指令处理信号量
        /// </summary>
        /// <param name="reason"></param>
        public virtual void ReleaseOrderAmountSem(string reason)
        {
            LogMessage(reason, EnumLogLevel.Info, false);
            _orderAmountSem.Release();
        }


        private readonly AutoResetEvent _pauseEvent = new AutoResetEvent(false);

        private readonly Semaphore _orderAmountSem = new Semaphore(0, 0x7FFFFFFF);





        #endregion


        #region 虚方法

        /// <summary>
        /// <para>Description: 获取下一阶段的目的地址</para>
        /// <para>Return-----: 不为null时：代表下一阶段的目的地址，格式必须满足协议规定</para>
        /// <para>-----------# 为null时：代表无法从order中解析出下一阶段的目的地址</para>
        /// <para>Exception--: 该方法的具体实现不允许抛异常</para>
        /// </summary>
        /// <param></param>
        /// <param name="assistants"></param>
        /// <param name="order">处理的指令</param>
        public virtual OperateResult<TransportMessage> ComputeTransportMessage(List<AssistantDevice> assistants, ExOrder order)
        {
            return WorkerBusiness.ComputeTransportMessage(assistants, order);
        }


        public override OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business)
        {
            WorkerLiveDataHandler = DependencyHelper.GetService<LiveStatusAbstract>();
            OrderManageHandler = DependencyHelper.GetService<IOrderManage>();
            WorkerBusiness = business as TWorkerBusiness;
            this.Id = id;
            if (WorkerBusiness == null)
            {
                return OperateResult.CreateFailedResult(string.Format("工作者业务类型转换出错，期望类型：OrderWorkerBuinessAbstract 传入类型：{0}", business.GetType().FullName), 1);
            }

            _orderThreadHandle = ThreadHandleManage.CreateNewThreadHandle(this.Name + ThreadHandleName, ThreadHandle);
            _orderThreadHandle.ThreadStopAction += StopThread;

            return OperateResult.CreateSuccessResult();
        }

        private const string ThreadHandleName = "_指令处理线程";

        /// <summary>
        /// 上报指令管理指令已完成
        /// </summary>
        /// <param name="finishDevice"></param>
        /// <param name="order"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual OperateResult NotifyOrderFinish(DeviceBaseAbstract finishDevice, ExOrder order, TaskHandleResultEnum type)
        {
            OperateResult notifyResult = new OperateResult();
            if (OrderUpdateStatusEvent != null)
            {
                if (finishDevice!=null)
                {
                    return OrderUpdateStatusEvent(finishDevice.DeviceName, order, type);    
                }
                else
                {
                    return OrderUpdateStatusEvent(this.WorkerName,order,type);
                }
            }
            notifyResult.Message = string.Format("工作者：{0} 没有注册业务逻辑上报接口:OrderUpdateStatusEvent", Name);
            LogMessage(notifyResult.Message, EnumLogLevel.Error, true);
            return notifyResult;
        }

        /// <summary>
        /// 指令转换到下一个地址通过组件
        /// </summary>
        /// <param name="transMsg">搬运信息</param>
        /// <returns></returns>
        private OperateResult ExcuteTransportMessage(TransportMessage transMsg)
        {
            //1.指令下发前的业务处理
            //2.指令下发
            //3.指令下发后的业务处理
            OperateResult beforeBusiness = WorkerBusiness.BeforeTransportBusinessHandle(transMsg);
            if (!beforeBusiness.IsSuccess)
            {
                LogMessage(beforeBusiness.Message, EnumLogLevel.Warning, true);
                return beforeBusiness;
            }
            OperateResult excuteResult = WorkerBusiness.ExcuteTransportMessage(transMsg);
            if (!excuteResult.IsSuccess)
            {
                LogMessage(excuteResult.Message, EnumLogLevel.Warning, true);
                return excuteResult;
            }


            LogMessage(excuteResult.Message, EnumLogLevel.Info, true);
            OperateResult afterBusiness = WorkerBusiness.AfterTransportBusinessHandle(transMsg);
            if (!afterBusiness.IsSuccess)
            {
                LogMessage(afterBusiness.Message, EnumLogLevel.Warning, true);
                return afterBusiness;
            }

           OperateResult clearResult= ClearUpTransport(transMsg);
           if (!clearResult.IsSuccess)
           {
               LogMessage(clearResult.Message,EnumLogLevel.Error,true);
               return clearResult;
           }
            return OperateResult.CreateSuccessResult();
        }

        #endregion

        #region 公用方法

        protected virtual OperateResult ClearUpTransport(TransportMessage transport)
        {
            //1.清除开始设备对应地址的所有设备的相关信息
            DeviceBaseAbstract startDevice = transport.StartDevice;
            if (startDevice == null)
            {
                return OperateResult.CreateFailedResult(string.Format("搬运信息 {0} 开始设备为空", transport.TransportOrderId));
            }
            List<AssistantDevice> startAssistantDevice =GetAssistantByAddress(startDevice.CurAddress);
            if (startAssistantDevice != null)
            {
                startAssistantDevice.ForEach(d => d.Device.ClearUpRunMessage(transport));
            }
            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="order">订单ExOrder</param>
        public OperateResult ReceiveOrder(ExOrder order)
        {
            //第一步判断工作者的工作状态
            //第二步判断工作者是否满负荷
            //添加到等待处理的指令列表

            if (CurRunState.Equals(RunStateMode.Stop))
            {
                //打印工作者处于停止状态不接收任务
                return OperateResult.CreateFailedResult(string.Format("{0} 处于停止状态，不能接收新指令", Name), 1);
            }
            if (IsFull)
            {
                //工作者处于满负荷状态
                return OperateResult.CreateFailedResult(string.Format("{0} 处于满负荷状态，不能接收新指令", Name), 1);
            }
            ////业务调度处理  1008 和2001
            //if (SystemConfig.Instance.WhCode.Equals("SNDL1"))
            //{
            //    if (((order.OrderType == OrderTypeEnum.Out || order.OrderType == OrderTypeEnum.PickOut))
            //        && order.Status == StatusEnum.Waiting)
            //    {
            //        //如果还有其它除本身以外的正在执行的出库 也不得执行

                  
            //        var ortherOutOrder = UnFinishedOrderPool.DataPool.Where(x => (x.OrderType == OrderTypeEnum.Out || x.OrderType == OrderTypeEnum.PickOut)
            //            && x.DestAddr.IsContain(order.DestAddr) && x.Status != StatusEnum.Waiting);
            //        if (ortherOutOrder != null && ortherOutOrder.Count() > 0)
            //        {
            //            //LogMessage("已有其它执行的出库任务，暂时不得下发指令", EnumLogLevel.Error, true);
            //            return OperateResult.CreateFailedResult(string.Format("{0} 已有其它执行的出库任务，暂时不得下发指令", Name), 1);
            //        }

            //        if (order.DestAddr.IsContain("PutGoodsPort:1_1_1"))
            //        {
            //            //1008
            //            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1008);
            //            TransportPointStation transDevice = curDevice as TransportPointStation;
            //            RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
            //            OperateResult<int> opcResult1008 = roller.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
            //            if (opcResult1008.IsSuccess)
            //            {
            //                if (opcResult1008.Content != 0)
            //                {
            //                    //有货
            //                    //LogMessage("1008当前位置有货未入库，暂时不得下发指令", EnumLogLevel.Error, true);
            //                    return OperateResult.CreateFailedResult(string.Format("{0} 1008当前位置有货未入库，暂时不得下发指令", Name), 1);
            //                }
            //                else
            //                {
            //                    //无货
            //                    //查看 指令 起始
            //                    lock (UnFinishedOrderPool)
            //                    {
            //                        var nn = UnFinishedOrderPool.DataPool.Where(x => x.OrderType == OrderTypeEnum.In && x.StartAddr.IsContain(order.DestAddr));

            //                        if (nn != null && nn.Count() > 0)
            //                        {
            //                            //有已经存在的入库指令 
            //                            //再次筛选 排  是否是同巷道的排  1-2   3-4 
            //                            int outGoodsRangeNum = order.StartAddr.Range;
            //                            if (outGoodsRangeNum == 1 || outGoodsRangeNum == 2)
            //                            {
            //                                var selectNN = nn.Where(x => x.DestAddr.Range == 1 || x.DestAddr.Range == 2);
            //                                if (selectNN != null && selectNN.Count() > 0)
            //                                {
            //                                    //有入库指令不得发
            //                                    //LogMessage("1008当前位置有货正在入库，暂时不得下发指令1", EnumLogLevel.Error, true);
            //                                    return OperateResult.CreateFailedResult(string.Format("{0} 1008当前位置有货正在入库，暂时不得下发指令1", Name), 1);
            //                                }
            //                            }
            //                            else if (outGoodsRangeNum == 3 || outGoodsRangeNum == 4)
            //                            {
            //                                var selectNN = nn.Where(x => x.DestAddr.Range == 3 || x.DestAddr.Range == 4);
            //                                if (selectNN != null && selectNN.Count() > 0)
            //                                {
            //                                    LogMessage("1008当前位置有货正在入库，暂时不得下发指令2", EnumLogLevel.Error, true);
            //                                    return OperateResult.CreateFailedResult(string.Format("{0} 1008当前位置有货正在入库，暂时不得下发指令2", Name), 1);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                //LogMessage("读取1008 包号 OPC失败", EnumLogLevel.Error, true);
            //                return OperateResult.CreateFailedResult(string.Format("{0} 读取1008 包号 OPC失败，暂时不得下发指令2", Name), 1);
            //            }
            //        }
            //        else if (order.DestAddr.IsContain("GetGoodsPort:3_1_1"))
            //        {
            //            //2001
            //            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(2001);
            //            TransportPointStation transDevice = curDevice as TransportPointStation;
            //            RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
            //            OperateResult<int> opcResult2001 = roller.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
            //            if (opcResult2001.IsSuccess)
            //            {
            //                if (opcResult2001.Content != 0)
            //                {
            //                    //有货
            //                    //LogMessage("2001当前位置有货未入库，暂时不得下发指令", EnumLogLevel.Error, true);
            //                    return OperateResult.CreateFailedResult(string.Format("{0} 2001当前位置有货未入库，暂时不得下发指令", Name), 1);
            //                }
            //                else
            //                {
            //                    //无货
            //                    //查看 指令 起始
            //                    lock (UnFinishedOrderPool)
            //                    {
            //                        var nn = UnFinishedOrderPool.DataPool.Where(x => x.OrderType == OrderTypeEnum.In && x.StartAddr.IsContain(order.DestAddr));

            //                        if (nn != null && nn.Count() > 0)
            //                        {
            //                            //有已经存在的入库指令 
            //                            //再次筛选 排  是否是同巷道的排  1-2   3-4 
            //                            int outGoodsRangeNum = order.StartAddr.Range;
            //                            if (outGoodsRangeNum == 1 || outGoodsRangeNum == 2)
            //                            {
            //                                var selectNN = nn.Where(x => x.DestAddr.Range == 1 || x.DestAddr.Range == 2);
            //                                if (selectNN != null && selectNN.Count() > 0)
            //                                {
            //                                    //有入库指令不得发
            //                                    //LogMessage("2001当前位置有货正在入库，暂时不得下发指令1", EnumLogLevel.Error, true);
            //                                    return OperateResult.CreateFailedResult(string.Format("{0} 2001当前位置有货正在入库，暂时不得下发指令1", Name), 1);
            //                                }
            //                            }
            //                            else if (outGoodsRangeNum == 3 || outGoodsRangeNum == 4)
            //                            {
            //                                var selectNN = nn.Where(x => x.DestAddr.Range == 3 || x.DestAddr.Range == 4);
            //                                if (selectNN != null && selectNN.Count() > 0)
            //                                {
            //                                    //LogMessage("2001当前位置有货正在入库，暂时不得下发指令2", EnumLogLevel.Error, true);
            //                                    return OperateResult.CreateFailedResult(string.Format("{0} 2001当前位置有货正在入库，暂时不得下发指令2", Name), 1);
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                LogMessage("读取2001 包号 OPC失败", EnumLogLevel.Error, true);
            //                return OperateResult.CreateFailedResult(string.Format("{0} 读取2001 包号 OPC失败，暂时不得下发指令", Name), 1);
            //            }
            //        }
            //    }
            //}
            OperateResult addResult = AddUnfinishedOrder(order);
            if (addResult.IsSuccess)
            {
                string message = string.Format("成功接收到新指令:{0} ", order.OrderId);
                LogMessage(message, EnumLogLevel.Info, true);
                ReleaseOrderAmountSem(string.Format("接收到新指令：{0} 释放信号量", order));
                //打印接收任务成功，把任务信息打印
                return OperateResult.CreateSuccessResult(this);
            }
            else
            {
                //打印接收任务失败，把任务信息打印
                return OperateResult.CreateFailedResult(string.Format("{0} 添加到未完成指令列表失败,失败原因：{1}", Name, addResult.Message), 1);
            }
        }


        /// <summary>
        /// 接收入库指令前缀
        /// </summary>
        public List<string> InSrcAddrPrefixList
        {
            get { return _inSrcAddrPrefixList; }
            set
            {
                lock (_inSrcAddrPrefixList)
                {
                    _inSrcAddrPrefixList = value;
                }
            }
        }

        /// <summary>
        /// 接收出库指令前缀
        /// </summary>
        public List<string> OutSrcAddrPrefixList
        {
            get { return _outSrcAddrPrefixList; }
            set
            {
                lock (_outSrcAddrPrefixList)
                {
                    _outSrcAddrPrefixList = value;
                }
            }
        }

        /// <summary>
        /// 接收移库指令前缀
        /// </summary>
        public List<string> MoveSrcAddrPrefixList
        {
            get { return _moveSrcAddrPrefixList; }
            set
            {
                lock (_moveSrcAddrPrefixList)
                {
                    _moveSrcAddrPrefixList = value;
                }
            }
        }



        private UseStateMode _curUseState = UseStateMode.Enable;
        public UseStateMode CurUseState
        {
            get { return _curUseState; }
            set
            {
                if (_curUseState != value)
                {
                    _curUseState = value;
                    RaisePropertyChanged("CurUseState");
                }
            }
        }

        /// <summary>
        /// 调度状态
        /// </summary>
        public DispatchState CurDispatchState
        {
            get { return _curDispatchState; }
            set
            {
                if (_curDispatchState != value)
                {
                    _curDispatchState = value;
                    RaisePropertyChanged("CurDispatchState");
                }
            }
        }

        private DispatchState _curDispatchState = DispatchState.On;


        /// <summary>
        /// 添加待完成的指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private OperateResult AddUnfinishedOrder(ExOrder order)
        {
            order.Status = StatusEnum.Processing;
            order.CurHandlerId = Id;
            OperateResult addResult = UnFinishedOrderPool.AddPool(order);
            RefreshWorkerState();
            if (!addResult.IsSuccess)
            {
                return addResult;
            }
            return addResult;
        }

        /// <summary>
        /// 取消指令
        /// </summary>
        /// <param name="order">订单</param>
        /// <returns></returns>
        public OperateResult CancelOrder(ExOrder order)
        {
            OperateResult<ExOrder> finishOrder = UnFinishedOrderPool.FindData(e => e.OrderId.Equals(order.OrderId));
            if (!finishOrder.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("找不到指令编号为：{0} 的未完成指令", order.OrderId), 1);
            }
            ExOrder cancelOrder = finishOrder.Content;
            OperateResult businessCancleResult = WorkerBusiness.CancleOrder(this.WorkerName, cancelOrder);
            if (!businessCancleResult.IsSuccess)
            {
                return businessCancleResult;
            }

            cancelOrder.Status = StatusEnum.Cancle;
            cancelOrder.FinishType = FinishType.Cancle;


            NotifyOrderFinish(null, cancelOrder, TaskHandleResultEnum.Cancle);
            LogMessage(string.Format("指令编号：{0} 取消完成", cancelOrder.OrderId), EnumLogLevel.Info, true);

            RemoveUnfinishedOrder(cancelOrder);

            return businessCancleResult;
        }

        /// <summary>
        /// 指令人工强制完成
        /// </summary>
        /// <param name="exOrder"></param>
        /// <returns></returns>
        public OperateResult ForceFinishOrder(ExOrder exOrder)
        {
            OperateResult<ExOrder> finishOrder = UnFinishedOrderPool.FindData(e => e.OrderId.Equals(exOrder.OrderId));
            if (!finishOrder.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("找不到指令编号为：{0} 的未完成指令", exOrder.OrderId), 1);
            }
            ExOrder forceFinishOrder = finishOrder.Content;
            OperateResult businessFinishResult = WorkerBusiness.ForceFinishOrder(this.WorkerName, forceFinishOrder);
            if (!businessFinishResult.IsSuccess)
            {
                return businessFinishResult;
            }

            forceFinishOrder.CurrAddr = forceFinishOrder.NextAddr.Clone();
            forceFinishOrder.Status = StatusEnum.TransportCompleted;
            forceFinishOrder.FinishType = FinishType.ForceFinish;

            NotifyOrderFinish(null, forceFinishOrder, TaskHandleResultEnum.Finish);
            LogMessage(string.Format("指令编号：{0} 强制执行完成", forceFinishOrder.OrderId), EnumLogLevel.Info, true);

            RemoveUnfinishedOrder(forceFinishOrder);

            return businessFinishResult;
        }

        public override OperateResult HandleRestoreData()
        {
            OperateResult baseHandleResult = base.HandleRestoreData();
            if (!baseHandleResult.IsSuccess)
            {
                return baseHandleResult;
            }
            foreach (ExOrder order in _unFinishedOrderPool.DataPool)
            {
                if (order.Status.Equals(StatusEnum.TransportCompleted))
                {
                    NotifyOrderFinish(null, order, TaskHandleResultEnum.Finish);
                    LogMessage(string.Format("断点恢复上报指令:{0} 完成", order), EnumLogLevel.Debug, false);
                }
            }
            return OperateResult.CreateSuccessResult();
        }


        public OperateResult ReDoTransportMessage(TransportMessage transport)
        {
            return ExcuteTransportMessage(transport);
        }


        /// <summary>
        /// 结束搬运指令
        /// </summary>
        /// <param name="finishDevice"></param>
        /// <param name="transport"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public OperateResult FinishTransport(DeviceBaseAbstract finishDevice, TransportMessage transport, TaskHandleResultEnum type)
        {
            //1.指令完成前的业务
            //2.指令完成业务
            //3.指令完成后的业务
            OperateResult result = OperateResult.CreateFailedResult();
            OperateResult<ExOrder> findUnfinishedOrder = UnFinishedOrderPool.FindData(o => o.OrderId.Equals(transport.TransportOrderId));
            if (!findUnfinishedOrder.IsSuccess)
            {
                string msg = string.Format("通过搬运信息：{0} 获取待完成指令失败", transport.TransportOrderId);
                LogMessage(msg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            ExOrder finishedOrder = findUnfinishedOrder.Content;
            finishedOrder.CurrAddr = transport.DestAddr;
            finishedOrder.NextAddr = null;
            finishedOrder.CurHandlerId = Id;
            finishedOrder.Status = StatusEnum.TransportCompleted;
            OrderManageHandler.UpdateOrder(finishedOrder);

            OperateResult businessFinishResult = WorkerBusiness.FinishTransport(finishDevice.DeviceName, transport);
            if (!businessFinishResult.IsSuccess)
            {
                transport.TransportStatus = TransportResultEnum.Transported;
                LogMessage(string.Format("根据指令编号：{0} 完成业务失败,失败原因：\r\n{1}", transport.TransportOrderId, businessFinishResult.Message), EnumLogLevel.Error, true);
                result.IsSuccess = false;
            }
            OperateResult notifyUpResult = NotifyOrderFinish(finishDevice, finishedOrder, type);
            if (!notifyUpResult.IsSuccess)
            {
                LogMessage(notifyUpResult.Message, EnumLogLevel.Warning, true);
                result.IsSuccess = false;
            }
            else
            {
                result.IsSuccess = true;
            }
            string finishMsg = string.Format("处理搬运信息：{0} 完成相关业务成功,相关指令：{1}", transport.TransportOrderId, finishedOrder);
            LogMessage(finishMsg, EnumLogLevel.Info, true);
            RemoveUnfinishedOrder(finishedOrder);
            return result;
        }



        private RunStateMode _curState = RunStateMode.Run;
        private ControlStateMode _curControlState = ControlStateMode.Auto;
        /// <summary>
        /// 当前运行状态
        /// </summary>
        public RunStateMode CurRunState
        {
            get
            {
                return _curState;
            }
            set
            {
                _curState = value;
                RaisePropertyChanged("CurRunState");

            }
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <returns></returns>
        public virtual OperateResult Run()
        {
            _pauseEvent.Set();
            _curState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public virtual OperateResult Pause()
        {
            if (CurRunState.Equals(RunStateMode.Pause))
            {
                _pauseEvent.Set();
                CurRunState = RunStateMode.Run;
            }
            else
            {
                CurRunState = RunStateMode.Pause;
            }

            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public OperateResult Reset()
        {
            _curState = RunStateMode.Reset;
            CurWorkState = WorkStateMode.Free;
            ReleaseOrderAmountSem("复位释放信号");
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Start()
        {
            OperateResult startResult = _orderThreadHandle.Start();
            if (!startResult.IsSuccess)
            {
                return startResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public virtual OperateResult Stop()
        {
            _curState = RunStateMode.Stop;
            return OperateResult.CreateSuccessResult();
        }


        public ControlStateMode CurControlMode
        {
            get { return _curControlState; }
            set { _curControlState = value; }
        }

        private OperateResult RegisterOrderReceive()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                DependencyHelper.GetService<IOrderAllocator>().RegisterOrderReceiver(this);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private void StopThread(string msg)
        {
            ReleaseOrderAmountSem("系统退出");
        }

        protected override OperateResult ParticularStart()
        {

            OperateResult regesterListener = RegisterOrderStatusListener();
            if (!regesterListener.IsSuccess)
            {
                return regesterListener;
            }

            OperateResult result = RegisterOrderReceive();
            if (!result.IsSuccess)
            {
                return result;
            }
            return Start();
        }

        public virtual OperateResult RegisterOrderStatusListener()
        {
            foreach (AssistantDevice assistant in WorkerAssistants)
            {
                if (assistant.IsRegisterFinish)
                {
                    //标记-为什么要注册了IsRegisterFinish才注册异常上报事件
                    DeviceBaseForTask<TransportMessage> taskDevice = assistant.Device as DeviceBaseForTask<TransportMessage>;
                    if (taskDevice != null)
                    {
                        taskDevice.NotifyDeviceExceptionEvent += DeviceExceptionHandle;
                    }
                }

                if (assistant.IsRegisterFinish)
                {
                    ITaskNotifyCentre<TransportMessage> taskDevice = assistant.Device as ITaskNotifyCentre<TransportMessage>;
                    if (taskDevice != null)
                    {
                        taskDevice.RegisterTaskExcuteStatusListener(this);
                    }
                }

            }
            return OperateResult.CreateSuccessResult();
        }



        #endregion

        protected void ThreadHandle()
        {
            //1.判断业务处理的运行状态，运行、暂停、停止
            //2.运行时判断等待执行的指令集合情况
            //3.根据等待执行指令集合的情况和命令处理信号进行命令处理
            //4.根据指令处理的条件获取一个需要处理的指令，条件：分发的次数 分发的时间
            //5.根据指令计算下一步地址，更改指令计算下步地址的次数
            //6.把指令下发到下一个设备
            //7.指令从等待执行指令的集合中移除
            //8.添加到等待完成的指令中

            while (_orderThreadHandle.IsContinuous)
            {
                try
                {
                    CurWorkState = WorkStateMode.Free;
                    _orderAmountSem.WaitOne();
                    if (CurRunState.Equals(RunStateMode.Stop))
                    {
                        return;
                    }
                    if (UnFinishedOrderPool.Count(e => e.Status.Equals(StatusEnum.Processing)) <= 0)
                    {
                        continue;
                    }
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        _pauseEvent.WaitOne();
                    }
                    CurWorkState = WorkStateMode.Working;
                    ExOrder executeOrder = ObtainOrder();
                    OperateResult<TransportMessage> operateResult = ComputeTransportMessage(WorkerAssistants, executeOrder);
                    if (!operateResult.IsSuccess)
                    {
                        LogMessage(operateResult.Message, EnumLogLevel.Info, true);
                        ReleaseOrderAmountSem(string.Format("计算搬运信息失败，指令：{0}", executeOrder));
                        executeOrder.AllocateTime++;
                        executeOrder.AllocateFailTime++;
                        OrderManageHandler.UpdateOrder(executeOrder);
                        Thread.Sleep(5000);
                        continue;
                    }

                    TransportMessage currentTransportMsg = operateResult.Content;


                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        _pauseEvent.WaitOne();
                    }
                    executeOrder.AllocateTime++;

                    OperateResult transportResult = ExcuteTransportMessage(currentTransportMsg);
                    if (!transportResult.IsSuccess)
                    {
                        LogMessage(transportResult.Message, EnumLogLevel.Info, true);
                        executeOrder.AllocateFailTime++;
                        executeOrder.Status = StatusEnum.Processing;
                        executeOrder.FinishType = FinishType.NotifyFail;
                        ReleaseOrderAmountSem("计算下步地址失败释放信号");
                        OrderManageHandler.UpdateOrder(executeOrder);
                        Thread.Sleep(5000);
                        continue;
                    }

                    executeOrder.NextAddr = currentTransportMsg.DestAddr.Clone();
                    executeOrder.CurrAddr = currentTransportMsg.CurAddr.Clone();
                    currentTransportMsg.TransportStatus = TransportResultEnum.UnFinish;
                    executeOrder.Status = StatusEnum.NotifyOPC;
                    executeOrder.IsAllocated = true;
                    OrderManageHandler.UpdateOrder(executeOrder);
                    LogMessage(string.Format("更新指令：{0} 成功", executeOrder), EnumLogLevel.Debug, false);
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        _pauseEvent.WaitOne();
                    }
                }
                catch (ThreadAbortException abort)
                {
                    LogMessage(string.Format("工作者处理异常发生异常，异常信息：{0}", abort.StackTrace), EnumLogLevel.Error, false);
                }
                finally
                {
                    if (UnFinishedOrderPool.Count(e => e.Status.Equals(StatusEnum.Processing)) > 0)
                    {
                        //还存在等待处理的指令，接着处理
                        ReleaseOrderAmountSem("还有需要被处理的指令 释放信号");
                    }
                    Thread.Sleep(50);
                }
            }
        }
        /// <summary>
        /// 是否能取消订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual OperateResult IsCanCancelOrder(ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 更新指令
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual OperateResult UpdateOrder(ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public void UpdateTaskStatus(DeviceBaseAbstract finishDevice, TransportMessage task, TaskExcuteStepStatusEnum taskStatus)
        {
            switch (taskStatus)
            {
                case TaskExcuteStepStatusEnum.Received:
                    break;
                case TaskExcuteStepStatusEnum.Processing:
                    break;
                case TaskExcuteStepStatusEnum.Picking:
                    break;
                case TaskExcuteStepStatusEnum.Transport:
                    break;
                case TaskExcuteStepStatusEnum.Putting:
                    break;
                case TaskExcuteStepStatusEnum.Finished:
                    FinishTransportHandle(finishDevice, task);
                    break;
                case TaskExcuteStepStatusEnum.Exception:
                    break;
                default:
                    break;
            }
        }

        #region 指令完成
        private void FinishTransportHandle(DeviceBaseAbstract finishDevice, TransportMessage trasnport)
        {
            OperateResult finishResult = FinishTransport(finishDevice, trasnport, TaskHandleResultEnum.Finish);
            if (!finishResult.IsSuccess)
            {
                ///处理完成指令失败的业务

            }
            else
            {
                ///完成指令完成成功的业务
            }

        }

        #endregion
     
        public override void RefreshWorkerState()
        {
            if (UnFinishedOrderPool.Lenght>0)
            {
                IsHasTask = true;
            }
            else
            {
                IsHasTask = false;
            }

        }

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<OrderWorkerAbstract<TWorkerBusiness>, OrderWorkerProperty> viewModel = new WorkerConfigViewModel<OrderWorkerAbstract<TWorkerBusiness>, OrderWorkerProperty>(this, WorkerProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected internal override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = WorkerProperty.Name;
                this.WorkSize = WorkerProperty.WorkSize;
                this.WorkerName = new DeviceName(WorkerProperty.WorkerName);
                this.NameSpace = WorkerProperty.NameSpace;
                this.ClassName = WorkerProperty.ClassName;
                this.Id = WorkerProperty.WorkerId;

                this.WorkerBusiness.Name = WorkerProperty.BusinessHandle.Name;
                this.WorkerBusiness.ClassName = WorkerProperty.BusinessHandle.ClassName;
                this.WorkerBusiness.NameSpace = WorkerProperty.BusinessHandle.NameSpace;

                OperateResult initWorkerConfig = this.InitConfig();
                if (!initWorkerConfig.IsSuccess)
                {
                    return initWorkerConfig;
                }

                OperateResult initBusinessConfig = this.WorkerBusiness.InitConfig();
                if (!initBusinessConfig.IsSuccess)
                {
                    return initBusinessConfig;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
                result.IsSuccess = false;
            }
            return result;
        }

        public DeviceName OrderReceiverName
        {
            get { return WorkerName; }
        }

        public event OrderUpdateStatusDelegate OrderUpdateStatusEvent;

        public OperateResult HandleTaskExcuteStatus(DeviceBaseAbstract finishDevice, TransportMessage task, TaskExcuteStepStatusEnum excuteStepStatus)
        {
            #region 处理需要更新指令信息的业务
            switch (excuteStepStatus)
            {
                case TaskExcuteStepStatusEnum.Received:
                    break;
                case TaskExcuteStepStatusEnum.Processing:
                    break;
                case TaskExcuteStepStatusEnum.Picking:
                    break;
                case TaskExcuteStepStatusEnum.Transport:
                    break;
                case TaskExcuteStepStatusEnum.Putting:
                    break;
                case TaskExcuteStepStatusEnum.Finished:
                    FinishTransportHandle(finishDevice, task);
                    break;
                case TaskExcuteStepStatusEnum.Exception:
                    break;
                default:
                    break;
            }
            #endregion
            return OperateResult.CreateSuccessResult();
        }
    }
}
