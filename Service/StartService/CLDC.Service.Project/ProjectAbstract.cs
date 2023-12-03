using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.OPCClientAbsPckg;
using CL.Framework.OPCClientImpPckg;
using CL.Framework.OPCClientSimulatePckg;
using CL.Framework.Testing.OPCClientImpPckg;
using CL.WCS.DBHelperBuilderPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CL.WCS.OPCMonitorImpPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWCS.Service.DataService;
using CLDC.CLWCS.Service.MenuService;
using CLDC.CLWCS.WareHouse.Architecture.Manage;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.TransportManage;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLDC.CLWS.CLWCS.WareHouse.OrderAllocator;
using CLDC.CLWS.CLWCS.WareHouse.OrderGenerate;
using CLDC.CLWS.CLWCS.WareHouse.OrderHandle;
using CLDC.CLWS.CLWCS.WareHouse.StringCharTaskHandle;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Manage;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WcsService;
using CLDC.Framework.Log.Helper;
using Infrastructrue.Ioc.DependencyFactory;
using Infrastructrue.Ioc.Interface;
using Microsoft.Practices.Unity;

namespace CLDC.Service.Project
{
    /// <summary>
    /// 项目加载虚拟类
    /// </summary>
    public abstract class ProjectAbstract
    {

        protected IDependency DependencyResolver;

        private string _logName = "系统启动";

        public OperateResult InitilizeProject()
        {
            OperateResult initilizeResult = Initilize();
            if (!initilizeResult.IsSuccess)
            {
                return initilizeResult;
            }
            OperateResult pInitlizeResult = ParticularInitlize();
            if (!pInitlizeResult.IsSuccess)
            {
                return pInitlizeResult;
            }

            //OperateResult rOpcServerResult = RegisterOpcServer();
            //if (!rOpcServerResult.IsSuccess)
            //{
            //    return rOpcServerResult;
            //}

            OperateResult rServiceResult = RegisterService();
            if (!rServiceResult.IsSuccess)
            {
                return rServiceResult;
            }
            OperateResult pServiceResult = ParticularRegisterService();
            if (!pServiceResult.IsSuccess)
            {
                return pServiceResult;
            }

            OperateResult initlizeConfigReuslt = InitilizeConfig();
            if (!initlizeConfigReuslt.IsSuccess)
            {
                return initlizeConfigReuslt;
            }

            OperateResult pInitlizeConfigResult = ParticularInitilizeConfig();
            if (!pInitlizeConfigResult.IsSuccess)
            {
                return pInitlizeConfigResult;
            }

            //OperateResult restoreResult = Restore();
            //if (!restoreResult.IsSuccess)
            //{
            //    return restoreResult;
            //}

            //OperateResult pRestoreResult = ParticularRestore();
            //if (!pRestoreResult.IsSuccess)
            //{
            //    return pRestoreResult;
            //}

            //OperateResult startResult = Start();
            //if (!startResult.IsSuccess)
            //{
            //    return startResult;
            //}

            //OperateResult pStartResult = ParticularStart();
            //if (!pStartResult.IsSuccess)
            //{
            //    return pStartResult;
            //}
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult Initilize()
        {
            DependencyResolver = DependencyFactory.GetDependency();
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult Restore()
        {
            //恢复搬运信息
            DependencyHelper.GetService<ITransportManage>().HandleRestoreData();

            foreach (DeviceBaseAbstract device in DeviceManage.Instance.GetAllData())
            {
                device.HandleRestoreData();
            }

            foreach (WorkerBaseAbstract worker in WorkerManage.Instance.GetAllData())
            {
                worker.HandleRestoreData();
            }

            return OperateResult.CreateSuccessResult();
        }

        public abstract OperateResult ParticularRestore();
        public abstract OperateResult ParticularInitlize();

        public virtual OperateResult RegisterOpcServer()
        {
            OperateResult<OPCClientAbstract> opcClientResult = RegisterOpcClient();
            if (!opcClientResult.IsSuccess)
            {
                return opcClientResult;
            }

            DependencyResolver.RegisterInstance(opcClientResult.Content);

            DependencyResolver.RegisterType<OPCMonitorAbstract, OPCMonitor>(new ContainerControlledLifetimeManager());

            return OperateResult.CreateSuccessResult();
        }

        private OperateResult RegisterService()
        {

            DependencyResolver.RegisterType<IStringCharTaskNotifyCentre, StringCharTaskNotifyHandle>(
                new ContainerControlledLifetimeManager());

            DependencyResolver.RegisterType<IOrderNotifyCentre, OrderNotifyHandle>(new ContainerControlledLifetimeManager());

            DependencyResolver.RegisterType<IOrderGenerate, OrderGenerate>(new ContainerControlledLifetimeManager());

            DependencyResolver.RegisterType<IOrderAllocator, OrderAllocator>(new ContainerControlledLifetimeManager());

            DependencyResolver.RegisterType<IOrderManage, OrderManage>(new ContainerControlledLifetimeManager());

            DependencyResolver.RegisterType<ITransportManage, TransportManage>(new ContainerControlledLifetimeManager());

            #region 数据库操作服务层注册

            #region 指令操作
            DependencyResolver.RegisterType<OrderDataAbstract, OrderDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            DependencyResolver.RegisterType<StringCharTaskAbstract, StringCharTaskForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));

            #endregion

            #region 搬运信息操作
            DependencyResolver.RegisterType<TransportMsgDataAbstract, TransportMsgDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion

            #region 数据库操作
            DependencyResolver.RegisterType<ReceiveDataAbstract, ReceiveDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion

            #region 上层服务接口操作
            DependencyResolver.RegisterType<UpperInterfaceDataAbstract, UpperInterfaceDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion


            #region 实时状态数据库操作
            DependencyResolver.RegisterType<LiveStatusAbstract, LiveStatusForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion

            #region 实时数据的数据库操作
            DependencyResolver.RegisterType<LiveDataAbstract, LiveDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion

            //#region 用户信息数据库操作
            //DependencyResolver.RegisterType<UserDataAbstract, UserDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            //#endregion

            //#region 上层接口数据库操作
            //DependencyResolver.RegisterType<ReceiveDataAbstract, ReceiveDataForSqlServer>(
            //    new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            //#endregion

            #region 任务指令数据库操作
            DependencyResolver.RegisterType<TaskOrderDataAbstract, TaskOrderDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion

            #region 地址基础数据操作
            DependencyResolver.RegisterType<WhAddressDataAbstract, WhAddressDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
            #endregion

            #region 机器人就绪
            DependencyResolver.RegisterType<RobotReadyRecordDataAbstract, RobotReadyRecordForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));

            #endregion
            #endregion

            return OperateResult.CreateSuccessResult();

        }

        public abstract OperateResult ParticularRegisterService();

        private OperateResult InitilizeConfig()
        {
            LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化指令管理", DateTime.Now), EnumLogLevel.Debug);
            OperateResult<OrderManage> createOrderManageHandler = CreateOrderManageHandler();
            if (!createOrderManageHandler.IsSuccess)
            {
                return OperateResult.CreateFailedResult("创建指令管理失败,失败原因：" + createOrderManageHandler.Message, 1);
            }
            DependencyResolver.RegisterInstance(createOrderManageHandler.Content);
            LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化指令管理", DateTime.Now), EnumLogLevel.Debug);


            //LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化设备", DateTime.Now), EnumLogLevel.Debug);
            //OperateResult initDevices = DeviceManage.Instance.InitDevices();
            //if (!initDevices.IsSuccess)
            //{
            //    return initDevices;
            //}
            //LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化设备", DateTime.Now), EnumLogLevel.Debug);


            //LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化组件", DateTime.Now), EnumLogLevel.Debug);
            //OperateResult initWorker = WorkerManage.Instance.InitWorkers();
            //if (!initWorker.IsSuccess)
            //{
            //    return initWorker;
            //}
            //LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化组件", DateTime.Now), EnumLogLevel.Debug);


            //LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化上层业务", DateTime.Now), EnumLogLevel.Debug);
            //OperateResult initilzeUpperResult = UpperServiceBusinessManage.Instance.InitilizeAllUpperService();
            //if (!initilzeUpperResult.IsSuccess)
            //{
            //    return initilzeUpperResult;
            //}
            //LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化上层业务", DateTime.Now), EnumLogLevel.Debug);


            LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化基础数据管理", DateTime.Now), EnumLogLevel.Debug);
            OperateResult initilizeArchitectureResult = ArchitectureManage.Instance.Initilize();
            if (!initilizeArchitectureResult.IsSuccess)
            {
                return initilizeArchitectureResult;
            }
            LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化基础数据管理", DateTime.Now), EnumLogLevel.Debug);


            LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化数据管理", DateTime.Now), EnumLogLevel.Debug);
            OperateResult initilizeOperateLogResult = WcsDataServiceManage.Instance.Initilize();
            if (!initilizeOperateLogResult.IsSuccess)
            {
                return initilizeOperateLogResult;
            }
            LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化数据管理", DateTime.Now), EnumLogLevel.Debug);


            LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化菜单", DateTime.Now), EnumLogLevel.Debug);
            OperateResult initilizeMenuResult = WcsMenuManage.Instance.Initilize();
            if (!initilizeMenuResult.IsSuccess)
            {
                return initilizeMenuResult;
            }
            LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化菜单", DateTime.Now), EnumLogLevel.Debug);


            return OperateResult.CreateSuccessResult();
        }

        public abstract OperateResult ParticularInitilizeConfig();

        private OperateResult Start()
        {
            LogHelper.WriteLog(_logName, string.Format("{0} 开始启动Webservice服务", DateTime.Now), EnumLogLevel.Debug);

            OperateResult startWebserviceResult = WcsServiceManage.Instance.StartWcsService();

            if (!startWebserviceResult.IsSuccess)
            {
                return startWebserviceResult;
            }

            LogHelper.WriteLog(_logName, string.Format("{0} 结束启动Webservice服务", DateTime.Now), EnumLogLevel.Debug);


            foreach (IStateControl thread in _needStart)
            {
                OperateResult runResult = thread.Start();
                if (!runResult.IsSuccess)
                {
                    return runResult;
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        public virtual OperateResult ParticularStart()
        {
            StartOpcMonitor();
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 记录需要Run的列表
        /// </summary>
        readonly List<IStateControl> _needStart = new List<IStateControl>();

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
                //try
                //{
                //    OperateResult<IDbHelper> dbHepler = RegisterAtsTestDbHelper();
                //    if (!dbHepler.IsSuccess)
                //    {
                //        return OperateResult.CreateFailedResult<OPCClientAbstract>(null, "自动化测试模式下，创建模拟Opc数据库连接失败：" + dbHepler.Message);
                //    }
                //    OpcItemAbstract opcServer = DependencyResolver.GetService<OpcItemAbstract>();
                //    opcClient = new OPCDataBaseRWClient(opcServer);
                //    opcClient.IsPrintRealTimeReadLog = SystemConfig.Instance.CurSystemConfig.IsRecordEventLog.Value;
                //    createResult = OperateResult.CreateSuccessResult(opcClient);
                //}
                //catch (Exception ex)
                //{
                //    createResult = OperateResult.CreateFailedResult<OPCClientAbstract>(null, string.Format("自动自测模式需要配置OPC数据库，请确认配置信息正确!：" + OperateResult.ConvertException(ex)));
                //}
                createResult = OperateResult.CreateSuccessResult(opcClient);
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
            var atsTestConnection = SystemConfig.Instance.CurSystemConfig.AtsDatabaseConn.Value;
            var databaseType = SystemConfig.Instance.CurSystemConfig.AtsDataBaseType.Value;

            IDbHelper atsDbHelper = DBHelperBuilder.GetAtsTest((int)databaseType,atsTestConnection);
            //switch (databaseType)
            //{
            //    case DatabaseTypeEnum.Oracle:
            //        atsDbHelper = DBHelperBuilder.GetAtsTestOracle(atsTestConnection);
            //        break;
            //    case DatabaseTypeEnum.SqlServer:
            //        atsDbHelper = DBHelperBuilder.GetAtsTestSqlServer(atsTestConnection);
            //        break;
            //    case DatabaseTypeEnum.MySql:
            //        atsDbHelper = DBHelperBuilder.GetAtsTestMySql(atsTestConnection);
            //        break;
            //    default:
            //        return OperateResult.CreateFailedResult((IDbHelper)null, "数据库类型无法识别");
            //}
            DependencyResolver.RegisterInstance<IDbHelper>(DatabaseForSysType.AtsTest.ToString(), atsDbHelper);
            DependencyResolver.RegisterType<OpcItemAbstract, OpcItemForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.AtsTest.ToString())));

            return OperateResult.CreateSuccessResult(atsDbHelper);
        }

        private OperateResult<OrderManage> CreateOrderManageHandler()
        {
            OrderManage orderManageHandler = DependencyResolver.GetService<IOrderManage>() as OrderManage;
            _needStart.Add(orderManageHandler);
            return OperateResult.CreateSuccessResult(orderManageHandler);
        }

        private void StartOpcMonitor()
        {
            OPCMonitorAbstract opcMonitorHandler = DependencyHelper.GetService<OPCMonitorAbstract>();
            int intervalTime = SystemConfig.Instance.MonitorIntervalTime;
            int threadCount = SystemConfig.Instance.ThreadCount;
            opcMonitorHandler.StartMonitor(threadCount, intervalTime);
        }

    }
}
