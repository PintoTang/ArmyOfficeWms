using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class WorkerConfigManage
    {
        private static List<CoordConfigInfo> WorkerInfoLst = new List<CoordConfigInfo>();
        private static WorkerConfigManage workerInfoManage;
        public static WorkerConfigManage Instance
        {
            get
            {
                if (workerInfoManage == null)
                {
                    workerInfoManage = new WorkerConfigManage();
                }
                return workerInfoManage;
            }
        }

        public void AddWorker(CoordConfigInfo workerInfo)
        {
            WorkerInfoLst.Add(workerInfo);
        }

        public List<CoordConfigInfo> GetAllDeviceInfoData()
        {
            return WorkerInfoLst;
        }
    }
}
