using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PalletierWorkers.Model
{
    public abstract class PalletierWorkerAbstract<TWorkerBusiness> : WorkerBaseAbstract where TWorkerBusiness : PalletierWorkerBusinessAbstract
    {

        private PalletierWorkerProperty _workerProperty = new PalletierWorkerProperty();

        public PalletierWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }

        protected override OperateResult ParticularConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetCoordinationConfig; 
                XmlNode xmlNode = doc.GetXmlNode("Coordination", "Id", Id.ToString());

                if (xmlNode == null)
                {
                    result.Message = string.Format("通过设备：{0} 获取配置失败", Id);
                    result.IsSuccess = false;
                    return result;
                }

                string devicePropertyXml = xmlNode.OuterXml;
                using (StringReader sr = new StringReader(devicePropertyXml))
                {
                    try
                    {
                        WorkerProperty = (PalletierWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(PalletierWorkerProperty));
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
                    return OperateResult.CreateFailedResult(string.Format("配置文件序列化失败：Xml {0} 模型：IdentifyWorkerProperty", devicePropertyXml));
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

        public abstract OperateResult PalletierFinishHandle(DeviceBaseAbstract device, int count, List<PalletizeContent> content);

        public abstract OperateResult PalletizerFinishEachHandle(DeviceBaseAbstract device, int count, List<PalletizeContent> content);

        public TWorkerBusiness WorkerBusiness;
        public abstract OperateResult NotifyOutbound(DeviceBaseAbstract device);
        public override OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business)
        {
            WorkerBusiness = business as TWorkerBusiness;
            if (WorkerBusiness == null)
            {
                return OperateResult.CreateFailedResult(string.Format("工作者业务转换失败，期待类型：PalletierWorkerBusinessAbstract 实际类型：{0}", business.GetType().FullName), 1);
            }

            return OperateResult.CreateSuccessResult();
            
        }

        protected override OperateResult ParticularStart()
        {
            OperateResult registerResult = RegisterPalletizerEvent();
            return registerResult;
        }

        private OperateResult RegisterPalletizerEvent()
        {
            foreach (AssistantDevice assistant in WorkerAssistants)
            {
                if (assistant.AssistantType.Equals(AssistantType.Palletizer))
                {
                    PalletizerDeviceAbstract palletierDevice = assistant.Device as PalletizerDeviceAbstract;
                    if (palletierDevice != null)
                    {
                        palletierDevice.NotifyPalletizerOutboundEvent += NotifyOutbound;
                        palletierDevice.PalletierFullEvent += PalletierFinishHandle;
                        palletierDevice.PalletizerFinishEachEvent += PalletizerFinishEachHandle;
                    }
                }
            }
            return OperateResult.CreateSuccessResult();
        }

    }
}
