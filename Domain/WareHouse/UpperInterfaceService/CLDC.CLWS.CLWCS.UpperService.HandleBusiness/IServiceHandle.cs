using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness
{
    public interface IServiceHandle
    {
     

        /// <summary>
        /// 同步调用服务
        /// </summary>
        /// <param name="notifyElement"></param>
        /// <returns></returns>
         OperateResult<object> Invoke(NotifyElement notifyElement);
        /// <summary>
        /// 异步调用服务
        /// </summary>
        /// <param name="notifyElement"></param>
        /// <returns></returns>
        OperateResult BeginInvoke(NotifyElement notifyElement);
    }
}
