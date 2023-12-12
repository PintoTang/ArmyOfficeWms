using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Controls;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DbHelper
{
	/// <summary>
	/// 打印界面
	/// </summary>
	/// <param name="databaseName"></param>
	/// <param name="strSql"></param>
	/// <param name="message"></param>
	public delegate void PrintLogToUiHanlder(string databaseName, string strSql, string message);

	/// <summary>
	/// 数据库操作
	/// </summary>
	public interface IDbHelper
	{
		ISqlSugarClient CreateContext();

		/// <summary>
		/// 获取系统运行的数据库名称
		/// </summary>
		/// <returns></returns>
		string GetDatabaseName();

        /// <summary>
        /// 获取配置界面
        /// </summary>
        /// <returns></returns>
	    UserControl GetConfigurationView();


		#region 新增操作
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>受影响行数</returns>
		int Add<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new();
		/// <summary>
		/// 批量新增实体
		/// </summary>
		/// <param name="entitys">实体集合</param>
		/// <returns>受影响行数</returns>
		int Add<TEntity>(ISqlSugarClient dbContext, List<TEntity> entitys) where TEntity : class, new();
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>返回当前实体</returns>
		TEntity AddReturnEntity<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new();
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>自增ID</returns>
		int AddReturnIdentity<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new();
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>成功或失败</returns>
		bool AddReturnBool<TEntity>(ISqlSugarClient dbContext, TEntity entity) where TEntity : class, new();
        #endregion

        #region 更新实体
        /// <summary>
        /// 更新实体(不是Dto)
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        int Update<TEntity>(ISqlSugarClient dbContext, TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 批量更新实体
		/// </summary>
		/// <param name="entitys">实体集合</param>
		/// <param name="lstIgnoreColumns">忽略列</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(ISqlSugarClient dbContext, List<TEntity> entitys, List<string> lstIgnoreColumns = null,
			bool isLock = true) where TEntity : class, new();

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="where">条件表达式</param>
        /// <param name="lstIgnoreColumns">忽略列</param>
        /// <param name="isLock">是否加锁</param>
        /// <returns>受影响行数</returns>
        int Update<TEntity>(ISqlSugarClient dbContext, TEntity entity, Expression<Func<TEntity, bool>> where,
			List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="update">实体对象</param>
		/// <param name="where">条件表达式</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, TEntity>> update,
			Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="keyValues">键:字段名称 值：值</param>
		/// <param name="where">条件表达式</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(ISqlSugarClient dbContext, Dictionary<string, object> keyValues,
			Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new();
		#endregion

		#region 删除操作
		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="id">主键ID</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		bool DeleteByPrimary<TEntity>(ISqlSugarClient dbContext, object id, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 批量删除实体
		/// </summary>
		/// <param name="primaryKeyValues">主键ID集合</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int DeleteByPrimary<TEntity>(ISqlSugarClient dbContext, List<object> primaryKeyValues, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Delete<TEntity>(ISqlSugarClient dbContext, TEntity entity, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 批量删除实体
		/// </summary>
		/// <param name="entitys">实体集合</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Delete<TEntity>(ISqlSugarClient dbContext, List<TEntity> entitys, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="whereLambda">条件表达式</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Delete<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda, bool isLock = true) where TEntity : class, new();
		#endregion

		#region 查询
		/// <summary>
		/// 查询单个
		/// </summary>
		/// <param name="expression">返回表达式</param>
		/// <param name="whereLambda">条件表达式</param>
		/// <typeparam name="TEntity">返回对象</typeparam>
		/// <returns>自定义数据</returns>
		TEntity Query<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// 实体列表
		/// </summary>
		/// <param name="expression">返回表达式</param>
		/// <param name="whereLambda">条件表达式</param>
		/// <typeparam name="TEntity">返回对象</typeparam>
		/// <returns>自定义数据</returns>
		List<TEntity> QueryList<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null, string orderBy = "") where TEntity : class, new();

		/// <summary>
		/// 实体列表
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <returns>实体列表</returns>
		List<TEntity> QuerySqlList<TEntity>(ISqlSugarClient dbContext, string sql) where TEntity : class, new();

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
		List<TEntity> QueryPageList<TEntity>(ISqlSugarClient dbContext, int pageIndex, int pageSize, string orderBy, out int totalCount, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="whereLambda"></param>
		/// <returns></returns>
		int QueryCount<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// DataTable数据源
		/// </summary>
		/// <param name="whereLambda">条件表达式</param>
		/// <returns>DataTable</returns>
		DataTable QueryDataTable<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// DataTable数据源
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <returns>DataTable</returns>
		DataTable QueryDataTable(ISqlSugarClient dbContext, string sql);

		object QuerySqlScalar(ISqlSugarClient dbContext, string sql);
		int ExecuteNonQuery(ISqlSugarClient dbContext, string sql, Hashtable paramList = null);
		#endregion

		#region 常用函数
		bool IsExist<TEntity>(ISqlSugarClient dbContext, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		int Sum<TEntity>(ISqlSugarClient dbContext, string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		TResult Max<TEntity, TResult>(ISqlSugarClient dbContext, string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		TResult Min<TEntity, TResult>(ISqlSugarClient dbContext, string field, Expression<Func<TEntity, bool>> whereLambda = null);
		#endregion

		#region 不带DbContext
		#region 新增操作
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>受影响行数</returns>
		int Add<TEntity>(TEntity entity) where TEntity : class, new();
		/// <summary>
		/// 批量新增实体
		/// </summary>
		/// <param name="entitys">实体集合</param>
		/// <returns>受影响行数</returns>
		int Add<TEntity>(List<TEntity> entitys) where TEntity : class, new();
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>返回当前实体</returns>
		TEntity AddReturnEntity<TEntity>(TEntity entity) where TEntity : class, new();
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>自增ID</returns>
		int AddReturnIdentity<TEntity>(TEntity entity) where TEntity : class, new();
		/// <summary>
		/// 新增实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <returns>成功或失败</returns>
		bool AddReturnBool<TEntity>(TEntity entity) where TEntity : class, new();
		#endregion

		#region 更新实体
		/// <summary>
		/// 更新实体(不是Dto)
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <param name="lstIgnoreColumns">忽略列</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(TEntity entity, List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 批量更新实体
		/// </summary>
		/// <param name="entitys">实体集合</param>
		/// <param name="lstIgnoreColumns">忽略列</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(List<TEntity> entitys, List<string> lstIgnoreColumns = null,
			bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <param name="where">条件表达式</param>
		/// <param name="lstIgnoreColumns">忽略列</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where,
			List<string> lstIgnoreColumns = null, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="update">实体对象</param>
		/// <param name="where">条件表达式</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(Expression<Func<TEntity, TEntity>> update,
			Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 更新实体
		/// </summary>
		/// <param name="keyValues">键:字段名称 值：值</param>
		/// <param name="where">条件表达式</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Update<TEntity>(Dictionary<string, object> keyValues,
			Expression<Func<TEntity, bool>> where = null, bool isLock = true) where TEntity : class, new();
		#endregion

		#region 删除操作
		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="id">主键ID</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		bool DeleteByPrimary<TEntity>(object id, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 批量删除实体
		/// </summary>
		/// <param name="primaryKeyValues">主键ID集合</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int DeleteByPrimary<TEntity>(List<object> primaryKeyValues, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="entity">实体对象</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Delete<TEntity>(TEntity entity, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 批量删除实体
		/// </summary>
		/// <param name="entitys">实体集合</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Delete<TEntity>(List<TEntity> entitys, bool isLock = true) where TEntity : class, new();

		/// <summary>
		/// 删除实体
		/// </summary>
		/// <param name="whereLambda">条件表达式</param>
		/// <param name="isLock">是否加锁</param>
		/// <returns>受影响行数</returns>
		int Delete<TEntity>(Expression<Func<TEntity, bool>> whereLambda, bool isLock = true) where TEntity : class, new();
		#endregion

		#region 查询
		/// <summary>
		/// 查询单个
		/// </summary>
		/// <param name="expression">返回表达式</param>
		/// <param name="whereLambda">条件表达式</param>
		/// <typeparam name="TEntity">返回对象</typeparam>
		/// <returns>自定义数据</returns>
		TEntity Query<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// 实体列表
		/// </summary>
		/// <param name="expression">返回表达式</param>
		/// <param name="whereLambda">条件表达式</param>
		/// <typeparam name="TEntity">返回对象</typeparam>
		/// <returns>自定义数据</returns>
		List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null,string orderBy="") where TEntity : class, new();

		/// <summary>
		/// 实体列表
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <returns>实体列表</returns>
		List<TEntity> QuerySqlList<TEntity>(string sql) where TEntity : class, new();

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
		List<TEntity> QueryPageList<TEntity>(int pageIndex, int pageSize, string orderBy, out int totalCount, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="whereLambda"></param>
		/// <returns></returns>
		int QueryCount<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// DataTable数据源
		/// </summary>
		/// <param name="whereLambda">条件表达式</param>
		/// <returns>DataTable</returns>
		DataTable QueryDataTable<TEntity>( Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		/// <summary>
		/// DataTable数据源
		/// </summary>
		/// <param name="sql">SQL</param>
		/// <returns>DataTable</returns>
		DataTable QueryDataTable(string sql);

		object QuerySqlScalar(string sql);

		int ExecuteNonQuery(string sql, Hashtable paramList = null);
		#endregion

		#region 常用函数
		bool IsExist<TEntity>(Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		int Sum<TEntity>(string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		TResult Max<TEntity, TResult>(string field, Expression<Func<TEntity, bool>> whereLambda = null) where TEntity : class, new();

		TResult Min<TEntity, TResult>(string field, Expression<Func<TEntity, bool>> whereLambda = null);
		#endregion
		#endregion
	}
}
