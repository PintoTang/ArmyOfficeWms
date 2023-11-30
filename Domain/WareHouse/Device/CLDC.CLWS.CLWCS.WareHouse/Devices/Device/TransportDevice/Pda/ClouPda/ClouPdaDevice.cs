using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.WebApi;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Pda.ClouPda;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Pda.ClouPda.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Roller.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Pda.CmdModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class ClouPdaDevice : TransportDeviceBaseAbstract, IClouPdaApi
    {
        

        private int _clouPdaApiPort;
        public override OperateResult ParticularStart()
        {

            DependencyFactory.GetDependency().RegisterInstance<IClouPdaApi>(this);

            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularConfig()
        {
            //获取Http 的值
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                XmlOperator doc = ConfigHelper.GetDeviceConfig;
                string path = "Config";
                XmlElement xmlElement = doc.GetXmlElement("Device", "DeviceId", Id.ToString());
                XmlNode xmlNode = xmlElement.SelectSingleNode(path);
                if (xmlNode == null)
                {
                    string msg = string.Format("设备编号：{0} 中 {1} 路径配置为空", Id, path);
                    return OperateResult.CreateFailedResult(msg, 1);
                }

                string communicationConfigXml = xmlNode.OuterXml;
                WebApiClientProperty communicationProperty = null;
                using (StringReader sr = new StringReader(communicationConfigXml))
                {
                    try
                    {
                        communicationProperty = (WebApiClientProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(WebApiClientProperty));
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = OperateResult.ConvertException(ex);
                    }
                }

                if (communicationProperty == null)
                {
                    return result;
                }

                _clouPdaApiPort = communicationProperty.ApiPort;
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
                result = OperateResult.CreateFailedResult();
            }
            return result;
        }

        public override bool Accessible(Addr destAddr)
        {
            return CurAddress.IsContain(destAddr);
        }

        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            TransportDeviceViewModel viewModel = new TransportDeviceViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;
        }

        protected override OperateResult<ViewAbstract> CreateView()
        {
            OperateResult<ViewAbstract> result = new OperateResult<ViewAbstract>();
            DeviceShowCard view = new DeviceShowCard();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override Window CreateAssistantView()
        {
            return null;
        }

        protected override Window CreateConfigView()
        {
            return null;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            ClouPdaDetailView view = new ClouPdaDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult UpdateProperty()
        {
            throw new NotImplementedException();
        }

        public override OperateResult IsCanChangeDispatchState(DispatchState destState)
        {
            return OperateResult.CreateFailedResult();
        }

        public override OperateResult RegisterOrderFinishFlag(TransportMessage transMsg)
        {
            return OperateResult.CreateSuccessResult();
        }

        public override void HandleOrderValueChange(int value)
        {
            return;
        }

        public override OperateResult IsCanChangeControlState(ControlStateMode destState)
        {
            return OperateResult.CreateFailedResult();
        }

        public SyncResReErr NotifyInstructFinish(string cmd)
        {
            SyncResReErr responseResult = new SyncResReErr(0, "无数据");
            try
            {
                string notifyMsg = string.Format("接收到信息：{0}", cmd);
                LogMessage(notifyMsg, EnumLogLevel.Info, true);
                NotifyInstructFinishCmd notifyInstructCmd = cmd.ToObject<NotifyInstructFinishCmd>();
                Task.Run(() =>
                {
                    OperateResult<TransportMessage> finishedTransport =
                        UnFinishedTask.FindData(o => o.UniqueCode.Equals(notifyInstructCmd.INSTRUCTION_CODE.ToString()));
                    if (!finishedTransport.IsSuccess)
                    {
                        string msg = string.Format("不存在未完成指令：{0} 不做处理", notifyInstructCmd.INSTRUCTION_CODE);
                        LogMessage(msg, EnumLogLevel.Info, false);
                        responseResult.RESULT = 0;
                        responseResult.ERR_MSG = msg;
                    }
                    else
                    {
                        OperateResult finishResult = FinishTransport(finishedTransport.Content, FinishType.AutoFinish);
                        if (!finishResult.IsSuccess)
                        {
                            responseResult.RESULT = 0;
                            responseResult.ERR_MSG = finishResult.Message;
                            LogMessage(finishResult.Message, EnumLogLevel.Info, false);
                        }
                    }
                });
                responseResult.RESULT = HandleResult.Success;
                responseResult.ERR_MSG = "成功";
                return responseResult;
            }
            catch (Exception ex)
            {
                responseResult.RESULT = 0;
                responseResult.ERR_MSG = ex.Message;
            }
            return responseResult;
        }
    }
}
