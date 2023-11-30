using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.MNLMCha.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.MNLMCha
{
    public class MNLMChaAgvCarWalkingPathManage
    {
        private static MNLMChaAgvCarWalkingPathManage _agvWalkingPathManage;

        /// <summary>
        /// AGV路径管理单例
        /// </summary>
        public static MNLMChaAgvCarWalkingPathManage Instance
        {
            get
            {
                if (_agvWalkingPathManage == null)
                    _agvWalkingPathManage = new MNLMChaAgvCarWalkingPathManage();
                return _agvWalkingPathManage;
            }
        }
        public MNLMChaAgvCarWalkingPathManage()
        {
            LoadWalkingPath();
        }
        /// <summary>
        /// 小车行走路径集合
        /// </summary>
        public List<MNLMChaAgvCarWalkingPath> WalkingPathList = new List<MNLMChaAgvCarWalkingPath>();
        private void LoadWalkingPath()
        {
            // Cell:1_1_1_TS|Cell:2_1_1_TS|Cell:3_1_1_TS|Cell:4_1_1_TS|Cell:5_1_1_TS|Cell:6_1_1_TS
            //|DetectionPort:2_1_1|Exit:2_1_1|DetectionPort:3_1_1|ShippingPort:1_1_1|ShippingPort:2_1_1
            //轻型货物缓存位
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Cell:1_1_1_TS", Location = "200102", Description = "异常口4" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Cell:2_1_1_TS", Location = "200202", Description = "出库口1" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Cell:3_1_1_TS", Location = "200302", Description = "出库口2" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Cell:4_1_1_TS", Location = "200402", Description = "出库口3" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Cell:5_1_1_TS", Location = "200502", Description = "入库口1" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Cell:6_1_1_TS", Location = "200602", Description = "入库口2" });

            //轻载货物 出入库口
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Entrance:2_1_1", Location = "100101", Description = "轻型入库口01" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Exit:2_1_1", Location = "100201", Description = "轻型出库口" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "Entrance:3_1_1", Location = "100301", Description = "轻型入库口02" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "ShippingPort:1_1_1", Location = "100402", Description = "拣选出库口01" });
            WalkingPathList.Add(new MNLMChaAgvCarWalkingPath { Addr = "ShippingPort:2_1_1", Location = "100502", Description = "拣选出库口02" });
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
                CLDC.Framework.Log.Helper.LogHelper.WriteLog("MNLMChaAgv", "协议转换对应不上，在字典中未找到:" + addr);
                return ""; //传入的地址 不在Key字典中
            }

            return WalkingPathList.FirstOrDefault(x => x.Addr.Contains(addr)).Location;
        }


    }
}
