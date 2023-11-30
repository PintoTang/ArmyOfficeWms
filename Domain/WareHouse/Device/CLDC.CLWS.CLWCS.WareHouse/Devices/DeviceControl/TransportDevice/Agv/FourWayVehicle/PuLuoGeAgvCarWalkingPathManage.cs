using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.MNLMCha.Model;
using CLDC.Framework.Log.Helper;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle
{
    public class PuLuoGeAgvCarWalkingPathManage
    {
        private static PuLuoGeAgvCarWalkingPathManage _agvWalkingPathManage;

        /// <summary>
        /// AGV路径管理单例
        /// </summary>
        public static PuLuoGeAgvCarWalkingPathManage Instance
        {
            get
            {
                if (_agvWalkingPathManage == null)
                    _agvWalkingPathManage = new PuLuoGeAgvCarWalkingPathManage();
                return _agvWalkingPathManage;
            }
        }
        public PuLuoGeAgvCarWalkingPathManage()
        {
            LoadWalkingPath();
        }
        /// <summary>
        /// 小车行走路径集合(WCS与RCS地图绑定)
        /// </summary>
        public List<PuluoGeAgvCarWalkingPath> WalkingPathList = new List<PuluoGeAgvCarWalkingPath>();

        private void GetWmsRowAndDepthForRcsY(int y, out int rowNum, out int depth)
        {
            switch (y)
            {
                case 10:
                    rowNum = 1;
                    depth = 1;
                    break;
                case 11:
                    rowNum = 1;
                    depth = 0;
                    break;
                case 12: //主通道
                case 19:
                case 26:
                    rowNum = 0;
                    depth = 0;
                    break;
                case 13:
                    rowNum = 2;
                    depth = 0;
                    break;
                case 14:
                    rowNum = 2;
                    depth = 1;
                    break;
                case 15:
                    rowNum = 2;
                    depth = 2;
                    break;
                case 16:
                    rowNum = 3;
                    depth = 2;
                    break;
                case 17:
                    rowNum = 3;
                    depth = 1;
                    break;
                case 18:
                    rowNum = 3;
                    depth = 0;
                    break;
                case 20:
                    rowNum = 4;
                    depth = 0;
                    break;
                case 21:
                    rowNum = 4;
                    depth = 1;
                    break;
                case 22:
                    rowNum = 5;
                    depth = 3;
                    break;
                case 23:
                    rowNum = 5;
                    depth = 2;
                    break;
                case 24:
                    rowNum = 5;
                    depth = 1;
                    break;
                case 25:
                    rowNum = 5;
                    depth = 0;
                    break;
                case 27:
                    rowNum = 6;
                    depth = 0;
                    break;
                case 28:
                    rowNum = 6;
                    depth = 1;
                    break;
                case 29:
                    rowNum = 6;
                    depth = 2;
                    break;
                default:
                    rowNum = 0;
                    depth = 0;
                    break;
            }
        }

        private void LoadWalkingPath()
        {
            //X45,10,X44,10,,,,,,X10,10
            //X45,29,X44,29,,,,,,X10,29
            // (X44,Y10,Z1)  0100440010
            int wmsRow = 0;
            int wmsDepth = 0;
            for (int z = 1; z < 5; z++)
            {
                for (int x = 10; x < 46; x++)
                {
                    for (int y = 10; y < 30; y++)
                    {
                        //不可用的坐标
                        //Cell:1_1_1_1_A01  Cell:1_1_1_0_A01
                        GetWmsRowAndDepthForRcsY(y, out wmsRow, out wmsDepth);

                        int row = wmsRow;
                        int lay = z;
                        int col = 45 - x;
                        int depth = wmsDepth;
                        string addr = "";
                        if (wmsRow != 0)
                        {
                            addr = "Cell:" + row.ToString() + "_" + lay.ToString() + "_" + col.ToString() + "_"
                                   + depth.ToString() + "_A01";
                        }
                        //充电桩、  主通道、链条机、提升机、立柱、检修位、输送线
                        if (y == 10)
                        {
                            //y=10,x={13,14,19,24,25,30,35,36,41,45} //柱子  45空白不存在
                            if (("13,14,19,24,25,30,35,36,41,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 11)
                        {
                            //y=11,x={13,14,19,24,25,30,35,36,41} //柱子
                            if (("13,14,19,24,25,30,35,36,41").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 12)
                        {
                            //y=12,x=10-45  //主通道 (需要)
                        }
                        else if (y == 13)
                        {
                            //y=13,x={12,27,40,45} //主通道(需要)
                            //待移库 新地图 为小车主通道，老地图没有
                            //if (z == 2 && x == 45) continue;
                            //if (z == 3 && x == 45) continue;
                            //if (z == 4 && x == 45) continue;
                        }
                        else if (y == 14)
                        {
                            //y=14,x={12,27,40,44,45} //44 空白不存在  45链条机
                            if (("44").Split(',').Contains(x.ToString())) continue;
                            if (z == 2 && x == 45 ) continue;
                            if (z == 3 && x == 45 ) continue;
                            if (z == 4 && x == 45 ) continue;
                        }
                        else if (y == 15)
                        {
                            //y=15,x={12,27,40,44,45} //44、45 空白不存在
                            if (("44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 16)
                        {
                            //y=16,x={12,27,40,44,45} //45 提升机存在
                        }
                        else if (y == 17)
                        {
                            //y=17,x={12,27,40,44,45} //44 空白不存在、45 输送线
                            if (("44").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 18)
                        {
                            //y=18,x={12,27,40,44,45} //44 充电装、45 链条机
                        }
                        else if (y == 19)
                        {
                            //y=19,x=10-45  //主通道(需要)
                        }
                        else if (y == 20)
                        {
                            //y=20,x={13,14,18,19,24,25,30,35,36,41,44,45}45 链条机
                            if (("13,14,19,24,25,30,35,36,41").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 21)
                        {
                            //y=21,x={13,14,18,19,24,25,30,35,36,41,44,45}45 提升机存在
                            if (("13,14,19,24,25,30,35,36,41,44").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 22)
                        {
                            //y=22,x={12,27,40}  //主通道 {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 23)
                        {
                            //y=23,x={12,27,40}  //主通道 {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 24)
                        {
                            //y=24,x={12,27,40}  //主通道 {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 25)
                        {
                            //y=25,x={12,27,40}  //主通道 {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 26)
                        {
                            //y=26,x=(10-40)//主通道   {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 27)
                        {
                            //y=27,x=  {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 28)
                        {
                            //y=28,x=  {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                        }
                        else if (y == 29)
                        {
                            //y=29,x=  {41,42,43,44,45} 空白不存在
                            if (("41,42,43,44,45").Split(',').Contains(x.ToString())) continue;
                            if (z == 3 || z == 4)
                            {
                                if (("13,14,19,24,25,30,35,36").Split(',').Contains(x.ToString())) continue;
                            }
                        }
                        WalkingPathList.Add(new PuluoGeAgvCarWalkingPath
                        {
                            Addr = addr,
                            Position_X = x.ToString(),
                            Position_Y = y.ToString(),
                            Position_Z = z.ToString(),
                            Location = "0" + z.ToString() + "00" + x.ToString() + "00" + y.ToString(),
                            Description = ""
                        });
                    }
                }

            }
        }

        /// <summary>
        /// 根据OPC协议地址 找到 普罗格AGV对应的位置号
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string GetPostionNumAddrByConvertAddrData(string addr)
        {
            if (!WalkingPathList.Exists(x => x.Addr.Contains(addr)))
            {
                LogHelper.WriteLog("PuLuoGeAgv", "协议转换对应不上，在字典中未找到:" + addr);
                return ""; //传入的地址 不在Key字典中
            }
            return WalkingPathList.FirstOrDefault(x => x.Addr.Contains(addr)).Location;
        }

    }
}