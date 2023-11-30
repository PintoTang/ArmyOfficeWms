using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.UserService.DataMode
{
    public sealed class PersonMode
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int AccId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelephoneNo { get; set; }

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
