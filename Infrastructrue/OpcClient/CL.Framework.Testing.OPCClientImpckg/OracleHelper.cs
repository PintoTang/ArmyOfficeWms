using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Configuration;
using CLDC.Framework.Log;
using CL.Framework.ConfigFilePckg;
using System.Data.OracleClient;

namespace CL.Framework.Testing.OPCClientImpPckg
{

    /// <summary>
    /// 数据库的通用访问代码
    /// 此类为抽象类，不允许实例化，在应用时直接调用即可
    /// </summary>
    public class OracleHelper
    {
        /// <summary>
        /// 获取数据库连接字符串，其属于静态变量且只读，项目中所有文档可以直接使用，但不能修改
        /// 此处暂不用配置文件，直接引用
        /// </summary>
        public static string DbConnString ="";// ConfigurationManager.AppSettings["DbConnString"];
       
        public OracleHelper()
        {
            try
            {
                ConfigFile configFile = new ConfigFile("Config/App.config");
                DbConnString = configFile.AppSettings["DbConnString"].ToString();

            }
            catch
            {
              
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="str">sql</param>
        /// <returns></returns>
        public bool Update(string str)
        {
            OracleConnection conn = new OracleConnection(DbConnString);
            OracleCommand cmd = new OracleCommand(str, conn);
            try
            {
                conn.Open();
                int i = cmd.ExecuteNonQuery();
                conn.Close();
                if (i >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                return false;
            }
        }
        /// <summary>
        /// 哈希表用来存储缓存的参数信息，哈希表可以存储任意类型的参数
        /// </summary>
        private Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 执行一个不需要返回值的OracleCommand命令，通过指定专用的连接字符串
        /// 使用参数数组形式提供参数列表
        /// </summary>
        /// <remarks>
        /// 使用示例：
        /// int result = ExecuteNonQuery(connString,CommandType.StoredProcedure,"PulishOrders",new OracleParameter("@prodid",24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParamters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParamters)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    //通过PrePareCommand方法将参数逐个加入到OracleCommand的参数集合中
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParamters);
                    int val = cmd.ExecuteNonQuery();
                    //清空OracleCommand中的参数列表
                    cmd.Parameters.Clear();
                    return val;
                }
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }
        /// <summary>
        /// 执行删除检定任务的存储过程
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="con_id">检定任务ID</param>
        /// <returns></returns>
        public int ExecuteInsertChkRuslt(string connectionString, int count, string guidid)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "EXEC_INSERTCHECKRESULT";
                    cmd.CommandType = CommandType.StoredProcedure;
					OracleParameter CONID = cmd.Parameters.Add("I_COUNT", OracleType.Int32);
					OracleParameter GUIDID = cmd.Parameters.Add("V_GUIDID", OracleType.Char, 15);
					OracleParameter RETURN = cmd.Parameters.Add("I_RETURN", OracleType.Int32);
                    //指定参数方向          
                    CONID.Direction = ParameterDirection.Input;
                    GUIDID.Direction = ParameterDirection.Input;
                    RETURN.Direction = ParameterDirection.Output;

                    CONID.Value = count;
                    GUIDID.Value = guidid;

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    connection.Dispose();
                    return int.Parse(RETURN.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
				Log.getMessageFile("TestingOPC").Info(ex.StackTrace.ToString());

                return 1;
            }

        }

        /// <summary>
        /// 执行一个不需要返回值的OracleCommand命令，通过一个已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：
        /// int result = ExecuteNonQuery(conn,CommandType.StoredProcedure,"PulishOrders",new OracleParameter("@prodid",24));
        /// </remarks>
        /// <param name="connection">一个现有的数据库连接</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();

                //清空OracleCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }


        /// <summary>
        /// 执行一个不需要返回值的OracleCommand命令，通过一个已经存在的数据库事务处理
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：
        /// int result = ExecuteNonQuery(trans,CommandType.StoredProcedure,"PulishOrders",new OracleParameter("@prodid",24));
        /// </remarks>
        /// <param name="trans">一个存在的Oracle事务处理</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此OracleCommand命令执行后影响的行数</returns>
        public int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();

                //清空OracleCommand中的参数列表
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }


        /// <summary>
        /// 执行一条返回结果集的OracleCommand命令，通过专用的连接字符串
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  OracleDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的OracleDataReader</returns>
        public OracleDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);
            OracleDataReader rdr = null;

            /**********
            在这里使用try/catch处理是因为如果方法出现异常，则OracleDataReader就不存在，
            CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            关闭数据库连接，并通过throw再次引发捕捉到的异常。
            **********/
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch (Exception ex)
            {
                conn.Close();
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="dataname"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, string dataname, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);
            //SqlDataReader rdr = null;

            /**********
            在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            关闭数据库连接，并通过throw再次引发捕捉到的异常。
            **********/
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataAdapter adp = new OracleDataAdapter();
                cmd.Connection = conn;
                adp.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adp.Fill(ds, dataname);

                cmd.Parameters.Clear();
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                return ds;
            }
            /*
            catch (OleDbException dbEx)
            {
                conn.Close();
                throw dbEx;
            }
            */
            catch (Exception ex)
            {
                conn.Close();
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }
        /// <summary>
        /// 执行一条返回第一条记录第一列的OracleCommand命令，通过专用的连接字符串
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例：  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    object val = cmd.ExecuteScalar();

                    cmd.Parameters.Clear();
                    return val;
                }
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }


        /// <summary>
        /// 执行一条返回第一条记录第一列的OracleCommand命令，通过已经存在的数据库连接
        /// 使用参数数组提供参数
        /// </summary>
        /// <remarks>
        /// 使用示例： 
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new OracleParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">一个已经存在的数据库连接</param>
        /// <param name="commandType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个object类型的数据，可以通过 Convert.To{Type}方法转换类型</returns>
        public object ExecuteScalar(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }


        /// <summary>
        /// 缓存参数数组
        /// </summary>
        /// <param name="cacheKey">参数缓存的键值</param>
        /// <param name="commandParameters">被缓存的参数列表</param>
        public void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }


        /// <summary>
        /// 获取被缓存的参数
        /// </summary>
        /// <param name="cacheKey">用于查找参数的KEY值</param>
        /// <returns>返回缓存的参数数组</returns>
        public OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] cachedParms = (OracleParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            //新建一个参数的克隆列表
            OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];

            //通过循环为克隆参数列表赋值
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                //使用clone方法复制参数列表中的参数
                clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }


        /// <summary>
        /// 为执行命令准备参数
        /// </summary>
        /// <param name="cmd">OracleCommand命令</param>
        /// <param name="conn">已经存在的数据库连接</param>
        /// <param name="trans">数据库事物处理</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="cmdParms">以数组形式提供OracleCommand命令中用到的参数列表</param>
        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //判断是否需要事务处理
            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// 执行一条返回第一条记录第一列的OracleCommand命令，通过专用的连接字符串
        /// 使用参数数组提供参数
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="cmdType">OracleCommand命令类型 (存储过程<StoredProcedure>，表的名称<TableDirect>， T-SQL 文本命令语句<Text>(默认)， 等等。)</param>
        /// <param name="cmdText">存储过程的名字或者表的名字或者 T-SQL 文本命令语句</param>
        /// <param name="commandParameters">以数组形式提供OracleCommand命令中用到的参数列表</param>
        /// <returns>返回一个包含结果的DataTable</returns>
        public DataTable ExecuteDataTable(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    OracleCommand cmd = new OracleCommand();
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    OracleDataAdapter adp = new OracleDataAdapter();
                    cmd.Connection = conn;
                    adp.SelectCommand = cmd;
                    adp.Fill(dt);
                    cmd.Parameters.Clear();
                }
                return dt;
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }

        /// <summary>
        /// 输入一条 查询的SQL语句，返回一个DataTable的结果集
        /// </summary>
        /// <param name="ConnString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string ConnString, string cmdText)
        {
            OracleCommand cmd = new OracleCommand();
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnString))
                {
                    DataSet ds = new DataSet();
                    OracleDataAdapter odda = new OracleDataAdapter(cmdText, conn);
                    odda.Fill(ds, "table");
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                //  return new DataTable();
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                throw ex;
            }
        }

        /// <summary>
        /// 输入一条 查询的SQL语句，返回一个DataTable的结果集
        /// </summary>
        /// <param name="ConnString"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public string[] ExecutedStrings(string ConnString, string cmdText)
        {
            string[] str = null;
            try
            {
                using (OracleConnection conn = new OracleConnection(ConnString))
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand(cmdText, conn);
                    OracleDataReader read = cmd.ExecuteReader();
                    if (read.Read())
                    {
                        str = new string[read.FieldCount];
                        for (int i = 0; i < read.FieldCount; i++)
                        {
                            str[i] = read[i].ToString();
                        }
                        return str;
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                //数据库错误多为致命错误，所以暂时全部上抛处理.
                //  return new DataTable();
                //数据库错误多为致命错误，所以暂时全部上抛处理.
				Log.getMessageFile("TestingOPC").Info(ex.StackTrace.ToString());
                return str;
            }
        }

        /// <summary>
        /// 组装Insert语句
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="Cols">字段</param>
        /// <returns>返回sql语句</returns>
        public string GetInsertSql(String TableName, Hashtable Cols)
        {
            int Count = 0;

            if (Cols.Count <= 0)
            {
                return "";
            }

            String Fields = " (";
            String Values = " Values(";
            foreach (DictionaryEntry item in Cols)
            {
                if (item.Value != null)
                {
                    if (Count != 0)
                    {
                        Fields += ",";
                        Values += ",";
                    }
                    Fields += item.Key.ToString();
                    Values += item.Value.ToString();
                }
                Count++;
            }
            Fields += ")";
            Values += ")";

            String SqlString = "Insert into " + TableName + Fields + Values;
            return SqlString;
        }

        /// <summary>
        /// 组装Update语句
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="Cols">字段</param>
        /// <param name="Where">条件语句</param>
        /// <returns>返回sql语句</returns>
        public string GetUpdateSql(String TableName, Hashtable Cols, String Where)
        {
            int Count = 0;
            if (Cols.Count <= 0)
            {
                return "";
            }
            String Fields = " ";
            foreach (DictionaryEntry item in Cols)
            {
                if (Count != 0)
                {
                    Fields += ",";
                }
                Fields += item.Key.ToString();
                Fields += "=";
                Fields += item.Value.ToString();
                Count++;
            }
            Fields += " ";

            String SqlString = "Update " + TableName + " Set " + Fields + " where 1=1 and " + Where;

            return SqlString;
        }

        /// <summary>
        /// 组装Select语句
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="Cols">字段</param>
        /// <param name="Where">条件语句</param>
        /// <returns>返回sql语句</returns>
        public string GetSelectSql(String TableName, List<string> Cols, String Where)
        {
            int Count = 0;
            if (Cols.Count <= 0)
            {
                return "";
            }
            String Fields = " ";
            foreach (string item in Cols)
            {
                if (Count != 0)
                {
                    Fields += ",";
                }
                Fields += item;
                Count++;
            }
            Fields += " ";

            String SqlString = "Select " + Fields + " from " + TableName + " where 1=1 and " + Where;

            return SqlString;
        }

        #region  青海添加
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(DbConnString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        throw new Exception(E.Message);
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(DbConnString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(DbConnString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.OracleClient.OracleException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string connStr, string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.OracleClient.OracleException e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }
        }



        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public OracleDataReader ExecuteReader(string SQLString, params OracleParameter[] cmdParms)
        {
            OracleConnection connection = new OracleConnection(DbConnString);
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.OracleClient.OracleException e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(DbConnString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.OracleClient.OracleException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Exists(string strSql, params OracleParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}