using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.ClouRcsAgv.RcsApi
{
   public interface IClouRcsAgvApi
    {
       OperateResult ReportTaskResult(ReportTaskResultMode cmd);

       OperateResult ReportTaskException(ReportTaskExceptionMode cmd);
    }
}
