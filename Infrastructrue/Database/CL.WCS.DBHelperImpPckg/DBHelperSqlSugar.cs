using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Framework.Log;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CL.WCS.DBHelperImpPckg
{
    public class DBHelperSqlSugar : IDbHelper
    {

        public event PrintLogToUiHanlder PrintLogToUIEvent;

        private string _dbConnectionString = string.Empty;

        private const int TimeOut = 180;
        private const string txtName = "数据库操作异常日志";
        private string _DatabaseName = string.Empty;
        private int _dbType = 0; 

        public DBHelperSqlSugar(int dbType,string connectionString, string strDatabaseName)
        {
            _dbType = dbType;
            _dbConnectionString = connectionString;
            _DatabaseName = strDatabaseName;
        }

        /// <summary>
        /// 获取系统运行的数据库名称
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            return _DatabaseName;
        }

        public UserControl GetConfigurationView()
        {
            throw new NotImplementedException();
        }

        public ISqlSugarClient CreateContext()
        {
            
            ISqlSugarClient dbContext = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _dbConnectionString,
                DbType = (SqlSugar.DbType)_dbType,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings
                {
                    IsAutoRemoveDataCache = true
                },
                InitKeyType = InitKeyType.Attribute,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, pars) =>
                    {
                        try
                        {
                            string fullSql = GetFullSql(pars, sql);
                            if (fullSql.Contains("OPC_"))
                            {
                                return;
                            }
                            Console.WriteLine(fullSql);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                },
            });
            return dbContext;
        }
        private string GetFullSql(SugarParameter[] paramArr, string sql)
        {
            foreach (var param in paramArr)
            {
                if (param.DbType == System.Data.DbType.String || param.DbType == System.Data.DbType.AnsiString
                    || param.DbType == System.Data.DbType.DateTime || param.DbType == System.Data.DbType.DateTime2)
                {
                    sql = sql.Replace(param.ParameterName, "'" + param.Value.ObjToString() + "'");
                }
                else
                {
                    sql = sql.Replace(param.ParameterName, param.Value.ObjToString());
                }
            }

            return sql;
        }


        #region 新增操作

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>受影响行数</returns>
        public int Add<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity:class,new()
        {
            try
            {
                var insert = dbContext.Insertable(entity);
                return insert.ExecuteCommand();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns>受影响行数</returns>
        public int Add<TEntity>(ISqlSugarClient dbContext, List<TEntity> entitys) where TEntity : class, new()
        {
            try
            {
                return dbContext.Insertable(entitys.ToArray()).ExecuteCommand();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>返回当前实体</returns>
        public TEntity AddReturnEntity<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new()
        {
            try
            {
                var result = dbContext.Insertable(entity).ExecuteReturnEntity();
                return result;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>自增ID</returns>
        public int AddReturnIdentity<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new()
        {
            try
            {
                var result = dbContext.Insertable(entity).ExecuteReturnIdentity();
                return result;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>成功或失败</returns>
        public bool AddReturnBool<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new()
        {
            try
            {
                var result = dbContext.Insertable(entity).ExecuteCommand() > 0;
                return result;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return false;
            }
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体(不是Dto)
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(ISqlSugarClient dbContext, TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                IUpdateable<TEntity> up = dbContext.Updateable(entity);
                if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
                {
                    up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
                }
                if (isLock)
                {
                    up = up.With(SqlWith.UpdLock);
                }
                var result = up.ExecuteCommand();
                return result;

            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(ISqlSugarClient dbContext, List<TEntity> entitys, List<string> lstIgnoreColumns = null,
            bool isLock = true) where TEntity : class, new()
        {
            try
            {

                IUpdateable<TEntity> up = dbContext.Updateable(entitys);
                if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
                {
                    up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
                }
                if (isLock)
                {
                    up = up.With(SqlWith.UpdLock);
                }
                var result = up.ExecuteCommand();
                return result;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="where">条件表达式</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(ISqlSugarClient dbContext, TEntity entity, Expression<Func<TEntity, bool>> where,
            List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                IUpdateable<TEntity> up = dbContext.Updateable(entity);
                if (lstIgnoreColumns != null && lstIgnoreColumns.Count > 0)
                {
                    up = up.IgnoreColumns(lstIgnoreColumns.ToArray());
                }
                up = up.Where(where);
                if (isLock)
                {
                    up = up.With(SqlWith.UpdLock);
                }
                var result = up.ExecuteCommand();
                return result;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="update">实体对象</param>
        /// <param name="where">条件表达式</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, TEntity>> update,
            Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new()
        {
            try
            {

                IUpdateable<TEntity> up = dbContext.Updateable<TEntity>().SetColumns(update);
                if (where != null)
                {
                    up = up.Where(where);
                }
                if (isLock)
                {
                    up = up.With(SqlWith.UpdLock);
                }
                var result = up.ExecuteCommand();
                return result;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="keyValues">键:字段名称 值：值</param>
        /// <param name="where">条件表达式</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(ISqlSugarClient dbContext, Dictionary<string, object> keyValues,
            Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                IUpdateable<TEntity> up = dbContext.Updateable<TEntity>(keyValues);
                if (where != null)
                {
                    up = up.Where(where);
                }
                if (isLock)
                {
                    up = up.With(SqlWith.UpdLock);
                }
                var result = up.ExecuteCommand();
                return result;

            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        #endregion

        #region 删除操作

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public bool DeleteByPrimary<TEntity>(ISqlSugarClient dbContext, object id, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                var del = dbContext.Deleteable<TEntity>(id);
                if (isLock)
                {
                    del = del.With(SqlWith.RowLock);
                }
                return del.ExecuteCommand() > 0;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="primaryKeyValues">主键ID集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int DeleteByPrimary<TEntity>(ISqlSugarClient dbContext, List<object> primaryKeyValues, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                var del = dbContext.Deleteable<TEntity>().In(primaryKeyValues);
                if (isLock)
                {
                    del = del.With(SqlWith.RowLock);
                }
                return del.ExecuteCommand();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Delete<TEntity>(ISqlSugarClient dbContext, TEntity entity, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                var del = dbContext.Deleteable(entity);
                if (isLock)
                {
                    del = del.With(SqlWith.RowLock);
                }
                return del.ExecuteCommand();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Delete<TEntity>(ISqlSugarClient dbContext, List<TEntity> entitys, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                var del = dbContext.Deleteable(entitys);
                if (isLock)
                {
                    del = del.With(SqlWith.RowLock);
                }
                return del.ExecuteCommand();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Delete<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda, bool isLock = true) where TEntity : class, new()
        {
            try
            {
                var del = dbContext.Deleteable<TEntity>().Where(whereLambda);
                if (isLock)
                {
                    del = del.With(SqlWith.RowLock);
                }
                return del.ExecuteCommand();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                return 0;
            }
        }

        #endregion

        #region 单表查询

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="expression">返回表达式</param>
        /// <param name="whereLambda">条件表达式</param>
        /// <typeparam name="TEntity">返回对象</typeparam>
        /// <returns>自定义数据</returns>
        public TEntity Query<TEntity>(ISqlSugarClient dbContext,Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            try
            {
                return dbContext.Queryable<TEntity>().WhereIF(whereLambda != null, whereLambda).First();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 实体列表
        /// </summary>
        /// <param name="expression">返回表达式</param>
        /// <param name="whereLambda">条件表达式</param>
        /// <typeparam name="TEntity">返回对象</typeparam>
        /// <returns>自定义数据</returns>
        public List<TEntity> QueryList<TEntity>(ISqlSugarClient dbContext,Expression<Func<TEntity, bool>> whereLambda = null, string orderBy = "") where TEntity : class, new()
        {
            try
            {
                return dbContext.Queryable<TEntity>().WhereIF(whereLambda != null, whereLambda).OrderByIF(!string.IsNullOrEmpty(orderBy),orderBy).ToList();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 实体列表
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>实体列表</returns>
        public List<TEntity> QuerySqlList<TEntity>(ISqlSugarClient dbContext, string sql) where TEntity : class, new()
        {
            try
            {
                return dbContext.SqlQueryable<TEntity>(sql).ToList();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }
        /// <summary>
		/// 实体列表 分页查询
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="orderBy"></param>
		/// <param name="totalCount"></param>
		/// <param name="whereLambda"></param>
		/// <returns></returns>
        public List<TEntity> QueryPageList<TEntity>(ISqlSugarClient dbContext, int pageIndex,int pageSize,string orderBy,out int totalCount, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            totalCount = 0;
            try
            {
                var query = dbContext.Queryable<TEntity>();
                query = query.WhereIF(whereLambda != null, whereLambda);

                query = query.OrderByIF(!string.IsNullOrEmpty(orderBy), orderBy);
                var list = query.ToPageList(pageIndex, pageSize, ref totalCount);
                return list;
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }

        public int QueryCount<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            try
            {
                var query = dbContext.Queryable<TEntity>();
                query = query.WhereIF(whereLambda != null, whereLambda);
                return query.Count();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// DataTable数据源
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <returns>DataTable</returns>
        public DataTable QueryDataTable<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            try
            {
                return dbContext.Queryable<TEntity>().WhereIF(whereLambda != null, whereLambda).ToDataTable();
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// DataTable数据源
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>DataTable</returns>
        public DataTable QueryDataTable(ISqlSugarClient dbContext, string sql)
        {
            try
            {
                return dbContext.Ado.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Object
        /// </summary> 
        /// <param name="sql">SQL</param> 
        /// <returns>Object</returns>
        public object QuerySqlScalar(ISqlSugarClient dbContext, string sql)
        {
            try
            {
                return dbContext.Ado.GetScalar(sql);
            }
            catch (Exception ex)
            {
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, "查询失败", ex.Message);
                return null;
            }
        }

        public int ExecuteNonQuery(ISqlSugarClient dbContext, string sql, Hashtable paramList=null)
        {
            SugarParameter[] paramters = null;
            if (paramList != null && paramList.Keys.Count > 0)
            {
                paramters = new SugarParameter[paramList.Keys.Count];
                int index = 0;
                foreach (string pn in paramList.Keys)
                {
                    paramters[index]=new SugarParameter(pn, paramList[pn]);
                    index++;
                }
            }
            return dbContext.Ado.ExecuteCommand(sql, paramters);
        }
        #endregion

        #region 常用函数

        /// <summary>
        /// 对象是否存在
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <returns>True or False</returns>
        public bool IsExist<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            return dbContext.Queryable<TEntity>().WhereIF(whereLambda != null, whereLambda).Any();
        }

        /// <summary>
        /// 总和
        /// </summary>
        /// <param name="field">字段名</param>
        /// <returns>总和</returns>
        public int Sum<TEntity>(ISqlSugarClient dbContext, string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            return dbContext.Queryable<TEntity>().WhereIF(whereLambda!=null,whereLambda).Sum<int>(field);
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="field">字段名</param>
        /// <typeparam name="TResult">泛型结果</typeparam>
        /// <returns>最大值</returns>
        public TResult Max<TEntity, TResult>(ISqlSugarClient dbContext, string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            return dbContext.Queryable<TEntity>().WhereIF(whereLambda != null, whereLambda).Max<TResult>(field);
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <param name="field">字段名</param>
        /// <typeparam name="TResult">泛型结果</typeparam>
        /// <returns>最小值</returns>
        public TResult Min<TEntity, TResult>(ISqlSugarClient dbContext, string field, Expression<Func<TEntity, bool>> whereLambda = null)
        {
            return dbContext.Queryable<TEntity>().WhereIF(whereLambda != null, whereLambda).Min<TResult>(field);
        }

        #endregion

        #region 不带DbContext

        #region 新增操作

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>受影响行数</returns>
        public int Add<TEntity>(TEntity entity) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Add(dbContext, entity);
            }
        }

        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns>受影响行数</returns>
        public int Add<TEntity>(List<TEntity> entitys) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Add(dbContext, entitys);
            }
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>返回当前实体</returns>
        public TEntity AddReturnEntity<TEntity>(TEntity entity) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return AddReturnEntity(dbContext, entity);
            }
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>自增ID</returns>
        public int AddReturnIdentity<TEntity>(TEntity entity) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return AddReturnIdentity(dbContext, entity);
            }
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>成功或失败</returns>
        public bool AddReturnBool<TEntity>(TEntity entity) where TEntity : class, new()
        {

            using (var dbContext = CreateContext())
            {
                return AddReturnBool(dbContext, entity);
            }
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体(不是Dto)
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new()
        {

            using (var dbContext = CreateContext())
            {
                return Update(dbContext, entity, lstIgnoreColumns, isLock);
            }
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(List<TEntity> entitys, List<string> lstIgnoreColumns = null,
            bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Update(dbContext, entitys, lstIgnoreColumns, isLock);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="where">条件表达式</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where,
            List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Update(dbContext, entity, where, lstIgnoreColumns, isLock);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="update">实体对象</param>
        /// <param name="where">条件表达式</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(Expression<Func<TEntity, TEntity>> update,
            Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Update(dbContext, update, where, isLock);
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="keyValues">键:字段名称 值：值</param>
        /// <param name="where">条件表达式</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Update<TEntity>(Dictionary<string, object> keyValues,
            Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Update(dbContext, keyValues, where, isLock);
            }
        }

        #endregion

        #region 删除操作

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public bool DeleteByPrimary<TEntity>(object id, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                try
                {
                    var del = dbContext.Deleteable<TEntity>(id);
                    if (isLock)
                    {
                        del = del.With(SqlWith.RowLock);
                    }
                    return del.ExecuteCommand() > 0;
                }
                catch (Exception ex)
                {
                    if (PrintLogToUIEvent != null)
                        PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="primaryKeyValues">主键ID集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int DeleteByPrimary<TEntity>(List<object> primaryKeyValues, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                try
                {
                    var del = dbContext.Deleteable<TEntity>().In(primaryKeyValues);
                    if (isLock)
                    {
                        del = del.With(SqlWith.RowLock);
                    }
                    return del.ExecuteCommand();
                }
                catch (Exception ex)
                {
                    if (PrintLogToUIEvent != null)
                        PrintLogToUIEvent(_DatabaseName, "保存失败", ex.Message);
                    return 0;
                }
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Delete<TEntity>(TEntity entity, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Delete(dbContext,entity,isLock);
            }
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Delete<TEntity>(List<TEntity> entitys, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Delete(dbContext, entitys, isLock);
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        public int Delete<TEntity>(Expression<Func<TEntity, bool>> whereLambda, bool isLock = true) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Delete(dbContext, whereLambda, isLock);
            }
        }

        #endregion

        #region 单表查询

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="expression">返回表达式</param>
        /// <param name="whereLambda">条件表达式</param>
        /// <typeparam name="TEntity">返回对象</typeparam>
        /// <returns>自定义数据</returns>
        public TEntity Query<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Query(dbContext, whereLambda);
            }
        }

        /// <summary>
        /// 实体列表
        /// </summary>
        /// <param name="expression">返回表达式</param>
        /// <param name="whereLambda">条件表达式</param>
        /// <typeparam name="TEntity">返回对象</typeparam>
        /// <returns>自定义数据</returns>
        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null,string orderBy="") where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return QueryList(dbContext, whereLambda, orderBy);
            }
        }

        /// <summary>
        /// 实体列表
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>实体列表</returns>
        public List<TEntity> QuerySqlList<TEntity>(string sql) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return QuerySqlList<TEntity>(dbContext, sql);
            }
        }
        /// <summary>
		/// 实体列表 分页查询
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="orderBy"></param>
		/// <param name="totalCount"></param>
		/// <param name="whereLambda"></param>
		/// <returns></returns>
        public List<TEntity> QueryPageList<TEntity>(int pageIndex, int pageSize, string orderBy, out int totalCount, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return QueryPageList(dbContext, pageIndex,pageSize,orderBy,out totalCount,whereLambda);
            }
        }

        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return QueryCount(dbContext, whereLambda);
            }
        }

        /// <summary>
        /// DataTable数据源
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <returns>DataTable</returns>
        public DataTable QueryDataTable<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return QueryDataTable(dbContext, whereLambda);
            }
        }

        /// <summary>
        /// DataTable数据源
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>DataTable</returns>
        public DataTable QueryDataTable(string sql)
        {

            using (var dbContext = CreateContext())
            {
                return QueryDataTable(dbContext, sql);
            }
        }

        /// <summary>
        /// Object
        /// </summary> 
        /// <param name="sql">SQL</param> 
        /// <returns>Object</returns>
        public object QuerySqlScalar(string sql)
        {
            using (var dbContext = CreateContext())
            {
                return QuerySqlScalar(dbContext, sql);
            }
        }
        public int ExecuteNonQuery(string sql, Hashtable paramList = null)
        {
            using (var dbContext = CreateContext())
            {
                return ExecuteNonQuery(dbContext, sql, paramList);
            }
        }
        #endregion

        #region 常用函数

        /// <summary>
        /// 对象是否存在
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <returns>True or False</returns>
        public bool IsExist<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return IsExist(dbContext, whereLambda);
            }
        }

        /// <summary>
        /// 总和
        /// </summary>
        /// <param name="field">字段名</param>
        /// <returns>总和</returns>
        public int Sum<TEntity>(string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Sum<TEntity>(dbContext, field, whereLambda);
            }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="field">字段名</param>
        /// <typeparam name="TResult">泛型结果</typeparam>
        /// <returns>最大值</returns>
        public TResult Max<TEntity, TResult>(string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new()
        {
            using (var dbContext = CreateContext())
            {
                return Max<TEntity, TResult>(dbContext, field, whereLambda);
            }
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <param name="field">字段名</param>
        /// <typeparam name="TResult">泛型结果</typeparam>
        /// <returns>最小值</returns>
        public TResult Min<TEntity, TResult>(string field, Expression<Func<TEntity, bool>> whereLambda = null)
        {
            using (var dbContext = CreateContext())
            {
                return Min<TEntity, TResult>(dbContext, field,whereLambda);
            }
        }

        #endregion
        #endregion


    }
}
