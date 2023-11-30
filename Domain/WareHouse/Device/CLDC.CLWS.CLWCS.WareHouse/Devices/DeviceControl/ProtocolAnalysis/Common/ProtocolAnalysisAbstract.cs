using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.ProtocolAnalysis.Common
{
    /// <summary>
    /// 协议解析虚类
    /// </summary>
    public abstract class ProtocolAnalysisAbstract
    {
        public int DeviceId { get; set; }
        public DeviceName DeviceName { get; set; }
        public OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;
            OperateResult particularInitialize = ParticularInitialize();
            if (!particularInitialize.IsSuccess)
            {
                return particularInitialize;
            }
            OperateResult config = InitConfig();
            if (!config.IsSuccess)
            {
                return config;
            }
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult InitConfig()
        {
            OperateResult particularInitConfig = ParticularInitConfig();
            if (!particularInitConfig.IsSuccess)
            {
                return particularInitConfig;
            }
            return OperateResult.CreateSuccessResult();
        }

        public abstract OperateResult ParticularInitialize();

        public abstract OperateResult ParticularInitConfig();
    }
}
