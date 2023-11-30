using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.UserService.DataMode
{
    /// <summary>
    /// 组数据结构
    /// </summary>
    public sealed class GroupMode
    {
        /// <summary>
        /// 组编号
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 创建者ID
        /// </summary>
        public int CreaterId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改者编号
        /// </summary>
        public int ModifierId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifieTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
