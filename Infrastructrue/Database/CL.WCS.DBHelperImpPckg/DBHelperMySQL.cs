using System;
using System.Data;
using CLDC.Framework.Log;
using System.Collections;
using System.Data.Common;
using System.Windows.Controls;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using MySql.Data.MySqlClient;

namespace CL.WCS.DBHelperImpPckg
{
    public class DBHelperMySql : IDbHelper
    {
        public event PrintLogToUiHanlder PrintLogToUIEvent;

        private readonly string _dbConnectionString = string.Empty;

        private const int TimeOut = 180;
        private const string txtName = "数据库操作异常日志";
        private readonly string _databaseName = string.Empty;

       public  DBHelperMySql()
        {
            
        }

        public DBHelperMySql(string connectionString, string strDatabaseName)
        {
            this._dbConnectionString = connectionString;
            _databaseName = strDatabaseName;
        }

        /// <summary>
        ///执行SQL语句，返回结果集中第一行的第一列
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public string ExecuteScalar(string strSql)
        {
            string val = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strSql))
                    {
                        cmd.Connection = conn;
                        cmd.CommandTimeout = TimeOut;
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            val = obj.ToString();
                        }
                        conn.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Log.getMessageFile(txtName).Info("方法名称：ExecuteScalar(string strSql)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strSql, ex.Message);
            }
            return val;
        }

        /// <summary>
        /// 执行SQL语句,并返回是否成功
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string strSql)
        {
            int ret = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strSql, conn))
                    {
                        cmd.CommandTimeout = TimeOut;
                        ret = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getMessageFile(txtName).Info("方法名称：ExecuteNonQuery(string strSql)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strSql, ex.Message);

            }
            return ret > 0 ? true : false;
        }

        /// <summary>
        ///  执行SQL语句,并返回是否成功
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="paramList">参数列表</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string strSql, Hashtable paramList)
        {
            int ret = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strSql, conn))
                    {
                        cmd.CommandTimeout = TimeOut;
                        if (paramList != null && paramList.Keys.Count > 0)
                        {
                            foreach (string pn in paramList.Keys)
                            {
                                cmd.Parameters.Add(new MySqlParameter(pn, paramList[pn]));
                            }
                        }
                        ret = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        conn.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Log.getMessageFile(txtName).Info("方法名称：ExecuteNonQuery(string strSql, Hashtable paramList)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strSql, ex.Message);

            }
            return ret > 0 ? true : false;
        }

        /// <summary>
        /// 执行多条SQL语句,并返回受影响的行数
        /// </summary>
        /// <param name="cmdText">SQL语句数组</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string[] cmdText)
        {
            int val = 0;
            using (MySqlConnection myConnection = new MySqlConnection(_dbConnectionString))
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                {
                    myConnection.Open();
                }
                using (MySqlCommand myCommand = myConnection.CreateCommand())
                {
                    myCommand.CommandTimeout = TimeOut;
                    using (MySqlTransaction myTrans = myConnection.BeginTransaction())
                    {
                        myCommand.Transaction = myTrans;
                        try
                        {
                            foreach (string strSql in cmdText)
                            {
                                if (string.IsNullOrEmpty(strSql))
                                {
                                    continue;
                                }
                                myCommand.CommandText = strSql;
                                int i = myCommand.ExecuteNonQuery();
                                if (i > 0)
                                {
                                    val = val + i;
                                }
                            }
                            myTrans.Commit();
                        }
                        catch (Exception ex)
                        {
                            val = 0;
                            myTrans.Rollback();
                            myConnection.Close();
                            string strSql = string.Join("\r\n", cmdText);
                            Log.getMessageFile(txtName).Info("方法名称：ExecuteNonQuery(string[] cmdText)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                            if (PrintLogToUIEvent != null)
                                PrintLogToUIEvent(_databaseName, strSql, ex.Message);
                        }
                        myConnection.Close();
                    }

                }

            }
            return val;
        }

        /// <summary>
        /// 执行多条SQL语句,并返回受影响的行数
        /// </summary>
        /// <param name="list">SQL语句集合</param>
        /// <returns></returns>
        public int ExecuteNonQuery(ArrayList AL)
        {
            string[] strs = new string[AL.Count];
            for (int i = 0; i < AL.Count; i++)
            {
                strs[i] = AL[i].ToString();
            }
            return ExecuteNonQuery(strs);
        }

        /// <summary>
        /// 执行多条SQL语句,并返回受影响的行数
        /// </summary>
        /// <param name="parameters">SQL参数</param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string strSql, DbParameter[] parameters)
        {
            int ret = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strSql, conn))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        cmd.CommandTimeout = TimeOut;
                        ret = cmd.ExecuteNonQuery();
                        conn.Close();  
                    }
                }
            }
            catch (Exception ex)
            {
                Log.getMessageFile(txtName).Info("方法名称：ExecuteNonQuery(string strSql)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strSql, ex.Message);

            }
            return ret > 0 ? true : false;
        }

        /// <summary>
        /// 执行单条SQL语句,并返回数据集
        /// </summary>
        /// <param name="cmdText">SQL语句数组</param>
        /// <returns></returns>
        public DataSet GetDataSet(string strSql)
        {
            DataSet ds = GetDataSet(new string[] { strSql });
            return ds;
        }

        /// <summary>
        /// 执行多条SQL语句,并返回数据集
        /// </summary>
        /// <param name="cmdText">SQL语句数组</param>
        /// <returns></returns>
        public DataSet GetDataSet(string[] cmdText)
        {
            DataSet ds = new DataSet();
            try
            {
                using (MySqlConnection myConnection = new MySqlConnection(_dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    foreach (string strSql in cmdText)
                    {
                        if (string.IsNullOrEmpty(strSql))
                        {
                            continue;
                        }
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(strSql, myConnection))
                        {
                            DataTable dt = new DataTable();
                            sda.SelectCommand.CommandTimeout = 180;
                            sda.Fill(dt);
                            ds.Tables.Add(dt);
                            sda.Dispose();
                            dt.Dispose();
                        }
                    }
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                ds = null;
                string strSql = string.Join("\r\n", cmdText);
                Log.getMessageFile(txtName)
                    .Info("方法名称：GetDataSet(string[] cmdText)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strSql, ex.Message);
            }
            return ds;


        }

        /// <summary>
        /// 执行存储过程,并返回数据集
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="InputParms">存储过程参数</param>
        /// <returns></returns>
        public DataSet ExecuteProcedure(string strProcedureName, params string[] InputParms)
        {
            DataSet ds = new DataSet();
            try
            {
                using (MySqlConnection myConnection = new MySqlConnection(_dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strProcedureName, myConnection))
                    {
                        cmd.CommandTimeout = TimeOut;
                        cmd.CommandType = CommandType.StoredProcedure;
                        MySqlCommandBuilder.DeriveParameters(cmd);
                        for (int i = 0; i < InputParms.Length; i++)
                        {
                            cmd.Parameters[i + 1].Value = InputParms[i];
                        }
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                            myConnection.Close();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                ds = null;
                string strSql = string.Join("\r\n", InputParms);
                Log.getMessageFile(txtName).Info("方法名称：ExecuteProcedure(string strProcedureName, params string[] InputParms)  " + "\r\n异常存储过程名称：" + strProcedureName + " 具体参数列表" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strProcedureName + "参数列表：" + strSql, ex.Message);
                throw ex;
            }
            return ds;

        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns></returns>
        public int ExecuteProcedure(string strProcedureName, params DbParameter[] parameters)
        {
            int result = -1;
            try
            {
                using (MySqlConnection myConnection = new MySqlConnection(_dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strProcedureName, myConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = TimeOut;
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                if (parameter == null)
                                    continue;
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        result = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        myConnection.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Log.getMessageFile(txtName).Info("方法名称：ExecuteProcedure(string strProcedureName,  params MySqlParameter[] parameters)  " + "\r\n异常存储过程名称：" + strProcedureName + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strProcedureName, ex.Message);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 执行存储过程,并返回数据集[DataSet]
        /// </summary>
        /// <param name="strProcedureName">存储过程名字</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回DataSet</returns>
        public DataSet ExecuteProcedurePar(string strProcedureName, params DbParameter[] parameters)
        {

            DataSet ds = new DataSet();
            try
            {
                using (MySqlConnection myConnection = new MySqlConnection(_dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(strProcedureName, myConnection))
                    {
                        cmd.CommandTimeout = TimeOut;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                if (parameter == null)
                                    continue;
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                            myConnection.Close();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                ds = null;
                Log.getMessageFile(txtName).Info("方法名称：ExecuteProcedurePar(string strProcedureName,  params MySqlParameter[] parameters)  " + "\r\n异常存储过程名称：" + strProcedureName + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_databaseName, strProcedureName, ex.Message);
                throw ex;
            }
            return ds;
        }

        public UserControl GetConfigurationView()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取系统运行的数据库名称
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            return _databaseName;
        }
    }
}
