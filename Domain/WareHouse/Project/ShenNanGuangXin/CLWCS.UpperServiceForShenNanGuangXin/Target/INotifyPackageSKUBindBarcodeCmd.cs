using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Target
{
    public interface INotifyPackageSKUBindBarcodeCmd
    {
        /// <summary>
        /// 通知上层系统包装条码-货物绑定
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string NotifyPackageSKUBindBarcodeCmd(string cmd);
    }
}
