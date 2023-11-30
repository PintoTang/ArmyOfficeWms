using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class EqDisplayDevice : DisplayDeviceAbstract
    {
        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
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
                        DeviceProperty = (DisplayDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(DisplayDeviceProperty));
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


                IsNeedClearScreen = DeviceProperty.Config.IsNeedClearScreen;
                ClearScreenInterval = DeviceProperty.Config.ClearScreenInterval;
                DefaultContent = DeviceProperty.Config.DefaultContent.Trim();
                CurTitleContent = DeviceProperty.Config.DefaultTitle.Trim();

                DeviceControl.TitleContent = CurTitleContent;

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();

        }

        public override OperateResult GetDeviceRealData()
        {
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

        public override OperateResult SendContent(string contentMsg)
        {
            OperateResult sendResult = DeviceControl.SendContent(contentMsg);
            string msg = string.Format("发送屏显：{0} {1}", contentMsg, sendResult.IsSuccess ? "成功" : "失败");
            LogMessage(msg, EnumLogLevel.Info, true);
            return sendResult;
        }

        public override OperateResult SendTitle(string title)
        {
            OperateResult sendResult = DeviceControl.SendTitle(title);
            string msg = string.Format("发送标题结果：{0}", sendResult.Message);
            LogMessage(msg, EnumLogLevel.Info, true);
            return sendResult;
        }

        public override OperateResult ClearScreen()
        {
            var task = Task.Run(() =>
            {
                System.Threading.Thread.Sleep(30000);
                //DeviceControl.ClearScreen();
                //System.Threading.Thread.Sleep(10000);
                DeviceControl.SendContent(" ");
            });
            return OperateResult.CreateSuccessResult();
            //OperateResult clearResult = DeviceControl.ClearScreen();
            //string msg = string.Format("Eq清屏结果：{0}", clearResult.Message);
            //LogMessage(msg, EnumLogLevel.Info, true);
            //return clearResult;
        }


        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            var task = Task.Run(() =>
            {
                ClearScreen();
            });
            return OperateResult.CreateSuccessResult();
        }
        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            DisplayDeviceViewModel viewModel = new DisplayDeviceViewModel(this);
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

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<EqDisplayDevice, DisplayDeviceProperty> viewModel = new DeviceConfigViewModel<EqDisplayDevice, DisplayDeviceProperty>(this, DeviceProperty);

            configView.DataContext = viewModel;
            return configView;
        }

     
        protected override Window CreateAssistantView()
        {
            return null;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            DisplayDeviceDetailView view = new DisplayDeviceDetailView();
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

        public override void RefreshDeviceState()
        {
            return;
        }
    }
}
