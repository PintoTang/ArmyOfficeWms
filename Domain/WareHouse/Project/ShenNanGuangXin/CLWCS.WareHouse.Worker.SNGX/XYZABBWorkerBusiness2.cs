using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using Newtonsoft.Json;
using CLDC.Infrastructrue.Xml;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.DispatchWorkers.FourWayVehicle.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.Model;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using Infrastructrue.Ioc.DependencyFactory;
using CLDC.CLWS.CLWCS.WareHouse.Manage;
using CLWCS.WareHouse.Device.HeFei.Simulate3D.Model;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService;
using CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel;
using ResponseResult = CLWCS.WareHouse.Worker.HeFei.XYZABB.XYZABBApiService.CmdModel.ResponseResult;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using System.Linq;

namespace CLWCS.WareHouse.Worker.HeFei
{
    /// <summary>
    /// XYZABB接口业务处理
    /// </summary>
    public class XYZABBWorkerBusiness2 : InAndOutCellBusinessAbstract, IXYZABBServiceApiTwo
    {

        private RobotReadyRecordDataAbstract _robotReadyRecordDataHandle;
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;
        protected List<AgvInfo> AgvInfoDic = new List<AgvInfo>();
        private int sim3DDeviceId = 3333;
        //private OrderManage _orderManage;
        //private readonly Semaphore _orderAmountSem = new Semaphore(0, 0x7FFFFFFF);
        //private readonly Semaphore _orderAmountSemOut = new Semaphore(0, 0x7FFFFFFF);
        protected override OperateResult ParticularInitlize()
        {
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            _robotReadyRecordDataHandle = DependencyHelper.GetService<RobotReadyRecordDataAbstract>();
            DependencyFactory.GetDependency().RegisterInstance<IXYZABBServiceApiTwo>(this);
            //_orderManage = DependencyHelper.GetService<OrderManage>();

            //if (inDic.Count == 0)
            //{
            //    //加载与3D仿真系统的基础数据
            //    LoadBaseData();
            //}
            if (roller1021 == null)
            {
                //InitWareHouseInNodeData();

                ////开启线程
                //Thread thread = new Thread(AbbRobotDispatchInBusiness);
                //thread.Name = "AbbRobotDispatchInBusiness";
                //thread.IsBackground = true;
                //thread.Start();
            }

            return OperateResult.CreateSuccessResult();
        }
        public int wmsWarehouseInBusinessType = 0;//wms入库业务类型  1入库或 2库存整理

        //RollerDeviceControl roller7501 = null;
        //RollerDeviceControl roller7502 = null;
        RollerDeviceControl roller1021 = null;
        RollerDeviceControl roller1022 = null;

        //RollerDeviceControl roller1111 = null;//当前执行的入库任务虚拟节点
        //RollerDeviceControl roller1114 = null;//1004对应的虚拟节点
        //RollerDeviceControl roller1117 = null;//1007对应的虚拟节点
        //RollerDeviceControl roller1118 = null;//1008对应的虚拟节点
        //RollerDeviceControl roller1119 = null;//清理NG对应的虚拟节点
        //RollerDeviceControl roller3008 = null;//库存整理任务节点


        Simulate3DControl roller3D = null;
        string wcsWareHouseInFullPath = "";
        private void InitWareHouseInNodeData()
        {
            roller1021 = GetRollerControlByNodeId(1021);
            roller1022 = GetRollerControlByNodeId(1022);

            //roller1111 = GetRollerControlByNodeId(3003);
            //roller1114 = GetRollerControlByNodeId(3004);
            //roller1117 = GetRollerControlByNodeId(3005);
            //roller1118 = GetRollerControlByNodeId(3006);
            //roller1119 = GetRollerControlByNodeId(3007);
            //roller3008 = GetRollerControlByNodeId(3008);


            DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
            if (xDevice == null)
            {
                string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                LogMessage(msg, EnumLogLevel.Error, true);
            }
            Simulate3D transDevice = xDevice as Simulate3D;
            roller3D = transDevice.DeviceControl as Simulate3DControl;


            string strAppPath = Directory.GetCurrentDirectory();
            wcsWareHouseInFullPath = Path.Combine(strAppPath, @"SerialFile\wcsWareHouseInFiles.ini");

            if (curInOrderFinishCmd == null)
            {
                curInOrderFinishCmd = GetABBOrderFinishedData("01");
            }
        }


        /// <summary>
        /// 是否启动 入库业务
        /// </summary>
        /// <param name="readyType">1-右边空容器到位；2-左边空容器到位；3-左右两个空容器都到位</param>
        /// <returns></returns>
        private bool isExecDispatchInBusiness(out int readyType)
        {
            readyType = 0;
            bool isArrived = _robotReadyRecordDataHandle.GetRobotReadyRecordsCount(new RobotReadyRecordQueryInput
            {
                WhereLambda = t => t.Status == 0 || t.Status == 1
            })>0;
            OperateResult<bool> readIsLoadResult1 = roller1021.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
            OperateResult<bool> readIsLoadResult2 = roller1022.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
            if (isArrived && ((readIsLoadResult1.IsSuccess && readIsLoadResult1.Content) || (readIsLoadResult2.IsSuccess && readIsLoadResult2.Content)))
            {
                if(readIsLoadResult1.IsSuccess && readIsLoadResult1.Content && readIsLoadResult2.IsSuccess && readIsLoadResult2.Content)
                {
                    readyType = 3;
                }
                else
                {
                    if (readIsLoadResult1.IsSuccess && readIsLoadResult1.Content && readIsLoadResult2.IsSuccess)
                    {
                        readyType = 1;
                    }
                    else
                    {
                        readyType = 2;
                    }
                }
                return true;
            }
            return false;
        }
       

        RollerDeviceControl GetRollerControlByNodeId(int nodeId)
        {
            DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(nodeId);
            TransportPointStation curTransDevice = curDevice as TransportPointStation;
            return curTransDevice.DeviceControl as RollerDeviceControl;
        }
        object objInOrderLock = new object();

        ReportExeOrderFinishCmd _curInOrderFinishCmd;
        ReportExeOrderFinishCmd curInOrderFinishCmd
        {
            get
            {
                lock (objInOrderLock)
                {
                    return _curInOrderFinishCmd;
                }
            }
            set
            {
                lock (objInOrderLock)
                {
                    _curInOrderFinishCmd = value;
                }
            }
        }

        private string _leftCachePort = "RobotCache:2_1_1";
        private string _leftRobotAddr = "A1";

        private string _rightCachePort = "RobotCache:1_1_1";
        private string _rightRobotAddr = "B1";
        /// <summary>
        /// 入库机器人业务调度
        /// </summary>
        private void AbbRobotDispatchInBusiness()
        {
            while (true)
            {
                try
                {
                    int readyType = 0;
                    if (!isExecDispatchInBusiness(out readyType))
                    {
                        continue;
                    }
                    List<RobotReadyRecord> list = _robotReadyRecordDataHandle.GetRobotReadyRecordList(new RobotReadyRecordQueryInput
                    {
                        WhereLambda=t=>t.Status==0|| t.Status==1,
                    });
                    if(list==null|| list.Count == 0)
                    {
                        continue;
                    }
                    RobotReadyRecord item = list.FirstOrDefault(t => t.Status == 1);
                    if (item == null)
                    {
                        if(readyType==1)//
                        {
                            item= list.FirstOrDefault(t => t.Status == 0 && t.Addr1== _leftCachePort);
                        }
                    }

                    #region 先注释
                    ////入库业务  指令类型 99锁定、98退、   除1111 类型表示状态
                    ////包号 - 1000，任务类型0，（1004或1007 条码为空）且目标地址等于当前，则表示来新托盘 生成1004托盘编号、生成1007托盘编号
                    //OperateResult<string> opcBarCodeResult = roller1021.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    //OperateResult<int> opcOrderTypeResult = roller1111.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                    ////OperateResult<int> opcDestResult = roller1111.Communicate.ReadInt(DataBlockNameEnum.WriteDirectionDataBlock);
                    //if (!opcOrderTypeResult.IsSuccess)
                    //{
                    //    LogMessage("Roller1111  3003 opcOrderTypeResult opc 读取失败", EnumLogLevel.Info, false);
                    //    continue;
                    //}

                    //switch (opcOrderTypeResult.Content)
                    //{
                    //    case 0://初始值 检测托盘信号
                    //        if (!opcBarCodeResult.IsSuccess || !string.IsNullOrEmpty(opcBarCodeResult.Content))
                    //        {
                    //            //当前1111有任务，不得接受其它任务
                    //            string msg = string.Format("当前1111任务状态{0} 原因:{1}", opcOrderTypeResult.Content, "当前1111有任务未完成，不得接受其它任务");
                    //            LogMessage(msg, EnumLogLevel.Info, false);
                    //            continue;
                    //        }

                    //        if (IsCheckNewTray(1008))
                    //        {
                    //            bool isABBInInit = IsABBInInit("01");
                    //            if (isABBInInit == false) continue;

                    //            //调用载具更新通知机器人
                    //            bool isNotifyOk = NotifyABBTrayChanged("01", 1008);
                    //            if (isNotifyOk == false) continue;
                    //            //写1008和1118新托盘号 
                    //            if (!IsUpdateSimTrayNo(1118)) continue;

                    //            ////判断类型 开始入库
                    //            bool isHaveGoods = IsHaveGoodsForABBVision("01", 1008);
                    //            NotifyIsEmpty("01", 0, isHaveGoods ? false : true, "4");



                    //            if (roller1008.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99).IsSuccess)
                    //            {
                    //                //更新1111 状态2
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 1).IsSuccess)
                    //                {
                    //                    //NotifyIsEmpty("01", 0, false, "1");
                    //                    NotifyTrayInAndOut(1008, false);
                    //                    continue;
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            OperateResult<int> opcOrderTypeResult1008 = roller1008.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                    //            if (opcOrderTypeResult1008.IsSuccess && opcOrderTypeResult1008.Content == 99)
                    //            {
                    //                //待处理 1008 托盘是否是等待回退托盘
                    //                if (ReadNodeIsTrayIn(1008)) continue;
                    //                //锁定
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 1).IsSuccess)
                    //                {
                    //                    continue;
                    //                }
                    //            }
                    //        }
                    //        break;
                    //    case 1://等待1008托盘达到
                    //        if (!opcBarCodeResult.IsSuccess || !string.IsNullOrEmpty(opcBarCodeResult.Content))
                    //        {
                    //            //当前1111有任务，不得接受其它任务
                    //            string msg = string.Format("当前1111任务状态{0} 原因:{1}", opcOrderTypeResult.Content, "当前1111有任务未完成，不得接受其它任务");
                    //            LogMessage(msg, EnumLogLevel.Info, false);
                    //            continue;
                    //        }
                    //        //0异常、99NG、180等待wms下发库存整理、181库存整理入库、182 库存整理  出库、10入库结束、11入库开始
                    //        int wcsType = GetCurWareHouseInWmsType();
                    //        if (wcsType == 0)
                    //        {
                    //            LogMessage("wcsType 0 GetCurWareHouseInWmsType  opc 类型读取失败", EnumLogLevel.Info, false);
                    //            continue;
                    //        }
                    //        else if (wcsType == 99)
                    //        {
                    //            if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess) continue;
                    //        }
                    //        else if (wcsType == 180)
                    //        {
                    //            LogMessage("wcsType 180  opc 库存整理等待wms下发具体 作业类型 1入或2出", EnumLogLevel.Info, false);
                    //            continue;
                    //        }

                    //        OperateResult<bool> opcIsLoadResult1004 = roller1004.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //        OperateResult<bool> opcIsLoadResult1007 = roller1007.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //        if (!opcIsLoadResult1004.IsSuccess || !opcIsLoadResult1007.IsSuccess)
                    //        {
                    //            LogMessage("1004 或1007 是否载货 opc 读取失败", EnumLogLevel.Info, false);
                    //            continue;
                    //        }
                    //        OperateResult<int> opcOrderTypeResult1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                    //        OperateResult<int> opcOrderTypeResult1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                    //        if (!opcOrderTypeResult1004.IsSuccess || !opcOrderTypeResult1004.IsSuccess)
                    //        {
                    //            LogMessage("1004 或1007 指令类型 opc读取失败", EnumLogLevel.Info, false);
                    //            continue;
                    //        }
                    //        if (opcIsLoadResult1004.Content && opcIsLoadResult1007.Content)
                    //        {
                    //            //都载货
                    //            if (opcOrderTypeResult1004.Content == 99)
                    //            {
                    //                //表示1004已锁定
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess) continue;
                    //            }

                    //            if (opcOrderTypeResult1007.Content == 99)
                    //            {
                    //                //表示1007已锁定
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess) continue;
                    //            }

                    //            //是否已经锁定
                    //            if (IsCheckNewTray(1004))
                    //            {
                    //                bool isABBInInit =  IsABBInInit("01");
                    //                if (isABBInInit == false) continue;

                    //                //调用载具更新通知机器人
                    //                bool isNotifyOk = NotifyABBTrayChanged("01", 1004);
                    //                if (isNotifyOk == false) continue;
                    //                //判断类型 开始入库
                    //                bool isHaveGoods = IsHaveGoodsForABBVision("01", 1004);
                    //                if (wcsType.Equals(10) || wcsType.Equals(11) || wcsType.Equals(181))
                    //                {
                    //                    if (isHaveGoods == false)
                    //                    {
                    //                        if (IsCheckABBNG("01"))
                    //                        {
                    //                            roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                        }
                    //                        else
                    //                        {
                    //                            NotifyPlcRunByOPCNoABB(1004, isHaveGoods);
                    //                            continue;
                    //                        }
                    //                    }
                    //                }

                    //                if (roller1004.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99).IsSuccess)
                    //                {
                    //                    //写1004和1114新托盘号 
                    //                    if (!IsUpdateSimTrayNo(1114)) continue;

                    //                    //更新1111 状态1
                    //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess)
                    //                    {
                    //                        //新托盘 写入非空托盘
                    //                        NotifyIsEmpty("01", 0, false, "1");
                    //                        ChangePickNumForABB(2);
                    //                        continue;
                    //                    }
                    //                }
                    //            }
                    //            if (IsCheckNewTray(1007))
                    //            {
                    //                bool isABBInInit = IsABBInInit("01");
                    //                if (isABBInInit == false) continue;
                    //                //调用载具更新通知机器人
                    //                bool isNotifyOk = NotifyABBTrayChanged("01", 1007);
                    //                if (isNotifyOk == false) continue;
                    //                //判断类型 开始入库
                    //                bool isHaveGoods = IsHaveGoodsForABBVision("01", 1007);

                    //                if (wcsType.Equals(10) || wcsType.Equals(11) || wcsType.Equals(181))
                    //                {
                    //                    if (isHaveGoods == false)
                    //                    {
                    //                        if (IsCheckABBNG("01"))
                    //                        {
                    //                            roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                        }
                    //                        else
                    //                        {
                    //                            NotifyPlcRunByOPCNoABB(1007, isHaveGoods);
                    //                            continue;
                    //                        }
                    //                    }
                    //                }

                    //                if (roller1007.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99).IsSuccess)
                    //                {
                    //                    //写1007和1117新托盘号 
                    //                    if (!IsUpdateSimTrayNo(1117)) continue;
                    //                    //更新1111 状态1
                    //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess)
                    //                    {
                    //                        NotifyIsEmpty("01", 0, false, "2");
                    //                        ChangePickNumForABB(2);
                    //                        continue;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //非都载货
                    //            if (opcIsLoadResult1004.Content)
                    //            {
                    //                if (opcOrderTypeResult1004.Content == 99)
                    //                {
                    //                    //表示1004已锁定
                    //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess)
                    //                    {
                    //                        continue;
                    //                    }
                    //                }
                    //                //1004载货
                    //                if (IsCheckNewTray(1004))
                    //                {
                    //                    bool isABBInInit = IsABBInInit("01");
                    //                    if (isABBInInit == false) continue;
                    //                    //调用载具更新通知机器人
                    //                    bool isNotifyOk = NotifyABBTrayChanged("01", 1004);
                    //                    if (isNotifyOk == false) continue;
                    //                    //判断类型 开始入库
                    //                    bool isHaveGoods = IsHaveGoodsForABBVision("01", 1004);
                    //                    if (wcsType.Equals(10) || wcsType.Equals(11) || wcsType.Equals(181))
                    //                    {
                    //                        if (isHaveGoods == false)
                    //                        {
                    //                            if (IsCheckABBNG("01"))
                    //                            {
                    //                                roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                            }
                    //                            else
                    //                            {
                    //                                NotifyPlcRunByOPCNoABB(1004, isHaveGoods);
                    //                                continue;
                    //                            }
                    //                        }
                    //                    }
                    //                    //写1004和1114新托盘号 
                    //                    if (!IsUpdateSimTrayNo(1114)) continue;
                    //                    ChangePickNumForABB(2);
                    //                    if (roller1004.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99).IsSuccess)
                    //                    {
                    //                        //更新1111 状态1
                    //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess)
                    //                        {
                    //                            NotifyIsEmpty("01", 0, false, "1");
                    //                            continue;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //            if (opcIsLoadResult1007.Content)
                    //            {
                    //                //1007载货
                    //                if (opcOrderTypeResult1007.Content == 99)
                    //                {
                    //                    //表示1004已锁定
                    //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess)
                    //                    {
                    //                        continue;
                    //                    }
                    //                }
                    //                if (IsCheckNewTray(1007))
                    //                {
                    //                    bool isABBInInit = IsABBInInit("01");
                    //                    if (isABBInInit == false) continue;
                    //                    //调用载具更新通知机器人
                    //                    bool isNotifyOk = NotifyABBTrayChanged("01", 1007);
                    //                    if (isNotifyOk == false) continue;
                    //                    //判断类型 开始入库
                    //                    bool isHaveGoods = IsHaveGoodsForABBVision("01", 1007);
                    //                    if (wcsType.Equals(10) || wcsType.Equals(11) || wcsType.Equals(181))
                    //                    {
                    //                        if (isHaveGoods == false)
                    //                        {
                    //                            if (IsCheckABBNG("01"))
                    //                            {
                    //                                roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                            }
                    //                            else
                    //                            {
                    //                                NotifyPlcRunByOPCNoABB(1007, isHaveGoods);
                    //                                continue;
                    //                            }
                    //                        }
                    //                    }

                    //                    //写1007和1117新托盘号 
                    //                    if (!IsUpdateSimTrayNo(1117)) continue;
                    //                    ChangePickNumForABB(2);
                    //                    if (roller1007.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99).IsSuccess)
                    //                    {
                    //                        //更新1111 状态1
                    //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 2).IsSuccess)
                    //                        {
                    //                            NotifyIsEmpty("01", 0, false, "2");
                    //                            continue;
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        break;
                    //    case 2://准备生成指令
                    //           //条码地址：虚拟托盘号|指令ID|优先级|起始目标位置|当前位置|发送抓取数量|实际抓取数量|机器人完成次数
                    //           //1111 目标地址为生成的托盘号
                    //        if (!opcBarCodeResult.IsSuccess || !string.IsNullOrEmpty(opcBarCodeResult.Content))
                    //        {
                    //            //当前1111有任务，不得接受其它任务
                    //            string msg = string.Format("当前1111任务状态{0} 原因:{1}", opcOrderTypeResult.Content, "当前1111有任务未完成，不得接受其它任务");
                    //            LogMessage(msg, EnumLogLevel.Info, false);
                    //            continue;
                    //        }

                    //        //安全检测  取货位和放货位是否都有托盘（1114、1117、1118）
                    //        OperateResult<string> opGetCmdResult = CheckAndGetCmd();
                    //        if (opGetCmdResult.IsSuccess)
                    //        {
                    //            if (roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, opGetCmdResult.Content).IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                    continue;
                    //            }
                    //        }
                    //        break;
                    //    case 3://下发指令给机器人
                    //        if (!opcBarCodeResult.IsSuccess || string.IsNullOrEmpty(opcBarCodeResult.Content)) continue;

                    //        //0异常、99NG、180等待wms下发库存整理、181库存整理入库、182 库存整理  出库、10入库结束、11入库开始
                    //        int wcsType2 = GetCurWareHouseInWmsType();
                    //        if (wcsType2.Equals("0") || wcsType2.Equals("180"))
                    //        {
                    //            continue;
                    //        }

                    //        if (wcsType2.Equals("10"))
                    //        //if (isForceTaskValue.Equals("0")|| isForceTaskValue.Equals("1"))
                    //        {
                    //            OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //            if (isLoaded.IsSuccess && isLoaded.Content)
                    //            {
                    //                string[] cmdArr = opcBarCodeResult.Content.Split('|');
                    //                if (cmdArr[3].Equals("34"))
                    //                {
                    //                    //叠盘接口调用成功
                    //                    OperateResult op = NotifyWmsPalletizerFinish("01");
                    //                    if (!op.IsSuccess) return; //通知失败
                    //                    NotifyTrayInAndOut(1008, true);
                    //                    //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);

                    //                    //生成 31或32回退指令
                    //                    OperateResult<int> isLocked1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    //                    OperateResult<bool> isLoaded1004 = roller1004.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //                    int actType = 0;
                    //                    if (isLocked1004.IsSuccess && isLocked1004.Content == 99 && isLoaded1004.IsSuccess && isLoaded1004.Content)
                    //                    {
                    //                        actType = 31;
                    //                    }
                    //                    else
                    //                    {
                    //                        OperateResult<int> isLocked1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.WriteOrderIDDataBlock);
                    //                        OperateResult<bool> isLoaded1007 = roller1007.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //                        if (isLocked1007.IsSuccess && isLocked1007.Content == 99 && isLoaded1007.IsSuccess && isLoaded1007.Content)
                    //                        {
                    //                            actType = 32;
                    //                        }
                    //                    }
                    //                    //任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量

                    //                    OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(int.Parse(cmdArr[0]), "", actType, 3, int.Parse(cmdArr[5]));
                    //                    if (writeOpc1111.IsSuccess)
                    //                    {
                    //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                        {
                    //                            SetABBOrderFinishedData("01", null);
                    //                            //NotifyTrayInAndOut(1008, true);
                    //                            NotifyIsEmpty("01", int.Parse(cmdArr[0]), false, actType.ToString());
                    //                            return;
                    //                        }
                    //                    }
                    //                }
                    //                else if (cmdArr[3].Equals("41"))
                    //                {
                    //                    //叠盘接口调用成功
                    //                    OperateResult op = NotifyWmsPalletizerFinish("01");
                    //                    if (!op.IsSuccess) return; //通知失败
                    //                    NotifyTrayInAndOut(1008, true);
                    //                    //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);
                    //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                    //                    {
                    //                        return;
                    //                    }
                    //                }
                    //                else if (cmdArr[3].Equals("42"))
                    //                {
                    //                    //叠盘接口调用成功
                    //                    OperateResult op = NotifyWmsPalletizerFinish("01");
                    //                    if (!op.IsSuccess) return; //通知失败
                    //                    NotifyTrayInAndOut(1008, true);
                    //                    //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);
                    //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                    //                    {
                    //                        return;
                    //                    }
                    //                }
                    //                else if (cmdArr[3].Equals("13"))
                    //                {
                    //                    bool isHaveGoods1004 = IsHaveGoodsForABBVision("01", 1004);
                    //                    if (isHaveGoods1004 == false)
                    //                    {
                    //                        NotifyPlcRunByOPC(1004);
                    //                    }
                    //                    roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                    //                }
                    //                else if (cmdArr[3].Equals("23"))
                    //                {
                    //                    bool isHaveGoods1007 = IsHaveGoodsForABBVision("01", 1007);
                    //                    if (isHaveGoods1007 == false)
                    //                    {
                    //                        NotifyPlcRunByOPC(1007);
                    //                    }
                    //                    roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                    //                }
                    //            }
                    //        }

                    //        if (IsSendCmdForAbbRobot("01", opcBarCodeResult.Content))
                    //        {
                    //            if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 4).IsSuccess)
                    //                continue;
                    //        }
                    //        break;
                    //    case 4://下发成功，等待机器上报完成
                    //        if (curInOrderFinishCmd == null)
                    //        {
                    //            //_orderAmountSem.WaitOne();
                    //            continue;
                    //        }
                    //        HandleOrderFinished(curInOrderFinishCmd);
                    //        //正常完成、异常结束、   通知托盘已空、清理NG
                    //        break;
                    //    case 5://库存整理 等待wms下发任务

                    //        break;
                    //    case 13://1004-扫码位

                    //        break;
                    //    case 23://1007-扫码位

                    //        break;
                    //    case 31://扫码位-1004 （回退）
                    //        break;
                    //    case 32://扫码位-1007 （回退）
                    //        break;
                    //    case 34://扫码位-1008
                    //        break;
                    //    case 35://扫码位-NG （未扫到）
                    //        break;


                    //    case 51://NG-1004 清理NG
                    //        break;
                    //    case 52://NG-1007 清理NG
                    //        break;

                    //    case 41://1008-1004（库存整理）
                    //        break;
                    //    case 42://1008-1007 （库存整理）
                    //        break;
                    //    case 12:
                    //    case 14:
                    //    case 15:
                    //    case 54:
                    //    case 21:
                    //    case 24:
                    //    case 25:
                    //    case 43://批次整理 1008-扫码 （不用扫 不处理）********
                    //    case 45:
                    //    case 53://NG-扫码  不处理（不用扫 不处理）************
                    //        break;

                    //    case -192837://清空所有环境 重新执行业务（）1114、1117、1118、1111 

                    //        break;
                    //    case 999://异常 人工干预
                    //        break;
                    //    default:
                    //        break;
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    LogMessage("AbbRobotDispatchInBusiness 异常:" + ex.ToString(), EnumLogLevel.Error, false);
                }
                finally
                {
                    Thread.Sleep(50);
                }
            }
        }



        /// <summary>
        /// 得到wcs当前的指令类型 
        /// </summary>
        /// <returns>//wcs指令类型  0异常、99NG、180等待wms下发库存整理、181库存整理入库、182 库存整理  出库、10入库结束、11入库开始</returns>
        private int GetCurWareHouseInWmsType()
        {
            //入库
            //startTaskCmd.ActionNum = 1
            string readValue = IniHelper.ReadIniData("3003", "IsStartTask", "", wcsWareHouseInFullPath);
            string readValue2 = IniHelper.ReadIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);
            if (!string.IsNullOrEmpty(readValue) && readValue.Equals("true"))
            {
                if (string.IsNullOrEmpty(readValue2))
                {
                    return 11;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(readValue2))
                {
                    if (readValue2.Equals("0") || readValue2.Equals("1"))
                    {
                        return 10;
                    }
                }
            }
            
            return 0;
        }


        string trayNo1 = "B01";
        string trayNo2 = "B02";

        string trayNo3 = "B03";
        string trayNo4 = "B04";
        private bool IsUpdateSimTrayNo(int nodeId)
        {
            bool isUpdated = false;
            return isUpdated;
        }
        /// <summary>
        /// 通过wcs任务类型 得到发送给ABB的任务类型 jobType
        /// </summary>
        /// <param name="wcsTaskType"></param>
        /// <returns></returns>
        private string GetABBTaskTypeByWcsTyp(string wcsTaskType)
        {
            //wcsTaskType: 1入库、2出库、3库存整理入库、4库存整理出库 5清理NG
            //ABBTaskType: 01入库、02出库、03库存整理入库
            switch (wcsTaskType)
            {
                case "1":
                    return "01";
                case "2":
                    return "02";
                case "3":
                case "4":
                    return "03";

                default:
                    return "01";
            }
        }


        /// <summary>
        /// 更新指令扩展数据
        /// </summary>
        /// <param name="areaCode">区域01 入库，02出库</param>
        /// <param name="barCodeList">条码列表</param>
        /// <param name="abbData">abb数据</param>
        private OperateResult UpdateOrderExtData(string areaCode, List<SendExeOrder_ext_data> abbData)
        {
            if (areaCode.Equals("01"))
            {
                bool isWriteOk = IniHelper.WriteIniData("3003", "OPCBarcodeDataBlock2", abbData.ToJson(), wcsWareHouseInFullPath);
                if (isWriteOk)
                {
                    LogMessage("写入opc 1111 OPCBarcodeDataBlock2 值：" + abbData.ToJson(), EnumLogLevel.Info, false);
                }
                return isWriteOk ? OperateResult.CreateSuccessResult() : OperateResult.CreateFailedResult();
                //return  roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock2, abbData.ToJson());
            }
            return OperateResult.CreateFailedResult();
        }
        private List<SendExeOrder_ext_data> GetSendExeOrderData(string areaCode)
        {
            List<SendExeOrder_ext_data> tempSendExeOrder_ext_dataList = new List<SendExeOrder_ext_data>();
            if (areaCode.Equals("01"))
            {
                string readValue = IniHelper.ReadIniData("3003", "OPCBarcodeDataBlock2", "", wcsWareHouseInFullPath);
                if (!string.IsNullOrEmpty(readValue))
                {
                    return readValue.ToObject<List<SendExeOrder_ext_data>>();
                }

                //OperateResult<string> opReadDataResult = roller1111.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock2);
                //if (opReadDataResult.IsSuccess)
                //{
                //    if(!string.IsNullOrEmpty(opReadDataResult.Content.Trim()))
                //    {
                //        return opReadDataResult.Content.ToObject<List<SendExeOrder_ext_data>>();
                //    }
                //}
            }
            return tempSendExeOrder_ext_dataList;
        }

        private bool IsSendCmdForAbbRobot(string areaCode, string cmd)
        {
            bool isSendOk = false;
            //任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量
            string[] cmdArr = cmd.Split('|');

            string startAddr = cmdArr[3].ToString().Substring(0, 1);
            string endAddr = cmdArr[3].ToString().Substring(1, 1);
            int nodeID = 0;
            if (areaCode.Equals("01"))
            {
                nodeID = 8001;
            }

            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(nodeID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;

            SendExeOrderMode sendOrderMode = new SendExeOrderMode
            {
                area_code = areaCode,
                order_no = cmdArr[2],
                job_type = GetABBTaskTypeByWcsTyp(cmdArr[0]),
                pick_num = int.Parse(cmdArr[5]),
                start_addr = startAddr,
                dest_addr = endAddr,

            };
            if (cmdArr[3].Equals("13") || cmdArr[3].Equals("23"))
            {
                sendOrderMode.ext_data = new List<SendExeOrder_ext_data>();
            }
            else
            {
                sendOrderMode.ext_data = GetSendExeOrderData(areaCode);
            }

            //屏蔽 无需浪费时间，直接下发指令2022/12/08 zhangxing
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //OperateResult opQueryRobotStatusResult = abbControl.QueryRobotStatus(sendOrderMode.area_code);
            //sw.Stop();
            //LogMessage("查询机器人状态 QueryRobotStatus 耗时：" + sw.ElapsedMilliseconds + "毫秒", EnumLogLevel.Info, false);

            //if (!opQueryRobotStatusResult.IsSuccess)
            //{
            //    LogMessage("即将 下发任务 QueryRobotStatus 结果：失败 + 具体返回信息：" + opQueryRobotStatusResult.Message, EnumLogLevel.Error, false);
            //    return false;
            //}
            //LogMessage("即将下发任务，调用QueryRobotStatus接口 结果: 成功",  EnumLogLevel.Info , false);

          
            OperateResult op = abbControl.SendExeOrder(sendOrderMode);
          
            string msg = string.Format("下发任务 SendExeOrder 参数:{0}  结果:{1} 返回信息:{2}", sendOrderMode.ToJson(), op.IsSuccess ? "成功" : "失败", op.Message);
            LogMessage(msg, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
            isSendOk = op.IsSuccess ? true : false;
            return isSendOk;
        }

        private void NotifyIsEmpty(string areaCode, int wmsTaskType, bool isEmpty, string startAndEndPostion)
        {
            string writeValue = isEmpty ? "true" : "false";
            if (areaCode.Equals("01"))
            {
                int code = int.Parse(startAndEndPostion.Substring(0, 1));
                string strResult = "";
                bool isWriteOk = false;
                switch (code)
                {
                    case 1:
                        isWriteOk = IniHelper.WriteIniData("3003", "Node1004IsEmpty", writeValue, wcsWareHouseInFullPath);
                        strResult = isWriteOk ? "成功" : "失败";
                        LogMessage("写入 IniHelper 3003 Node1004IsEmpty 值： " + writeValue + strResult, EnumLogLevel.Info, false);
                        break;
                    case 2:
                        isWriteOk = IniHelper.WriteIniData("3003", "Node1007IsEmpty", writeValue, wcsWareHouseInFullPath);
                        strResult = isWriteOk ? "成功" : "失败";
                        LogMessage("写入 IniHelper 3003 Node1007IsEmpty 值： " + writeValue + strResult, EnumLogLevel.Info, false);
                        break;
                    case 4:
                        isWriteOk = IniHelper.WriteIniData("3003", "Node1008IsEmpty", writeValue, wcsWareHouseInFullPath);
                        strResult = isWriteOk ? "成功" : "失败";
                        LogMessage("写入 IniHelper 3003 Node1008IsEmpty 值： " + writeValue + strResult, EnumLogLevel.Info, false);
                        break;
                    case 5:
                        isWriteOk = IniHelper.WriteIniData("3003", "NodeNGEmpty", writeValue, wcsWareHouseInFullPath);
                        strResult = isWriteOk ? "成功" : "失败";
                        LogMessage("写入 IniHelper 3003 NodeNGEmpty 值： " + writeValue + strResult, EnumLogLevel.Info, false);
                        break;
                    default:
                        break;
                }
                if (code == 1 || code == 2 || code == 4 || code == 5)
                {
                    string strValue = isEmpty ? code.ToString() : "0";

                    isWriteOk = IniHelper.WriteIniData("3003", "StartPostionIsEmpty", strValue, wcsWareHouseInFullPath);
                    strResult = isWriteOk ? " 成功" : " 失败";
                    LogMessage("写入 IniHelper 3003 StartPostionIsEmpty 值：" + code.ToString() + strResult, EnumLogLevel.Info, false);
                }
            }
            
        }

        /// <summary>
        /// 1008 托盘入和出 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="isTrayIn">是否入，true 入，false 出</param>
        private void NotifyTrayInAndOut(int nodeID, bool isTrayIn)
        {
            string writeValue = isTrayIn ? "true" : "false";
            if (nodeID == 1008)
            {
                bool isWriteOk = IniHelper.WriteIniData("3003", "Node1008IsReBack", writeValue, wcsWareHouseInFullPath);
            }
        }
        private string ReadIsForceTask(string areaCode)
        {
            string readValue = "";
            if (areaCode.Equals("01"))
            {
                readValue = IniHelper.ReadIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);
            }
            return readValue;
        }

        /// <summary>
        /// 读取1008 和 2001 是否是托盘入库
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private bool ReadNodeIsTrayIn(int nodeID)
        {
            if (nodeID == 1008)
            {
                string readValue = IniHelper.ReadIniData("3003", "Node1008IsReBack", "", wcsWareHouseInFullPath);
                return readValue.Equals("true") ? true : false;
            }
            return false;
        }
        /// <summary>
        /// 发送 NotifyPackagePutFinishNew 通过托盘已空
        /// </summary>
        /// <param name="areaCode">区域01 入库，02出库</param>
        /// <returns>是否下发成功</returns>
        private bool SendPackagePutFinishNew(string areaCode)
        {
            bool isSendOK = false;
            try
            {
                string strCmd = "";
                if (areaCode.Equals("01"))
                {
                    string readValue = IniHelper.ReadIniData("3003", "OPCBarcodeDataBlock22", "", wcsWareHouseInFullPath);
                    if (string.IsNullOrEmpty(readValue))
                    {
                        return false;
                    }
                    if (readValue.Equals("0"))
                    {
                        return false;
                    }
                    strCmd = readValue.Trim();
                    //bool isWriteOk= IniHelper.WriteIniData("3003", "OPCBarcodeDataBlock22", "", fullPath);


                    //OperateResult<string> opBarCodeResult = roller1111.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock22);
                    //if(opBarCodeResult.IsSuccess)
                    //{
                    //    if(string.IsNullOrEmpty(opBarCodeResult.Content.Trim()))
                    //    {
                    //        return false;
                    //    }
                    //    if(opBarCodeResult.Content.Equals("0"))
                    //    {
                    //        return false;
                    //    }
                    //    strCmd = opBarCodeResult.Content.Trim();
                    //}
                }
                
                if (string.IsNullOrEmpty(strCmd)) return false;


                NotifyPackagePutFinishNewModel cmdModel = strCmd.ToObject<NotifyPackagePutFinishNewModel>();
                string cmdPara = JsonConvert.SerializeObject(cmdModel);


                string outInterfaceName = "NotifyPackagePutFinishNew";
                NotifyElement upLoadElement = new NotifyElement("", outInterfaceName, "单次码垛完成上报", null, cmdPara);

            
                OperateResult<object> uploadResult = UpperServiceHelper.WmsServiceInvoke(upLoadElement);
                if (!uploadResult.IsSuccess)
                {
                    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", outInterfaceName, uploadResult.Message);
                    LogMessage(msg, EnumLogLevel.Info, false);
                    isSendOK = false;
                }
                else
                {
                    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", outInterfaceName, uploadResult.Message);
                    LogMessage(msg, EnumLogLevel.Info, false);
                    isSendOK = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage("SendPackagePutFinishNew 异常：" + ex.ToString(), EnumLogLevel.Error, false);
                return false;
            }
            return isSendOK;
        }

        /// <summary>
        /// 处理入库完成指令（入库）
        /// </summary>
        /// <param name="cmd"></param>
        private void HandleOrderFinished(ReportExeOrderFinishCmd cmd)
        {
            try
            {
                //入库完成
                WcsOrderInfoForABB wcsOrderInfo = GetWcsCurOrderInfo();
                if (wcsOrderInfo == null)
                {
                    LogMessage("wcs GetWcsCurOrderInfo 获取1111 节点数据 错误", EnumLogLevel.Info, false);
                    return;
                }

                if (!wcsOrderInfo.order_no.Equals(cmd.order_no))
                {
                    //机器人上报的完成指令 错误
                    LogMessage("机器人上报的完成指令号错误，与wcs下发的不一致:" + cmd.order_no, EnumLogLevel.Info, false);
                    return;
                }
                int wcsTasktype = int.Parse(wcsOrderInfo.taskType);
                int wmsTaskType = GetWmsBusinessTypeByWcsTaskType(wcsOrderInfo.taskType);
                int realityCatchNum = cmd.customized_info.Count;
                int startAndEndPostion = int.Parse(wcsOrderInfo.startAndEndPostion);

                NotifyPackagePutFinishNewModel putFinishModel = new NotifyPackagePutFinishNewModel();
                List<PackagePutDetailModel> tempPackagePutDetailModelList = new List<PackagePutDetailModel>();
                List<SendExeOrder_ext_data> tempList = new List<SendExeOrder_ext_data>();
                string putFinishCmdPara = "";
                string outInterfaceName = "";
                NotifyElement upLoadElement = null;
                bool startIsEmpty = false;
                //string isForceTaskValue = ReadIsForceTask("01");
              
              
                switch (startAndEndPostion)
                {
                    case 13://1004-扫码位
                    case 23://1007-扫码位
                            //机器人实际抓的箱子数量
                        List<string> scannerInfoList = GetScannerData(cmd.area_code, realityCatchNum);
                        //XXXAO67510200/A2303-07-A33/495E20/2030327/50/20/495/SR1#1761/1009
                        if (realityCatchNum == 1)
                        {
                            //单抓
                            if (scannerInfoList.Contains("ERROR") || scannerInfoList.Count == 0)
                            {
                                //表示未扫到 则生成 35指令
                                OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 35, 3, realityCatchNum);
                                if (writeOpc1111.IsSuccess)
                                {
                                    tempList.Add(new SendExeOrder_ext_data
                                    {
                                        index = cmd.customized_info[0].index.ToString(),
                                        box_barcode = "",
                                        start_position = cmd.customized_info[0].start_position.ToString()
                                    });
                                    OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                                    if (!opUpdateOrderExeDataResult.IsSuccess) break;
                                    #region 暂时注释
                                    //if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                                    //{
                                    //    if (cmd.start_isempty)
                                    //    {
                                    //        //需要清理NG 
                                    //        OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                                    //        //起始位置已空
                                    //        NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                                    //        //1122
                                    //        OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                                    //        if (isLoaded.IsSuccess && isLoaded.Content)
                                    //        {
                                    //            bool isHaveGoods = IsHaveGoodsForABBVision("01", 1008);
                                    //            if (isHaveGoods)
                                    //            {
                                    //                //zxzx20221230
                                    //                //叠盘接口调用成功
                                    //                OperateResult op = NotifyWmsPalletizerFinish("01");
                                    //                if (!op.IsSuccess) return; //通知失败
                                    //                NotifyTrayInAndOut(1008, true);
                                    //            }
                                    //            //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                                    //    }
                                    //    SetABBOrderFinishedData("01", null);
                                    //    return;
                                    //}
                                    #endregion
                                }
                            }
                            else
                            {
                                tempList.Add(new SendExeOrder_ext_data
                                {
                                    index = cmd.customized_info[0].index.ToString(),
                                    box_barcode = scannerInfoList[0].Split('#')[0],
                                    start_position = cmd.customized_info[0].start_position.ToString()
                                });
                                OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                                if (!opUpdateOrderExeDataResult.IsSuccess) break;

                                //正常 则调用wms 数据校验
                                NotifyPackageSkuBindBarcodeCmdModeNew wmsData = new NotifyPackageSkuBindBarcodeCmdModeNew();
                                wmsData.DATA.BusinessType = wmsTaskType;
                                wmsData.DATA.PalletBarcode = GetTrayNoByNodeID(1008);
                                wmsData.DATA.SrcAddr = GetWmsAddrByWcsAddr("01", int.Parse(startAndEndPostion.ToString().Substring(0, 1)));

                                List<PackageCheckDetailModelNew> pkgCheckData = new List<PackageCheckDetailModelNew>();
                                pkgCheckData.Add(new PackageCheckDetailModelNew
                                {
                                    PackageBarcode = scannerInfoList[0].Split('#')[0],
                                    SrcPosIndex = cmd.customized_info[0].start_position
                                });
                                wmsData.DATA.PackageCheckData = pkgCheckData;
                                //2、调用WMS接口进行上报校验(同步)
                                string cmdPara = JsonConvert.SerializeObject(wmsData);
                                NotifyElement element = new NotifyElement("", "NotifyPackageBarcodeCheckNew", "扫码数据上报", null, cmdPara);

                                OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

                                if (!result.IsSuccess || result.Content == null)
                                {
                                    string msg = string.Format("调用上层接口失败，详情：\r\n {0}", result.Message);
                                    LogMessage(msg, EnumLogLevel.Error, true);
                                    return;
                                }
                                //3、将WMS同步返回的数据，返回给机器人
                                //解析WMS返回的PackageId
                                NotifyPackageBarcodeCheckNewResponse packgeBarcodeResponse = result.Content.ToString().ToObject<NotifyPackageBarcodeCheckNewResponse>();
                                //NotifyPackageBarcodeCheckNewResponse packgeBarcodeResponse = JsonConvert.DeserializeObject<NotifyPackageBarcodeCheckNewResponse>(result.Content.ToString());
                                if (packgeBarcodeResponse.DATA.DestAddrType == 1)
                                {
                                    //验证通过 去目标 码垛位4
                                    OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, GetTrayNoByNodeID(1008), 34, 3, realityCatchNum);
                                    if (writeOpc1111.IsSuccess)
                                    {
                                        if (IsSendCmdForAbbRobot("01", writeOpc1111.Content))
                                        {
                                            #region 暂注释
                                            //if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 4).IsSuccess)
                                            //{
                                            //    if (cmd.start_isempty)
                                            //    {
                                            //        //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口
                                            //        bool isCheckABBNG = IsCheckABBNG("01");
                                            //        if (isCheckABBNG)
                                            //        {
                                            //            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                                            //        }
                                            //        //起始位置已空
                                            //        NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                                            //    }
                                            //    else
                                            //    {
                                            //        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                                            //    }
                                            //    SetABBOrderFinishedData("01", null);
                                            //    return;
                                            //}
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 暂注释
                                            //if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                                            //{
                                            //    if (cmd.start_isempty)
                                            //    {
                                            //        //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口
                                            //        bool isCheckABBNG = IsCheckABBNG("01");
                                            //        if (isCheckABBNG)
                                            //        {
                                            //            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                                            //        }
                                            //        //起始位置已空
                                            //        NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                                            //    }
                                            //    else
                                            //    {
                                            //        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                                            //    }
                                            //    SetABBOrderFinishedData("01", null);
                                            //    return;
                                            //}
                                            #endregion
                                        }
                                    }
                                }
                                else if (packgeBarcodeResponse.DATA.DestAddrType == 2 || packgeBarcodeResponse.DATA.DestAddrType == 4)
                                {
                                    #region 暂注释
                                    //OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                                    //if (isLoaded.IsSuccess && isLoaded.Content)
                                    //{
                                    //    //叠盘接口调用成功
                                    //    OperateResult op = NotifyWmsPalletizerFinish("01");
                                    //    if (!op.IsSuccess) return; //通知失败
                                    //    NotifyTrayInAndOut(1008, true);
                                    //    //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                                    //}

                                    ////回起始位 
                                    //int actType = startAndEndPostion == 13 ? 31 : 32;
                                    //OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", actType, 3, realityCatchNum);
                                    //if (writeOpc1111.IsSuccess)
                                    //{
                                    //    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                                    //    {
                                    //        SetABBOrderFinishedData("01", null);
                                    //        //NotifyTrayInAndOut(1008, true);

                                    //        NotifyIsEmpty("01", wmsTaskType, false, actType.ToString());
                                    //        //roller1008.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -100);
                                    //        //roller1008.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 98);
                                    //        return;
                                    //    }
                                    //}
                                    #endregion
                                }
                                else if (packgeBarcodeResponse.DATA.DestAddrType == 3)
                                {
                                    #region 暂注释
                                    ////验证失败去 NG
                                    //OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 35, 3, realityCatchNum);
                                    //if (writeOpc1111.IsSuccess)
                                    //{
                                    //    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                                    //    {
                                    //        //记录当前为其它批次333333
                                    //        IniHelper.WriteIniData("3003", "NodeNGIsHaveMultipleBatches", "true", wcsWareHouseInFullPath);

                                    //        if (cmd.start_isempty)
                                    //        {
                                    //            //需要清理NG  
                                    //            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                                    //            //起始位置已空
                                    //            NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());

                                    //            OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                                    //            if (isLoaded.IsSuccess && isLoaded.Content)
                                    //            {
                                    //                //叠盘接口调用成功
                                    //                OperateResult op = NotifyWmsPalletizerFinish("01");
                                    //                if (!op.IsSuccess) return; //通知失败
                                    //                NotifyTrayInAndOut(1008, true);
                                    //                //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                                    //        }
                                    //        SetABBOrderFinishedData("01", null);
                                    //        return;
                                    //    }
                                    //}
                                    #endregion
                                }
                            }
                        }
                        break;
                    case 31://扫码位-1004 （回退）
                    case 32://扫码位-1007 （回退）
                        
                        break;
                    case 34://扫码位-1008
                        putFinishModel.BusinessType = wmsTaskType;
                        putFinishModel.PalletBarcode = GetTrayNoByNodeID(1008);//垛号
                        //putFinishModel.SrcAddr = GetWmsAddrByWcsAddr("01", 3);
                        putFinishModel.DestAddr = GetWmsAddrByWcsAddr("01", 4);

                        //putFinishModel.SrcIsEmpty = curInOrderFinishCmd.start_isempty;
                        putFinishModel.DestIsFull = curInOrderFinishCmd.dest_isfull;
                        startIsEmpty = startPostionLockedIsEmpty("01");
                        putFinishModel.SrcIsEmpty = startIsEmpty;
                        putFinishModel.SrcAddr = GetLockedAddrByAreaCode("01", startIsEmpty);

                        if (curInOrderFinishCmd.customized_info != null)
                        {
                            for (int i = 0; i < curInOrderFinishCmd.customized_info.Count; i++)
                            {
                                tempPackagePutDetailModelList.Add(new PackagePutDetailModel
                                {
                                    PackageBarcode = curInOrderFinishCmd.customized_info[i].box_barcode,
                                    SrcPosIndex = curInOrderFinishCmd.customized_info[i].start_position,
                                    DestPosIndex = curInOrderFinishCmd.customized_info[i].dest_position
                                });
                            }
                        }
                        #region 暂注释
                        //OperateResult<int> weightResult = roller1008.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
                        //if (weightResult.IsSuccess)
                        //{
                        //    putFinishModel.Weight = weightResult.Content;
                        //}
                        //putFinishModel.PackageDetailData = tempPackagePutDetailModelList;

                        //putFinishCmdPara = JsonConvert.SerializeObject(putFinishModel);
                        //outInterfaceName = "NotifyPackagePutFinishNew";
                        //upLoadElement = new NotifyElement("", outInterfaceName, "单次码垛完成上报", null, putFinishCmdPara);
                        //OperateResult<object> uploadResult2 = UpperServiceHelper.WmsServiceInvoke(upLoadElement);
                    
                        //if (!uploadResult2.IsSuccess)
                        //{
                        //    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", outInterfaceName, uploadResult2.Message);
                        //}
                        //else
                        //{
                        //    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", outInterfaceName, uploadResult2.Message);
                        //    //resResponseResult.error = 0;
                        //    //resResponseResult.error_message = "上报成功！";
                        //    NotifyIsEmpty("01", 0, false , "4");

                        //    //生成机器人指令 异步发送
                        //    if (putFinishModel.DestIsFull)
                        //    {
                        //        OperateResult<bool> isLoaded1008 = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //        if (isLoaded1008.IsSuccess && isLoaded1008.Content)
                        //        {
                        //            OperateResult op = NotifyWmsPalletizerFinish("01");
                        //            if (!op.IsSuccess) return; //通知失败
                        //            NotifyTrayInAndOut(1008, true);
                        //            OperateResult<int> taskTypeResult = roller1008.Communicate.ReadInt(DataBlockNameEnum.PickingReadyDataBlock);
                        //            if (taskTypeResult.IsSuccess && taskTypeResult.Content == 18)
                        //            {
                        //                //清理库存整理
                        //                roller3008.Communicate.Write(DataBlockNameEnum.OPCWeightDataBlock, 0);
                        //                roller1008.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                        //            }

                        //            //判断空托盘和NG
                        //            if (putFinishModel.SrcAddr.Equals("GetGoodsPort:1_1_1"))
                        //            {
                        //                //判断托盘是否已空 以及
                        //                string read1004Value = IniHelper.ReadIniData("3003", "Node1004IsEmpty", "", wcsWareHouseInFullPath);
                        //                if (read1004Value.Equals("true"))
                        //                {
                        //                    //起始位置已空
                        //                    bool isCheckABBNG = IsCheckABBNG("01");
                        //                    if (isCheckABBNG)
                        //                    {
                        //                        OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                        //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                        {
                        //                            UpdateOrderExtData("01", tempList);
                        //                            //清空WCS任务
                        //                            roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                            SetABBOrderFinishedData("01", null);
                        //                        }
                        //                        return;
                        //                    }
                        //                    else
                        //                    {
                        //                        //通知PLC放走
                        //                        bool isNotifyOK = NotifyPlcRunByOPC(1004);
                        //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                        {
                        //                            UpdateOrderExtData("01", tempList);
                        //                            //清空WCS任务
                        //                            roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                            SetABBOrderFinishedData("01", null);
                        //                        }
                        //                        return;
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    //非空托盘 继续发送
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        //清空WCS任务
                        //                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                }
                        //            }
                        //            else if (putFinishModel.SrcAddr.Equals("GetGoodsPort:2_1_1"))
                        //            {
                        //                //判断托盘是否已空 以及
                        //                string read1007Value = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);
                        //                if (read1007Value.Equals("true"))
                        //                {
                        //                    //起始位置已空
                        //                    bool isCheckABBNG = IsCheckABBNG("01");
                        //                    if (isCheckABBNG)
                        //                    {
                        //                        OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                        //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                        {
                        //                            UpdateOrderExtData("01", tempList);
                        //                            //清空WCS任务
                        //                            roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                            SetABBOrderFinishedData("01", null);
                        //                        }
                        //                        return;
                        //                    }
                        //                    else
                        //                    {
                        //                        //通知PLC放走
                        //                        bool isNotifyOK = NotifyPlcRunByOPC(1007);
                        //                        if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                        {
                        //                            UpdateOrderExtData("01", tempList);
                        //                            //清空WCS任务
                        //                            roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                            SetABBOrderFinishedData("01", null);
                        //                        }
                        //                        return;
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    //非空托盘 继续发送
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        //清空WCS任务
                        //                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //目标没有满 可以继续下发 13或23指令
                        //        if (putFinishModel.SrcAddr.Equals("GetGoodsPort:1_1_1"))
                        //        {

                        //            //判断托盘是否已空 以及
                        //            string read1004Value = IniHelper.ReadIniData("3003", "Node1004IsEmpty", "", wcsWareHouseInFullPath);
                        //            if (read1004Value.Equals("true"))
                        //            {
                        //                OperateResult op = NotifyWmsPalletizerFinish("01");
                        //                if (!op.IsSuccess) return; //通知失败
                        //                NotifyTrayInAndOut(1008, true);

                        //                //起始位置已空
                        //                bool isCheckABBNG = IsCheckABBNG("01");
                        //                if (isCheckABBNG)
                        //                {
                        //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        //清空WCS任务
                        //                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                    return;
                        //                }
                        //                else
                        //                {
                        //                    //通知PLC放走
                        //                    bool isNotifyOK = NotifyPlcRunByOPC(1004);
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        //清空WCS任务
                        //                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                    //通知wms成功后，清除已空 信息
                        //                    ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                        //                    return;
                        //                }
                        //            }
                        //            else
                        //            {
                        //                //直接发送13
                        //                OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 13, 1, AbbCatchNum);
                        //                if(IsSendCmdForAbbRobot("01", writeOpc1111.Content))
                        //                {
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 4).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        else if (putFinishModel.SrcAddr.Equals("GetGoodsPort:2_1_1"))
                        //        {
                        //            //判断托盘是否已空 以及
                        //            string read1007Value = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);
                                 
                        //            if (read1007Value.Equals("true"))
                        //            {
                        //                OperateResult op = NotifyWmsPalletizerFinish("01");
                        //                if (!op.IsSuccess) return; //通知失败
                        //                NotifyTrayInAndOut(1008, true);
                                       
                        //                OperateResult<int> taskTypeResult = roller1008.Communicate.ReadInt(DataBlockNameEnum.PickingReadyDataBlock);
                        //                if (taskTypeResult.IsSuccess && taskTypeResult.Content == 18)
                        //                {
                        //                    //清理库存整理
                        //                    roller3008.Communicate.Write(DataBlockNameEnum.OPCWeightDataBlock, 0);
                        //                    roller1008.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                        //                }

                        //                //起始位置已空
                        //                bool isCheckABBNG = IsCheckABBNG("01");
                        //                if (isCheckABBNG)
                        //                {
                        //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        //清空WCS任务
                        //                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                    return;
                        //                }
                        //                else
                        //                {
                        //                    //通知PLC放走
                        //                    bool isNotifyOK = NotifyPlcRunByOPC(1007);
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        //清空WCS任务
                        //                        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                    //通知wms成功后，清除已空 信息
                        //                    ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                        //                    return;
                        //                }
                        //            }
                        //            else
                        //            {
                        //                //直接发送23
                        //                OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 23, 2, AbbCatchNum);
                        //                if (IsSendCmdForAbbRobot("01", writeOpc1111.Content))
                        //                {
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 4).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                }
                        //                else
                        //                {
                        //                    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                        //                    {
                        //                        UpdateOrderExtData("01", tempList);
                        //                        SetABBOrderFinishedData("01", null);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }

                        //    //if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //    //{
                        //    //    UpdateOrderExtData("01", tempList);
                        //    //    //清空WCS任务
                        //    //    roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //    //    SetABBOrderFinishedData("01", null);

                        //    //    if (putFinishModel.DestIsFull)
                        //    //    {
                        //    //        OperateResult<bool> isLoaded1008 = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //    //        if (isLoaded1008.IsSuccess && isLoaded1008.Content)
                        //    //        {
                        //    //            OperateResult op = NotifyWmsPalletizerFinish("01");
                        //    //            if (!op.IsSuccess) return; //通知失败
                        //    //            NotifyTrayInAndOut(1008, true);
                        //    //            //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);
                        //    //            OperateResult<int> taskTypeResult = roller1008.Communicate.ReadInt(DataBlockNameEnum.PickingReadyDataBlock);
                        //    //            if (taskTypeResult.IsSuccess && taskTypeResult.Content == 18)
                        //    //            {
                        //    //                //清理库存整理
                        //    //                roller3008.Communicate.Write(DataBlockNameEnum.OPCWeightDataBlock, 0);
                        //    //                roller1008.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                        //    //            }
                        //    //        }
                        //    //    }

                        //    //    //判断托盘是否已空 以及
                        //    //    //string read1004Value = IniHelper.ReadIniData("3003", "Node1004IsEmpty", "", wcsWareHouseInFullPath);
                        //    //    //string read1007Value = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);

                        //    //    //整改
                        //    //    //GetGoodsPort:1_1_1  GetGoodsPort:1_1_1
                        //    //    if(putFinishModel.SrcAddr.Equals("GetGoodsPort:1_1_1"))
                        //    //    {
                        //    //        //判断托盘是否已空 以及
                        //    //        string read1004Value = IniHelper.ReadIniData("3003", "Node1004IsEmpty", "", wcsWareHouseInFullPath);
                        //    //        if (read1004Value.Equals("true"))
                        //    //        {
                        //    //            OperateResult<bool> nodeIsloaed1004 = roller1004.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //    //            OperateResult<int> nodeOrderId1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                        //    //            if (nodeIsloaed1004.IsSuccess && nodeIsloaed1004.Content)
                        //    //            {
                        //    //                if (nodeOrderId1004.IsSuccess && nodeOrderId1004.Content == 99)
                        //    //                {
                        //    //                    OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //    //                    if (isLoaded.IsSuccess && isLoaded.Content)
                        //    //                    {
                        //    //                        OperateResult op = NotifyWmsPalletizerFinish("01");
                        //    //                        if (!op.IsSuccess) return; //通知失败
                        //    //                        NotifyTrayInAndOut(1008, true);
                        //    //                        //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);

                        //    //                        OperateResult<int> taskTypeResult = roller1008.Communicate.ReadInt(DataBlockNameEnum.PickingReadyDataBlock);
                        //    //                        if (taskTypeResult.IsSuccess && taskTypeResult.Content == 18)
                        //    //                        {
                        //    //                            //清理库存整理
                        //    //                            roller3008.Communicate.Write(DataBlockNameEnum.OPCWeightDataBlock, 0);
                        //    //                            roller1008.Communicate.Write(DataBlockNameEnum.PickingReadyDataBlock, 0);
                        //    //                        }

                        //    //                        bool isCheckABBNG = IsCheckABBNG("01");
                        //    //                        if (isCheckABBNG)
                        //    //                        {
                        //    //                            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                        //    //                        }
                        //    //                        else
                        //    //                        {
                        //    //                            //通知PLC放走
                        //    //                            NotifyPlcRunByOPC(1004);
                        //    //                        }
                        //    //                    }
                        //    //                    //通知wms成功后，清除已空 信息
                        //    //                    ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                        //    //                    return;
                        //    //                }
                        //    //            }
                        //    //        }

                        //    //    }
                        //    //    else if (putFinishModel.SrcAddr.Equals("GetGoodsPort:2_1_1"))
                        //    //    {
                        //    //        string read1007Value = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);
                        //    //        if (read1007Value.Equals("true"))
                        //    //        {
                        //    //            OperateResult<bool> nodeIsloaed1007 = roller1007.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //    //            OperateResult<int> nodeOrderId1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                        //    //            if (nodeIsloaed1007.IsSuccess && nodeIsloaed1007.Content)
                        //    //            {
                        //    //                if (nodeOrderId1007.IsSuccess && nodeOrderId1007.Content == 99)
                        //    //                {
                        //    //                    OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //    //                    if (isLoaded.IsSuccess && isLoaded.Content)
                        //    //                    {
                        //    //                        OperateResult op = NotifyWmsPalletizerFinish("01");
                        //    //                        if (!op.IsSuccess) return; //通知失败
                        //    //                        NotifyTrayInAndOut(1008, true);
                        //    //                        //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);

                        //    //                        bool isCheckABBNG = IsCheckABBNG("01");
                        //    //                        if (isCheckABBNG)
                        //    //                        {
                        //    //                            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                        //    //                        }
                        //    //                        else
                        //    //                        {
                        //    //                            //通知PLC放走
                        //    //                            NotifyPlcRunByOPC(1007);
                        //    //                        }
                        //    //                    }
                        //    //                    //通知wms成功后，清除已空 信息
                        //    //                    ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                        //    //                    return;
                        //    //                }
                        //    //            }
                        //    //        }
                        //    //    }
                        //    //    //string read1007Value = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);
                        //    //}
                        //}
                        #endregion
                        break;
                    case 35://扫码位-NG （未扫到）
                            //NotifyPackagePutFinishNewModel putFinishModel = new NotifyPackagePutFinishNewModel();
                        putFinishModel.BusinessType = wmsTaskType;
                        putFinishModel.PalletBarcode = GetTrayNoByNodeID(1008);//垛号

                        //putFinishModel.SrcAddr = GetWmsAddrByWcsAddr("01", 3);
                        putFinishModel.DestAddr = GetWmsAddrByWcsAddr("01", 5);
                        //putFinishModel.SrcIsEmpty = curInOrderFinishCmd.start_isempty;
                        putFinishModel.DestIsFull = curInOrderFinishCmd.dest_isfull;


                        startIsEmpty = startPostionLockedIsEmpty("01");
                        putFinishModel.SrcIsEmpty = startIsEmpty;
                        putFinishModel.SrcAddr = GetLockedAddrByAreaCode("01", startIsEmpty);

                        //putFinishModel.SrcAddr = GetLockedAddrByAreaCode("01");
                        //putFinishModel.SrcIsEmpty = startPostionLockedIsEmpty("01");


                        //List<PackagePutDetailModel> tempPackagePutDetailModelList = new List<PackagePutDetailModel>();
                        if (curInOrderFinishCmd.customized_info != null)
                        {
                            for (int i = 0; i < curInOrderFinishCmd.customized_info.Count; i++)
                            {
                                tempPackagePutDetailModelList.Add(new PackagePutDetailModel
                                {
                                    PackageBarcode = curInOrderFinishCmd.customized_info[i].box_barcode,
                                    SrcPosIndex = curInOrderFinishCmd.customized_info[i].start_position,
                                    DestPosIndex = curInOrderFinishCmd.customized_info[i].dest_position
                                });
                            }
                        }

                        putFinishModel.PackageDetailData = tempPackagePutDetailModelList;
                        putFinishCmdPara = JsonConvert.SerializeObject(putFinishModel);
                        outInterfaceName = "NotifyPackagePutFinishNew";
                        upLoadElement = new NotifyElement("", outInterfaceName, "单次码垛完成上报", null, putFinishCmdPara);
                      
                        OperateResult<object> uploadResul1t1 = UpperServiceHelper.WmsServiceInvoke(upLoadElement);

                        if (!uploadResul1t1.IsSuccess)
                        {
                            string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", outInterfaceName, uploadResul1t1.Message);
                        }
                        else
                        {
                            string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", outInterfaceName, uploadResul1t1.Message);

                            #region 暂注释
                            ////判断托盘是否已空 以及
                            //string read1004Value = IniHelper.ReadIniData("3003", "Node1004IsEmpty", "", wcsWareHouseInFullPath);
                            //if (read1004Value.Equals("true"))
                            //{
                            //    OperateResult<bool> nodeIsloaed1004 = roller1004.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                            //    OperateResult<int> nodeOrderId1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                            //    if (nodeIsloaed1004.IsSuccess && nodeIsloaed1004.Content)
                            //    {
                            //        if (nodeOrderId1004.IsSuccess && nodeOrderId1004.Content == 99)
                            //        {
                            //            OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                            //            if (isLoaded.IsSuccess && isLoaded.Content)
                            //            {
                            //                bool isHaveGoods = IsHaveGoodsForABBVision("01", 1008);
                            //                if(isHaveGoods)
                            //                {
                            //                    //有货
                            //                    OperateResult op = NotifyWmsPalletizerFinish("01");
                            //                    if (!op.IsSuccess) return; //通知失败
                            //                    NotifyTrayInAndOut(1008, true);
                            //                }
                            //                //else
                            //                //{
                            //                //    //无货
                            //                //}
                            //                //OperateResult op = NotifyWmsPalletizerFinish("01");
                            //                //if (!op.IsSuccess) return; //通知失败
                            //                //NotifyTrayInAndOut(1008, true);
                            //                //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);

                            //                bool isCheckABBNG = IsCheckABBNG("01");
                            //                if (isCheckABBNG)
                            //                {
                            //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                            //                }
                            //                else
                            //                {
                            //                    //通知PLC放走
                            //                    NotifyPlcRunByOPC(1004);
                            //                }
                            //            }
                            //            ////通知wms成功后，清除已空 信息
                            //            //ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);

                            //            if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                            //            {
                            //                SendPackagePutFinishNew("01");
                            //                UpdateOrderExtData("01", tempList);
                            //                //清空WCS任务
                            //                roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                            //                if (curInOrderFinishCmd.dest_isfull)
                            //                {
                            //                    //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口 to do
                            //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                            //                }
                            //                SetABBOrderFinishedData("01", null);
                            //                ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                            //                return;
                            //            }

                            //            return;
                            //        }
                            //    }
                            //}
                            //string read1007Value = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);
                            //if (read1007Value.Equals("true"))
                            //{
                            //    OperateResult<bool> nodeIsloaed1007 = roller1007.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                            //    OperateResult<int> nodeOrderId1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                            //    if (nodeIsloaed1007.IsSuccess && nodeIsloaed1007.Content)
                            //    {
                            //        if (nodeOrderId1007.IsSuccess && nodeOrderId1007.Content == 99)
                            //        {
                            //            OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                            //            if (isLoaded.IsSuccess && isLoaded.Content)
                            //            {
                            //                bool isHaveGoods = IsHaveGoodsForABBVision("01", 1008);
                            //                if (isHaveGoods)
                            //                {
                            //                    //有货
                            //                    OperateResult op = NotifyWmsPalletizerFinish("01");
                            //                    if (!op.IsSuccess) return; //通知失败
                            //                    NotifyTrayInAndOut(1008, true);
                            //                }

                            //                //OperateResult op = NotifyWmsPalletizerFinish("01");
                            //                //if (!op.IsSuccess) return; //通知失败
                            //                //NotifyTrayInAndOut(1008, true);

                            //                //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);

                            //                bool isCheckABBNG = IsCheckABBNG("01");
                            //                if (isCheckABBNG)
                            //                {
                            //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                            //                }
                            //                else
                            //                {
                            //                    //通知PLC放走
                            //                    NotifyPlcRunByOPC(1007);
                            //                }
                            //            }
                            //            ////通知wms成功后，清除已空 信息
                            //            //ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                            //            if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                            //            {
                            //                SendPackagePutFinishNew("01");
                            //                UpdateOrderExtData("01", tempList);
                            //                //清空WCS任务
                            //                roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                            //                if (curInOrderFinishCmd.dest_isfull)
                            //                {
                            //                    //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口 to do
                            //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                            //                }
                            //                SetABBOrderFinishedData("01", null);
                            //                ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                            //                return;
                            //            }
                            //            return;
                            //        }
                            //    }
                            //}

                            //if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                            //{
                            //    SendPackagePutFinishNew("01");
                            //    UpdateOrderExtData("01", tempList);
                            //    //清空WCS任务
                            //    roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                            //    if (curInOrderFinishCmd.dest_isfull)
                            //    {
                            //        //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口 to do
                            //        OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                            //        bool isHaveGoods = IsHaveGoodsForABBVision("01", 1008);
                            //        if (isHaveGoods)
                            //        {
                            //            //有货
                            //            OperateResult op = NotifyWmsPalletizerFinish("01");
                            //            if (!op.IsSuccess) return; //通知失败
                            //            NotifyTrayInAndOut(1008, true);
                            //        }
                                  
                            //    }
                            //    SetABBOrderFinishedData("01", null);
                            //    ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                            //    return;
                            //}
                            #endregion
                        }
                        break;
                    case 51://NG-1004 清理NG
                    case 52://NG-1007 清理NG
                        putFinishModel.BusinessType = wmsTaskType;
                        putFinishModel.PalletBarcode = GetTrayNoByNodeID(1008);//垛号

                        putFinishModel.SrcAddr = GetWmsAddrByWcsAddr("01", 5);
                        putFinishModel.DestAddr = GetWmsAddrByWcsAddr("01", int.Parse(startAndEndPostion.ToString().Substring(1, 1)));
                        putFinishModel.SrcIsEmpty = curInOrderFinishCmd.start_isempty;
                        putFinishModel.DestIsFull = curInOrderFinishCmd.dest_isfull;

                        //List<PackagePutDetailModel> tempPackagePutDetailModelList = new List<PackagePutDetailModel>();
                        if (curInOrderFinishCmd.customized_info != null)
                        {
                            for (int i = 0; i < curInOrderFinishCmd.customized_info.Count; i++)
                            {
                                tempPackagePutDetailModelList.Add(new PackagePutDetailModel
                                {
                                    PackageBarcode = curInOrderFinishCmd.customized_info[i].box_barcode,
                                    SrcPosIndex = curInOrderFinishCmd.customized_info[i].start_position,
                                    DestPosIndex = curInOrderFinishCmd.customized_info[i].dest_position
                                });
                            }
                        }

                        putFinishModel.PackageDetailData = tempPackagePutDetailModelList;
                        putFinishCmdPara = JsonConvert.SerializeObject(putFinishModel);
                        outInterfaceName = "NotifyPackagePutFinishNew";
                        upLoadElement = new NotifyElement("", outInterfaceName, "单次码垛完成上报", null, putFinishCmdPara);
                        OperateResult<object> uploadResul1t2 = UpperServiceHelper.WmsServiceInvoke(upLoadElement);

                        #region 暂注释
                        //if (!uploadResul1t2.IsSuccess)
                        //{
                        //    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", outInterfaceName, uploadResul1t2.Message);
                        //}
                        //else
                        //{
                        //    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", outInterfaceName, uploadResul1t2.Message);
                        //    //resResponseResult.error = 0;
                        //    //resResponseResult.error_message = "上报成功！";
                        //    if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0).IsSuccess)
                        //    {
                        //        UpdateOrderExtData("01", tempList);
                        //        //清空WCS任务
                        //        roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                        //        if (curInOrderFinishCmd.start_isempty)
                        //        {
                        //            //if (string.IsNullOrEmpty(isForceTaskValue) || string.IsNullOrEmpty(isForceTaskValue.Trim()))
                        //            //{
                        //            //    //表示 没有收到wms的任务
                        //            //}
                        //            //else
                        //            //{
                        //            //    if (isForceTaskValue.Equals("0"))
                        //            //    {
                        //            //        OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                        //            //        if (isLoaded.IsSuccess && isLoaded.Content)
                        //            //        {
                        //            //            //叠盘接口调用成功
                        //            //            OperateResult op = NotifyWmsPalletizerFinish("01");
                        //            //            if (!op.IsSuccess) return; //通知失败
                        //            //            NotifyTrayInAndOut(1008, true);
                        //            //            bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", "", wcsWareHouseInFullPath);
                        //            //        }
                        //            //    }
                        //            //    else if (isForceTaskValue.Equals("1"))
                        //            //    {
                        //            //        //1结束整个任务
                        //            //    }
                        //            //}

                        //            //NG已经清空
                        //            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0);
                        //            //生成 plc放行托盘 到入库口
                        //            string readValue = IniHelper.ReadIniData("3003", "NodeNGIsHaveMultipleBatches", "", wcsWareHouseInFullPath);
                        //            if (!string.IsNullOrEmpty(readValue))
                        //            {
                        //                if (readValue.Equals("false"))
                        //                {
                        //                    if (startAndEndPostion == 51)
                        //                    {
                        //                        //51  1004
                        //                        NotifyPlcRunByOPC(1004);
                        //                    }
                        //                    else
                        //                    {
                        //                        //52  1007
                        //                        NotifyPlcRunByOPC(1007);
                        //                    }
                        //                }
                        //            }
                                   
                        //            //to do 判断是否有货 
                        //            bool isHaveGoods1008 = IsHaveGoodsForABBVision("01", 1008);
                        //            if (isHaveGoods1008)
                        //            {
                        //                //叠盘接口调用成功
                        //                OperateResult op = NotifyWmsPalletizerFinish("01");
                        //                if (!op.IsSuccess) return; //通知失败
                        //                NotifyTrayInAndOut(1008, true);
                        //                //bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                        //            }
                        //            IniHelper.WriteIniData("3003", "NodeNGIsHaveMultipleBatches", "false", wcsWareHouseInFullPath);
                        //            ClearWcsNodeIsEmpty("01", putFinishModel.SrcAddr);
                        //        }
                        //        if (curInOrderFinishCmd.dest_isfull)
                        //        {
                        //            //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口 to do
                        //            //只有一种情况，之前的NG 一直没有清理。。 人工搬走
                        //        }
                        //        SetABBOrderFinishedData("01", null);
                        //        return;
                        //    }
                        //}
                        #endregion
                        break;
                    case 12:
                    case 14://1004-1008(需要扫描 所以不处理)********
                    case 15:
                    case 54:
                    case 21:
                    case 24://1007-1008(需要扫描 所以不处理)********
                    case 25:
                    case 43://批次整理 1008-扫码 （不用扫 不处理）********
                    case 45:
                    case 53://NG-扫码  不处理（不用扫 不处理）************
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogMessage("wcs HandleInWareHouseCompleteTaskCmd 处理入库完成指令时错误：" + ex.ToString(), EnumLogLevel.Error, false);
            }
        }


        private void DispatchWmsStockBus()
        {
            //Thread.Sleep(1 * 1000);//等待plc称重平稳
            try
            {
                int times = 10;
                while (true)
                {
                    if (times == 0)
                    {
                        break;
                    }
                    Thread.Sleep(10);
                    times--;
                }

                DeviceBaseAbstract xDevice2001 = DeviceManage.Instance.FindDeivceByDeviceId(2001);
                TransportPointStation xTransDevice2001 = xDevice2001 as TransportPointStation;
                RollerDeviceControl xRoller2001 = xTransDevice2001.DeviceControl as RollerDeviceControl;
                OperateResult<int> xOpWeight2001 = xRoller2001.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
                OperateResult<string> xOpReadBarCode2001 = xRoller2001.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                //NotifyStocktakingInstructResult
                NotifyStocktakingResultCmdMode cmdMode = new NotifyStocktakingResultCmdMode();
                cmdMode.DATA.DST_ADDR = xTransDevice2001.CurAddress.FullName;
                if (xOpWeight2001.IsSuccess)
                {
                    cmdMode.DATA.WEIGHT = xOpWeight2001.Content;
                }
                if (xOpReadBarCode2001.IsSuccess)
                {
                    cmdMode.DATA.PALLETBARCODE = xOpReadBarCode2001.Content;
                }

                string cmdPara = JsonConvert.SerializeObject(cmdMode);
                string interfaceName = "NotifyStocktakingInstructResult";
                NotifyElement element = new NotifyElement("", interfaceName, "盘点完成结果上报", null, cmdPara);
                OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

                if (!result.IsSuccess)
                {
                    string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                    LogMessage("上报盘点结果异常：" + msg, EnumLogLevel.Error, false);
                }
                else
                {
                    string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                    LogMessage("上报盘点结果成功：" + msg, EnumLogLevel.Info, false);
                }
            }
            catch (Exception ex)
            {
                LogMessage("上报盘点结果异常，参数数据：" + ex.ToString(), EnumLogLevel.Error, false);
            }

        }

        /// <summary>
        /// 查询节点ID 托盘编号 1008或2001
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private string GetTrayNoByNodeID(int nodeID)
        {
            string trayNo = "";
            //if (nodeID == 1008)
            //{
            //    OperateResult<string> opcBarCodeResult = roller1008.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            //    if (opcBarCodeResult.IsSuccess)
            //    {
            //        trayNo = opcBarCodeResult.Content;
            //    }
            //}
            return trayNo;
        }

        private int GetWmsBusinessTypeByWcsTaskType(string taskType)
        {
            //wcs 1入库、2出库、3库存整理入库、4库存整理出库 5、NG清理、6盘点

            //wms:1-入库；2-出库/拣选；3-盘点；4-库存整理
            switch (taskType)
            {
                case "1":
                    return 1;
                case "2":
                    return 2;
                case "3":
                case "4":
                    return 4;
                case "5":
                    return 1;
                case "6":
                    return 3;
                default:
                    break;
            }
            return 0;
        }
        private string GetWmsAddrByWcsAddr(string areaCode, int addrIndex)
        {
            if (areaCode.Equals("01"))
            {
                switch (addrIndex)
                {
                    case 1:
                        return "RobotCache:1_1_1";
                    case 2:
                        return "RobotCache:2_1_1";
                    case 3:
                        return "ScannerPort:1_1_1";
                    case 4:
                        return "RobotStacking:1_1_1";
                    case 5:
                        return "NGPort:1_1_1";
                    case 6:
                        return "RobotStacking:1_1_1";
                    default:
                        return "";
                }
            }
            return "";
        }

        private string GetLockedAddrByAreaCode(string areaCode, bool startPostionIsEmpty)
        {
            if (areaCode.Equals("01"))
            {
                string readValue = IniHelper.ReadIniData("3003", "StartPostionIsEmpty", "", wcsWareHouseInFullPath);
                if (startPostionIsEmpty)
                {
                    return GetWmsAddrByWcsAddr(areaCode, int.Parse(readValue.Trim()));
                }

                //OperateResult<int> result1004 = roller7501.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                //if (result1004.IsSuccess && result1004.Content == 99)
                //{
                //    return "RobotCache:1_1_1";
                //}

                //OperateResult<int> result1007 = roller7502.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                //if (result1007.IsSuccess && result1007.Content == 99)
                //{
                //    return "RobotCache:2_1_1";
                //}
                //to do
                return "RobotCache:1_1_1";
            }
            return "";
        }

        private bool startPostionLockedIsEmpty(string areaCode)
        {
            if (areaCode.Equals("01"))
            {
                string readValue = IniHelper.ReadIniData("3003", "StartPostionIsEmpty", "", wcsWareHouseInFullPath);
                if (string.IsNullOrEmpty(readValue) || readValue.Equals(0)) return false;
                if (readValue.Equals("1") || readValue.Equals("2") || readValue.Equals("4") || readValue.Equals("5"))
                {
                    return true;
                }
                //OperateResult<int> result1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                //if (result1004.IsSuccess && result1004.Content == 99)
                //{
                //    string readValue = IniHelper.ReadIniData("3003", "Node1004IsEmpty", "", wcsWareHouseInFullPath);
                //    if(readValue.Equals("true"))
                //    {
                //        return true;
                //    }
                //}

                //OperateResult<int> result1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                //if (result1007.IsSuccess && result1007.Content == 99)
                //{
                //    string readValue = IniHelper.ReadIniData("3003", "Node1007IsEmpty", "", wcsWareHouseInFullPath);
                //    if (readValue.Equals("true"))
                //    {
                //        return true;
                //    }
                //}
            }
            return false;
        }

        private void ClearWcsNodeIsEmpty(string areaCode, string wmsAddr)
        {
            if (areaCode.Equals("01"))
            {
                if (wmsAddr.Equals("GetGoodsPort:1_1_1"))
                {
                    IniHelper.WriteIniData("3003", "Node1004IsEmpty", "false", wcsWareHouseInFullPath);
                }
                else if (wmsAddr.Equals("GetGoodsPort:2_1_1"))
                {
                    IniHelper.WriteIniData("3003", "Node1007IsEmpty", "false", wcsWareHouseInFullPath);
                }
                else if (wmsAddr.Equals("PutGoodsPort:1_1_1"))
                {
                    IniHelper.WriteIniData("3003", "Node1008IsEmpty", "false", wcsWareHouseInFullPath);
                }
                else if (wmsAddr.Equals("NGPort:1_1_1"))
                {
                    IniHelper.WriteIniData("3003", "NodeNGIsEmpty", "false", wcsWareHouseInFullPath);
                }

            }
            else if (areaCode.Equals("02"))
            {
                //to do
                //return "GetGoodsPort:2_1_1";
            }

        }
        /// <summary>
        /// 调用ABB NG接口 检测是否有货
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        private bool IsCheckABBNG(string areaCode)
        {
            bool isNotifyOk = false;
            //调用机器人通知载具更新接口
            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(3001) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
         
            OperateResult op = abbControl.GetNgStation(new GetNgStationMode { });
            LogMessage("查询NG GetNgStation 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
            if (op.IsSuccess)
            {
                isNotifyOk = true;
            }
            return isNotifyOk;
        }


        private bool IsABBInInit(string areaCode)
        {
            int abbNodeID = 0;
         
            if (areaCode.Equals("01"))
            {
                abbNodeID = 3001;
              
            }
            else if (areaCode.Equals("02"))
            {
                abbNodeID = 3002;
            }

            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(abbNodeID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;
          
            //调用视觉是否在原位
            OperateResult opGetInitResult = abbControl.GetInit(areaCode);
         
            if (!opGetInitResult.IsSuccess)
            {
                LogMessage("通知机器人初始位 GetInit 返回失败！", EnumLogLevel.Info, false);

                return false;
            }
            if (opGetInitResult.Message.Equals("false"))
            {
                LogMessage("通知机器人初始位 GetInit 成功，但不在初始位！", EnumLogLevel.Info, false);
                return false;
            }
            return true;
        }



        private bool IsHaveGoodsForABBVision(string areaCode, int nodeID)
        {
            int abbNodeID = 0;
            string addr = "";
            if (areaCode.Equals("01"))
            {
                abbNodeID = 3001;
                if (nodeID == 1004)
                {
                    addr = "1";
                }
                else if (nodeID == 1007)
                {
                    addr = "2";
                }
                else if (nodeID == 1008)
                {
                    addr = "4";
                }
            }
            else if (areaCode.Equals("02"))
            {
                abbNodeID = 3002;
                if (nodeID == 2001)
                {
                    addr = "1";
                }
            }

            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(abbNodeID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;

         
            OperateResult<InvokRobotVision_Response> op = abbControl.InvokRobotVision(new InvokRobotVisionMode { area_code = areaCode, visi_addr = addr });
            if (op.IsSuccess && op.Content != null)
            {
                //{"result": "1", "message": "success", "data": {"is_empty": 0, "count": 14}}
                return (op.Content.data != null && op.Content.data.is_empty == 0);
            }
            return false;
        }

        private List<string> GetScannerData(string areCode, int num)
        {
            List<string> tempBarCodeList = new List<string>();

            int deviceID = areCode.Equals("01") ? 111101 : 222201;
            DeviceBaseAbstract device = DeviceManage.Instance.FindDeivceByDeviceId(deviceID);
            if (device is KeyenceScanner)
            {
                KeyenceScanner scanner = (KeyenceScanner)device;

                List<string> tempCatchNumList = new List<string>();
                if (num == 1)
                {
                    tempCatchNumList.Add("1");
                }
                else if (num == 2)
                {
                    tempCatchNumList.Add("1");
                    tempCatchNumList.Add("2");
                }

                OperateResult<List<string>> opResult = scanner.GetIdentifyMessageSync(tempCatchNumList);
                if (opResult.IsSuccess)
                {
                    //调用读取条码成功
                    if (opResult.Content != null) return opResult.Content;
                }
            }
            return tempBarCodeList;
        }

        /// <summary>
        /// 检测是否一致  1004-1114、1007-1117、1008-1118
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private OperateResult<string> IsCheckLoadedAndBarCode(int nodeId)
        {
            OperateResult<string> opResult = OperateResult.CreateFailedResult<string>("");
            RollerDeviceControl startNodeControl = null;
            RollerDeviceControl endNodeControl = null;
            //if (nodeId == 1004)
            //{
            //    startNodeControl = roller1004;
            //    endNodeControl = roller1114;
            //}
            //else if (nodeId == 1007)
            //{
            //    startNodeControl = roller1007;
            //    endNodeControl = roller1117;
            //}
            //else if (nodeId == 1008)
            //{
            //    startNodeControl = roller1008;
            //    endNodeControl = roller1118;
            //}
            if (startNodeControl == null || endNodeControl == null) return opResult;
            OperateResult<int> opcOrderTypeResult = startNodeControl.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
            if (opcOrderTypeResult.IsSuccess)
            {
                if (opcOrderTypeResult.Content == 99)
                {
                    OperateResult<bool> opcStartIsloadedResult = startNodeControl.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    OperateResult<bool> opcEndIsloadedResult = endNodeControl.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    OperateResult<string> opcStartBarCodeResult = startNodeControl.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
                    OperateResult<string> opcEndBarCodeResult = endNodeControl.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                    if (opcStartIsloadedResult.IsSuccess && opcEndIsloadedResult.IsSuccess
                        && opcStartBarCodeResult.IsSuccess && opcEndBarCodeResult.IsSuccess)
                    {
                        if (nodeId == 2001)
                        {
                            if (opcStartIsloadedResult.Content && opcEndIsloadedResult.Content)
                            {
                                if (opcStartBarCodeResult.Content.Equals(opcEndBarCodeResult.Content))
                                {
                                    opResult.IsSuccess = true;
                                    opResult.Content = opcStartBarCodeResult.Content;
                                    opResult.Message = "匹配成功";
                                    return opResult;
                                }
                            }
                        }
                        else
                        {
                            if (opcStartIsloadedResult.Content)
                            {
                                if (opcStartBarCodeResult.Content.Equals(opcEndBarCodeResult.Content))
                                {
                                    opResult.IsSuccess = true;
                                    opResult.Content = opcStartBarCodeResult.Content;
                                    opResult.Message = "匹配成功";
                                    return opResult;
                                }
                            }
                        }
                    }
                }
            }
            return opResult;
        }

        /// <summary>
        /// 写入指令信息到节点1111
        /// </summary>
        /// <param name="taskType">托盘任务类型：1入库、2出库、3库存整理入库、4库存整理出库、5清理NG</param>
        /// <param name="trayNo">托盘编号</param>
        /// <param name="actType">起始目标 动作指令 13、23、41、42等等</param>
        /// <param name="curPostion">当前位置：1、2、3、4、5、6、7</param>
        /// <param name="catchNum">wcs下发机器人抓取的爪数量 ：1、2</param>
        ///// <param name="pickNum">机器人实际夹爪抓取数量：1、2</param>
        /// <returns>写入成功的cmd信息  任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量</returns>
        private OperateResult<string> WriteOrderInfoForNode1111(int taskType, string trayNo, int actType, int curPostion, int catchNum)
        {
            OperateResult<string> opResult = OperateResult.CreateFailedResult<string>("");

            #region 暂注释
            //OperateResult<int> opcOrderIdResult1111 = roller1111.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            //if (opcOrderIdResult1111.IsSuccess)
            //{
            //    int orderId = opcOrderIdResult1111.Content + 1;
            //    if (catchNum == 0)
            //    {
            //        catchNum = AbbCatchNum;
            //    }
            //    //任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量
            //    string cmd = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", taskType, trayNo, orderId, actType, curPostion, catchNum, 0);

            //    OperateResult opWriteOrderIdResult = roller1111.Communicate.Write(DataBlockNameEnum.OPCOrderIdDataBlock, orderId);
            //    if (opWriteOrderIdResult.IsSuccess)
            //    {
            //        OperateResult opWriteOrderInfoResult = roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, cmd);
            //        if (opWriteOrderInfoResult.IsSuccess)
            //        {
            //            opResult.IsSuccess = true;
            //            opResult.Content = cmd;
            //            opResult.ErrorCode = 0;
            //            return opResult;
            //        }
            //    }
            //}
            #endregion
            return opResult;
        }


        /// <summary>
        /// 创建指令信息 
        /// </summary>
        /// <param name="taskType">托盘任务类型：1入库、2出库、3库存整理入库、4库存整理出库、5清理NG</param>
        /// <param name="trayNo">托盘编号</param>
        /// <param name="actType">起始目标 动作指令 13、23、41、42等等</param>
        /// <param name="curPostion">当前位置：1、2、3、4、5、6、7</param>
        /// <param name="catchNum">wcs下发机器人抓取的爪数量 ：1、2</param>
        ///// <param name="pickNum">机器人实际夹爪抓取数量：1、2</param>
        /// <returns>写入成功的cmd信息  任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量</returns>
        private OperateResult<string> CreateOrderInfo(int taskType, string trayNo, int actType, int curPostion, int catchNum)
        {
            OperateResult<string> opResult = OperateResult.CreateFailedResult<string>("");

            #region 暂注释
            //OperateResult<int> opcOrderIdResult1111 = roller1111.Communicate.ReadInt(DataBlockNameEnum.OPCOrderIdDataBlock);
            //if (opcOrderIdResult1111.IsSuccess)
            //{
            //    int orderId = opcOrderIdResult1111.Content + 1;
            //    OperateResult opWriteOrderIDResult = roller1111.Communicate.Write(DataBlockNameEnum.OPCOrderIdDataBlock, orderId);
            //    if (opWriteOrderIDResult.IsSuccess)
            //    {

            //    }
            //    if (catchNum == 0)
            //    {
            //        catchNum = AbbCatchNum;
            //    }
            //    //任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量
            //    string cmd = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}", taskType, trayNo, orderId, actType, curPostion, catchNum, 0);

            //    opResult.IsSuccess = true;
            //    opResult.Content = cmd;
            //    opResult.ErrorCode = 0;
            //    return opResult;
            //}
            #endregion
            return opResult;
        }



        private OperateResult NotifyWmsPalletizerFinish(string areaCode)
        {
            OperateResult handleResult = OperateResult.CreateFailedResult();
            PalletizerFinishParaMode parmData = new PalletizerFinishParaMode();
            NotifyPalletizerFinishCmdMode mode = new NotifyPalletizerFinishCmdMode();
            if (areaCode.Equals("01"))
            {
                //调用wms叠盘完成
                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(1008);
                TransportPointStation transDevice = curDevice as TransportPointStation;
                RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                OperateResult<string> opcResult1008 = roller.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);

                OperateResult<int> goodsWeightResult = roller.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
                if (goodsWeightResult.IsSuccess)
                {
                    parmData.Weight = goodsWeightResult.Content;
                }
                parmData.ADDR = "RobotStacking:1_1_1";
                if (opcResult1008.IsSuccess && !string.IsNullOrEmpty(opcResult1008.Content))
                {
                    parmData.PALLETBARCODE = opcResult1008.Content;
                }
                mode.DATA = parmData;
            }

            string cmdPara = JsonConvert.SerializeObject(mode);
            string interfaceName = "NotifyPalletizerFinish";
            NotifyElement element = new NotifyElement("", interfaceName, "码盘完成上报", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                handleResult.ErrorCode = 2;
                handleResult.IsSuccess = false;
                handleResult.Message = result.Message;
            }
            else
            {


                string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                handleResult.IsSuccess = true;
                handleResult.Message = "上报码垛箱已满接口成功!";
            }
            return handleResult;
        }

        /// <summary>
        /// 根据任务类型 检查并生成指令
        /// </summary>
        /// <param name="taskType">1入  2库存整理出、3库存整理入</param>
        /// <returns>指令及结果</returns>
        private OperateResult<string> CheckAndGetCmd()
        {
            OperateResult<string> opResult = OperateResult.CreateFailedResult<string>("默认不符合");

            #region 暂注释
            ////安全检测  取货位和放货位是否都有托盘（1114、1117、1118）
            ////条码地址：虚拟托盘号|指令ID|优先级|起始目标位置|当前位置|发送抓取数量|实际抓取数量|机器人完成次数
            //OperateResult<int> opcOrderTypeResult1119 = roller1119.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
            //if (opcOrderTypeResult1119.IsSuccess && opcOrderTypeResult1119.Content == 99)
            //{
            //    //需要清理NG 41
            //    OperateResult<string> opcResult1004 = IsCheckLoadedAndBarCode(1004);
            //    if (opcResult1004.IsSuccess)
            //    {
            //        OperateResult<string> opcResult1111 = CreateOrderInfo(5, "", 51, 5, 1);
            //        if (opcResult1111.IsSuccess)
            //        {
            //            opResult.IsSuccess = true;
            //            opResult.Content = opcResult1111.Content;
            //            opResult.ErrorCode = 0;
            //            return opResult;
            //        }
            //    }
            //    //需要清理NG 42
            //    OperateResult<string> opcResult1007 = IsCheckLoadedAndBarCode(1007);
            //    if (opcResult1007.IsSuccess)
            //    {
            //        OperateResult<string> opcResult1111 = CreateOrderInfo(5, "", 52, 5, 1);
            //        if (opcResult1111.IsSuccess)
            //        {
            //            opResult.IsSuccess = true;
            //            opResult.Content = opcResult1111.Content;
            //            opResult.ErrorCode = 0;
            //            return opResult;
            //        }
            //    }
            //}
            //else
            //{
            //    //非NG
            //    //检查是否有库存整理任任务
            //    OperateResult<bool> isInventoryArrangeOutResult = IsCheckInventoryArrangeOut();
            //    if (!isInventoryArrangeOutResult.IsSuccess)
            //    {
            //        opResult.IsSuccess = false;
            //        opResult.Content = "";
            //        opResult.ErrorCode = 1;
            //        opResult.Message = isInventoryArrangeOutResult.Message;
            //        return opResult;
            //    }
            //    if (isInventoryArrangeOutResult.Content)
            //    {
            //        //读取wms库存整理任务
            //        OperateResult<int> opcTaskTypeResult = roller3008.Communicate.ReadInt(DataBlockNameEnum.OPCWeightDataBlock);
            //        if (!opcTaskTypeResult.IsSuccess || opcTaskTypeResult.Content <= 0)
            //        {
            //            opResult.IsSuccess = false;
            //            opResult.Content = opcTaskTypeResult.Content == 0 ? "WMS 未下发" : "下发的任务不正确:" + opcTaskTypeResult.Content;
            //            opResult.ErrorCode = 1;
            //            return opResult;
            //        }
            //        if (opcTaskTypeResult.Content == 1)
            //        {
            //            //库存整理  入库  14、24（中间需要条码扫描）
            //            OperateResult<string> opcResult1004 = IsCheckLoadedAndBarCode(1004);
            //            if (opcResult1004.IsSuccess)
            //            {
            //                OperateResult<string> opcResult1111 = CreateOrderInfo(3, opcResult1004.Content, 13, 1, 1);
            //                if (opcResult1111.IsSuccess)
            //                {
            //                    opResult.IsSuccess = true;
            //                    opResult.Content = opcResult1111.Content;
            //                    opResult.ErrorCode = 0;
            //                    return opResult;
            //                }
            //            }
            //            else
            //            {
            //                OperateResult<string> opcResult1007 = IsCheckLoadedAndBarCode(1007);
            //                if (opcResult1007.IsSuccess)
            //                {
            //                    OperateResult<string> opcResult1111 = CreateOrderInfo(3, opcResult1007.Content, 23, 2, 1);
            //                    if (opcResult1111.IsSuccess)
            //                    {
            //                        opResult.IsSuccess = true;
            //                        opResult.Content = opcResult1111.Content;
            //                        opResult.ErrorCode = 0;
            //                        return opResult;
            //                    }
            //                }
            //            }
            //        }
            //        else if (opcTaskTypeResult.Content == 2)
            //        {
            //            //库存整理  出库   41、42
            //            OperateResult<string> opcResult1004 = IsCheckLoadedAndBarCode(1004);
            //            if (opcResult1004.IsSuccess)
            //            {
            //                OperateResult<string> opcResult1111 = CreateOrderInfo(4, opcResult1004.Content, 41, 4, 1);
            //                if (opcResult1111.IsSuccess)
            //                {
            //                    opResult.IsSuccess = true;
            //                    opResult.Content = opcResult1111.Content;
            //                    opResult.ErrorCode = 0;
            //                    return opResult;
            //                }
            //            }
            //            else
            //            {
            //                OperateResult<string> opcResult1007 = IsCheckLoadedAndBarCode(1007);
            //                if (opcResult1007.IsSuccess)
            //                {
            //                    OperateResult<string> opcResult1111 = CreateOrderInfo(4, opcResult1007.Content, 42, 4, 1);
            //                    if (opcResult1111.IsSuccess)
            //                    {
            //                        opResult.IsSuccess = true;
            //                        opResult.Content = opcResult1111.Content;
            //                        opResult.ErrorCode = 0;
            //                        return opResult;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            opResult.IsSuccess = false;
            //            opResult.Content = "WMS 下发的任务类型不正确:" + opcTaskTypeResult.Content;
            //            opResult.ErrorCode = 1;
            //            return opResult;
            //        }
            //        ////是库存整理 等待wms下发指令
            //        //if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 5).IsSuccess)
            //        //{
            //        //    opResult.IsSuccess = false;
            //        //    opResult.Content = "";
            //        //    opResult.ErrorCode = 1;
            //        //    return opResult;
            //        //}
            //    }
            //    //LogMessage("311", EnumLogLevel.Info, false);
            //    OperateResult<string> opcResult1008 = IsCheckLoadedAndBarCode(1008);
            //    //LogMessage("312"+ opcResult1008.IsSuccess, EnumLogLevel.Info, false);
            //    if (opcResult1008.IsSuccess)
            //    {
            //        //检测1004或1007
            //        OperateResult<string> opcResult1004 = IsCheckLoadedAndBarCode(1004);
            //        //LogMessage("313" + opcResult1004.IsSuccess, EnumLogLevel.Info, false);
            //        if (opcResult1004.IsSuccess)
            //        {
            //            //生成13 搬运指令
            //            //LogMessage("314" + opcResult1004.IsSuccess, EnumLogLevel.Info, false);
            //            OperateResult<string> opcResult1111 = CreateOrderInfo(1, opcResult1004.Content, 13, 1, AbbCatchNum);
            //            //LogMessage("315" + opcResult1111.IsSuccess, EnumLogLevel.Info, false);
            //            if (opcResult1111.IsSuccess)
            //            {
            //                opResult.IsSuccess = true;
            //                opResult.Content = opcResult1111.Content;
            //                opResult.ErrorCode = 0;
            //                return opResult;
            //            }
            //        }

            //        OperateResult<string> opcResult1007 = IsCheckLoadedAndBarCode(1007);
            //        if (opcResult1007.IsSuccess)
            //        {
            //            //生成23 搬运指令
            //            OperateResult<string> opcResult1111 = CreateOrderInfo(1, opcResult1007.Content, 23, 2, AbbCatchNum);
            //            if (opcResult1111.IsSuccess)
            //            {
            //                opResult.IsSuccess = true;
            //                opResult.Content = opcResult1111.Content;
            //                opResult.ErrorCode = 0;
            //                return opResult;
            //            }
            //        }
            //    }
            //}
            #endregion
            return opResult;
        }


        /// <summary>
        /// 通过wcs的节点ID 转换成 ABB 对应的地址
        /// </summary>
        /// <param name="nodeId">wcs节点ID</param>
        /// <returns>ABB对应的协议地址</returns>
        private string GetABBNodeAddrByWcsNodeID(int nodeId)
        {
            string addr = "";
            if (nodeId == 1004)
            {
                addr = "1";
            }
            else if (nodeId == 1007)
            {
                addr = "2";
            }
            else if (nodeId == 1008)
            {
                addr = "4";
            }
            else if (nodeId == 2001)
            {
                addr = "1";
            }
            else if (nodeId == 2007)
            {
                addr = "4";
            }
            return addr;
        }

        /// <summary>
        /// 通知机器人载具更换（当前是新载具过来）
        /// </summary>
        /// <param name="nodeID">节点ID 1004、1007、1008、2001、2007</param>
        /// <returns>是否调用机器人载具更换成功</returns>
        private bool NotifyABBTrayChanged(string areaCode, int nodeID)
        {
            bool isNotifyOk = false;
            int abbNodeID = 0;
            if (areaCode.Equals("01"))
            {
                abbNodeID = 3001;
            }
            else if (areaCode.Equals("02"))
            {
                abbNodeID = 3002;
            }
            //调用机器人通知载具更新接口
            CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB abbDevice = DeviceManage.Instance.FindDeivceByDeviceId(abbNodeID) as CLDC.CLWS.CLWCS.WareHouse.Device.XYZABB;
            XYZABBControl abbControl = abbDevice.DeviceControl as XYZABBControl;

            OperateResult op = abbControl.NoticeRobotReplaceCarrier(new NoticeRobotReplaceCarrierMode
            {
                area_code = areaCode,
                job_addr = GetABBNodeAddrByWcsNodeID(nodeID)
            });
        
            LogMessage("通知载具更新 结果：" + (op.IsSuccess ? "成功" : "失败") + "具体返回信息：" + op.Message, op.IsSuccess ? EnumLogLevel.Info : EnumLogLevel.Error, false);
            if (op.IsSuccess)
            {
                isNotifyOk = true;
            }
            return isNotifyOk;
        }


        #region  系统默认方法 
        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transportMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ForceFinishOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult CancleOrder(DeviceName deviceName, ExOrder order)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult BeforeTransportBusinessHandle(TransportMessage transportMsg)
        {

            //1.判断搬运设备是否空闲
            //2.判断搬运设备的运行模式
            //1.询问目标设备是否空闲
            DeviceBaseAbstract startDevice = transportMsg.StartDevice;
            DeviceBaseAbstract destDevice = transportMsg.DestDevice;
            DeviceBaseAbstract transportDevice = transportMsg.TransportDevice;

            if (transportDevice == null)
            {
                return OperateResult.CreateFailedResult("搬运设备为空", 1);
            }
            OperateResult transportIsavailable = transportDevice.Availabe();
            if (!transportIsavailable.IsSuccess)
            {
                return
                    OperateResult.CreateFailedResult(
                        string.Format(" 搬运设备：{0} 当前状态不可用，原因：{1}", transportDevice.Name, transportIsavailable.Message), 1);
            }
            if (destDevice == null)
            {
                return OperateResult.CreateSuccessResult();
            }
            OperateResult destDeviceIsavailable = destDevice.Availabe();
            if (!destDeviceIsavailable.IsSuccess)
            {
                return
                    OperateResult.CreateFailedResult(
                        string.Format(" 目标设备：{0} 当前状态不可用，原因：{1}", destDevice.Name, destDeviceIsavailable.Message), 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult NotifyWmsDeviceException(string barcode, string cmdPara)
        {
            NotifyElement element = new NotifyElement(barcode, "NotifyInstructException", "上报搬运指令异常", null, cmdPara);
         
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口 NotifyInstructException 失败，详情：\r\n {0}", result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            try
            {
                SyncResReErr serviceReturn = (SyncResReErr)result.Content.ToString();
                if (serviceReturn.IsSuccess)
                {
                    return OperateResult.CreateSuccessResult("调用接口 NotifyInstructException 成功，并且WMS返回成功信息");
                }
                else
                {
                    return OperateResult.CreateSuccessResult(string.Format("调用接口 NotifyInstructException 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.ERR_MSG));
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


        /// <summary>
        /// ABB上报异常的业务处理
        /// </summary>
        /// <param name="exceptionMsg"></param>
        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            //1.调用NotifyInstructException通知WMS异常信息 需要的入参：指令编号 包装条码 异常代号 当前地址
            //01 - 放货异常，仓位有货
            //02 - 取货异常，仓位取空
            //03 - 取货异常，取深仓位，浅仓位有货
            OperateResult<NotifyInstructExceptionMode> getMode = null; //To Do
            if (!getMode.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("异常转换协议失败，失败原因：\r\n{0}", getMode.Message), 1);
            }
            string cmdPara = JsonConvert.SerializeObject(getMode.Content);
            return NotifyWmsDeviceException(getMode.Content.PACKAGE_BARCODE, cmdPara);
        }

        protected override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig;
                string path = "BusinessHandle";
                XmlElement xmlElement = doc.GetXmlElement("Coordination", "Id", WorkerId.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", WorkerId, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }
                return InitalizeConfig(xmlNode);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }
        private OperateResult InitalizeConfig(XmlNode xmlNode)
        {
            AgvInfoDic.Clear();
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                FourWayVehicleWorkerBusinessHandleProperty handleProperty = null;
                string businessPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(businessPropertyXml))
                {
                    try
                    {
                        handleProperty = (FourWayVehicleWorkerBusinessHandleProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(FourWayVehicleWorkerBusinessHandleProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess || handleProperty == null)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：IdentifyWorkerProperty", businessPropertyXml));
                }
                string content = handleProperty.Config.DeviceNoConvert.Trim();
                InitilizeAgvInfo(content);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }

            return result;
        }
        private void InitilizeAgvInfo(string deviceConvert)
        {
            string[] keyValue = deviceConvert.Split(';');
            foreach (string key_value in keyValue)
            {
                if (string.IsNullOrEmpty(key_value))
                {
                    continue;
                }
                string[] wcs_hangCha = key_value.Split('_');
                if (wcs_hangCha.Length >= 2)
                {
                    AgvInfo agvinfo = new AgvInfo
                    {
                        WcsId = int.Parse(wcs_hangCha[0]),
                        HangChaId = int.Parse(wcs_hangCha[1])
                    };
                    AgvInfoDic.Add(agvinfo);
                }
            }
        }
        private int XChaToWcs(int xChaId)
        {
            if (AgvInfoDic.Count <= 0)
            {
                return 9999;
            }
            if (AgvInfoDic.Exists(a => a.HangChaId.Equals(xChaId)))
            {
                return AgvInfoDic.Find(a => a.HangChaId.Equals(xChaId)).WcsId;
            }
            return 9999;
        }

        public Func<int, int, string, ResponseResult> NotifyExeResultEvent;
        #endregion

        private WcsOrderInfoForABB GetWcsCurOrderInfo()
        {
            #region 暂注释
            ////任务类型|托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量
            //OperateResult<string> opcBarCodeResult = roller1111.Communicate.ReadString(DataBlockNameEnum.OPCBarcodeDataBlock);
            //if (opcBarCodeResult.IsSuccess && !string.IsNullOrEmpty(opcBarCodeResult.Content))
            //{
            //    //虚拟托盘号|指令ID|起始目标位置|当前位置|发送抓取数量|实际抓取数量|机器人完成次数
            //    string[] barCodeArr = opcBarCodeResult.Content.Split('|');

            //    WcsOrderInfoForABB orderInfo = new WcsOrderInfoForABB
            //    {
            //        taskType = barCodeArr[0],
            //        simTrayNo = barCodeArr[1],
            //        order_no = barCodeArr[2],
            //        startAndEndPostion = barCodeArr[3],
            //        curPostion = barCodeArr[4],
            //        sendCatchNum = int.Parse(barCodeArr[5].Trim()),
            //        realityCatchNum = int.Parse(barCodeArr[6].Trim()),
            //    };
            //    return orderInfo;
            //}
            #endregion
            return null;
        }


        /// <summary>
        /// 设置机器人完成指令 存储OPC
        /// </summary>
        /// <param name="areaCode"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool SetABBOrderFinishedData(string areaCode, ReportExeOrderFinishCmd cmd)
        {
            if (areaCode.Equals("01"))
            {
                curInOrderFinishCmd = cmd;
                if (cmd == null)
                {
                    bool isWriteOk = IniHelper.WriteIniData("3003", "OPCBarcodeDataBlock21", "", wcsWareHouseInFullPath);
                    if (isWriteOk)
                    {
                        LogMessage("ABB完成指令 3003 OPCBarcodeDataBlock21 清空成功", EnumLogLevel.Info, false);
                        return true;
                    }
                    LogMessage("ABB完成指令  3003 OPCBarcodeDataBlock21 清空失败！", EnumLogLevel.Error, false);
                    return false;
                }
                bool isWriteOk2 = IniHelper.WriteIniData("3003", "OPCBarcodeDataBlock21", cmd.ToJson(), wcsWareHouseInFullPath);
                if (isWriteOk2)
                {
                    LogMessage("接收到ABB完成指令 并写入 1111 OPC成功" + cmd.ToJson(), EnumLogLevel.Info, false);
                    return true;
                }
                LogMessage("ABB完成指令 写入 1111 失败！", EnumLogLevel.Error, false);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从OPC中获取 机器人上报的完成指令
        /// </summary>
        /// <param name="areaCode">01入库 02出库</param>
        /// <returns></returns>
        private ReportExeOrderFinishCmd GetABBOrderFinishedData(string areaCode)
        {
            if (areaCode.Equals("01"))
            {
                string readValue = IniHelper.ReadIniData("3003", "OPCBarcodeDataBlock21", "", wcsWareHouseInFullPath);
                if (string.IsNullOrEmpty(readValue))
                {
                    return null;
                }
                return readValue.ToObject<ReportExeOrderFinishCmd>();

            }
            return null;
        }

        public OperateResult NotifyWmsOutFinish(string addr, string trayNo, int weight)
        {
            OperateResult opResult = OperateResult.CreateFailedResult("默认失败");

            NotifyOutFinishParaMode parmData = new NotifyOutFinishParaMode
            {
                Addr = addr,
                PalletBarcode = trayNo,
                WEIGHT = weight
            };
            NotifyOutFinishCmdMode mode = new NotifyOutFinishCmdMode
            {
                DATA = parmData
            };

            string cmdPara = JsonConvert.SerializeObject(mode);
            string interfaceName = "NotifyOutFinish";
            NotifyElement element = new NotifyElement("", interfaceName, "出库完成上报", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interfaceName, result.Message);
                opResult.IsSuccess = false;
            }
            else
            {
                string msg = string.Format("调用上层接口{0}成功，详情：\r\n {1}", interfaceName, result.Message);
                opResult.IsSuccess = true;
            }
            opResult.Message = result.Message;
            return opResult;
        }



        Dictionary<string, GoodsTrackDataUploadCmd> inDic = new Dictionary<string, GoodsTrackDataUploadCmd>();
        Dictionary<string, GoodsTrackDataUploadCmd> outDic = new Dictionary<string, GoodsTrackDataUploadCmd>();
        private void LoadBaseData()
        {
            //入库
            string In_A_Id = "1004";//取料位A
            string In_B_Id = "1007";//取料位B
            string In_R_Id = "10071";//扫描位R
            string In_S_Id = "1008";//放料位S
            string In_NG_Id = "10091";//NG台
            string In_O_Id = "10081";//机器人原点
            string In_DeviceId = "3001";//入库设备ID

            inDic.Add("0", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_O_Id,
                NEXTPOSITION = In_O_Id,
            });
            inDic.Add("1", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_O_Id,
                NEXTPOSITION = In_A_Id,
            });
            inDic.Add("2", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_A_Id,
                NEXTPOSITION = In_R_Id,
            });
            inDic.Add("3", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_R_Id,
                NEXTPOSITION = In_S_Id,
            });
            inDic.Add("4", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_R_Id,
                NEXTPOSITION = In_NG_Id,
            });
            inDic.Add("5", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_S_Id,
                NEXTPOSITION = In_A_Id,
            });
            inDic.Add("6", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_NG_Id,
                NEXTPOSITION = In_A_Id,
            });
            inDic.Add("7", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_S_Id,
                NEXTPOSITION = In_O_Id,
            });
            inDic.Add("8", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_NG_Id,
                NEXTPOSITION = In_O_Id,
            });
            inDic.Add("11", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_O_Id,
                NEXTPOSITION = In_B_Id,
            });
            inDic.Add("12", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_B_Id,
                NEXTPOSITION = In_R_Id,
            });
            inDic.Add("15", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_S_Id,
                NEXTPOSITION = In_B_Id,
            });
            inDic.Add("16", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = In_DeviceId,
                ISHAVEGOODS = false,
                POSITION = In_NG_Id,
                NEXTPOSITION = In_B_Id,
            });
            //出库
            string Out_O_Id = "20021";//机器人原点
            string Out_P_Id = "2001";//出库位
            string Out_R_Id = "20011";//扫描位R
            string Out_C_Id = "2007";//输送线C
            string Out_DeviceId = "3002";//出库设备ID
            outDic.Add("0", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_O_Id,
                NEXTPOSITION = Out_O_Id,
            });
            outDic.Add("1", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_O_Id,
                NEXTPOSITION = Out_P_Id,
            });
            outDic.Add("2", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_P_Id,
                NEXTPOSITION = Out_R_Id,
            });
            outDic.Add("3", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_R_Id,
                NEXTPOSITION = Out_C_Id,
            });
            outDic.Add("4", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_R_Id,
                NEXTPOSITION = Out_P_Id,
            });
            outDic.Add("5", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_P_Id,
                NEXTPOSITION = Out_O_Id,
            });
            outDic.Add("6", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_C_Id,
                NEXTPOSITION = Out_P_Id,
            });
            outDic.Add("7", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_P_Id,
                NEXTPOSITION = Out_C_Id,
            });
            outDic.Add("8", new GoodsTrackDataUploadCmd
            {
                DEVICECODE = Out_DeviceId,
                ISHAVEGOODS = false,
                POSITION = Out_C_Id,
                NEXTPOSITION = Out_O_Id,
            });
        }


        //private GoodsTrackDataUploadCmd GetRobotTrackDataUploadCmd(string areaCode, string status)
        //{
        //    return areaCode.Equals("01") ? inDic[status] : outDic[status];
        //}

        #region 处理机器人调用接口 
        /// <summary>
        /// 7.1	通知机器人作业状态 (OK)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string NoticeRobotStatus(NoticeRobotStatusCmd cmd)
        {
            ResponseResult2 resResponseResult = new ResponseResult2();
            try
            {
                string logMsg = string.Format("接收到ABB上报的 NoticeRobotStatus 接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);
                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {

                }
                else
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "传入的area_code错误:" + cmd.area_code;
                    return resResponseResult.ToString();
                }
                if (cmd.robot_status.Equals("0") || cmd.robot_status.Equals("1"))
                {

                }
                else
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "传入的 robot_status 错误:" + cmd.robot_status;
                    return resResponseResult.ToString();
                }
                resResponseResult.result = "1";
            }
            catch (Exception ex)
            {
                resResponseResult.result = "0";
                resResponseResult.message = ex.ToString();
            }
            return resResponseResult.ToString();
        }
        /// <summary>
        /// 7.2	通知指令执行异常
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ReportExeOrderException(ReportExeOrderExceptionCmd cmd)
        {
            ResponseResult2 resResponseResult = new ResponseResult2();
            try
            {
                string logMsg = string.Format("接收到ABB上报的 ReportExeOrderException 接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);

                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {

                }
                else
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "传入的area_code错误:" + cmd.area_code;
                    return resResponseResult.ToString();
                }
                //HandleReportExeOrderException(cmd);
                var tsk = Task.Run(() =>
                {
                    Thread.Sleep(20);
                    HandleReportExeOrderException(cmd);
                });

                resResponseResult.result = "1";
            }
            catch (Exception ex)
            {
                resResponseResult.result = "0";
                resResponseResult.message = ex.ToString();
            }
            return resResponseResult.ToString();
        }
        /// <summary>
        /// 处理机器人指令异常
        /// </summary>
        /// <param name="cmd"></param>
        private void HandleReportExeOrderException(ReportExeOrderExceptionCmd cmd)
        {
            if (cmd.area_code.Equals("01"))
            {
                //入库异常
                if (cmd.exception_code.Equals("E00001"))
                {
                    ////机械臂手动清除指令任务

                    //roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                    ////清空WCS任务
                    //roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                    //SetABBOrderFinishedData("01", null);
                    //roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0);
                    //roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, true);
                }
                else if (cmd.exception_code.Equals("E00002"))
                {
                    ////初始指令的起始点不能为扫码位，请清除当前指令

                    //roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                    ////清空WCS任务
                    //roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                    //SetABBOrderFinishedData("01", null);
                    //roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 0);
                    //roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, true);
                }
                else if (cmd.exception_code.Equals("E00003"))
                {
                    //上段结束点为扫码位，下段指令起始点不能为非扫码位，下段指令起始点为扫码位

                    ////入库完成
                    //WcsOrderInfoForABB wcsOrderInfo = GetWcsCurOrderInfo();

                    //int wcsTasktype = int.Parse(wcsOrderInfo.taskType);
                    //int wmsTaskType = GetWmsBusinessTypeByWcsTaskType(wcsOrderInfo.taskType);
                    //int realityCatchNum = wcsOrderInfo.realityCatchNum;
                    //int startAndEndPostion = int.Parse(wcsOrderInfo.startAndEndPostion);

                    //NotifyPackagePutFinishNewModel putFinishModel = new NotifyPackagePutFinishNewModel();
                    //List<PackagePutDetailModel> tempPackagePutDetailModelList = new List<PackagePutDetailModel>();
                    //List<SendExeOrder_ext_data> tempList = new List<SendExeOrder_ext_data>();

                    //string isForceTaskValue = ReadIsForceTask("01");

                    //List<string> scannerInfoList = GetScannerData(cmd.area_code, realityCatchNum);
                    ////XXXAO67510200/A2303-07-A33/495E20/2030327/50/20/495/SR1#1761/1009
                    //if (realityCatchNum == 1)
                    //{
                    //    //单抓
                    //    if (scannerInfoList.Contains("ERROR") || scannerInfoList.Count == 0)
                    //    {
                    //        //表示未扫到 则生成 35指令
                    //        OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 35, 3, realityCatchNum);
                    //        if (writeOpc1111.IsSuccess)
                    //        {
                    //            tempList.Add(new SendExeOrder_ext_data
                    //            {
                    //                index = cmd.customized_info[0].index.ToString(),
                    //                box_barcode = "",
                    //                start_position = cmd.customized_info[0].start_position.ToString()
                    //            });
                    //            OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                    //            if (!opUpdateOrderExeDataResult.IsSuccess) break;
                    //            if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //            {
                    //                if (cmd.start_isempty)
                    //                {
                    //                    //需要清理NG 
                    //                    OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                    //起始位置已空
                    //                    NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                    //                    //1122
                    //                    OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //                    if (isLoaded.IsSuccess && isLoaded.Content)
                    //                    {
                    //                        //叠盘接口调用成功
                    //                        OperateResult op = NotifyWmsPalletizerFinish("01");
                    //                        if (!op.IsSuccess) return; //通知失败
                    //                        NotifyTrayInAndOut(1008, true);
                    //                        bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                    //                }
                    //                SetABBOrderFinishedData("01", null);
                    //                return;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        tempList.Add(new SendExeOrder_ext_data
                    //        {
                    //            index = cmd.customized_info[0].index.ToString(),
                    //            box_barcode = scannerInfoList[0].Split('#')[0],
                    //            start_position = cmd.customized_info[0].start_position.ToString()
                    //        });
                    //        OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                    //        if (!opUpdateOrderExeDataResult.IsSuccess) break;

                    //        //正常 则调用wms 数据校验
                    //        NotifyPackageSkuBindBarcodeCmdModeNew wmsData = new NotifyPackageSkuBindBarcodeCmdModeNew();
                    //        wmsData.DATA.BusinessType = wmsTaskType;
                    //        wmsData.DATA.PalletBarcode = GetTrayNoByNodeID(1008);
                    //        wmsData.DATA.SrcAddr = GetWmsAddrByWcsAddr("01", int.Parse(startAndEndPostion.ToString().Substring(0, 1)));

                    //        List<PackageCheckDetailModelNew> pkgCheckData = new List<PackageCheckDetailModelNew>();
                    //        pkgCheckData.Add(new PackageCheckDetailModelNew
                    //        {
                    //            PackageBarcode = scannerInfoList[0].Split('#')[0],
                    //            SrcPosIndex = cmd.customized_info[0].start_position
                    //        });
                    //        wmsData.DATA.PackageCheckData = pkgCheckData;
                    //        //2、调用WMS接口进行上报校验(同步)
                    //        string cmdPara = JsonConvert.SerializeObject(wmsData);
                    //        NotifyElement element = new NotifyElement("", "NotifyPackageBarcodeCheckNew", "扫码数据上报", null, cmdPara);
                    //        OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                    //        if (!result.IsSuccess || result.Content == null)
                    //        {
                    //            string msg = string.Format("调用上层接口失败，详情：\r\n {0}", result.Message);
                    //            LogMessage(msg, EnumLogLevel.Error, true);
                    //            return;
                    //        }
                    //        //3、将WMS同步返回的数据，返回给机器人
                    //        //解析WMS返回的PackageId
                    //        NotifyPackageBarcodeCheckNewResponse packgeBarcodeResponse = result.Content.ToString().ToObject<NotifyPackageBarcodeCheckNewResponse>();
                    //        //NotifyPackageBarcodeCheckNewResponse packgeBarcodeResponse = JsonConvert.DeserializeObject<NotifyPackageBarcodeCheckNewResponse>(result.Content.ToString());
                    //        if (packgeBarcodeResponse.DATA.DestAddrType == 1)
                    //        {
                    //            //验证通过 去目标 码垛位4
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, GetTrayNoByNodeID(1008), 34, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    if (cmd.start_isempty)
                    //                    {
                    //                        //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口
                    //                        bool isCheckABBNG = IsCheckABBNG("01");
                    //                        if (isCheckABBNG)
                    //                        {
                    //                            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                        }
                    //                        //else
                    //                        //{
                    //                        //    //通知PLC放走
                    //                        //    int nodeID = startAndEndPostion==13 ? 1004 : 1007;
                    //                        //    bool isNotifyOK = NotifyPlcRunByOPC(nodeID);
                    //                        //}
                    //                        //起始位置已空
                    //                        NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                    //                    }
                    //                    else
                    //                    {
                    //                        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                    //                    }
                    //                    SetABBOrderFinishedData("01", null);
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //        else if (packgeBarcodeResponse.DATA.DestAddrType == 2 || packgeBarcodeResponse.DATA.DestAddrType == 4)
                    //        {
                    //            OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //            if (isLoaded.IsSuccess && isLoaded.Content)
                    //            {
                    //                //叠盘接口调用成功
                    //                OperateResult op = NotifyWmsPalletizerFinish("01");
                    //                if (!op.IsSuccess) return; //通知失败
                    //                NotifyTrayInAndOut(1008, true);
                    //                bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                    //            }

                    //            //回起始位 
                    //            int actType = startAndEndPostion == 13 ? 31 : 32;
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", actType, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    SetABBOrderFinishedData("01", null);
                    //                    //NotifyTrayInAndOut(1008, true);

                    //                    NotifyIsEmpty("01", wmsTaskType, false, actType.ToString());
                    //                    //roller1008.Communicate.Write(DataBlockNameEnum.WriteOrderIDDataBlock, -100);
                    //                    //roller1008.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 98);
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //        else if (packgeBarcodeResponse.DATA.DestAddrType == 3)
                    //        {
                    //            //验证失败去 NG
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 35, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    if (cmd.start_isempty)
                    //                    {
                    //                        //需要清理NG  
                    //                        OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                        //起始位置已空
                    //                        NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());

                    //                        OperateResult<bool> isLoaded = roller1008.Communicate.ReadBool(DataBlockNameEnum.IsLoaded);
                    //                        if (isLoaded.IsSuccess && isLoaded.Content)
                    //                        {
                    //                            //叠盘接口调用成功
                    //                            OperateResult op = NotifyWmsPalletizerFinish("01");
                    //                            if (!op.IsSuccess) return; //通知失败
                    //                            NotifyTrayInAndOut(1008, true);
                    //                            bool isWriteOk = IniHelper.WriteIniData("3003", "IsForceInTask", isForceTaskValue.ToString(), wcsWareHouseInFullPath);
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                    //                    }
                    //                    SetABBOrderFinishedData("01", null);
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //else if (realityCatchNum == 2)
                    //{
                    //    //两爪  13、23    退回31、32
                    //    if (scannerInfoList.Contains("ERROR") || scannerInfoList.Count == 0)
                    //    {
                    //        //表示未扫到 则生成 35指令
                    //        int actType = startAndEndPostion == 13 ? 31 : 32;

                    //        //退回31、32 并生成单爪
                    //        OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", actType, 3, realityCatchNum);
                    //        if (writeOpc1111.IsSuccess)
                    //        {
                    //            for (int i = 0; i < cmd.customized_info.Count; i++)
                    //            {
                    //                tempList.Add(new SendExeOrder_ext_data
                    //                {
                    //                    index = cmd.customized_info[i].index.ToString(),
                    //                    box_barcode = "",//有一个扫不到都给空
                    //                    start_position = cmd.customized_info[i].start_position.ToString()
                    //                });
                    //            }
                    //            OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                    //            if (!opUpdateOrderExeDataResult.IsSuccess) break;

                    //            ChangePickNumForABB(1);
                    //            if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //            {
                    //                SetABBOrderFinishedData("01", null);
                    //                return;
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (scannerInfoList.Count != realityCatchNum)
                    //        {
                    //            //扫描的数据 与实际机器人给的不符合，切换成单抓
                    //            //回起始位 
                    //            int actType = startAndEndPostion == 13 ? 31 : 32;
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", actType, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                for (int i = 0; i < cmd.customized_info.Count; i++)
                    //                {
                    //                    tempList.Add(new SendExeOrder_ext_data
                    //                    {
                    //                        index = cmd.customized_info[i].index.ToString(),
                    //                        box_barcode = "",//有一个扫不到都给空
                    //                        start_position = cmd.customized_info[i].start_position.ToString()
                    //                    });
                    //                }
                    //                OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                    //                if (!opUpdateOrderExeDataResult.IsSuccess) break;
                    //                ChangePickNumForABB(1);
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    SetABBOrderFinishedData("01", null);
                    //                    return;
                    //                }
                    //            }
                    //        }

                    //        //正常2爪 没有ERROR
                    //        //正常 则调用wms 数据校验
                    //        NotifyPackageSkuBindBarcodeCmdModeNew wmsData = new NotifyPackageSkuBindBarcodeCmdModeNew();
                    //        wmsData.DATA.BusinessType = wmsTaskType;
                    //        wmsData.DATA.PalletBarcode = GetTrayNoByNodeID(1008);//入库为虚拟托盘号 无意义
                    //        wmsData.DATA.SrcAddr = GetWmsAddrByWcsAddr("01", int.Parse(startAndEndPostion.ToString().Substring(0, 1)));                        //
                    //        List<PackageCheckDetailModelNew> pkgCheckData = new List<PackageCheckDetailModelNew>();

                    //        for (int i = 0; i < scannerInfoList.Count; i++)
                    //        {
                    //            string[] barCodeArr = scannerInfoList[i].Split('#');
                    //            string barCode = barCodeArr[0];
                    //            int width = int.Parse(barCodeArr[1].Split('/')[0]);
                    //            if (width < 1000)
                    //            {
                    //                //相机对应右，机器人对应左
                    //                int posIndex = cmd.customized_info.Find(x => x.index == 1).start_position;
                    //                pkgCheckData.Add(new PackageCheckDetailModelNew
                    //                {
                    //                    PackageBarcode = barCode,
                    //                    SrcPosIndex = posIndex
                    //                });
                    //                tempList.Add(new SendExeOrder_ext_data
                    //                {
                    //                    index = "1",
                    //                    box_barcode = barCode,
                    //                    start_position = posIndex.ToString()
                    //                });
                    //            }
                    //            else
                    //            {
                    //                //相机对应左，机器人对应右
                    //                int posIndex = cmd.customized_info.Find(x => x.index == 2).start_position;
                    //                pkgCheckData.Add(new PackageCheckDetailModelNew
                    //                {
                    //                    PackageBarcode = barCode,
                    //                    SrcPosIndex = posIndex
                    //                });

                    //                tempList.Add(new SendExeOrder_ext_data
                    //                {
                    //                    index = "2",
                    //                    box_barcode = barCode,
                    //                    start_position = posIndex.ToString()
                    //                });

                    //            }
                    //            OperateResult opUpdateOrderExeDataResult = UpdateOrderExtData("01", tempList);
                    //            if (!opUpdateOrderExeDataResult.IsSuccess) break;

                    //        }
                    //        wmsData.DATA.PackageCheckData = pkgCheckData;
                    //        //2、调用WMS接口进行上报校验(同步)
                    //        string cmdPara = JsonConvert.SerializeObject(wmsData);
                    //        NotifyElement element = new NotifyElement("", "NotifyPackageBarcodeCheckNew", "扫码数据上报", null, cmdPara);
                    //        OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                    //        if (!result.IsSuccess || result.Content == null)
                    //        {
                    //            string msg = string.Format("调用上层接口失败，详情：\r\n {0}", result.Message);
                    //            LogMessage(msg, EnumLogLevel.Error, true);
                    //            return;
                    //        }
                    //        //3、将WMS同步返回的数据，返回给机器人
                    //        //解析WMS返回的PackageId
                    //        NotifyPackageBarcodeCheckNewResponse packgeBarcodeResponse = JsonConvert.DeserializeObject<NotifyPackageBarcodeCheckNewResponse>(result.Content.ToString());
                    //        if (packgeBarcodeResponse.DATA.DestAddrType == 1)
                    //        {
                    //            //验证通过 去目标 码垛位4
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 34, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    if (cmd.start_isempty)
                    //                    {
                    //                        //需要清理NG  判断下是否有NG，有NG则清理，无NG则不需要清理，需要通知托盘去入库口
                    //                        bool isCheckABBNG = IsCheckABBNG("01");
                    //                        if (isCheckABBNG)
                    //                        {
                    //                            OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                        }
                    //                        else
                    //                        {
                    //                            //通知PLC放走
                    //                            //int nodeID = startAndEndPostion == 13 ? 1004 : 1007;
                    //                            //bool isNotifyOK = NotifyPlcRunByOPC(nodeID);

                    //                            //起始位置已空 不通知 ？？？？
                    //                            NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                    //                    }
                    //                    SetABBOrderFinishedData("01", null);
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //        else if (packgeBarcodeResponse.DATA.DestAddrType == 2 || packgeBarcodeResponse.DATA.DestAddrType == 4)
                    //        {
                    //            //回原点
                    //            int actType = startAndEndPostion == 13 ? 31 : 32;
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", actType, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    if (packgeBarcodeResponse.DATA.DestAddrType == 2)
                    //                    {
                    //                        ChangePickNumForABB(1);
                    //                    }
                    //                    SetABBOrderFinishedData("01", null);
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //        else if (packgeBarcodeResponse.DATA.DestAddrType == 3)
                    //        {
                    //            //验证失败去 NG
                    //            OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", 35, 3, realityCatchNum);
                    //            if (writeOpc1111.IsSuccess)
                    //            {
                    //                if (roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3).IsSuccess)
                    //                {
                    //                    if (cmd.start_isempty)
                    //                    {
                    //                        //需要清理NG  
                    //                        OperateResult writeOrderTypeResult = roller1119.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 99);
                    //                        //起始位置已空
                    //                        NotifyIsEmpty("01", wmsTaskType, true, startAndEndPostion.ToString());
                    //                    }
                    //                    else
                    //                    {
                    //                        NotifyIsEmpty("01", wmsTaskType, false, startAndEndPostion.ToString());
                    //                    }

                    //                    SetABBOrderFinishedData("01", null);
                    //                    return;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                else if (cmd.exception_code.Equals("E00004"))
                {
                    ////扫码位到结束点无法放置，可能是放置位无法放置两个，请从扫码位放回到抓取位，再开启单抓抓取
                    //roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, false);
                    //WcsOrderInfoForABB wcsOrderInfo = GetWcsCurOrderInfo();

                    //int wcsTasktype = int.Parse(wcsOrderInfo.taskType);
                    ////清空WCS任务
                    //roller1111.Communicate.Write(DataBlockNameEnum.OPCBarcodeDataBlock, "");
                    //SetABBOrderFinishedData("01", null);
                    //int newStartAndEndPostion = 0;
                
                    //OperateResult<int> result1004 = roller1004.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                    //if (result1004.IsSuccess && result1004.Content == 99)
                    //{
                    //    newStartAndEndPostion = 31;
                    //}
                    //else
                    //{
                    //    OperateResult<int> result1007 = roller1007.Communicate.ReadInt(DataBlockNameEnum.OrderTypeDataBlock);
                    //    if (result1007.IsSuccess && result1007.Content == 99)
                    //    {
                    //        newStartAndEndPostion = 32;
                    //    }
                    //}
                    //OperateResult<string> writeOpc1111 = WriteOrderInfoForNode1111(wcsTasktype, "", newStartAndEndPostion, 3, wcsOrderInfo.realityCatchNum);
                    //if (writeOpc1111.IsSuccess)
                    //{
                    //    roller1111.Communicate.Write(DataBlockNameEnum.OrderTypeDataBlock, 3);
                    //    roller1111.Communicate.Write(DataBlockNameEnum.IsLoaded, true);
                    //}
                }
                else if (cmd.exception_code.Equals("E00005"))
                {
                    //当前指令起始点为扫码位，wcs回传扫码数据错误，数量与实际抓取数量不一致，请确认数据后从新下发任务
                }
                else if (cmd.exception_code.Equals("E00006"))
                {
                    //检测到起始点为空托盘，无法抓取，请确认后清除当前指令
                }
                else if (cmd.exception_code.Equals("E00007"))
                {
                    //检测到结束点已满，无法放置，请确认后清除当前指令
                }

            }
        }



        private object recWareHouseInObj = new object();
        private object recWareHouseOutObj = new object();

        /// <summary>
        /// 7.3	通知指令执行完成  (OK)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ReportExeOrderFinish(ReportExeOrderFinishCmd cmd)
        {
            ResponseResult2 resResponseResult = new ResponseResult2();
            try
            {

                string logMsg = string.Format("接收到ABB上报的 ReportExeOrderFinish 接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, false);

                if (string.IsNullOrEmpty(cmd.area_code))
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "area_code 不得为空！";
                    return resResponseResult.ToString();
                }
                if (string.IsNullOrEmpty(cmd.order_no))
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "order_no 不得为空！";
                    return resResponseResult.ToString();
                }
                if (cmd.customized_info == null || cmd.customized_info.Count <= 0)
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "customized_info 上报完成的数量不得为空！";
                    return resResponseResult.ToString();
                }


                if (cmd.area_code.Equals("01"))
                {
                    lock (recWareHouseInObj)
                    {
                        string readValue = IniHelper.ReadIniData("3003", "RecOrderNo", "", wcsWareHouseInFullPath);
                        if (string.IsNullOrEmpty(readValue) || string.IsNullOrEmpty(readValue.Trim()))
                        {
                            bool isWriteOk = IniHelper.WriteIniData("3003", "RecOrderNo", cmd.order_no, wcsWareHouseInFullPath);
                        }
                        else
                        {
                            if (readValue.Trim().Equals(cmd.order_no))
                            {
                                //收到重复order_No上报
                                resResponseResult.result = "0";
                                resResponseResult.message = "ABB重复上报完成指令，指令号:" + cmd.order_no;
                                return resResponseResult.ToString();
                            }
                            else
                            {
                                bool isWriteOk = IniHelper.WriteIniData("3003", "RecOrderNo", cmd.order_no, wcsWareHouseInFullPath);
                            }
                        }

                        //入库完成
                        WcsOrderInfoForABB wcsOrderInfo = GetWcsCurOrderInfo();
                        string msg = "";
                        if (wcsOrderInfo == null)
                        {
                            msg = "wcs GetWcsCurOrderInfo 获取3003 节点数据 错误";
                            LogMessage(msg, EnumLogLevel.Error, false);
                            resResponseResult.result = "0";
                            resResponseResult.message = msg;
                            return resResponseResult.ToString();
                        }

                        if (!wcsOrderInfo.order_no.Equals(cmd.order_no))
                        {
                            //机器人上报的完成指令 错误
                            msg = string.Format("机器人上报的完成指令号错误，与wcs下发的不一致,WCS当前指令:{0} ABB上报指令:{1}", wcsOrderInfo.order_no, cmd.order_no);
                            LogMessage(msg, EnumLogLevel.Error, false);
                            resResponseResult.result = "0";
                            resResponseResult.message = msg;
                            return resResponseResult.ToString();
                        }
                        SetABBOrderFinishedData("01", cmd);
                        //_orderAmountSem.Release();
                        //curInOrderFinishCmd = cmd;
                        resResponseResult.result = "1";
                        resResponseResult.message = "成功";
                        return resResponseResult.ToString();
                    }
                }
                else
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "area_code 不正确！:" + cmd.area_code;
                    return resResponseResult.ToString();
                }
            }
            catch (Exception ex)
            {
                resResponseResult.result = "0";
                resResponseResult.message = ex.ToString();
            }

            return resResponseResult.ToString();
        }
        /// <summary>
        /// 7.4	上报机器人作业信息 (3D 仿真)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ReportRobotWorkInfo(ReportRobotWorkInfoCmd cmd)
        {
            ResponseResult2 resResponseResult = new ResponseResult2();
            try
            {
                string logMsg = string.Format("接收到ABB上报的 ReportRobotWorkInfo 接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, false);

                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {

                }
                else
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "area_code 错误，实际值：" + cmd.area_code;
                    return resResponseResult.ToString();
                }
                var task2 = Task.Run(() =>
                {
                    ////调用仿真接口
                    ////转发给模拟3D仿真
                    //DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
                    //if (xDevice == null)
                    //{
                    //    string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                    //    LogMessage(msg, EnumLogLevel.Error, true);
                    //}
                    //Simulate3D transDevice = xDevice as Simulate3D;
                    //Simulate3DControl roller = transDevice.DeviceControl as Simulate3DControl;
                    //GoodsTrackDataUploadCmd actionCmd = GetRobotTrackDataUploadCmd(cmd.area_code,
                    //    cmd.return_data.work_status);
                    GoodsTrackDataUploadCmd actionCmd = new GoodsTrackDataUploadCmd
                    {
                        DEVICECODE = cmd.area_code.Equals("01") ? "3001" : "3002",
                        ISHAVEGOODS = true,
                        POSITION = "",
                        NEXTPOSITION = "",
                        TASKCODE = "",
                        PACKAGEBARCODE = ""
                    };
                    if (cmd.work_flag.Equals("wait"))
                    {
                        actionCmd.POSITION = "";
                        actionCmd.NEXTPOSITION = "";
                        actionCmd.ISHAVEGOODS = false;
                    }
                    else
                    {
                        actionCmd.POSITION = Get3DAddrByABBAddr(cmd.area_code, cmd.start_addr);
                        actionCmd.NEXTPOSITION = Get3DAddrByABBAddr(cmd.area_code, cmd.dest_addr); ;
                        actionCmd.ISHAVEGOODS = true;
                    }

                    OperateResult opResult = roller3D.RobotTrackDataUpload(actionCmd);
                    if (opResult.IsSuccess)
                    {
                        LogMessage("调用3D仿真接口成功： RobotTrackDataUpload", EnumLogLevel.Info, false);
                    }
                    else
                    {
                        LogMessage("调用3D仿真接口失败： RobotTrackDataUpload" + opResult.Message, EnumLogLevel.Error, false);
                    }
                });
                //resResponseResult.error = 0;
            }
            catch (Exception ex)
            {
                //resResponseResult.error = 2;
                //resResponseResult.error_message = ex.ToString();
            }
            resResponseResult.result = "1";
            resResponseResult.message = "";
            return resResponseResult.ToString();
        }

        private string Get3DAddrByABBAddr(string area_code, string abbAddr)
        {
            if (area_code.Equals("01"))
            {
                //入库
                string In_A_Id = "1004";//取料位A 1
                string In_B_Id = "1007";//取料位B 2
                string In_R_Id = "10071";//扫描位R 3
                string In_S_Id = "1008";//放料位S 4
                string In_NG_Id = "10091";//NG台 5
                string In_O_Id = "10081";//机器人原点 6
                //string In_DeviceId = "3001";//入库设备ID

                switch (abbAddr)
                {
                    case "1":
                        return In_A_Id;
                    case "2":
                        return In_B_Id;
                    case "3":
                        return In_R_Id;
                    case "4":
                        return In_S_Id;
                    case "5":
                        return In_NG_Id;
                    case "6":
                        return In_O_Id;
                }


            }
            else if (area_code.Equals("02"))
            {
                string Out_O_Id = "20021";//机器人原点 6
                string Out_P_Id = "2001";//出库位  1
                string Out_R_Id = "20011";//扫描位R 3
                string Out_C_Id = "2007";//输送线C 4
                //string Out_DeviceId = "3002";//出库设备ID
                switch (abbAddr)
                {
                    case "1":
                        return Out_P_Id;
                    case "3":
                        return Out_R_Id;
                    case "4":
                        return Out_C_Id;
                    case "6":
                        return Out_O_Id;
                }
            }
            return "";
        }


        /// <summary>
        /// 7.5	上报机器人故障 (OK)
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string ReportFaultInfo(ReportFaultInfoCmd cmd)
        {
            ResponseResult2 resResponseResult = new ResponseResult2();
            try
            {
                string logMsg = string.Format("接收到ABB上报的 ReportFaultInfo 接口信息:{0}", cmd.ToJson());
                LogMessage(logMsg, EnumLogLevel.Info, true);

                if (cmd.area_code.Equals("01") || cmd.area_code.Equals("02"))
                {

                }
                else
                {
                    resResponseResult.result = "0";
                    resResponseResult.message = "传入的area_code错误:" + cmd.area_code;
                    return resResponseResult.ToString();
                }
                var tsk = Task.Run(() =>
                {
                    Thread.Sleep(20);
                    HandleReportFaultInfoCmdForSimulation3D(cmd);
                });

                resResponseResult.result = "1";
            }
            catch (Exception ex)
            {
                resResponseResult.result = "0";
                resResponseResult.message = ex.ToString();
            }
            return resResponseResult.ToString();
        }

        private void HandleReportFaultInfoCmdForSimulation3D(ReportFaultInfoCmd cmd)
        {
            try
            {

                //调用wms
                //cmd.fault_code
                //string zh_msg = IniHelper.ReadIniData(cmd.fault_code, "zh_msg", "", abbFullPath);
                //string tip = IniHelper.ReadIniData(cmd.fault_code, "tip", "", abbFullPath);
                NotifyWmsReportTroubleStatus(new NotifyReportTroubleStatusCmdMode
                {
                    WarnType = 2,
                    MESSAGE = cmd.fault_desc,
                    HandlingSuggest = cmd.fault_info
                });

                //调用仿真接口
                //转发给模拟3D仿真
                //DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
                //if (xDevice == null)
                //{
                //    string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                //    LogMessage(msg, EnumLogLevel.Error, true);
                //}
                //Simulate3D transDevice = xDevice as Simulate3D;
                //Simulate3DControl roller = transDevice.DeviceControl as Simulate3DControl;
                DeviceStautsDataUploadCmd sendData = new DeviceStautsDataUploadCmd
                {
                    DEVICECODE = cmd.area_code.Equals("01") ? "3001" : "3002",
                    ONLINE_STATUS = 1,
                    JOB_STATUS = 2,
                    EXCEPTION_STATUS = 2,
                    EXCEPTION_CODE = cmd.fault_code,
                    EXCEPTION_MSG = cmd.fault_desc,
                    EXT1 = "",
                    EXT2 = ""
                };
                OperateResult opResult = roller3D.DeviceStautsDataUpload(sendData);
                if (opResult.IsSuccess)
                {
                    LogMessage("调用3D仿真接口成功： DeviceStautsDataUpload", EnumLogLevel.Info, true);
                }
                else
                {
                    LogMessage("调用3D仿真接口失败： DeviceStautsDataUpload" + opResult.Message, EnumLogLevel.Info, true);
                }

            }
            catch (Exception ex)
            {
                LogMessage("调用3D仿真接口失败： DeviceStautsDataUpload" + ex.Message, EnumLogLevel.Info, true);
            }
        }

        /// <summary>
        /// 设备异常信息上报
        /// </summary>
        /// <param name="deviceErrMsg"></param>
        /// <returns></returns>
        private OperateResult NotifyWmsReportTroubleStatus(NotifyReportTroubleStatusCmdMode errMode)
        {
            string cmdPara = JsonConvert.SerializeObject(errMode);
            string interFaceName = "ReportTroubleStatus";
            NotifyElement element = new NotifyElement("", interFaceName, "上报设备异常信息WMS", null, cmdPara);
         
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            else
            {
                try
                {
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult("调用接口 " + interFaceName + "成功，并且WMS返回成功信息");
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口  " + interFaceName + " 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            return result;
        }

        #endregion
    }



}
