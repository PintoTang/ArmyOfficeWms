using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Service.ThreadHandle;
using CLDC.CLWS.CLWCS.UpperService.Communicate;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.View;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.OrderHandle;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.Xml;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness
{
    /// <summary>
    /// 上层服务接口调用虚拟类
    /// </summary>
    public abstract class UpperServiceBusinessAbstract : ViewModelBase, IServiceHandle, IStateControl, IRestore, IManageable, IHandleOrderExcuteStatus
    {
        //1.接收上层接口调用信息
        //2.处理上层接口调用信息
        //3.返回上层接口调用的结果
        //4.处理调用完成的上层接口信息

        public OperateResult Initilize()
        {
            TaskOrderDataHandle = DependencyHelper.GetService<TaskOrderDataAbstract>();

            RegisterOrderHandle();

            this._notifyDatabaseHandler = DependencyHelper.GetService<UpperInterfaceDataAbstract>();
            OperateResult initilizeResult = ParticularInitilize();

            if (!initilizeResult.IsSuccess)
            {
                return initilizeResult;
            }
            OperateResult initilizeCommunicateResult = InitilizeCommunicate();
            if (!initilizeCommunicateResult.IsSuccess)
            {
                return initilizeCommunicateResult;
            }
            OperateResult startResult = Start();
            if (!startResult.IsSuccess)
            {
                return startResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        private void RegisterOrderHandle()
        {
            IOrderNotifyCentre orderNotifyCentre = DependencyHelper.GetService<IOrderNotifyCentre>();
            if (orderNotifyCentre != null)
            {
                orderNotifyCentre.RegisterOrderStatusListener(this);
            }
        }

        public virtual OperateResult InitilizeCommunicate()
        {
            OperateResult initilizeConfigResult = OperateResult.CreateFailedResult();
            try
            {
                string fileName = "Config/UpperServiceConfig.xml";
                string path = "Communication";
                var config = new XmlOperator(fileName);
                XmlNode xmlElement = config.GetXmlElement("UpperService", "Id", Id.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);

                if (xmlNode == null)
                {
                    initilizeConfigResult.Message = string.Format("通过上层服务配置：{0} 获取配置失败", Id);
                    initilizeConfigResult.IsSuccess = false;
                    return initilizeConfigResult;
                }

                string businessConfigXml = xmlNode.OuterXml;

                WebClientCommunicationProperty webserviceConfig = null;
                using (StringReader sr = new StringReader(businessConfigXml))
                {
                    try
                    {
                        webserviceConfig =
                            (WebClientCommunicationProperty)
                                XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebClientCommunicationProperty));
                    }
                    catch (Exception ex)
                    {
                        initilizeConfigResult.IsSuccess = false;
                        initilizeConfigResult.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (webserviceConfig == null)
                {
                    return initilizeConfigResult;
                }

                MethodExcute communicate = (MethodExcute)Assembly.Load(webserviceConfig.NameSpace)
                 .CreateInstance(webserviceConfig.NameSpace + "." + webserviceConfig.ClassName);
                if (communicate == null)
                {
                    initilizeConfigResult.IsSuccess = false;
                    initilizeConfigResult.Message = string.Format("UpperServiceBusinessAbstract:{2} 命名空间：{0} 类名：{1} 反射出错", webserviceConfig.NameSpace, webserviceConfig.ClassName, webserviceConfig.Name);
                    return initilizeConfigResult;
                }
                communicate.Name = webserviceConfig.Name;
                communicate.NameSpace = webserviceConfig.NameSpace;
                communicate.ClassName = webserviceConfig.ClassName;


                OperateResult initilizeResult = communicate.Initialize(webserviceConfig.Config.Http, webserviceConfig.Config.TimeOut, webserviceConfig.CommunicationMode);
                if (!initilizeResult.IsSuccess)
                {
                    initilizeConfigResult.IsSuccess = false;
                    initilizeConfigResult.Message = initilizeResult.Message;
                    return initilizeConfigResult;
                }

                UpperService = communicate;

                initilizeConfigResult.IsSuccess = true;
                return initilizeConfigResult;

            }
            catch (Exception ex)
            {
                initilizeConfigResult.IsSuccess = false;
                initilizeConfigResult.Message = OperateResult.ConvertException(ex);
            }
            return initilizeConfigResult;
        }

        public abstract OperateResult ParticularInitilize();
        protected TaskOrderDataAbstract TaskOrderDataHandle;

        private ThreadHandleProcess _serviceThreadHandler;
        private UpperInterfaceDataAbstract _notifyDatabaseHandler;

        public MethodExcute UpperService { get; set; }

        private Pool<NotifyElement> _unFinishedNotifyElementDataPool = new Pool<NotifyElement>();

        /// <summary>
        /// 待调用的接口数据池
        /// </summary>
        public Pool<NotifyElement> UnFinishedNotifyElementDataPool
        {
            get { return _unFinishedNotifyElementDataPool; }
            set { _unFinishedNotifyElementDataPool = value; }
        }



        /// <summary>
        /// 同步调用接口
        /// </summary>
        /// <param name="notifyElement"></param>
        /// <returns></returns>
        public OperateResult<object> Invoke(NotifyElement notifyElement)
        {
            OperateResult<object> invokeResult = new OperateResult<object>();

            if (notifyElement == null)
            {
                invokeResult.IsSuccess = false;
                invokeResult.Message = "输入参数为空";
                return invokeResult;
            }
            notifyElement.InovkeResult = InvokeStatusMode.UnInvoke;
            notifyElement.FirstInvokeDatetime = DateTime.Now;
            _notifyDatabaseHandler.SaveAsync(notifyElement.ConverToDatabaseMode());
            try
            {
                if (this.UpperService != null)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    invokeResult = UpperService.Invoke(notifyElement);
                    watch.Stop();
                    long consume = watch.ElapsedMilliseconds;
                    notifyElement.InvokeDelay = consume;
                    notifyElement.Message = invokeResult.ToString();
                    notifyElement.InvokeTime++;
                    if (!invokeResult.IsSuccess)
                    {
                        notifyElement.InovkeResult = InvokeStatusMode.Failed;
                        notifyElement.Result = InvokeResultMode.Failed;
                        return invokeResult;
                    }
                    notifyElement.Result = InvokeResultMode.Success;
                    notifyElement.InovkeResult = InvokeStatusMode.Success;
                }
                else
                {
                    invokeResult = OperateResult.CreateFailedResult<object>(null, "上层接口协议尚未注册");
                }
            }
            catch (Exception ex)
            {
                notifyElement.InovkeResult = InvokeStatusMode.Exception;
                notifyElement.Result = InvokeResultMode.Failed;
                invokeResult = OperateResult.CreateFailedResult<object>(null, string.Format("调用发生异常：{0}", OperateResult.ConvertExMessage(ex)));
            }
            finally
            {
                notifyElement.InvokeFinishDatetime = DateTime.Now;
                _notifyDatabaseHandler.SaveAsync(notifyElement.ConverToDatabaseMode());
            }
            return invokeResult;
        }


        /// <summary>
        /// 异步调用接口
        /// </summary>
        /// <param name="notifyElement"></param>
        /// <returns></returns>
        public OperateResult BeginInvoke(NotifyElement notifyElement)
        {
            _notifyDatabaseHandler.SaveAsync(notifyElement.ConverToDatabaseMode());
            return Receive(notifyElement);
        }

        private OperateResult Receive(NotifyElement notifyElement)
        {
            lock (_unFinishedNotifyElementDataPool)
            {
                OperateResult addResult = _unFinishedNotifyElementDataPool.AddPool(notifyElement);
                if (addResult.IsSuccess)
                {
                    _notifyAmountSem.Release();
                }
                return addResult;
            }
        }


        private readonly AutoResetEvent _pauseEvent = new AutoResetEvent(false);

        private readonly Semaphore _notifyAmountSem = new Semaphore(0, 0x7FFFFFFF);



        private void ThreadHandle()
        {
            //1.判断业务处理的运行状态，运行、暂停、停止
            //2.运行时判断等待调用接口集合情况
            //3.根据等待调用接口集合的情况和命令处理信号进行命令处理
            //4.根据接口调用的情况 成功 失败 异常 处理对应的业务
            while (_serviceThreadHandler.IsContinuous)
            {
                try
                {
                    _notifyAmountSem.WaitOne();
                    if (CurRunState.Equals(RunStateMode.Stop))
                    {
                        return;
                    }
                    if (_unFinishedNotifyElementDataPool.Count <= 0)
                    {
                        continue;
                    }
                    if (CurRunState.Equals(RunStateMode.Pause))
                    {
                        _pauseEvent.WaitOne();
                    }
                    //1.获取一条调用信息
                    //2.进行调用
                    //3.返回调用的情况
                    //4.根据调用的情况执行回调信息
                    NotifyElement currentNotify = ObtainNotifyElement();
                    if (currentNotify == default(NotifyElement))
                    {
                        _notifyAmountSem.Release();
                        continue;
                    }

                    //操过调用次数不进行调用
                    if (currentNotify.InvokeTime >= currentNotify.MaxTime)
                    {
                        _unFinishedNotifyElementDataPool.RemovePool(currentNotify);
                        _notifyAmountSem.Release();
                        continue;
                    }

                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    OperateResult<object> invokeResult = UpperService.Invoke(currentNotify);
                    watch.Stop();
                    currentNotify.InvokeDelay = watch.ElapsedMilliseconds;
                    currentNotify.InvokeTime++;
                    if (!invokeResult.IsSuccess)
                    {
                        if (currentNotify.InvokeTime > currentNotify.MaxTime)
                        {
                            currentNotify.InovkeResult = InvokeStatusMode.Failed;   
                            _unFinishedNotifyElementDataPool.RemovePool(currentNotify);
                        }
                        else
                        {
                            currentNotify.InovkeResult = InvokeStatusMode.Process;   
                        }
                        currentNotify.Message = invokeResult.Message;
                        LogMessage(string.Format("调用接口:{0} 失败，调用的参数：{1} \r\n  失败的原因：{2}", currentNotify.MethodName, currentNotify.ParameterToString(), invokeResult.Message), EnumLogLevel.Warning, true);
                        _notifyDatabaseHandler.SaveAsync(currentNotify.ConverToDatabaseMode());
                        continue;
                    }
                    currentNotify.Message = invokeResult.Content.ToString();
                    currentNotify.InvokeFinishDatetime = DateTime.Now;
                    currentNotify.Result = InvokeResultMode.Success;
                    currentNotify.InovkeResult = InvokeStatusMode.Success;
                    LogMessage(string.Format("调用接口:{0} 成功 \r\n 调用的参数：{1} ", currentNotify.MethodName, currentNotify.ParameterToString()), EnumLogLevel.Info, true);
                    _notifyDatabaseHandler.SaveAsync(currentNotify.ConverToDatabaseMode());
                    if (currentNotify.CallBackFunc == null)
                    {
                        _unFinishedNotifyElementDataPool.RemovePool(currentNotify);
                        LogMessage(string.Format("接口：{0} 没有注册回调函数", currentNotify.MethodName), EnumLogLevel.Debug, false);
                        continue;
                    }
                    OperateResult callBackResult = currentNotify.CallBackFunc(invokeResult, invokeResult.Content);
                    if (!callBackResult.IsSuccess)
                    {
                        LogMessage(string.Format("接口：{0} 执行回调函数失败  参数：{1} \r\n 失败原因：{2}", currentNotify.MethodName, currentNotify.ParameterToString(), callBackResult.Message), EnumLogLevel.Warning, false);
                    }
                    LogMessage(string.Format("接口：{0} 执行回调函数成功", currentNotify.MethodName), EnumLogLevel.Info, false);
                    _unFinishedNotifyElementDataPool.RemovePool(currentNotify);

                }
                catch (ThreadAbortException abort)
                {
                    LogMessage(string.Format("接口调用发生异常，异常信息：{0}", abort.StackTrace), EnumLogLevel.Error, false);
                }
                finally
                {
                    if (_unFinishedNotifyElementDataPool.Count > 0)
                    {
                        _notifyAmountSem.Release();
                    }
                }
            }
        }

        private NotifyElement ObtainNotifyElement()
        {
            lock (_unFinishedNotifyElementDataPool)
            {
                if (_unFinishedNotifyElementDataPool.Count > 0)
                {
                    return _unFinishedNotifyElementDataPool.Container.OrderBy(n => n.InvokeTime).FirstOrDefault();
                }
                else
                {
                    return default(NotifyElement);
                }
            }
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
            LogHelper.WriteLog(Name, msg, level);
            if (isNotifyUi)
            {
                if (NotifyMsgToUiEvent != null)
                {
                    NotifyMsgToUiEvent(msg, level);
                }
            }
        }


        private RunStateMode _curState = RunStateMode.Pause;
        private ControlStateMode _curControlState = ControlStateMode.Auto;

        /// <summary>
        /// 当前运行状态
        /// </summary>
        public RunStateMode CurRunState
        {
            get { return _curState; }
            set
            {
                _curState = value;
                RaisePropertyChanged();
            }
        }

        public ControlStateMode CurControlMode
        {
            get { return _curControlState; }
            set
            {
                _curControlState = value;
                RaisePropertyChanged();
            }
        }

        public OperateResult Run()
        {
            _pauseEvent.Set();
            CurRunState = RunStateMode.Run;
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public OperateResult Pause()
        {

            CurRunState = RunStateMode.Pause;


            return OperateResult.CreateSuccessResult();
        }



        /// <summary>
        /// 复位
        /// </summary>
        /// <returns></returns>
        public OperateResult Reset()
        {
            CurRunState = RunStateMode.Run;
            _pauseEvent.Set();
            _notifyAmountSem.Release();
            return OperateResult.CreateSuccessResult();
        }

        public OperateResult Restore()
        {
            List<InvokeStatusMode> statusLst = new List<InvokeStatusMode> { InvokeStatusMode.UnInvoke, InvokeStatusMode.Process };
            OperateResult<List<UpperInterfaceInvoke>> getResult = _notifyDatabaseHandler.GetNotifyElement(statusLst);
            if (!getResult.IsSuccess)
            {
                string msg = string.Format("断点恢复失败 原因：{0}", getResult.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            List<UpperInterfaceInvoke> upperInterfaceInvoke = getResult.Content;
            foreach (UpperInterfaceInvoke invoke in upperInterfaceInvoke)
            {
                if (invoke.InvokeTime>=invoke.MaxTime)
                {
                    continue;
                }
                NotifyElement notify = invoke.ConverToNotifyElement();
                OperateResult result = Receive(notify);
                if (!result.IsSuccess)
                {
                    string msg = string.Format("断点恢复失败：{0} 原因：{1}", invoke.MethodName, result.Message);
                    result.Message = msg;
                    return result;
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public OperateResult Stop()
        {
            CurRunState = RunStateMode.Stop;
            return OperateResult.CreateSuccessResult();
        }

        private const string UpperManageThreadName = "上层接口调用线程";


        public OperateResult Start()
        {
            OperateResult result = Restore();
            if (!result.IsSuccess)
            {
                return result;
            }
            OperateResult runResult = Run();
            if (!runResult.IsSuccess)
            {
                return runResult;
            }
            _serviceThreadHandler = ThreadHandleManage.CreateNewThreadHandle(UpperManageThreadName, ThreadHandle);
            _serviceThreadHandler.ThreadStopAction += StopThread;
            return _serviceThreadHandler.Start();
        }

        private void StopThread(string msg)
        {
            _notifyAmountSem.Release();
        }

        public OperateResult HandleRestoreData()
        {
            return OperateResult.CreateSuccessResult();
        }

        public virtual UserControl GetDetailView()
        {
            UserControl serviceHandleView = new ServiceBusinessHandleView();
            serviceHandleView.DataContext = new ServiceBusinessHandleViewModel(this);
            return serviceHandleView;
        }

        public abstract Window GetAssistantView();

        public int Id { get; set; }
        public string Name { get; set; }

        public string NameSpace { get; set; }

        public string ClassName { get; set; }

        public UpperSystemEnum SystemName { get; set; }

        public OperateResult HandleOrderChange(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type)
        {
            switch (type)
            {
                case TaskHandleResultEnum.Finish:
                case TaskHandleResultEnum.ForceFinish:
                    return OrderFinishHandler(order);
                case TaskHandleResultEnum.Discard:
                    break;
                case TaskHandleResultEnum.Cancle:
                    break;
                case TaskHandleResultEnum.Update:
                    break;
            }
            return OperateResult.CreateSuccessResult();
        }

        protected abstract OperateResult OrderFinishHandler(ExOrder order);
    }
}
