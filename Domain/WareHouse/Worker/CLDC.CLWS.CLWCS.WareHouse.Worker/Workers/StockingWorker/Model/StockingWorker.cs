using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.Interface;
using CLDC.CLWS.CLWCS.WareHouse.OrderHandle;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.PickingWorker.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.StockingWorker.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.StockingWorker.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.StockingWorker.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker
{
    public class StockingWorker : StockingWorkerAbstract<StockingWorkerBusinessAbstract>, IHandleOrderExcuteStatus
    {

        private StockingWorkerProperty _workerProperty = new StockingWorkerProperty();

        public StockingWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }


        protected override OperateResult ParticularStart()
        {
            OperateResult registeResult = RegisteHandler();
            if (!registeResult.IsSuccess)
            {
                return registeResult;
            }

            RegisterOrderHandle();

            //_contentScanner.GetIdentifyMessageAsync();

            return OperateResult.CreateSuccessResult();
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
                        WorkerProperty = (StockingWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(StockingWorkerProperty));
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

        public override OperateResult GetWorkerRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetWorkerRealData()
        {
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult HandleIdentifyMsg(DeviceBaseAbstract device, List<string> identifyMsg, params object[] para)
        {
            string barcode = string.Join(",", identifyMsg);
            string barcodeMsg = string.Format("收到条码信息：{0}", string.Join(",", identifyMsg));
            LogMessage(barcodeMsg, EnumLogLevel.Info, true);

            OperateResult handleBarcode = WorkerBusiness.HandleIdentifyMsg(device, WorkerAssistants, barcode);
            if (handleBarcode.IsSuccess)
            {
                LogMessage(handleBarcode.Message, EnumLogLevel.Info, true);
            }
            else
            {
                LogMessage(handleBarcode.Message, EnumLogLevel.Error, true);
            }
            return handleBarcode;
        }

        private void RegisterOrderHandle()
        {
            IOrderNotifyCentre orderNotifyCentre = DependencyHelper.GetService<IOrderNotifyCentre>();
            if (orderNotifyCentre != null)
            {
                orderNotifyCentre.RegisterOrderStatusListener(this);
            }
        }

        private IdentityDeviceAbstract<List<string>> _contentScanner;
        private OperateResult RegisteHandler()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                foreach (AssistantDevice assistant in WorkerAssistants)
                {
                    if (assistant.AssistantType.Equals(AssistantType.ContentScanner))
                    {
                        _contentScanner = assistant.Device as IdentityDeviceAbstract<List<string>>;
                        if (_contentScanner != null)
                        {
                            _contentScanner.IdentifyMsgCallbackHandler += HandleIdentifyMsg;
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



        protected override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            StockingWorkerDeviceViewModel viewModel = new StockingWorkerDeviceViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<StockingWorker, StockingWorkerProperty> viewModel = new WorkerConfigViewModel<StockingWorker, StockingWorkerProperty>(this, WorkerProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected internal override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = WorkerProperty.Name;
                this.WorkSize = WorkerProperty.WorkSize;
                this.WorkerName = new DeviceName(WorkerProperty.WorkerName);
                this.NameSpace = WorkerProperty.NameSpace;
                this.ClassName = WorkerProperty.ClassName;
                this.Id = WorkerProperty.WorkerId;

                this.WorkerBusiness.Name = WorkerProperty.BusinessHandle.Name;
                this.WorkerBusiness.ClassName = WorkerProperty.BusinessHandle.ClassName;
                this.WorkerBusiness.NameSpace = WorkerProperty.BusinessHandle.NameSpace;

                OperateResult initWorkerConfig = this.InitConfig();
                if (!initWorkerConfig.IsSuccess)
                {
                    return initWorkerConfig;
                }

                OperateResult initBusinessConfig = this.WorkerBusiness.InitConfig();
                if (!initBusinessConfig.IsSuccess)
                {
                    return initBusinessConfig;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
                result.IsSuccess = false;
            }
            return result;
        }

        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            WorkerShowCard view = new WorkerShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            StockingWorkerDetailView view = new StockingWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public OperateResult HandleOrderChange(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type)
        {
            switch (type)
            {
                case TaskHandleResultEnum.Finish:
                case TaskHandleResultEnum.ForceFinish:
                    return FinishOrderHandle(order);
                case TaskHandleResultEnum.Discard:
                    break;
                case TaskHandleResultEnum.Cancle:
                    break;
                case TaskHandleResultEnum.Update:
                    break;
            }
            return OperateResult.CreateSuccessResult();
        }

        private OperateResult FinishOrderHandle(ExOrder order)
        {
            if (order.DestAddr.Type.Equals("Stocking")&&order.SourceTaskType.Equals(SourceTaskEnum.InventoryOut))
            {
                if (_contentScanner != null)
                {
                    _contentScanner.GetIdentifyMessageAsync();
                    LogMessage(string.Format("根据盘点指令：{1} 控制RFID设备：{0} 扫描成功", _contentScanner.Name,order),EnumLogLevel.Info, true);
                    return OperateResult.CreateSuccessResult();
                }
                LogMessage(string.Format("根据盘点指令：{0}，获取到的条码识别设备为空",order), EnumLogLevel.Info, true);
                return OperateResult.CreateFailedResult("获取到的条码识别设备为空");
            }
            return OperateResult.CreateSuccessResult();
        }
    }
}
