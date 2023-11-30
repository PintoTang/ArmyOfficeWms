using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common
{
    public interface IInvokeCmd
    {
        /// <summary>
        /// 获取Cmd的字符串
        /// </summary>
        /// <returns></returns>
        string GetCmd();
    }
}
