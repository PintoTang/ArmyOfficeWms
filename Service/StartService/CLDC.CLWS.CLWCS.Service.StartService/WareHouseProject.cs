using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CL.Framework.OPCClientAbsPckg;
using CL.Framework.OPCClientImpPckg;
using CL.Framework.OPCClientSimulatePckg;
using CL.Framework.Testing.OPCClientImpPckg;
using CL.Framework.Testing.OPCClientImpPckg.Oracle;
using CL.Framework.Testing.OPCClientImpPckg.SqlSever;
using CL.WCS.DBHelperBuilderPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CL.WCS.OPCMonitorImpPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.SqlServer;
using Infrastructrue.Ioc.DependencyFactory;
using Infrastructrue.Ioc.Interface;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Oracle;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.SqlServer;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.OrderAllocator;
using CLDC.CLWS.CLWCS.WareHouse.OrderGenerate;
using CLDC.CLWS.CLWCS.WareHouse.OrderHandle;
using CLDC.CLWS.CLWCS.WareHouse.StringCharTaskHandle;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Manage;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WcsService;
using CLDC.Framework.Log.Helper;
using Microsoft.Practices.Unity;

namespace CLDC.CLWS.CLWCS.Service.StartService
{
    /// <summary>
    /// 创建一个立库项目
    /// </summary>
    public class WareHouseProject
    {
        private readonly IDependency _dependencyResolver;
        public WareHouseProject()
        {
            _dependencyResolver = DependencyFactory.GetDependency();
        }

        /// <summary>
        /// 记录需要Run的列表
        /// </summary>
        readonly List<IStateControl> _needStart = new List<IStateControl>();



        public Action<double, string> ReportProgress { get; set; }



        /// <summary>
        /// 指令分发
        /// </summary>
        public IOrderAllocator OrderAllocatorHandler { get; set; }

        /// <summary>
        /// 指令生成器
        /// </summary>
        public IOrderGenerate OrderGenerateHandler { get; set; }


        /// <summary>
        /// Opc监控
        /// </summary>
        public OPCMonitorAbstract OpcMonitorHandler { get; set; }





        private void StartOpcMonitor()
        {
            OpcMonitorHandler = DependencyHelper.GetService<OPCMonitorAbstract>();
            int intervalTime = SystemConfig.Instance.MonitorIntervalTime;
            int threadCount = SystemConfig.Instance.ThreadCount;
            OpcMonitorHandler.StartMonitor(threadCount, intervalTime);
        }

        private OperateResult HandleAllRestoreData()
        {



            //恢复搬运信息
            DependencyHelper.GetService<ITransportManage>().HandleRestoreData();



            foreach (WorkerBaseAbstract worker in WorkerManage.Instance.GetAllData())
            {
                worker.HandleRestoreData();
            }


            foreach (DeviceBaseAbstract device in DeviceManage.Instance.GetAllData())
            {
                device.HandleRestoreData();
            }

            return OperateResult.CreateSuccessResult();
        }


        /// <summary>
        /// 开始所有的设备及服务，等到所有设备就绪后才开启OPC监控，断点恢复需要
        /// </summary>
        /// <returns></returns>
        private OperateResult RunAll()
        {


            lock (_needStart)
            {
                foreach (IStateControl thread in _needStart)
                {
                    OperateResult runResult = thread.Start();
                    if (!runResult.IsSuccess)
                    {
                        return runResult;
                    }
                }
            }

            StartOpcMonitor();

            return OperateResult.CreateSuccessResult();
        }


        private OperateResult<OPCClientAbstract> RegisterOpcClient()
        {
            OPCClientAbstract opcClient = null;
            OperateResult<OPCClientAbstract> createResult;
            if (SystemConfig.Instance.IsTrueOPC.Equals(OpcModeEnum.Production))
            {
                try
                {
                    opcClient = new OPCRWClient();
                    opcClient.IsPrintRealTimeReadLog = SystemConfig.Instance.CurSystemConfig.IsRecordEventLog.Value;
                    createResult = OperateResult.CreateSuccessResult(opcClient);
                }
                catch (Exception ex)
                {
                    createResult = OperateResult.CreateFailedResult<OPCClientAbstract>(null, string.Format("OPC连接异常：" + OperateResult.ConvertException(ex)));
                }
                return createResult;
            }
            else if (SystemConfig.Instance.IsTrueOPC.Equals(OpcModeEnum.Manual))
            {

                OPCClientSimulateAbstract opcClientSimulateAbstract = new OPCClientSimulateFactory();
                opcClient = new OPCClientSimulate(opcClientSimulateAbstract);
                opcClient.IsPrintRealTimeReadLog = SystemConfig.Instance.CurSystemConfig.IsRecordEventLog.Value;

                createResult = OperateResult.CreateSuccessResult(opcClient);
                return createResult;
            }
            else if (SystemConfig.Instance.IsTrueOPC.Equals(OpcModeEnum.Automatic))
            {
                try
                {
                    OperateResult<IDbHelper> dbHepler = RegisterAtsTestDbHelper();
                    if (!dbHepler.IsSuccess)
                    {
                        return OperateResult.CreateFailedResult<OPCClientAbstract>(null, "自动化测试模式下，创建模拟Opc数据库连接失败：" + dbHepler.Message);
                    }
                    OpcItemAbstract opcServer = _dependencyResolver.GetService<OpcItemAbstract>();
                    opcClient = new OPCDataBaseRWClient(opcServer);
                    opcClient.IsPrintRealTimeReadLog = SystemConfig.Instance.CurSystemConfig.IsRecordEventLog.Value;
                    createResult = OperateResult.CreateSuccessResult(opcClient);
                }
                catch (Exception ex)
                {
                    createResult = OperateResult.CreateFailedResult<OPCClientAbstract>(null, string.Format("自动自测模式需要配置OPC数据库，请确认配置信息正确!：" + OperateResult.ConvertException(ex)));
                }
                return createResult;
            }
            else
            {
                createResult = OperateResult.CreateFailedResult<OPCClientAbstract>(null, "请检查App.config文件，IsTrueOPC值为T代表真实OPC，F代表仿真OPC!");
            }
            return createResult;
        }


        private OperateResult<IDbHelper> RegisterAtsTestDbHelper()
        {
            IDbHelper atsDbHelper = null;
            var atsTestConnection = SystemConfig.Instance.CurSystemConfig.AtsDatabaseConn.Value;
            var databaseType = SystemConfig.Instance.CurSystemConfig.AtsDataBaseType.Value;

            switch (databaseType)
            {
                case DatabaseTypeEnum.Oracle:
                    atsDbHelper = DBHelperBuilder.GetAtsTestOracle(atsTestConnection);
                    break;
                case DatabaseTypeEnum.SqlServer:
                    atsDbHelper = DBHelperBuilder.GetAtsTestSqlServer(atsTestConnection);
                    break;
                case DatabaseTypeEnum.MySql:
                    atsDbHelper = DBHelperBuilder.GetAtsTestMySql(atsTestConnection);
                    break;
                default:
                    return OperateResult.CreateFailedResult((IDbHelper)null, "数据库类型无法识别");
            }
            _dependencyResolver.RegisterInstance<IDbHelper>(DatabaseForSysType.AtsTest.ToString(), atsDbHelper);
            switch (databaseType)
            {
                case DatabaseTypeEnum.Oracle:
                    #region 指令操作
                    _dependencyResolver.RegisterType<OpcItemAbstract, OpcItemForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.AtsTest.ToString())));
                    #endregion
                    break;
                case DatabaseTypeEnum.SqlServer:
                    #region 指令操作
                    _dependencyResolver.RegisterType<OpcItemAbstract, OpcItemForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.AtsTest.ToString())));
                    #endregion
                    break;
                case DatabaseTypeEnum.MySql:
                    break;
                case DatabaseTypeEnum.Sqlite:
                    break;
                default:
                    break;
            }
            return OperateResult.CreateSuccessResult(atsDbHelper);
        }

        private OperateResult RegisterProjectService()
        {

            OperateResult<OPCClientAbstract> opcClientResult = RegisterOpcClient();
            if (!opcClientResult.IsSuccess)
            {
                return opcClientResult;
            }


            _dependencyResolver.RegisterInstance<OPCClientAbstract>(opcClientResult.Content);

            _dependencyResolver.RegisterType<OPCMonitorAbstract, OPCMonitor>(new ContainerControlledLifetimeManager());


            _dependencyResolver.RegisterType<IStringCharTaskNotifyCentre, StringCharTaskNotifyHandle>(
                new ContainerControlledLifetimeManager());

            _dependencyResolver.RegisterType<IOrderNotifyCentre, OrderNotifyHandle>(new ContainerControlledLifetimeManager());

            _dependencyResolver.RegisterType<IOrderGenerate, OrderGenerate>(new ContainerControlledLifetimeManager());

            _dependencyResolver.RegisterType<IOrderAllocator, OrderAllocator>(new ContainerControlledLifetimeManager());

            _dependencyResolver.RegisterType<IOrderManage, OrderManage>(new ContainerControlledLifetimeManager());

            _dependencyResolver.RegisterType<ITransportManage, TransportManage>(new ContainerControlledLifetimeManager());

            switch (SystemConfig.Instance.WcsDataBaseType)
            {
                case DatabaseTypeEnum.Oracle:

                    #region 指令操作
                    _dependencyResolver.RegisterType<OrderDataAbstract, OrderDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    _dependencyResolver.RegisterType<StringCharTaskAbstract, StringCharTaskForOracle>(
                        new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 搬运信息操作
                    _dependencyResolver.RegisterType<TransportMsgDataAbstract, TransportMsgDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 数据库操作
                    _dependencyResolver.RegisterType<ReceiveDataAbstract, ReceiveDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 上层服务接口操作
                    _dependencyResolver.RegisterType<UpperInterfaceDataAbstract, UpperInterfaceDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    _dependencyResolver.RegisterType<LiveStatusAbstract, LiveStatusForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));

                    _dependencyResolver.RegisterType<LiveDataAbstract, LiveDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));

                    #region 用户信息数据库操作
                    _dependencyResolver.RegisterType<UserDataAbstract, UserDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 上层接口数据库操作
                    _dependencyResolver.RegisterType<ReceiveDataAbstract, ReceiveDataForOracle>(
                        new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    break;
                case DatabaseTypeEnum.SqlServer:
                    #region 指令操作
                    _dependencyResolver.RegisterType<OrderDataAbstract, OrderDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    _dependencyResolver.RegisterType<StringCharTaskAbstract, StringCharTaskForSqlServer>(
    new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));

                    #endregion

                    #region 搬运信息操作
                    _dependencyResolver.RegisterType<TransportMsgDataAbstract, TransportMsgDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 数据库操作
                    _dependencyResolver.RegisterType<ReceiveDataAbstract, ReceiveDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 上层服务接口操作
                    _dependencyResolver.RegisterType<UpperInterfaceDataAbstract, UpperInterfaceDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion


                    #region 实时状态数据库操作
                    _dependencyResolver.RegisterType<LiveStatusAbstract, LiveStatusForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 实时数据的数据库操作
                    _dependencyResolver.RegisterType<LiveDataAbstract, LiveDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 用户信息数据库操作
                    _dependencyResolver.RegisterType<UserDataAbstract, UserDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 上层接口数据库操作
                    _dependencyResolver.RegisterType<ReceiveDataAbstract, ReceiveDataForSqlServer>(
                        new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    #region 任务指令数据库操作
                    _dependencyResolver.RegisterType<TaskOrderDataAbstract, TaskOrderDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                    #endregion

                    break;
                case DatabaseTypeEnum.MySql:
                    break;
                case DatabaseTypeEnum.Sqlite:
                    break;
                default:
                    break;
            }

            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 立库项目的初始化
        /// </summary>
        /// <returns></returns>
        public OperateResult Initilize()
        {
            OperateResult initResult = OperateResult.CreateFailedResult();
            try
            {

                OperateResult registerProjectService = RegisterProjectService();
                if (!registerProjectService.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("注册项目服务失败,失败原因：{0}", registerProjectService.Message), 1);

                }

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始初始化指令管理", DateTime.Now), EnumLogLevel.Debug);

                OperateResult<OrderManage> createOrderManageHandler = CreateOrderManageHandler();
                if (!createOrderManageHandler.IsSuccess)
                {
                    return OperateResult.CreateFailedResult("创建指令管理失败,失败原因：" + createOrderManageHandler.Message, 1);
                }

                _dependencyResolver.RegisterInstance(createOrderManageHandler.Content);

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束初始化指令管理", DateTime.Now), EnumLogLevel.Debug);


                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始初始化设备", DateTime.Now), EnumLogLevel.Debug);


                OperateResult initDevices = DeviceManage.Instance.InitDevices();
                if (!initDevices.IsSuccess)
                {
                    return initDevices;
                }

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束初始化设备", DateTime.Now), EnumLogLevel.Debug);
                
                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始初始化组件", DateTime.Now), EnumLogLevel.Debug);

                OperateResult initWorker = WorkerManage.Instance.InitWorkers();

                if (!initWorker.IsSuccess)
                {
                    return initWorker;
                }

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束初始化组件", DateTime.Now), EnumLogLevel.Debug);

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始初始化上层业务", DateTime.Now), EnumLogLevel.Debug);

                OperateResult initilzeUpperResult = UpperServiceBusinessManage.Instance.InitilizeAllUpperService();
                if (!initilzeUpperResult.IsSuccess)
                {
                    return initilzeUpperResult;
                }
                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束初始化上层业务", DateTime.Now), EnumLogLevel.Debug);


                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始初始化处理恢复数据", DateTime.Now), EnumLogLevel.Debug);

                HandleAllRestoreData();

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束初始化处理恢复数据", DateTime.Now), EnumLogLevel.Debug);
                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始启动运行", DateTime.Now), EnumLogLevel.Debug);


                OperateResult runAllResult = RunAll();
                if (!runAllResult.IsSuccess)
                {
                    return runAllResult;
                }
                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束启动运行", DateTime.Now), EnumLogLevel.Debug);


                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 开始启动Webservice服务", DateTime.Now), EnumLogLevel.Debug);

                OperateResult startWebserviceResult = WcsServiceManage.Instance.StartWcsService();

                if (!startWebserviceResult.IsSuccess)
                {
                    return startWebserviceResult;
                }

                LogHelper.WriteLog(AppStart.LogName, string.Format("{0} 结束启动Webservice服务", DateTime.Now), EnumLogLevel.Debug);


                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                initResult.IsSuccess = false;
                initResult.Message = OperateResult.ConvertException(ex);
            }
            return initResult;
        }

        private OperateResult<OrderManage> CreateOrderManageHandler()
        {
            OrderManage orderManageHandler = _dependencyResolver.GetService<IOrderManage>() as OrderManage;
            _needStart.Add(orderManageHandler);
            return OperateResult.CreateSuccessResult(orderManageHandler);
        }
    }
}
