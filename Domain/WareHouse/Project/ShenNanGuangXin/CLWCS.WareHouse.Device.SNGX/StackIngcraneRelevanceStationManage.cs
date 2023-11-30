using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;

namespace CLWCS.WareHouse.Device.HeFei
{
    public class StackIngcraneRelevanceStationManage
    {
    
        private static StackIngcraneRelevanceStationManage _stackIngcraneRelevanceStationManage;
        private static List<StackingcraneRelevanceStationInfo> StackingStationInfoList =new List<StackingcraneRelevanceStationInfo>();
        /// <summary>
        /// 堆垛机与出入库站台单例管理类
        /// </summary>
        public static StackIngcraneRelevanceStationManage Instance
        {
            get
            {
                if (_stackIngcraneRelevanceStationManage == null)
                {
                    _stackIngcraneRelevanceStationManage = new StackIngcraneRelevanceStationManage();
                    InitData();
                }
                return _stackIngcraneRelevanceStationManage;
            }
        }

        private static void InitData()
        {
            StackingStationInfoList.Add(new StackingcraneRelevanceStationInfo {DeviceId = 9001,InStationNum = 1052,OutStationNum = 1056});
            StackingStationInfoList.Add(new StackingcraneRelevanceStationInfo { DeviceId = 9002, InStationNum = 1062, OutStationNum = 1066 });
           
        }
        public StackingcraneRelevanceStationInfo  GetStackingcraneStationInfo(int deviceId)
        {
            return StackingStationInfoList.Where(x => x.DeviceId == deviceId).FirstOrDefault();
        }
    }

    /// <summary>
    /// 堆垛机编号与站台编号信息类
    /// </summary>
    public class StackingcraneRelevanceStationInfo
    {
         public  int DeviceId { get; set; }
         public  int InStationNum { get; set; }
         public  int OutStationNum { get; set; }
    }
}

