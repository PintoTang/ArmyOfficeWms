using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Framework.Log;

namespace CL.Framework.Testing.OPCClientImpPckg.SqlSever
{
    public sealed class OpcItemForSqlServer : OpcItemAbstract
    {
        private readonly IDbHelper DbHelper;
        public OpcItemForSqlServer(IDbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }
        /// <summary>
        /// 模拟OPC读取项值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回 DataItem</returns>
        public override DataTable Read(string groupName, string itemName)
        {
            try
            {
                string sql = "SELECT * FROM OPC_ITEMMANAGER where GROUPNAME='" + groupName + "'" + "and ITEMNAME='" + itemName + "'";
                using (DataSet dt = DbHelper.GetDataSet(sql))
                {
                    if (dt != null && dt.Tables.Count > 0)
                    {
                        return dt.Tables[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123, ex.Message);
                return null;
            }
        }

        public override DataTable Read(string groupName, List<string> itemNames)
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
                using (DataSet dt = DbHelper.GetDataSet(sql))
                {
                    if (dt != null && dt.Tables.Count > 0)
                    {
                        return dt.Tables[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询所有的地址项
        /// </summary>
        /// <returns></returns>
        public override DataTable ReadAllItems(string groupName)
        {
            try
            {
                string sql = "SELECT ITEMNAME FROM OPC_ITEMMANAGER where GroupName='" + groupName + "'";
                using (DataSet dt = DbHelper.GetDataSet(sql))
                {
                    if (dt != null && dt.Tables.Count > 0)
                    {
                        return dt.Tables[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 模拟OPC添加值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>添加值的结果true成功，false 失败</returns>
        public override bool Write(string groupName, string itemName)
        {
            string queryStr = "select count(1) from OPC_ITEMMANAGER where ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
            object obj=DbHelper.ExecuteScalar(queryStr);
            int count = Convert.ToInt32(obj);
            if (count > 0)
            {
                return true;
            }
            //using (DataSet dt = DbHelper.GetDataSet(queryStr))
            //{
            //    if (dt != null && dt.Tables.Count > 0)
            //    {
            //        if (int.Parse(dt.Tables[0].Rows[0][0].ToString()) > 0)
            //        {
            //            return true;
            //        }
            //    }
            //}
            //向数据库中添加单项
            string sql = "INSERT INTO OPC_ITEMMANAGER (GROUPNAME,ITEMNAME,Value) VALUES('"
                          + groupName + "','" + itemName + "','0') ";
            bool result = DbHelper.ExecuteNonQuery(sql);
            return result;
        }

        public override void Write(string groupName, Dictionary<string, object> itemValueList)
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

        public override void Write(string groupName, Dictionary<string, int> itemValueList)
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

        public override void Write(string groupName, string itemName, string value = "0")
        {
            string queryStr = "select count(1) from OPC_ITEMMANAGER where ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
            object obj = DbHelper.ExecuteScalar(queryStr);
            int count = Convert.ToInt32(obj);
            if (count > 0)
            {
                string cmd = "update OPC_ITEMMANAGER set VALUE='" + value + "' where ITEMNAME='" + itemName + "' and GROUPNAME='" + groupName + "'";
                bool ret = DbHelper.ExecuteNonQuery(cmd);
                if (!ret)
                {
                    Console.WriteLine("TestOpcWrite:" + "itemName " + " 写入失败");
                }
                return;
            }
            string sql = "INSERT INTO OPC_ITEMMANAGER (GROUPNAME,ITEMNAME,Value) VALUES('"
                         + groupName + "','" + itemName + "','" + value + "') ";
            DbHelper.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 修改某项值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">PLC地址名称</param>
        /// <param name="setValue">PLC地址对应的值</param>
        /// <returns></returns>
        public override bool Update(string groupName, string itemName, string setValue)
        {

            string sql = "UPDATE OPC_ITEMMANAGER  set value='" + setValue + "' where groupname='" + groupName + "' and itemname='" + itemName + "'";
            bool result = DbHelper.ExecuteNonQuery(sql);
            return result;
        }
        /// <summary>
        /// 添加组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>返回添加结果True成功，False失败</returns>
        public override bool AddGroup(string groupName)
        {
            string sql = "insert into OPC_GROUPMANAGER (GROUPID,GROUPNAME)values (NEXT VALUE FOR GROUPID,'"
                                                                                            + groupName + "')";
            bool result = DbHelper.ExecuteNonQuery(sql);
            return result;
        }

        /// <summary>
        /// 读去组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>组信息，ID,和组名称</returns>
        public override DataTable ReadGroup(string groupName)
        {
            try
            {
                //先查询数据库中是否存在
                string sql = "select * from OPC_GROUPMANAGER where GROUPNAME='" + groupName + "'";
                using (DataSet dt = DbHelper.GetDataSet(sql))
                {
                    if (dt != null && dt.Tables.Count > 0)
                    {
                        return dt.Tables[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public override DataTable ReadMaxGroupID()
        {
            try
            {
                string sql = "select MAX(groupId) from OPC_GROUPMANAGER";
                using (DataSet dt = DbHelper.GetDataSet(sql))
                {
                    if (dt != null && dt.Tables.Count > 0)
                    {
                        return dt.Tables[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
