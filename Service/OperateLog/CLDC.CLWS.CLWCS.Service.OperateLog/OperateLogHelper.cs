using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Service.OperateLog.Model;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.Service.OperateLog
{
    public static class OperateLogHelper
    {
        private static readonly OperateLogDataAbstract DbHandle = DependencyHelper.GetService<OperateLogDataAbstract>();
        public static void Record(string content)
        {
            if (DbHandle!=null)
            {
                OperateLogModel model=new OperateLogModel(content);
                DbHandle.SaveAsync(model);
            }
        }
    }
}
