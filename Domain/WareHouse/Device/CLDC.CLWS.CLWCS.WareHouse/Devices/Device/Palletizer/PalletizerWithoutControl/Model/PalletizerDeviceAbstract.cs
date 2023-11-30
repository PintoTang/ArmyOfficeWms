using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CL.WCS.DataModelPckg;
using CL.WCS.OPCMonitorAbstractPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model.Config;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Palletizer.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Palletizer.Common;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Palletizer.Model
{
    public abstract class PalletizerDeviceAbstract : DeviceBaseAbstract
    {
        public override void RefreshDeviceState()
        {
            IsHasTask = CurContent.Lenght > 0;
        }

        public OperateResult SavePallizerContentToDb(PalletizeContent content)
        {
            LiveData liveData = PalletizerContentToLiveData(content);
            return SaveLiveDataToDb(liveData);
        }

        public OperateResult RemovePallizerContentToDb(PalletizeContent content)
        {
            LiveData liveData = PalletizerContentToLiveData(content);
            return RemoveLiveDataToDb(liveData);
        }

        protected LiveData PalletizerContentToLiveData(PalletizeContent content)
        {
            LiveData data = new LiveData();
            data.DataValue = content.ContentBarcode;
            data.Index = content.ContentIndex;
            data.DeviceId = this.Id;
            data.Name = this.DeviceName.FullName;
            data.Alias = this.Name;
            data.HandleStatus = HandleStatusEnum.UnFinished;
            return data;
        }



        public UniqueDataObservablePool<PalletizeContent> CurContent
        {
            get { return _curContent; }
            set
            {
                _curContent = value;
                RaisePropertyChanged();
            }
        }

        public OperateResult AddContent(PalletizeContent content)
        {
            SavePallizerContentToDb(content);
            _curContent.AddPool(content);
            RefreshDeviceState();
            return OperateResult.CreateSuccessResult();
        }

        protected void ClearContent()
        {

            _curContent.ClearPool();
            ClearLiveDataToDb();
            RefreshDeviceState();
        }

        public OperateResult RemoveContent(PalletizeContent content)
        {

            _curContent.RemovePool(content);
            RefreshDeviceState();
            return OperateResult.CreateSuccessResult();
        }

        protected OperateResult RemoveContent(string contentUnique)
        {
            return _curContent.RemovePool(c => c.UniqueCode.Equals(contentUnique));
        }

        private PalletizerDeviceProperty _deviceProperty = new PalletizerDeviceProperty();

        public PalletizerDeviceProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
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
                        DeviceProperty = (PalletizerDeviceProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(PalletizerDeviceProperty));
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

        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }
        public override OperateResult GetDeviceRealData()
        {
            //断点恢复

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

            foreach (PalletizeContent content in getLiveData.Content.OrderBy(l => l.Index).Select(value => new PalletizeContent(value.Index.GetValueOrDefault(), value.DataValue)))
            {
                CurContent.AddPool(content);
            }

            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularStart()
        {
            OperateResult registerCountChange = RegisterValueChange(DataBlockNameEnum.CountDataBlock, HandleOrderValueChange);
            if (!registerCountChange.IsSuccess)
            {
                return registerCountChange;
            }
            OperateResult registerSignChange = RegisterValueChange(DataBlockNameEnum.IsReleaseDataBlock, HandleNeedSignChange);
            if (!registerSignChange.IsSuccess)
            {
                return registerSignChange;
            }
            return registerSignChange;
        }

        /// <summary>
        /// 处理碟盘完成的事件
        /// </summary>
        public Func<DeviceBaseAbstract, int, List<PalletizeContent>, OperateResult> PalletierFullEvent;

        /// <summary>
        /// 碟盘机每完成一个的事件通知
        /// </summary>
        public Func<DeviceBaseAbstract, int, List<PalletizeContent>, OperateResult> PalletizerFinishEachEvent;
        /// <summary>
        /// 申请空托盘事件
        /// </summary>
        public Func<DeviceBaseAbstract, OperateResult> OutboundEvent;

        public Func<DeviceBaseAbstract, OperateResult> NotifyPalletizerOutboundEvent;


        protected internal PalletizerBusinessAbstract DeviceBusiness;

        protected internal PalletierControlAbstract DeviceControl;


        protected OperateResult<int> GetPalletizerCount()
        {
            OperateResult<int> readResult = DeviceControl.GetPalletizerCount();
            return readResult;
        }

        public OperateResult ReNotifyPalletizerFinish()
        {
            if (PalletizerFinishEachEvent != null)
            {
                if (CurPalletizerCount <= 0)
                {
                    return OperateResult.CreateFailedResult("碟盘机无数据");
                }
                OperateResult finishEventResult = PalletizerFinishEachEvent(this, CurPalletizerCount, CurContent.ToList());
                return finishEventResult;
            }
            return OperateResult.CreateFailedResult("PalletizerFinishEachEvent 尚未注册");
        }

        protected OperateResult<PalletizeContent> GetCurFinishContent()
        {
            return DeviceControl.GetCurFinishContent();
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

        /// <summary>
        /// 注册OPC监控的值变化处理
        /// </summary>
        /// <param name="dbBlockEnum"></param>
        /// <param name="monitervaluechange"></param>
        public OperateResult RegisterValueChange(DataBlockNameEnum dbBlockEnum, CallbackContainOpcBoolValue monitervaluechange)
        {
            OperateResult registerResutl = DeviceControl.RegisterValueChange(dbBlockEnum, monitervaluechange);
            LogMessage(registerResutl.Message, EnumLogLevel.Info, false);
            return registerResutl;
        }


        private int _curOrderValue = 0;

        public int CurOrderValue
        {
            get
            {
                return _curOrderValue;
            }
            set
            {
                if (_curOrderValue != value)
                {
                    _curOrderValue = value;
                }
                RaisePropertyChanged("CurOrderValue");
            }
        }

        private bool _curNeedSignValue;

        private UniqueDataObservablePool<PalletizeContent> _curContent = new UniqueDataObservablePool<PalletizeContent>();

        public bool CurNeedSignValue
        {
            get
            {
                return _curNeedSignValue;
            }
            set
            {
                if (_curNeedSignValue != value)
                {
                    _curNeedSignValue = value;
                }
                RaisePropertyChanged("CurNeedSignValue");
            }
        }

        public int CurPalletizerCount
        {
            get { return CurContent.DataPool.Count; }
        }

        public bool IsFull(int count)
        {
            return CurPalletizerCount >= DeviceBusiness.Capacity;
        }


        private bool IsEmpty(int count)
        {
            return count <= 0;
        }
        protected void HandleOrderValueChange(int newValue)
        {
            //1.判断当前叠盘机数量，小于等于0 清空当前叠盘内容
            //2.处理当前叠盘完成的信息
            if (IsEmpty(newValue))
            {
                ClearContent();
                LogMessage(string.Format("获取到当前碟盘机数量：{0} 清空碟盘机内容", newValue), EnumLogLevel.Info, true);
                return;
            }

            if (newValue.Equals(CurPalletizerCount))
            {
                LogMessage(string.Format("获取到当前的数量与容量相等：{0} 不做业务处理", newValue), EnumLogLevel.Info, true);
                return;
            }

            OperateResult<PalletizeContent> curFinishContent = GetCurFinishContent();
            if (!curFinishContent.IsSuccess)
            {
                LogMessage(string.Format("获取碟盘机当前完成的信息失败：{0}", curFinishContent.Message), EnumLogLevel.Error, true);
                return;
            }

            OperateResult<PalletizeContent> GetBarcode = _curContent.FindData(t => t.ContentBarcode.Equals(curFinishContent.Content.ContentBarcode));
            if(!GetBarcode.IsSuccess)
            AddContent(curFinishContent.Content);



            bool isNeedHandle = DeviceBusiness.GetIsNeedHandleEachFinish();
            if (!isNeedHandle)
            {
                LogMessage(string.Format("收到碟盘机完成数量：{0} 业务判断不需要对每个业务完成进行处理", newValue), EnumLogLevel.Info, true);
            }
            else
            {
                if (PalletizerFinishEachEvent != null)
                {
                    OperateResult finishEventResult = PalletizerFinishEachEvent(this, newValue, CurContent.ToList());
                    LogMessage(string.Format("通知每个叠盘完成处理结果：{0} {1}", finishEventResult.IsSuccess, finishEventResult.Message), EnumLogLevel.Info, true);
                }
                else
                {
                    LogMessage(string.Format("收到碟盘机完成数量：{0} 业务判断每个业务完成需要处理，但是PalletizerFinishEachEvent为空", newValue), EnumLogLevel.Error, true);
                }
            }
            bool isNeedVerifyCapacity = DeviceBusiness.GetIsNeedVerifyCapacity();
            if (isNeedVerifyCapacity)
            {
                bool isFull = IsFull(newValue);
                if (isFull)
                {
                    if (PalletierFullEvent != null)
                    {
                        OperateResult finishEventResult = PalletierFullEvent(this, newValue, CurContent.ToList());
                        LogMessage(string.Format("通知叠盘完成处理结果：{0} {1}", finishEventResult.IsSuccess, finishEventResult.Message), EnumLogLevel.Info, true);
                    }
                    else
                    {
                        LogMessage(string.Format("收到碟盘机完成数量：{0} 业务判断需要处理，但是PalletierFinishEvent为空", newValue), EnumLogLevel.Error, true);
                    }
                }
            }
        }

        protected abstract void HandleNeedSignChange(int newValue);

        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.DeviceBusiness = business as PalletizerBusinessAbstract;
            this.DeviceControl = control as PalletierControlAbstract;
            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "PalletizerBusinessAbstract", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "PalletierControlAbstract", control.GetType().FullName);

                return OperateResult.CreateFailedResult(msg, 1);
            }
            return OperateResult.CreateSuccessResult();
        }

    }
}
