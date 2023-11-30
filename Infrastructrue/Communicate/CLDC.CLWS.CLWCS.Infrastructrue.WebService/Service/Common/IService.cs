using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common
{
    public interface IService
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; set; }
    }
}
