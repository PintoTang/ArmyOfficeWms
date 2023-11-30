using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Station.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.Simulate3D.Model;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;

namespace CLWCS.WareHouse.Device.HeFei
{
    public class SwitchableStationBusinessEx : StationDeviceBusinessAbstract
    {
        public override OperateResult ParticularInitlize()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult IsCanChangeMode(DeviceModeEnum destMode)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ChangeMode(DeviceModeEnum destMode)
        {
            CurMode = destMode;
            return OperateResult.CreateSuccessResult();
        }

        public override bool CheckMode(DeviceModeEnum destMode)
        {
            if (CurMode.Equals(destMode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override OperateResult ParticularConfig()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool IsNeedHandleOrderValue(int newValue)
        {
            if (newValue.Equals(CurOrderValue) || newValue <= 0)
            {
                CurOrderValue = newValue;
                return false;
            }
            else
            {
                CurOrderValue = newValue;
                return true;
            }
        }

        public override bool IsNeedHandleDestValue(int newValue)
        {
            if (newValue.Equals(CurDestValue))
            {
                return false;
            }
            else
            {
                CurDestValue = newValue;
                return true;
            }
        }

        public override void HandleFault(int deviceId,string deviceName,string faultDesc,string handlingSuggest)
        {
            try
            {
                NotifyReportTroubleStatusCmdMode data = new NotifyReportTroubleStatusCmdMode
                {
                    MESSAGE = "【"+deviceName+"】" + faultDesc,
                    HandlingSuggest = handlingSuggest,
                    WarnType = 2,
                    DeviceId = deviceId,
                    DeviceName = deviceName,
                    FaultCode = "1",
                };
                //string cmdPara = data.ToJson();
                //string interFaceName = "ReportTroubleStatus";
                //NotifyElement element = new NotifyElement("", interFaceName, "上报设备异常信息WMS", null, cmdPara);
                //OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
                //Task.Factory.StartNew(new Action<object>(obj =>
                //{
                //    ReportDeviceTroubleStatusTo3D(obj as NotifyReportTroubleStatusCmdMode);
                //}), data);
                //CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                //if (serviceReturn.IsSuccess)
                //{
                //    Console.WriteLine("调用接口 " + interFaceName + "成功，并且WMS返回成功信息");
                //}
                //else
                //{
                //    Console.WriteLine(string.Format("调用接口  " + interFaceName + " 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
                CLDC.CLWS.CLWCS.WareHouse.Device.Simulate3D transDevice3D = xDevice as CLDC.CLWS.CLWCS.WareHouse.Device.Simulate3D;
                if (transDevice3D == null)
                {
                    return;
                }
                Simulate3DControl simulate3DControl = transDevice3D.DeviceControl as Simulate3DControl;
                if (simulate3DControl == null)
                {
                    return;
                }
                simulate3DControl.ReportDeviceTroubleStatus(new DeviceFaultRecCmd
                {
                    DeviceID = errMode.DeviceId,
                    DeviceName = errMode.DeviceName,
                    DeviceType = 2,
                    FaultDec = errMode.MESSAGE,
                    FaultCode = errMode.FaultCode,
                    FaultName = errMode.MESSAGE,
                    FaultClass = 1,
                    FaultGrade = 2,
                    FaultType = 4,
                    FaultSc = 2,
                    FaultCreatTime = DateTime.Now,
                    RecCeateTime = DateTime.Now,
                });
            }
            catch (Exception ex)
            {
                LogMessage("上报给3D仿真设备故障失败：" + ex.Message, EnumLogLevel.Info, false);

            }
        }
    }
}
