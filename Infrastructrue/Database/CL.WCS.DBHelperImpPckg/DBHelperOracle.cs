using System;
using System.Data;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Framework.Log;
using System.Collections;
using System.Data.Common;
using System.Data.OracleClient;
using System.Windows.Controls;

namespace CL.WCS.DBHelperImpPckg
{
    public class DBHelperOracle : IDbHelper
    {
        public event PrintLogToUiHanlder PrintLogToUIEvent;

        private string dbConnectionString = string.Empty;

        private const int TimeOut = 180;
        private const string txtName = "数据库操作异常日志";
        private string _DatabaseName = string.Empty;

        public DBHelperOracle(string ConnectionString, string strDatabaseName)
        {
            this.dbConnectionString = ConnectionString;
            _DatabaseName = strDatabaseName;
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
                using (OracleConnection conn = new OracleConnection(dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strSql))
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
                    PrintLogToUIEvent(_DatabaseName, strSql, ex.Message);
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
                using (OracleConnection conn = new OracleConnection(dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strSql, conn))
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
                    PrintLogToUIEvent(_DatabaseName, strSql, ex.Message);

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
                using (OracleConnection conn = new OracleConnection(dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strSql, conn))
                    {
                        cmd.CommandTimeout = TimeOut;
                        if (paramList != null && paramList.Keys.Count > 0)
                        {
                            foreach (string pn in paramList.Keys)
                            {
                                cmd.Parameters.Add(new OracleParameter(pn, paramList[pn]));
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
                    PrintLogToUIEvent(_DatabaseName, strSql, ex.Message);

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
            using (OracleConnection myConnection = new OracleConnection(dbConnectionString))
            {
                if (myConnection.State != System.Data.ConnectionState.Open)
                {
                    myConnection.Open();
                }
                using (OracleCommand myCommand = myConnection.CreateCommand())
                {
                    myCommand.CommandTimeout = TimeOut;
                    using (OracleTransaction myTrans = myConnection.BeginTransaction())
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
                                PrintLogToUIEvent(_DatabaseName, strSql, ex.Message);
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
                using (OracleConnection conn = new OracleConnection(dbConnectionString))
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strSql, conn))
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
                    PrintLogToUIEvent(_DatabaseName, strSql, ex.Message);

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
                using (OracleConnection myConnection = new OracleConnection(dbConnectionString))
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
                        DataTable dt = new DataTable();
                        using (OracleDataAdapter sda = new OracleDataAdapter(strSql, myConnection))
                        {
                            sda.SelectCommand.CommandTimeout = 180;
                            sda.Fill(dt);
                            ds.Tables.Add(dt);
                        }
                    }
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                ds = null;
                string strSql = string.Join("\r\n", cmdText);
                Log.getMessageFile(txtName).Info("方法名称：GetDataSet(string[] cmdText)  " + "\r\n异常SQL语句：" + strSql + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, strSql, ex.Message);
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
                using (OracleConnection myConnection = new OracleConnection(dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strProcedureName, myConnection))
                    {
                        cmd.CommandTimeout = TimeOut;
                        cmd.CommandType = CommandType.StoredProcedure;
                        OracleCommandBuilder.DeriveParameters(cmd);
                        for (int i = 0; i < InputParms.Length; i++)
                        {
                            cmd.Parameters[i + 1].Value = InputParms[i];
                        }
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
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
                    PrintLogToUIEvent(_DatabaseName, strProcedureName + "参数列表：" + strSql, ex.Message);
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
                using (OracleConnection myConnection = new OracleConnection(dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strProcedureName, myConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = TimeOut;
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (OracleParameter parameter in parameters)
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
                Log.getMessageFile(txtName).Info("方法名称：ExecuteProcedure(string strProcedureName,  params OracleParameter[] parameters)  " + "\r\n异常存储过程名称：" + strProcedureName + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, strProcedureName, ex.Message);
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
                using (OracleConnection myConnection = new OracleConnection(dbConnectionString))
                {
                    if (myConnection.State != System.Data.ConnectionState.Open)
                    {
                        myConnection.Open();
                    }
                    using (OracleCommand cmd = new OracleCommand(strProcedureName, myConnection))
                    {
                        cmd.CommandTimeout = TimeOut;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (OracleParameter parameter in parameters)
                            {
                                if (parameter == null)
                                    continue;
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
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
                Log.getMessageFile(txtName).Info("方法名称：ExecuteProcedurePar(string strProcedureName,  params OracleParameter[] parameters)  " + "\r\n异常存储过程名称：" + strProcedureName + "\r\n异常具体信息：" + ex.Message);
                if (PrintLogToUIEvent != null)
                    PrintLogToUIEvent(_DatabaseName, strProcedureName, ex.Message);
                throw ex;
            }
            return ds;
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
    }
}
