using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.Testing.OPCClientImpPckg
{
    public sealed class OpcItemForSqlSugar : OpcItemAbstract
    {
        private readonly IDbHelper DbHelper;
        public OpcItemForSqlSugar(IDbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }

        /// <summary>
        /// 模拟OPC读取项值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">项名称</param>
        /// <returns>返回 DataItem</returns>
        public override OpcItemManagerEntity Read(string groupName, string itemName)
        {
            try
            {
                OpcItemManagerEntity item = DbHelper.Query<OpcItemManagerEntity>(t => t.GROUPNAME == groupName && t.ITEMNAME == itemName);
                return item;
            }
            catch (Exception ex)
            {
                ConsoleManager.WriteLog(123, 123, ex.Message);
                return null;
            }
        }

        public override List<OpcItemManagerEntity> Read(string groupName, List<string> itemNames)
        {
            try
            {

                List<OpcItemManagerEntity> list = DbHelper.QueryList<OpcItemManagerEntity>(t => t.GROUPNAME == groupName && itemNames.Contains(t.ITEMNAME));
                return list;
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
        public override List<OpcItemManagerEntity> ReadAllItems(string groupName)
        {
            try
            {
                List<OpcItemManagerEntity> list = DbHelper.QueryList<OpcItemManagerEntity>(t => t.GROUPNAME == groupName);
                return list;
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
            int count=DbHelper.QueryCount<OpcItemManagerEntity>(t => t.GROUPNAME == groupName && t.ITEMNAME == itemName);

            if (count > 0)
            {
                return true;
            }
            bool result = DbHelper.AddReturnBool(new OpcItemManagerEntity
            {
                ITEMNAME=itemName,
                GROUPNAME=groupName,
                VALUE="0",
            });
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
            OpcItemManagerEntity item = DbHelper.Query<OpcItemManagerEntity>(t => t.GROUPNAME == groupName && t.ITEMNAME == itemName);
            bool ret = false;
            if (item!=null)
            {
                item.VALUE = value;
                ret = DbHelper.Update(item)>0;
                if (!ret)
                {
                    Console.WriteLine("TestOpcWrite:" + "itemName " + " 写入失败");
                }
                return;
            }
            ret=DbHelper.AddReturnBool(new OpcItemManagerEntity
            {
                GROUPNAME=groupName,
                ITEMNAME=itemName,
                VALUE=value,
            });
            if (!ret)
            {
                Console.WriteLine("TestOpcWrite:" + "itemName " + " 写入失败");
            }
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
            bool result=DbHelper.Update<OpcItemManagerEntity>(t => new OpcItemManagerEntity { VALUE = setValue }, t => t.GROUPNAME == groupName && t.ITEMNAME == itemName)>0;
            //string sql = "UPDATE OPC_ITEMMANAGER  set value='" + setValue + "' where groupname='" + groupName + "' and itemname='" + itemName + "'";
            //bool result = DbHelper.ExecuteNonQuery(sql);
            return result;
        }
        /// <summary>
        /// 添加组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>返回添加结果True成功，False失败</returns>
        public override bool AddGroup(string groupName)
        {
            bool result=DbHelper.AddReturnBool(new OpcGroupManagerEntity
            {
                GROUPNAME=groupName
            });
            return result;
        }

        /// <summary>
        /// 读去组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>组信息，ID,和组名称</returns>
        public override OpcGroupManagerEntity ReadGroup(string groupName)
        {
            try
            {
                OpcGroupManagerEntity item=DbHelper.Query<OpcGroupManagerEntity>(t => t.GROUPNAME == groupName);
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public override decimal ReadMaxGroupID()
        {
            try
            {
                decimal maxId= DbHelper.Max<OpcGroupManagerEntity,decimal>("GROUPID");
                return maxId;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
