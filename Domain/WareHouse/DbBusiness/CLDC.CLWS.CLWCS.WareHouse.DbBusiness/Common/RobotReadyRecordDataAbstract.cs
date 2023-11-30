using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DbBusiness
{
    public abstract class RobotReadyRecordDataAbstract : DatabaseBusinessAbstract<RobotReadyRecord>
    {
        protected RobotReadyRecordDataAbstract(IDbHelper dbHelper) : base(dbHelper)
        {
        }


        //public bool IsExistDataBase(RobotReadyRecord data)
        //{
        //    bool isExists = DbHelper.IsExist<RobotReadyRecord>(t => t.Addr1==data.Addr1 && t.Addr2==data.Addr2 && t.ContainerNO1==data.ContainerNO1 && t.ContainerNO2==data.ContainerNO2 && (t.Status==0|| t.Status==1));

        //    return isExists;
        //}

        public abstract RobotReadyRecord GetRobotReadyRecord(RobotReadyRecordQueryInput query);
        public abstract List<RobotReadyRecord> GetRobotReadyRecordList(RobotReadyRecordQueryInput query);

        public abstract int GetRobotReadyRecordsCount(RobotReadyRecordQueryInput query);

    }
}
