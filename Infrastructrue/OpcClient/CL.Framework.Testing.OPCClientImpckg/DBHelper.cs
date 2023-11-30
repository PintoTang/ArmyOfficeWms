//using CL.Test.Framework.OPCClientImpPckg;
using CLDC.Framework.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CL.Framework.Testing.OPCClientImpPckg
{
    public class DBHelper
    {
        private static DBHelper ins = null;
        public static DBHelper CreateInstance()
        {
            if (ins == null)
            {
                ins = new DBHelper();
            }
            return ins;
        }

        private DBHelper()
        {
            _OracleContrl = new OracleHelper();
            //string[] cmdstr = new string[] { "delete from OPC_ITEMMANAGER", "delete from OPC_GROUPMANAGER" };
            //for (int i = 0; i < cmdstr.Length; i++)
            //{
            //    int ret = _OracleContrl.ExecuteSql(cmdstr[i]);
            //}
           

        }

        private OracleHelper _OracleContrl;


        /// <summary>
        /// 模拟OPC读取项值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回 DataItem</returns>
        public DataTable Read(string groupName, string itemName)
        {
            try
            {
                string sql = "SELECT * FROM OPC_ITEMMANAGER where GROUPNAME='" + groupName + "'" + "and ITEMNAME='" + itemName + "'";
                DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, sql);
                return dt;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123,ex.Message);
                return null;
            }
        }

        public DataTable Read(string groupName, List<string> itemNames)
        {
            try
            {
                string sqlValue = "";
                foreach (var item in itemNames)
                {
                    sqlValue += "'" + item + "',";
                }
                sqlValue = sqlValue.TrimEnd(',');
                string sql = "SELECT * FROM OPC_ITEMMANAGER m where m.itemname in (" + sqlValue + ") and m.groupname='" + groupName + "'";
                DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, sql);
                return dt;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 查询所有的地址项
        /// </summary>
        /// <returns></returns>
        public DataTable ReadAllItems(string groupName)
        {
            try
            {
                string sql = "SELECT ITEMNAME FROM OPC_ITEMMANAGER where GroupName='" + groupName + "'";
                DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, sql);
                return dt;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123,123, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 模拟OPC添加值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>添加值的结果true成功，false 失败</returns>
        public bool Write(string groupName, string itemName)
        {
            string queryStr = "select count(*) from OPC_ITEMMANAGER where ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
            DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, queryStr);
            if (int.Parse(dt.Rows[0][0].ToString())>0)
            {
             //   string  str= "UPDATE OPC_ITEMMANAGER SET Value='0' WHERE ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
             //int   _OracleContrl.ExecuteSql(str);
                return true;
            }
            //向数据库中添加单项
            string sql = "INSERT INTO OPC_ITEMMANAGER (ITEMID,GROUPNAME,ITEMNAME,Value) VALUES(ITEMID.nextval,'"
                          + groupName + "','" + itemName + "','0') ";
            int rowCount = _OracleContrl.ExecuteSql(sql);
            if (rowCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Write(string groupName, Dictionary<string, object> itemValueList)
        {
            lock (this)
            {
                foreach (var item in itemValueList)
                {
                    Write(groupName, item.Key, item.Value.ToString());
                    Console.WriteLine("TestOpcWrite:" + item.Key + "   " + item.Value.ToString());
                }
            }
        }

        public void Write(string groupName, Dictionary<string, int> itemValueList)
        {
            lock (this)
            {
                foreach (var item in itemValueList)
                {
                    Write(groupName, item.Key, item.Value.ToString());
                    Console.WriteLine("TestOpcWrite:" + item.Key + "   " + item.Value.ToString());
                }
            }
        }

        public void Write(string groupName, string itemName, string value="0")
        {
            string queryStr = "select count(*) from OPC_ITEMMANAGER where ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
            DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, queryStr);
            if (int.Parse(dt.Rows[0][0].ToString()) > 0)
            {
                string cmd = "update OPC_ITEMMANAGER set VALUE='" + value + "' where ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
                int ret = _OracleContrl.ExecuteSql(cmd);
                if (ret==0)
                {
                      Console.WriteLine("TestOpcWrite:"+"itemName "+" 写入失败");
                }
                return;
            }

            string sql = "INSERT INTO OPC_ITEMMANAGER (ITEMID,GROUPNAME,ITEMNAME,Value) VALUES(ITEMID.nextval,'"
                         + groupName + "','" + itemName + "','" + value + "') ";
            int rowCount = _OracleContrl.ExecuteSql(sql);
        }

        /// <summary>
        /// 修改某项值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">PLC地址名称</param>
        /// <param name="setValue">PLC地址对应的值</param>
        /// <returns></returns>
        public bool Update(string groupName, string itemName, string setValue)
        {

            string sql = "UPDATE OPC_ITEMMANAGER ot set ot.value='" + setValue + "' where ot.groupname='" + groupName + "' and ot.itemname='" + itemName + "'";
            int rowCount = _OracleContrl.ExecuteSql(sql);
            if (rowCount > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>返回添加结果True成功，False失败</returns>
        public bool AddGroup(string groupName)
        {
            string sql = "insert into OPC_GROUPMANAGER (GROUPID,GROUPNAME)values (groupid.nextval,'"
                                                                                            + groupName + "')";
            int rowCount = _OracleContrl.ExecuteSql(sql);
            if (rowCount < 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读去组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>组信息，ID,和组名称</returns>
        public DataTable ReadGroup(string groupName)
        {
            try
            {
                //先查询数据库中是否存在
                string sql2 = "select * from OPC_GROUPMANAGER where GROUPNAME='" + groupName + "'";
                DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, sql2);
                return dt;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123, ex.Message);
                return null;
            }
         
        }

        public DataTable ReadMaxGroupID()
        {
            try
            {
                string sql = "select max( groupId) from OPC_GROUPMANAGER";
                DataTable dt = _OracleContrl.ExecuteDataTable(OracleHelper.DbConnString, sql);
                return dt;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123, ex.Message);
                return null;
            }
            
        }
    }
}
