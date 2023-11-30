using System;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common
{
    /// <summary>
    /// 设备控制的基类
    /// </summary>
    public abstract class DeviceControlBaseAbstract
    {
        public Action<string, EnumLogLevel, bool> LogMessageAction { get; set; }
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            if (LogMessageAction != null)
            {
                LogMessageAction(msg, level, isNotifyUi);
            }
        }
        private OperateResult InitConfig()
        {
            return ParticularInitConfig();
        }
        public  abstract OperateResult ParticularInitConfig();
        public abstract OperateResult ParticularInitlize();

        public abstract OperateResult Start();

        internal OperateResult Initailize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
           
            OperateResult initParticularResult = ParticularInitlize();
            if (!initParticularResult.IsSuccess)
            {
                return initParticularResult;
            }
            OperateResult initConfigResult = InitConfig();
            if (!initConfigResult.IsSuccess)
            {
                return initConfigResult;
            }

            OperateResult startResult = Start();
            if (!startResult.IsSuccess)
            {
                return startResult;
            }

            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 设备标识名
        /// </summary>
        public DeviceName DeviceName { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public string ClassName { get; set; }

        public string NameSpace { get; set; }

        public abstract UserControl GetPropertyView();

    }
}
