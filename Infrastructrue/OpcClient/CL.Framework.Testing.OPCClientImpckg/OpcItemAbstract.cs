using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.Framework.Log;

namespace CL.Framework.Testing.OPCClientImpPckg
{
    public abstract class OpcItemAbstract
    {
        public abstract OpcItemManagerEntity Read(string groupName, string itemName);
        public abstract List<OpcItemManagerEntity> Read(string groupName, List<string> itemNames);
        /// <summary>
        /// 查询所有的地址项
        /// </summary>
        /// <returns></returns>
        public abstract List<OpcItemManagerEntity> ReadAllItems(string groupName);
        public abstract bool Write(string groupName, string itemName);
        public abstract void Write(string groupName, Dictionary<string, object> itemValueList);

        public abstract void Write(string groupName, Dictionary<string, int> itemValueList);
        public abstract void Write(string groupName, string itemName, string value = "0");
        /// <summary>
        /// 修改某项值
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="itemName">PLC地址名称</param>
        /// <param name="setValue">PLC地址对应的值</param>
        /// <returns></returns>
        public abstract bool Update(string groupName, string itemName, string setValue);
        /// <summary>
        /// 添加组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>返回添加结果True成功，False失败</returns>
        public abstract bool AddGroup(string groupName);
        /// <summary>
        /// 读去组信息
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>组信息，ID,和组名称</returns>
        public abstract OpcGroupManagerEntity ReadGroup(string groupName);
        public abstract decimal ReadMaxGroupID();
    }
}
