using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLWCS.UpperServiceForHeFei.Target
{
    public interface IWmsService
    {
        /// <summary>
        /// 上报盘点结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string NotifyStocktakingInstructResult(string cmd);

        /// <summary>
        /// 上报拆盘机动作完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string NotifyUnstackFinish(string cmd);
        /// <summary>
        /// 上报碟盘机叠盘完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string NotifyPalletizerFinish(string cmd);
    }
}
