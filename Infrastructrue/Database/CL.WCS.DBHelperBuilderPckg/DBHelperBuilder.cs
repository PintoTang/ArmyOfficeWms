using System.Threading;
using CL.Framework.ConfigFilePckg;
using CL.WCS.DBHelperImpPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CL.WCS.DBHelperBuilderPckg
{
    public class DBHelperBuilder
    {
        private static DBHelperSqlSugar _wcsDbHelper = null;
        public static IDbHelper GetWcsLocal(int dbType,string connectionString)
        {
            if (_wcsDbHelper == null)
            {
                _wcsDbHelper = new DBHelperSqlSugar(dbType,connectionString, "Clou_WCS");
                _wcsDbHelper.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
                return _wcsDbHelper;
            }
            return _wcsDbHelper;
        }

        private static DBHelperSqlSugar _atsTestDbHelper = null;
        public static IDbHelper GetAtsTest(int dbType, string connectionString)
        {
            if (_atsTestDbHelper == null)
            {
                _atsTestDbHelper = new DBHelperSqlSugar(dbType, connectionString, "Ats_Test");
                _atsTestDbHelper.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
                return _atsTestDbHelper;
            }
            return _atsTestDbHelper;
        }
        //private static DBHelperOracle db_WCS_Oracle = null;
        ///// <summary>
        ///// WCS系统的Oracle数据库访问对象
        ///// </summary>
        //public static IDbHelper GetWcsLocalOracle(string connectionString)
        //{

        //    if (db_WCS_Oracle == null)
        //    {
        //        db_WCS_Oracle = new DBHelperOracle(connectionString, "Clou_WCS");
        //        db_WCS_Oracle.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //        return db_WCS_Oracle;
        //    }
        //    return db_WCS_Oracle;
        //}

        //private static DBHelperOracle db_Ats_Oracle = null;
        ///// <summary>
        ///// WCS系统的Oracle数据库访问对象
        ///// </summary>
        //public static IDbHelper GetAtsTestOracle(string connectionString)
        //{

        //    if (db_Ats_Oracle == null)
        //    {
        //        db_Ats_Oracle = new DBHelperOracle(connectionString, "Ats_Test");
        //        db_Ats_Oracle.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //        return db_Ats_Oracle;
        //    }
        //    return db_Ats_Oracle;
        //}


        //private static DBHelperSqlLite db_WCS_SQLLite = null;
        ///// <summary>
        ///// WCS系统的SQLLite数据库访问对象
        ///// </summary>
        //public static IDbHelper GetWcsLocalSqlLite(string connectionString)
        //{

        //    if (db_WCS_SQLLite == null)
        //    {
        //        db_WCS_SQLLite = new DBHelperSqlLite(connectionString, "Clou_WCS");
        //        db_WCS_SQLLite.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_WCS_SQLLite;

        //}





        //private static DBHelperMySql db_WCS_MySQL = null;
        ///// <summary>
        ///// WCS系统的MYSQL数据库访问对象
        ///// </summary>
        //public static IDbHelper GetWcsLocalMySql(string connectionString)
        //{
        //    if (db_WCS_MySQL == null)
        //    {
        //        db_WCS_MySQL = new DBHelperMySql(connectionString, "Clou_WCS");
        //        db_WCS_MySQL.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_WCS_MySQL;
        //}

        //private static DBHelperMySql db_Ats_MySQL = null;
        ///// <summary>
        ///// WCS系统的MYSQL数据库访问对象
        ///// </summary>
        //public static IDbHelper GetAtsTestMySql(string connectionString)
        //{
        //    if (db_Ats_MySQL == null)
        //    {
        //        db_Ats_MySQL = new DBHelperMySql(connectionString, "Ats_Test");
        //        db_Ats_MySQL.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_Ats_MySQL;
        //}

        //private static DBHelperOracle db_WCS_GCP_MiddleOracle = null;
        ///// <summary>
        ///// WCS与GCP系统中间库的Oracle数据库访问对象
        ///// </summary>
        //public static IDbHelper DB_WCS_GCP_MiddleOracle(string connectionString)
        //{
        //    if (db_WCS_GCP_MiddleOracle == null)
        //    {
        //        db_WCS_GCP_MiddleOracle = new DBHelperOracle(connectionString, "DB_WCS_GCP_MiddleOracle");
        //        db_WCS_GCP_MiddleOracle.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_WCS_GCP_MiddleOracle;

        //}


        private static void ExecuteSQLError_PrintLogToUIEvent(string DatabaseName, string strSql, string message)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                SnackbarQueue.Enqueue(string.Format("操作数据异常：{0}", message));
            });
            
        }


        //private static DBHelperSQLServer db_Sorting_SQLServer = null;
        ///// <summary>
        ///// WCS系统对分拣机的SQLServer数据库访问对象
        ///// </summary>
        //public static IDbHelper DB_Sorting_SQLServer(string connectionString)
        //{
        //    if (db_Sorting_SQLServer == null)
        //    {
        //        db_Sorting_SQLServer = new DBHelperSQLServer(connectionString, "DB_Sorting_SQLServer");
        //        db_Sorting_SQLServer.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_Sorting_SQLServer;
        //}


        //private static DBHelperOracle db_WCS_DEAO_RGV_MiddleOracle = null;
        ///// <summary>
        ///// WCS与德奥RGV系统中间库的Oracle数据库访问对象
        ///// </summary>
        //public static IDbHelper DB_WCS_DEAO_RGV_MiddleOracle
        //{
        //    get
        //    {
        //        if (db_WCS_DEAO_RGV_MiddleOracle == null)
        //        {
        //            ConfigFile configFile = new ConfigFile("Config/App.config");
        //            db_WCS_DEAO_RGV_MiddleOracle = new DBHelperOracle(configFile.AppSettings["DB_WCS_DEAO_RGV_MiddleOracle"].ToString(), "DB_WCS_DEAO_RGV_MiddleOracle");
        //            db_WCS_DEAO_RGV_MiddleOracle.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //        }
        //        return db_WCS_DEAO_RGV_MiddleOracle;
        //    }
        //}


        //private static DBHelperSQLServer db_SQLServer = null;
        ///// <summary>
        ///// WCS系统的SQLServer数据库访问对象
        ///// </summary>
        //public static IDbHelper GetWcsLocalSqlServer(string connectionString)
        //{
        //    if (db_SQLServer == null)
        //    {
        //        db_SQLServer = new DBHelperSQLServer(connectionString, "Clou_WCS");
        //        db_SQLServer.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_SQLServer;
        //}

        //private static DBHelperSQLServer db_Ats_SQLServer = null;
        ///// <summary>
        ///// WCS系统的SQLServer数据库访问对象
        ///// </summary>
        //public static IDbHelper GetAtsTestSqlServer(string connectionString)
        //{
        //    if (db_Ats_SQLServer == null)
        //    {
        //        db_Ats_SQLServer = new DBHelperSQLServer(connectionString, "Ats_Test");
        //        db_Ats_SQLServer.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_Ats_SQLServer;
        //}


        //private static DBHelperSqlLite db_CSConfig_SQLLite = null;
        ///// <summary>
        ///// WCS实例配置系统的SQLLite数据库访问对象
        ///// </summary>
        //public static IDbHelper DB_CSConfig_SQLLite(string connectionString)
        //{
        //    if (db_CSConfig_SQLLite == null)
        //    {
        //        db_CSConfig_SQLLite = new DBHelperSqlLite(connectionString, "DB_CSConfig_SQLLite");
        //        db_CSConfig_SQLLite.PrintLogToUIEvent += ExecuteSQLError_PrintLogToUIEvent;
        //    }
        //    return db_CSConfig_SQLLite;
        //}

    }
}
