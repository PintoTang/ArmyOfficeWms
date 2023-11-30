using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Swith.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Swith.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.Model
{
    public abstract class SwitchDeviceAbstract : DeviceBaseAbstract
    {
        //1.监控指定DB值的变化
        //2.处理DB值的变化
        //3.通知注册监听DB值变化的监听者

      public  SwitchDeviceBusinessAbstract DeviceBusiness { get; set; }
      public  SwitchDeviceControlAbstract DeviceControl { get; set; }


        private SwitchDeviceProperty _deviceProperty = new SwitchDeviceProperty();

      public SwitchDeviceProperty DeviceProperty
      {
          get
          {
              return _deviceProperty;
          }
          set { _deviceProperty = value; }
      }

     

        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
           return DeviceBusiness.ComputeNextAddr(destAddr);
        }
        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.DeviceBusiness = business as SwitchDeviceBusinessAbstract;
            this.DeviceControl = control as SwitchDeviceControlAbstract;
            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "SwitchDeviceBusinessAbstract", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "SwitchDeviceControlAbstract", control.GetType().FullName);

                return OperateResult.CreateFailedResult(msg, 1);
            }
            return OperateResult.CreateSuccessResult();
        }
        public override OperateResult Availabe()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool Accessible(Addr destAddr)
        {
            return false;
        }

        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override void RefreshDeviceState()
        {
            return;
        }

        public override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                XmlNode xmlNode = doc.GetXmlNode("Device", "DeviceId", Id.ToString());


                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string stationPropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(stationPropertyXml))
                {
                    try
                    {
                        DeviceProperty = (SwitchDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(SwitchDeviceProperty));
                        result.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (!result.IsSuccess)
                {
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：StationProperty", stationPropertyXml));
                }

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult GetDeviceRealData()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularStart()
        {
            OperateResult registerChange= RegisterValueChange(DataBlockNameEnum.PickingReadyDataBlock, HandlePickingValueChange);
            if (!registerChange.IsSuccess)
            {
                return registerChange;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 注册OPC监控的值变化处理
        /// </summary>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcValue monitervaluechange)
        {
            OperateResult registerResutl = DeviceControl.RegisterValueChange(dbBlockEnum, monitervaluechange);
            LogMessage(registerResutl.Message, EnumLogLevel.Info, false);
            return registerResutl;
        }

        private void HandlePickingValueChange(int newValue)
        {
            bool isNeedHandle = DeviceBusiness.IsNeedHandleValue(newValue);
            if (isNeedHandle)
            {
                if (SwithValueChangeEvent==null)
                {
                    LogMessage("处理按钮值变化的业务SwithValueChangeHandle:为空",EnumLogLevel.Error, true);
                    return;
                }
                OperateResult handleResult = SwithValueChangeEvent(this,newValue);
                string msg = string.Empty;
                EnumLogLevel level= EnumLogLevel.Info;
                if (handleResult.IsSuccess)
                {
                    msg = string.Format("处理值：{0} 变化业务成功",newValue);
                    level= EnumLogLevel.Info;
                }
                else
                {
                    msg = string.Format("处理值：{0} 变化业务失败，原因：{1}", newValue,handleResult.Message);
                    level= EnumLogLevel.Error;
                }
                LogMessage(msg, level, true);
            }
        }
        /// <summary>
        ///     转换按钮值变化的处理
        /// </summary>
        public Func<object, int, OperateResult> SwithValueChangeEvent;
       
    }
}
