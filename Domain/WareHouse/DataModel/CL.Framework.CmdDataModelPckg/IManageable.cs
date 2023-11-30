using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.CmdDataModelPckg
{
    /// <summary>
    /// 可管理的对象接口
    /// </summary>
   public interface IManageable
   {
        /// <summary>
        /// 唯一标识
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
   }
}
