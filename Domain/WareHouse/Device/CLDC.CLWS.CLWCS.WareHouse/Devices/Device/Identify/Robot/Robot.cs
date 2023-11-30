using CL.Framework.CmdDataModelPckg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Infrastructrue.Xml;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Robot;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.View;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    /// <summary>
    /// Robot SR 2000
    /// </summary>
    public class Robot : IdentityDeviceAbstract<List<string>>
    {
        private RobotDeviceProperty _deviceProperty = new RobotDeviceProperty();

        public RobotDeviceProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
        }

        #region 内部接口

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
                        DeviceProperty = (RobotDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(RobotDeviceProperty));
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

                this.ExposedUnId = DeviceProperty.Config.ExposedUnId;

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult Availabe()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override bool Accessible(Addr destAddr)
        {
            return CurAddress.IsContain(destAddr);
        }

        public override OperateResult GetDeviceRealData()
        {
            if (LiveDataDbHelper == null)
            {
                return OperateResult.CreateSuccessResult();
            }
            OperateResult<List<LiveData>> getLiveData = LiveDataDbHelper.GetAllLiveData(this.Id);
            if (!getLiveData.IsSuccess)
            {
                string logmsg = "获取实时数据失败";
                LogMessage(logmsg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(logmsg);
            }
            CurrentLiveData.DataPool = getLiveData.Content;
            NotifyAttributeChange("CurrentLiveData", CurrentLiveData.Clone());
            foreach (LiveData value in CurrentLiveData.DataPool.OrderBy(l => l.Index))
            {
                CurrentContent.Add(value.DataValue);
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult GetDeviceRealStatus()
        {
            return OperateResult.CreateSuccessResult();
        }



        public override OperateResult ClearUpRunMessage(TransportMessage transport)
        {
            SaveContentToDatabase(CurrentContent, HandleStatusEnum.Finished);
            SetCurrentContent(new List<string>());
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult<WareHouseViewModelBase> CreateViewModel()
        {
            OperateResult<WareHouseViewModelBase> viewModelResult = new OperateResult<WareHouseViewModelBase>();
            RobotViewModel viewModel = new RobotViewModel(this);
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
            return new RobotAssistantView();
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            UserControl view = new RobotDetailView();
            return OperateResult.CreateSuccessResult(view);
        }

        protected override Window CreateConfigView()
        {
            Window configView = new DeviceConfigView();
            DeviceConfigViewModel<Robot, RobotDeviceProperty> viewModel = new DeviceConfigViewModel<Robot, RobotDeviceProperty>(this, DeviceProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }

        public override OperateResult UpdateProperty()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                this.Name = DeviceProperty.Name;
                this.WorkSize = DeviceProperty.WorkSize;
                this.CurAddress = new Addr(DeviceProperty.CurAddress);
                this.DeviceName = new DeviceName(DeviceProperty.DeviceName);
                this.NameSpace = DeviceProperty.NameSpace;
                this.ClassName = DeviceProperty.ClassName;
                this.Id = DeviceProperty.DeviceId;
                this.IsShowUi = DeviceProperty.IsShowUi;

                this.DeviceControl.Name = DeviceProperty.ControlHandle.Name;
                this.DeviceControl.ClassName = DeviceProperty.ControlHandle.ClassName;
                this.DeviceControl.NameSpace = DeviceProperty.ControlHandle.NameSpace;

                this.DeviceBusiness.Name = DeviceProperty.BusinessHandle.Name;
                this.DeviceBusiness.NameSpace = DeviceProperty.BusinessHandle.NameSpace;
                this.DeviceBusiness.ClassName = DeviceProperty.BusinessHandle.ClassName;

                OperateResult initControlConfig = this.DeviceControl.ParticularInitConfig();
                if (!initControlConfig.IsSuccess)
                {
                    return initControlConfig;
                }

                OperateResult initBusinessConfig = this.DeviceBusiness.ParticularConfig();

                if (!initBusinessConfig.IsSuccess)
                {
                    return initBusinessConfig;
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                LogMessage(string.Format("属性更新到内存失败：{0}", OperateResult.ConvertException(ex)), EnumLogLevel.Error, false);
            
            }
            return result;
        }

        public override bool IsRepeated(List<string> toCompare)
        {
            if (SystemConfig.Instance.WhCode.Equals("FS1") || SystemConfig.Instance.WhCode.Equals("SNDL1"))
            {
                return false;
            }
            else
            {
                if (CurrentContent == null || CurrentContent.Count <= 0)
                {
                    return false;
                }
                bool isRepeat = true;
                for (int i = 0; i < CurrentContent.Count; i++)
                {
                    if (toCompare.Count > i)
                    {
                        if (!toCompare[i].Equals(CurrentContent[i]))
                        {
                            isRepeat = false;
                            break;
                        }
                    }
                }
                return isRepeat;
            }
        }

        public override OperateResult SaveContentToDatabase(List<string> content, HandleStatusEnum status)
        {
            CurrentLiveData.DataPool.Clear();
            OperateResult saveContentResult = new OperateResult();
            if (LiveDataDbHelper == null)
            {
                saveContentResult.IsSuccess = false;
                saveContentResult.Message = "实时数据库操作为空";
                return saveContentResult;
            }
            for (int i = 1; i <= content.Count; i++)
            {
                LiveData liveData = new LiveData
                {
                    DeviceId = Id,
                    Alias = Name,
                    Name = DeviceName.FullName,
                    HandleStatus = status,
                    Index = i,
                    DataValue = content[i - 1]
                };
                CurrentLiveData.AddPool(liveData);
                NotifyAttributeChange("CurrentLiveData", CurrentLiveData.Clone());
                OperateResult saveResult = LiveDataDbHelper.Save(liveData);
                if (!saveResult.IsSuccess)
                {
                    string logmsg = string.Format("保存当前数据 值：{0} 序号：{1}", content[i - 1], i);
                    LogMessage(logmsg, EnumLogLevel.Warning, true);
                }
            }
            saveContentResult.IsSuccess = true;
            return saveContentResult;
        }

        public override OperateResult SetCurrentContent(List<string> content)
        {
            OperateResult result = new OperateResult();
            try
            {
                if (content.Count <= 0)
                {
                    CurrentContent = new List<string>();
                    return OperateResult.CreateSuccessResult();
                }
                List<string> cloneList = content.Select(value => (string)value.Clone()).ToList();
                CurrentContent = cloneList;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        public override OperateResult DeleteLiveDataContent(LiveData liveData)
        {
            OperateResult deleteContentResult = new OperateResult();
            if (LiveDataDbHelper == null)
            {
                deleteContentResult.IsSuccess = false;
                deleteContentResult.Message = "实时数据库操作为空";
                return deleteContentResult;
            }
            OperateResult deleteResult = LiveDataDbHelper.DeleteLiveData(liveData);
            if (!deleteResult.IsSuccess)
            {
                string logmsg = string.Format("删除数据：{0} 失败", liveData.DataValue);
                LogMessage(logmsg, EnumLogLevel.Warning, true);
                return OperateResult.CreateFailedResult();
            }
            CurrentLiveData.RemovePool(liveData);
            CurrentContent.Remove(liveData.DataValue);
            NotifyAttributeChange("CurrentLiveData", CurrentLiveData.Clone());
            deleteContentResult.IsSuccess = true;
            return deleteContentResult;
        }
        #endregion 
    
      
    }
}
