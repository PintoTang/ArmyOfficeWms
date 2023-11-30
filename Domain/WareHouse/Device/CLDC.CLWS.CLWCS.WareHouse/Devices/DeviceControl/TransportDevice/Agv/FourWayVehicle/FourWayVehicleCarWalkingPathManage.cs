using System.Collections.Generic;
using System.Linq;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class FourWayVehicleCarWalkingPathManage
    {
        private static FourWayVehicleCarWalkingPathManage _agvWalkingPathManage;

        /// <summary>
        /// AGV路径管理单例
        /// </summary>
        public static FourWayVehicleCarWalkingPathManage Instance
        {
            get
            {
                if (_agvWalkingPathManage == null)
                    _agvWalkingPathManage = new FourWayVehicleCarWalkingPathManage();
                return _agvWalkingPathManage;
            }
        }
        public FourWayVehicleCarWalkingPathManage()
        {
            LoadWalkingPath();
        }
        /// <summary>
        /// 小车行走路径集合
        /// </summary>
        public List<FourWayVehicleCarWalkingPath> WalkingPathList = new List<FourWayVehicleCarWalkingPath>();
        private void LoadWalkingPath()
        {

            //重载货物缓存位
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:1_1_1_I01", Location = "4", Description = "重载货物缓存位4" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:2_1_1_I01", Location = "5", Description = "重载货物缓存位5" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:3_1_1_I01", Location = "6", Description = "重载货物缓存位6" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:4_1_1_I01", Location = "7", Description = "重载货物缓存位7" });

            //轻载货物缓存位
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:5_1_1_I01", Location = "8", Description = "轻载货物缓存位8" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:6_1_1_I01", Location = "9", Description = "轻载货物缓存位9" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:7_1_1_I01", Location = "10", Description = "轻载货物缓存位10" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Cell:8_1_1_I01", Location = "11", Description = "轻载货物缓存位11" });

            //充电位
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Charg:1_1_1", Location = "17", Description = "小车充电位17" });

            //重型货物入库口 
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "FrontEntrance:1_1_1", Location = "2", Description = "重型货物入库口2" });

            //重型货物出库口
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Exit:1_1_1", Location = "3", Description = "重型货物出库口3" });

            //轻载货物入库口
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "FrontEntrance:2_1_1", Location = "12", Description = "轻载货物入库口12" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "FrontEntrance:3_1_1", Location = "13", Description = "轻载货物入库口13" });

            //轻载货物出库口
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Exit:2_1_1", Location = "14", Description = "轻载货物出库口14" });
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "Exit:3_1_1", Location = "15", Description = "轻载货物出库口15" });

            //空托盘出入口
            WalkingPathList.Add(new FourWayVehicleCarWalkingPath { Addr = "InAndOutStation:1_1_1", Location = "16", Description = "空托盘出入口16" });
        }

        /// <summary>
        /// 根据OPC协议地址 找到 杭叉AGV对应的位置号
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetPostionNumAddrByConvertAddrData(string addr)
        {
            if (!WalkingPathList.Exists(x => x.Addr.Contains(addr)))
            {
                CLDC.Framework.Log.Helper.LogHelper.WriteLog("FourWayVehicle", "协议转换对应不上，在字典中未找到:" + addr);
                return ""; //传入的地址 不在Key字典中
            }

            return WalkingPathList.FirstOrDefault(x => x.Addr.Contains(addr)).Location;
        }


    }
}
