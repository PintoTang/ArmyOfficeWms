using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.OrderWorkers.InAndOutCell.Model;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.WareHouse.Device.HeFei;
using Infrastructrue.Ioc.DependencyFactory;
using Newtonsoft.Json;

namespace CLWCS.WareHouse.Worker.HeFei
{
    public class StackingcraneWorkerBusiness : InAndOutCellBusinessAbstract
    {
        private IWmsWcsArchitecture _wmsWcsDataArchitecture;

        protected override OperateResult ParticularInitlize()
        {
            InitilizeFaultCodeDictionary();
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult AfterFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }


        public override OperateResult BeforeFinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult FinishTransportMsgBusiness(DeviceName deviceName, TransportMessage transport)
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

        public override OperateResult BeforeTransportBusinessHandle(TransportMessage transMsg)
        {
            {
                //1.判断搬运设备是否空闲
                //2.判断搬运设备的运行模式
                //1.询问目标设备是否空闲
                DeviceBaseAbstract startDevice = transMsg.StartDevice;
                DeviceBaseAbstract destDevice = transMsg.DestDevice;
                DeviceBaseAbstract transportDevice = transMsg.TransportDevice;

                if (transportDevice == null)
                {
                    return OperateResult.CreateFailedResult("搬运设备为空", 1);
                }
                OperateResult transportIsavailable = transportDevice.Availabe();
                if (!transportIsavailable.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format(" 搬运设备：{0} 当前状态不可用，原因：{1}", transportDevice.Name, transportIsavailable.Message), 1);
                }
                //if (!CurWorkState.Equals(WorkStateMode.Free))
                //{
                //    string msg = string.Format("当前设备工作状态为：{0} 不可切换调度状态：{1}", CurWorkState, destState);
                //    return OperateResult.CreateFailedResult(msg, 1);
                //}

                //TransportPointStation transDevice = transportDevice as TransportPointStation;
                //RollerDeviceControl roller = transDevice.DeviceControl as RollerDeviceControl;
                //OperateResult<int> readOpcResult = roller.Communicate.ReadInt(DataBlockNameEnum.DeviceWorkStatus);
                //if (!readOpcResult.IsSuccess)
                //{
                //    return OperateResult.CreateFailedResult(string.Format(" 搬运设备：{0} 当前设备状态不可用，原因：{1}", transportDevice.Name, transportIsavailable.Message), 1);
                //}
                //if (readOpcResult.Content != 0)
                //{
                //    return OperateResult.CreateFailedResult(string.Format(" 搬运设备：{0} 当前设备状态：{1}不是空闲，原因：{2}", transportDevice.Name, transportIsavailable.Message, readOpcResult.Content), 1);
                //}


                if (destDevice == null)
                {
                    return OperateResult.CreateSuccessResult();
                }
                OperateResult destDeviceIsavailable = destDevice.Availabe();
                if (!destDeviceIsavailable.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format(" 目标设备：{0} 当前状态不可用，原因：{1}", destDevice.Name, destDeviceIsavailable.Message), 1);
                }
                return OperateResult.CreateSuccessResult();
            }
        }

        private readonly Dictionary<int, int> _faultCodeDictionary = new Dictionary<int, int>();

        private readonly Dictionary<int, string> _newFaultCodeDictionary = new Dictionary<int, string>();

        private void InitilizeFaultCodeDictionary()
        {
            _faultCodeDictionary.Add((int)TaskExcuteStatusType.OutButEmpty, 2);
            _faultCodeDictionary.Add((int)TaskExcuteStatusType.InButExist, 1);

            _newFaultCodeDictionary.Clear();
            _newFaultCodeDictionary.Add(1, "接收任务不完整_检查通讯数据");
            _newFaultCodeDictionary.Add(2, "任务类型错_检查通讯数据");
            _newFaultCodeDictionary.Add(3, "入口信息错误_检查通讯数据");
            _newFaultCodeDictionary.Add(4, "出口信息错误_检查通讯数据");
            _newFaultCodeDictionary.Add(5, "库位信息错误_检查通讯数据");
            _newFaultCodeDictionary.Add(6, "空闲时货叉不在原点_检查货叉原点");
            _newFaultCodeDictionary.Add(7, "行走时货叉不在原点_检查货叉原点");
            _newFaultCodeDictionary.Add(8, "货物左坍塌_检查货物外形");
            _newFaultCodeDictionary.Add(9, "货物右坍塌_检查货物外形");
            _newFaultCodeDictionary.Add(10, "取货时货叉有货_检查有货光电，核对是否有货");

            _newFaultCodeDictionary.Add(11, "取货后货叉无货_检查有货光电，核对是否无货");
            _newFaultCodeDictionary.Add(12, "货物超宽_检查货物外形");
            _newFaultCodeDictionary.Add(13, "货物超高_检查货物外形");
            _newFaultCodeDictionary.Add(14, "放货重复_检查弹货光电");
            _newFaultCodeDictionary.Add(15, "放货时货叉无货_检查有货光电，核对是否无货");
            _newFaultCodeDictionary.Add(16, "放货后货叉有货_检查有货光电，核对是否有货");
            _newFaultCodeDictionary.Add(17, "安全门打开_检查安全门是否关闭");
            _newFaultCodeDictionary.Add(18, "堆垛机急停_检查堆垛机是否急停");
            _newFaultCodeDictionary.Add(19, "维修模式_检查堆垛机是否维修");
            _newFaultCodeDictionary.Add(20, "行走轴运行超时报警_检查运行电机变频器和轨道");


            _newFaultCodeDictionary.Add(21, "行走轴松绳报警_行走轴松绳报警");
            _newFaultCodeDictionary.Add(22, "行走轴运行时货叉不在中位_检查货叉原点");
            _newFaultCodeDictionary.Add(23, "行走轴正软限位报警_检查堆垛机行走轴的位置");
            _newFaultCodeDictionary.Add(24, "行走轴负软限位报警_检查堆垛机行走轴的位置");
            _newFaultCodeDictionary.Add(25, "行走轴正限位报警_检查堆垛机行走轴的位置");
            _newFaultCodeDictionary.Add(26, "行走轴负限位报警_检查堆垛机行走轴的位置");
            _newFaultCodeDictionary.Add(27, "行走轴变频器故障_检查堆垛机行走变频器");
            _newFaultCodeDictionary.Add(28, "行走轴目标值超限位_检查目标值");
            _newFaultCodeDictionary.Add(29, "行走轴速度反馈异常_检查行走速度");
            _newFaultCodeDictionary.Add(30, "升降轴运行超时报警_检查升降电机变频器和轨道");

            _newFaultCodeDictionary.Add(31, "升降轴松绳报警_升降轴松绳报警");
            _newFaultCodeDictionary.Add(32, "升降轴干涉报警_检查货叉原点");
            _newFaultCodeDictionary.Add(33, "升降轴正软限位报警_检查堆垛机升降轴的位置");
            _newFaultCodeDictionary.Add(34, "升降轴负软限位报警_检查堆垛机升降轴的位置");
            _newFaultCodeDictionary.Add(35, "升降轴正限位报警_检查堆垛机升降轴的位置");
            _newFaultCodeDictionary.Add(36, "升降轴负限位报警_检查堆垛机升降轴的位置");
            _newFaultCodeDictionary.Add(37, "升降轴变频器故障_检查堆垛机升降变频器");
            _newFaultCodeDictionary.Add(38, "升降轴目标值超限位_检查目标值");
            _newFaultCodeDictionary.Add(39, "升降轴速度反馈异常_检查行走速度");
            _newFaultCodeDictionary.Add(40, "货叉运行超时报警_检查货叉电机变频器和轨道");

            _newFaultCodeDictionary.Add(41, "货叉松绳报警_货叉轴松绳报警");
            _newFaultCodeDictionary.Add(42, "货叉干涉报警_检查货叉原点");
            _newFaultCodeDictionary.Add(43, "货叉正软限位报警_检查堆垛机货叉轴的位置");
            _newFaultCodeDictionary.Add(44, "货叉负软限位报警_检查堆垛机货叉轴的位置");
            _newFaultCodeDictionary.Add(45, "货叉正限位报警_检查堆垛机货叉轴的位置");
            _newFaultCodeDictionary.Add(46, "货叉负限位报警_检查堆垛机货叉轴的位置");
            _newFaultCodeDictionary.Add(47, "货叉变频器故障_检查堆垛机货叉变频器");
            _newFaultCodeDictionary.Add(48, "货叉目标值超限位_检查目标值");
            _newFaultCodeDictionary.Add(49, "货叉速度反馈异常_检查货叉速度");
            _newFaultCodeDictionary.Add(50, "货叉动作时行走升降轴不在目标范围内_检查升降是否移动");

            _newFaultCodeDictionary.Add(51, "升降动作时行走和货叉不在目标范围内_检查货叉是否移动");
            _newFaultCodeDictionary.Add(52, "升降距离超出范围_检查升降距离");
            _newFaultCodeDictionary.Add(53, "取深仓位时浅仓位有货_检查浅仓位是否有货，或者探货光电");
            _newFaultCodeDictionary.Add(54, "超高异常_检查货物外形高度");
            _newFaultCodeDictionary.Add(55, "前超长_检查货物外形前超出");
            _newFaultCodeDictionary.Add(56, "后超出_检查货物外形后超出");
            _newFaultCodeDictionary.Add(57, "货叉放货时行走升降轴位置偏移过大_检查行走升降位置");
            _newFaultCodeDictionary.Add(58, "输送机长时间未给允许取货信号_检查线体是否自动、站台是否有货和光电");
            _newFaultCodeDictionary.Add(59, "输送机长时间未反馈取货完成信号_检查线体是否自动、站台是否有货和光电");
            _newFaultCodeDictionary.Add(60, "输送机长时间未给允许放货信号_检查线体是否自动、站台是否有货和光电");

            _newFaultCodeDictionary.Add(61, "输送机长时间未反馈放货完成信号_检查线体是否自动、站台是否有货和光电");
            _newFaultCodeDictionary.Add(62, "货叉取货时行走升降轴位置偏移过大_检查货叉位置");
            _newFaultCodeDictionary.Add(63, "和输送线通讯中断_检查和输送线通讯");
        }

        public string GetFaultErrInfoByCode(int faultCode)
        {
            if (_newFaultCodeDictionary.ContainsKey(faultCode))
            {
                return _newFaultCodeDictionary[faultCode];
            }
            return "PLC下发的异常不在wcs列表中，默认异常消息";
        }

        private int DeviceFaultCodeChangeToWmsFaultCode(TaskExcuteStatusType exceptionType)
        {
            int exceptionCode = (int)exceptionType;
            if (_faultCodeDictionary.ContainsKey(exceptionCode))
            {
                return _faultCodeDictionary[exceptionCode];
            }
            else
            {
                return exceptionCode;
            }
        }

        private OperateResult<NotifyInstructExceptionMode> GetNotifyInstructExceptionMsg(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            NotifyInstructExceptionMode exceptionMsgMode = new NotifyInstructExceptionMode();
            OperateResult<NotifyInstructExceptionMode> result = OperateResult.CreateFailedResult(exceptionMsgMode, "无数据");
            if (exceptionMsg.ExcuteTask == null)
            {
                return result;
            }
            try
            {
                exceptionMsgMode.INSTRUCTION_CODE = exceptionMsg.ExcuteTask.TransportOrder.DocumentCode.ToString();
                exceptionMsgMode.PACKAGE_BARCODE = exceptionMsg.ExcuteTask.TransportOrder.PileNo;
                exceptionMsgMode.EXCEPTION_CODE = DeviceFaultCodeChangeToWmsFaultCode(exceptionMsg.ExcuteTaskStatus).ToString();
                OperateResult<string> curAddress = new OperateResult<string>();
                curAddress.IsSuccess = false;
                switch (exceptionMsg.ExcuteTaskStatus)
                {
                    case TaskExcuteStatusType.OutButEmpty:
                        curAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(exceptionMsg.ExcuteTask.StartAddr.FullName);
                        break;
                    case TaskExcuteStatusType.OutDepthButShallowExist:
                        curAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(exceptionMsg.ExcuteTask.StartAddr.FullName);
                        break;
                    case TaskExcuteStatusType.InDepthButShallowExist:
                        curAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(exceptionMsg.ExcuteTask.DestAddr.FullName);
                        break;
                    case TaskExcuteStatusType.InButExist:
                        curAddress = _wmsWcsDataArchitecture.WcsToWmsAddr(exceptionMsg.ExcuteTask.DestAddr.FullName);
                        break;
                    default:
                        break;
                }
                if (!curAddress.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("Wcs转换成Wms的基础数据失败，失败原因：\r\n{0}", curAddress.Message);
                    return result;
                }
                if (string.IsNullOrEmpty(exceptionMsgMode.INSTRUCTION_CODE) || string.IsNullOrEmpty(exceptionMsgMode.PACKAGE_BARCODE) || string.IsNullOrEmpty(exceptionMsgMode.EXCEPTION_CODE))
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("设备：{0} 上报异常：{1} 转换异常信息失败，异常指令：{2}", exceptionMsg.ExcuteDevice.Name, exceptionMsg.ExcuteTaskStatus, exceptionMsg.ExcuteTask);
                    return result;
                }

                exceptionMsgMode.CURRENT_ADDR = curAddress.Content;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;

            }
            return result;
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
        /// SNDL的堆垛机上报异常的业务处理
        /// </summary>
        /// <param name="exceptionMsg"></param>
        public override OperateResult DeviceExceptionHandle(TaskExcuteMessage<TransportMessage> exceptionMsg)
        {
            //上报异常
            try
            {
                int deviceID = exceptionMsg.ExcuteDevice.Id;
                //int deviceID = this.WorkerId == 3001 ? 9001 : 9002;
                DeviceBaseAbstract curDevice = DeviceManage.Instance.FindDeivceByDeviceId(deviceID);
                ClouRobotTecStackingcrane transDevice = curDevice as ClouRobotTecStackingcrane;
                ClouRobotTecStackingcraneControl roller = transDevice.DeviceControl as ClouRobotTecStackingcraneControl;
                OperateResult<int> readOpcResult = roller.Communicate.ReadInt(DataBlockNameEnum.DeviceFaultCode);
                if(readOpcResult.IsSuccess)
                {
                    if (readOpcResult.Content == 0)
                    {
                        Task.Factory.StartNew(new Action<object>(obj =>
                        {
                            ReportDeviceTroubleStatusTo3D(obj as NotifyReportTroubleStatusCmdMode);
                        }), new NotifyReportTroubleStatusCmdMode
                        {
                            DeviceId = curDevice.Id,
                            DeviceName = curDevice.Name,
                            FaultCode = "0",
                        });
                    }
                    else
                    {
                        string strErrMsg = GetFaultErrInfoByCode(readOpcResult.Content);
                        string[] arr = strErrMsg.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        string strShowMessage = "[" + curDevice.Name + "]" + arr[0];
                        NotifyWmsReportTroubleStatus(curDevice, 2, strShowMessage, arr.Length > 1 ? arr[1] : "", readOpcResult.Content.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            if (exceptionMsg.ExcuteTask == null)
            {
                return OperateResult.CreateSuccessResult();
            }
            //1.调用NotifyInstructException通知WMS异常信息 需要的入参：指令编号 包装条码 异常代号 当前地址
            //01 - 放货异常，仓位有货
            //02 - 取货异常，仓位取空
            //03 - 取货异常，取深仓位，浅仓位有货
            OperateResult<NotifyInstructExceptionMode> getMode = GetNotifyInstructExceptionMsg(exceptionMsg);
            if (!getMode.IsSuccess)
            {
                return OperateResult.CreateFailedResult(string.Format("异常转换协议失败，失败原因：\r\n{0}", getMode.Message), 1);
            }
            string cmdPara = JsonConvert.SerializeObject(getMode.Content);
            return NotifyWmsDeviceException(getMode.Content.PACKAGE_BARCODE, cmdPara);
        }
        
        /// <summary>
        /// 设备异常信息上报
        /// </summary>
        /// <param name="deviceErrMsg"></param>
        /// <returns></returns>
        private OperateResult NotifyWmsReportTroubleStatus(DeviceBaseAbstract device,int warnType,string deviceErrMsg,string handlingSuggest,string faultCode)
        {
            NotifyReportTroubleStatusCmdMode mode = new NotifyReportTroubleStatusCmdMode
            {
                MESSAGE = deviceErrMsg,
                HandlingSuggest= handlingSuggest,
                WarnType=warnType,
                DeviceId=device.Id,
                DeviceName=device.Name,
                FaultCode=faultCode,
            };

            string cmdPara = JsonConvert.SerializeObject(mode);
            string interFaceName = "ReportTroubleStatus";
            NotifyElement element = new NotifyElement("", interFaceName, "上报设备异常信息WMS", null, cmdPara);
            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);

            Task.Factory.StartNew(new Action<object>(obj =>
            {
                ReportDeviceTroubleStatusTo3D(obj as NotifyReportTroubleStatusCmdMode);
            }), mode);
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
        protected override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }
        private void ReportDeviceTroubleStatusTo3D(NotifyReportTroubleStatusCmdMode errMode)
        {
            try
            {
                if (errMode == null)
                {
                    return;
                }
                int sim3DDeviceId = 3333;
                DeviceBaseAbstract xDevice = DeviceManage.Instance.FindDeivceByDeviceId(sim3DDeviceId);
                if (xDevice == null)
                {
                    string msg = string.Format("查找不到设备ID：{0} 的设备，请核实设备信息", sim3DDeviceId);
                    LogMessage(msg, EnumLogLevel.Error, true);
                    return;
                }
                Simulate3D transDevice3D = xDevice as Simulate3D;
                if (transDevice3D == null)
                {
                    return;
                }
                Simulate3DControl simulate3DControl = transDevice3D.DeviceControl as Simulate3DControl;
                if (simulate3DControl == null)
                {
                    return;
                }
                string strErrMsg = GetFaultErrInfoByCode(Convert.ToInt32(errMode.FaultCode));
                string[] arr = strErrMsg.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                simulate3DControl.ReportDeviceTroubleStatus(new DeviceFaultRecCmd
                {
                    DeviceID = errMode.DeviceId,
                    DeviceName=errMode.DeviceName,
                    DeviceType=2,
                    FaultDec=errMode.MESSAGE,
                    FaultCode=errMode.FaultCode,
                    FaultName=arr[0],
                    FaultClass = 1,
                    FaultGrade = 2,
                    FaultType = 4,
                    FaultSc = 2,
                    FaultCreatTime=DateTime.Now,
                    RecCeateTime=DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                LogMessage("上报给3D仿真设备故障失败：" + ex.Message, EnumLogLevel.Info, false);

            }
        }

    }
}
