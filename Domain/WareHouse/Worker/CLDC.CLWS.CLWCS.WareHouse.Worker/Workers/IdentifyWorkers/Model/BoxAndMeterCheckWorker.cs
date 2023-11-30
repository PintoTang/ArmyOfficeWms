using System;
using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Interface;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.IdentifyWorkers.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public sealed class BoxAndMeterCheckWorker : InCheckWorkerAbstract
    {
        public override OperateResult ParticularInitlize(int id, DeviceName workerName, WorkerBusinessAbstract business)
        {
            OperateResult baseInitlizeResult = base.ParticularInitlize(id, workerName, business);
            if (!baseInitlizeResult.IsSuccess)
            {
                return baseInitlizeResult;
            }
            return OperateResult.CreateSuccessResult();
        }
        protected override OperateResult ParticularStart()
        {
            OperateResult regesteResult = RegisteHandler();
            if (!regesteResult.IsSuccess)
            {
                return regesteResult;
            }
            return OperateResult.CreateSuccessResult();
        }

        private IdentityDeviceAbstract<List<string>> _boxBarcodeScanner;

        private IdentityDeviceAbstract<List<string>> _meterBarcodeScanner;

        private string _curBoxBarcode = string.Empty;

        private string _lastBoxBarcode = string.Empty;



        private List<string> _meterBarcodeList = new List<string>();


        private OperateResult RegisteHandler()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                foreach (AssistantDevice assistant in WorkerAssistants)
                {
                    if (assistant.AssistantType.Equals(AssistantType.ContainerScanner))
                    {
                        _boxBarcodeScanner = assistant.Device as IdentityDeviceAbstract<List<string>>;
                        if (_boxBarcodeScanner != null)
                        {
                            _boxBarcodeScanner.IdentifyMsgCallbackHandler += HandleBoxBarcode;
                        }
                    }
                    else if (assistant.AssistantType.Equals(AssistantType.ContentScanner))
                    {
                        _meterBarcodeScanner = assistant.Device as IdentityDeviceAbstract<List<string>>;
                        if (_meterBarcodeScanner != null)
                        {
                            _meterBarcodeScanner.IdentifyMsgCallbackHandler += HandleMeterBarcode;
                        }
                    }
                    else if (assistant.AssistantType.Equals(AssistantType.ReadyPoint))
                    {
                        IReadyMonitor readyPoint = assistant.Device as IReadyMonitor;
                        if (readyPoint != null)
                        {
                            readyPoint.ReadyValueChangeEvent += HandleIdentifyReady;
                        }
                    }
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        private OperateResult HandleMeterBarcode(DeviceBaseAbstract meterBarcodeDevice, List<string> meterBarcodes, object[] arg3)
        {
            string meterBarcode = string.Format("收到表条码：{0}", string.Join(",", meterBarcodes));
            LogMessage(meterBarcode, EnumLogLevel.Info, true);
            _meterBarcodeList = meterBarcodes;

            bool notifyResult = NotifyBoxAndMeterBarcode(meterBarcodeDevice, _curBoxBarcode, _meterBarcodeList, _lastBoxBarcode);
            if (!notifyResult)
            {
                return OperateResult.CreateFailedResult("调用WMS接口业务出错");
            }

            _lastBoxBarcode = _curBoxBarcode;

            return OperateResult.CreateSuccessResult();
        }

        private bool NotifyBoxAndMeterBarcode(DeviceBaseAbstract device, string boxBarcode, List<string> meterBarcode, string lastBoxBarcode)
        {
            INotifyBoxAndMeterBarcode notifyBoxAndMeter = WorkerBusiness as INotifyBoxAndMeterBarcode;
            if (notifyBoxAndMeter == null)
            {
                LogMessage(string.Format("协助者业务：{0} 不支持INotifyBoxAndMeterBarcode接口,请核查配置", WorkerBusiness.Name), EnumLogLevel.Error, true);
                return false;
            }
            return notifyBoxAndMeter.NotifyBoxAndMeterBarcode(device, boxBarcode, meterBarcode, lastBoxBarcode);
        }
        private OperateResult HandleBoxBarcode(DeviceBaseAbstract boxBarcodeScanner, List<string> boxBarcodes, object[] arg3)
        {
            if (boxBarcodes.Count <= 0)
            {
                LogMessage("接收到箱条码为空", EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult();
            }
            string boxBarcode = string.Format("收到箱条码：{0}", string.Join(",", boxBarcodes));
            LogMessage(boxBarcode, EnumLogLevel.Info, true);
            _curBoxBarcode = boxBarcodes[0];
            if (_meterBarcodeScanner == null)
            {
                LogMessage("表条码设备为空，请检查配置文件", EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult("表条码设备为空，请检查配置文件");
            }
            _meterBarcodeScanner.GetIdentifyMessageAsync();
            LogMessage("触发读取表条码成功", EnumLogLevel.Info, true);
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<string> identifyMsg, params object[] para)
        {
            throw new NotImplementedException();
        }

        public override void HandleIdentifyReady(DeviceName deviceName, int newValue)
        {
            if (_boxBarcodeScanner == null)
            {
                LogMessage(string.Format("收到就绪：{0} 但是当前箱条码设备为空，请检查配置文件", newValue), EnumLogLevel.Info, true);
                return;
            }
            _boxBarcodeScanner.GetIdentifyMessageAsync();
            LogMessage(string.Format("收到就绪：{0} 触发设备：{1} 读取箱条码成功", newValue, _boxBarcodeScanner.Name), EnumLogLevel.Info, true);
        }
    }
}
