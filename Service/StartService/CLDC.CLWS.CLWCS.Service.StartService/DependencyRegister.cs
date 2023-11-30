using CL.WCS.DBHelperBuilderPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using Infrastructrue.Ioc.Interface;
using Microsoft.Practices.Unity;
using System;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.License;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Service.StartService
{
    public class DependencyRegister
    {
        private readonly IDependency _dependencyContainer;
        public DependencyRegister()
        {
            _dependencyContainer = DependencyFactory.GetDependency();

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public OperateResult InitializeSysBaseService()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                _dependencyContainer.RegisterType<ILicenseService, LicenseService>(new ContainerControlledLifetimeManager());

                OperateResult createDbResult = RegisterWcsLocalDbHelper();
                if (!createDbResult.IsSuccess)
                {
                    return createDbResult;
                }

                #region 用户信息数据库操作
                _dependencyContainer.RegisterType<UserDataAbstract, UserDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                #endregion

                #region 操作日志数据库操作
                _dependencyContainer.RegisterType<OperateLogDataAbstract, OperateLogDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                #endregion
                //switch (SystemConfig.Instance.WcsDataBaseType)
                //{
                //    case DatabaseTypeEnum.Oracle:

                //        #region 用户信息数据库操作
                //        _dependencyContainer.RegisterType<UserDataAbstract, UserDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                //        #endregion

                //        #region 操作日志数据库操作
                //        _dependencyContainer.RegisterType<OperateLogDataAbstract, OperateLogDataForOracle>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                //        #endregion

                //        break;
                //    case DatabaseTypeEnum.SqlServer:
                //        #region 用户信息数据库操作
                //        _dependencyContainer.RegisterType<UserDataAbstract, UserDataForSqlSugar>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                //        #endregion

                //        #region 操作日志数据库操作
                //        _dependencyContainer.RegisterType<OperateLogDataAbstract, OperateLogDataForSqlServer>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                //        #endregion


                //        break;
                //    case DatabaseTypeEnum.MySql:
                //        break;
                //    case DatabaseTypeEnum.Sqlite:
                //        #region 用户信息数据库操作
                //        _dependencyContainer.RegisterType<UserDataAbstract, UserDataForSqlite>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                //        #endregion
                //        #region 操作日志数据库操作
                //        _dependencyContainer.RegisterType<OperateLogDataAbstract, OperateLogDataForSqlite>(new InjectionConstructor(new ResolvedParameter<IDbHelper>(DatabaseForSysType.WcsLocal.ToString())));
                //        #endregion
                //        break;
                //    default:
                //        break;
                //}
                OperateResult<AuthorizeService> createUserAuthorizeService = CreateAuthorizeService();
                if (!createUserAuthorizeService.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("创建用户权限管理服务失败,失败原因：{0}", createUserAuthorizeService.Message), 1);
                }
                _dependencyContainer.RegisterInstance(createUserAuthorizeService.Content);


                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;

        }

        private OperateResult<AuthorizeService> CreateAuthorizeService()
        {
            UserDataAbstract userDataAbstract = DependencyHelper.GetService<UserDataAbstract>();
            AuthorizeService authorizeService = new AuthorizeService(userDataAbstract);
            return OperateResult.CreateSuccessResult(authorizeService);
        }
       

        private OperateResult<IDbHelper> RegisterWcsLocalDbHelper()
        {

            var wcsLocalConnection = SystemConfig.Instance.WcsConnectionString;

            var databaseType = SystemConfig.Instance.WcsDataBaseType;
            
            IDbHelper wcsLocalDbHelper = DBHelperBuilder.GetWcsLocal((int)databaseType,wcsLocalConnection);

            //switch (databaseType)
            //{
            //    case DatabaseTypeEnum.Oracle:
            //        wcsLocalDbHelper = DBHelperBuilder.GetWcsLocalOracle(wcsLocalConnection);
            //        break;
            //    case DatabaseTypeEnum.SqlServer:
            //        wcsLocalDbHelper = DBHelperBuilder.GetWcsLocalSqlServer(wcsLocalConnection);
            //        break;
            //    case DatabaseTypeEnum.MySql:
            //        wcsLocalDbHelper = DBHelperBuilder.GetWcsLocalMySql(wcsLocalConnection);
            //        break;
            //    case DatabaseTypeEnum.Sqlite:
            //        wcsLocalDbHelper = DBHelperBuilder.GetWcsLocalSqlLite(wcsLocalConnection);
            //        break;
            //    default:
            //        return OperateResult.CreateFailedResult((IDbHelper) null, "数据库类型无法识别");
            //}
            _dependencyContainer.RegisterInstance<IDbHelper>(DatabaseForSysType.WcsLocal.ToString(), wcsLocalDbHelper);
            return OperateResult.CreateSuccessResult(wcsLocalDbHelper);
        }


    }
}
