using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Swith.Model;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.Model;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.View;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.SwitchingWorker.ViewModel;
using CLDC.Infrastructrue.Xml;
using CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode;
using CLWCS.WareHouse.Device.HeFei;
using Newtonsoft.Json;

namespace CLWCS.WareHouse.Worker.HeFei
{
    public class ClSwitchingNotifyWmsErrrorWorker : SwtichingWorkerAbstract<SwitchingWorkerBusinessAbstract>
    {

        private SwitchingWorkerProperty _workerProperty = new SwitchingWorkerProperty();

        public SwitchingWorkerProperty WorkerProperty
        {
            get { return _workerProperty; }
            set { _workerProperty = value; }
        }

        public override OperateResult SwithValueChangeHandle(object sender,int newValue)
        {
            string deviceName = this.Id == 900101 ? "1#堆垛机 " : "2#堆垛机 ";
            if (newValue <=0)
            {
                LogMessage(string.Format("收到就绪：{0}，非小于等于0不做处理", newValue), EnumLogLevel.Info, true);
                return OperateResult.CreateFailedResult();
            }
            NotifyReportTroubleStatusCmdMode mode = new NotifyReportTroubleStatusCmdMode();
            mode.WarnType = 2;
            switch (newValue)
            {
                case 1:
                    mode.MESSAGE = deviceName + "接收任务不完整";
                    mode.HandlingSuggest = "检查通讯数据";
                    break;
                case 2:
                    mode.MESSAGE = deviceName + "任务类型错误";
                    mode.HandlingSuggest = "检查通讯数据";
                    break;
                case 3:
                    mode.MESSAGE = deviceName + "入口信息错误";
                    mode.HandlingSuggest = "检查通讯数据";
                    break;
                case 4:
                    mode.MESSAGE = deviceName + "出口信息错误";
                    mode.HandlingSuggest = "检查通讯数据";
                    break;
                case 5:
                    mode.MESSAGE = deviceName + "库位信息错误";
                    mode.HandlingSuggest = "检查通讯数据";
                    break;
                case 6:
                    mode.MESSAGE = deviceName + "空闲时货叉不在原点";
                    mode.HandlingSuggest = "检查货叉原点";
                    break;
                case 7:
                    mode.MESSAGE = deviceName + "行走时货叉不在原点";
                    mode.HandlingSuggest = "检查货叉原点";
                    break;
                case 8:
                    mode.MESSAGE = deviceName + "货物左坍塌";
                    mode.HandlingSuggest = "检查货物外形";
                    break;
                case 9:
                    mode.MESSAGE = deviceName + "货物右坍塌";
                    mode.HandlingSuggest = "检查货物外形";
                    break;
                case 10:
                    mode.MESSAGE = deviceName + "取货时货叉有货";
                    mode.HandlingSuggest = "检查有货光电，核对是否有货";
                    break;
                case 11:
                    mode.MESSAGE = deviceName + "取货后货叉无货";
                    mode.HandlingSuggest = "检查有货光电，核对是否无货";
                    break;
                case 12:
                    mode.MESSAGE = deviceName + "货物超宽";
                    mode.HandlingSuggest = "检查货物外形";
                    break;
                case 13:
                    mode.MESSAGE = deviceName + "货物超高";
                    mode.HandlingSuggest = "检查货物外形";
                    break;
                case 14:
                    mode.MESSAGE = deviceName + "放货重复";
                    mode.HandlingSuggest = "检查弹货光电";
                    break;
                case 15:
                    mode.MESSAGE = deviceName + "放货时货叉无货";
                    mode.HandlingSuggest = "检查有货光电，核对是否无货";
                    break;
                case 16:
                    mode.MESSAGE = deviceName + "放货后货叉有货";
                    mode.HandlingSuggest = "检查有货光电，核对是否有货";
                    break;
                case 17:
                    mode.MESSAGE = deviceName + "安全门打开";
                    mode.HandlingSuggest = "检查安全门是否关闭";
                    break;
                case 18:
                    mode.MESSAGE = deviceName + "堆垛机急停";
                    mode.HandlingSuggest = "检查堆垛机是否急停";
                    break;
                case 19:
                    mode.MESSAGE = deviceName + "维修模式";
                    mode.HandlingSuggest = "检查堆垛机是否维修";
                    break;
                case 20:
                    mode.MESSAGE = deviceName + "行走轴运行超时报警";
                    mode.HandlingSuggest = "检查运行电机变频器和轨道";
                    break;

                case 21:
                    mode.MESSAGE = deviceName + "行走轴松绳报警";
                    mode.HandlingSuggest = "行走轴松绳报警";
                    break;
                case 22:
                    mode.MESSAGE = deviceName + "行走轴运行时货叉不在中位";
                    mode.HandlingSuggest = "检查货叉原点";
                    break;
                case 23:
                    mode.MESSAGE = deviceName + "行走轴正软限位报警";
                    mode.HandlingSuggest = "检查堆垛机行走轴的位置";
                    break;
                case 24:
                    mode.MESSAGE = deviceName + "行走轴负软限位报警";
                    mode.HandlingSuggest = "检查堆垛机行走轴的位置";
                    break;
                case 25:
                    mode.MESSAGE = deviceName + "行走轴正限位报警";
                    mode.HandlingSuggest = "检查堆垛机行走轴的位置";
                    break;
                case 26:
                    mode.MESSAGE = deviceName + "行走轴负限位报警";
                    mode.HandlingSuggest = "检查堆垛机行走轴的位置";
                    break;
                case 27:
                    mode.MESSAGE = deviceName + "行走轴变频器故障";
                    mode.HandlingSuggest = "检查堆垛机行走变频器";
                    break;
                case 28:
                    mode.MESSAGE = deviceName + "行走轴目标值超限位";
                    mode.HandlingSuggest = "检查目标值";
                    break;
                case 29:
                    mode.MESSAGE = deviceName + "行走轴速度反馈异常";
                    mode.HandlingSuggest = "检查行走速度";
                    break;
                case 30:
                    mode.MESSAGE = deviceName + "升降轴运行超时报警";
                    mode.HandlingSuggest = "检查升降电机变频器和轨道";
                    break;

                case 31:
                    mode.MESSAGE = deviceName + "升降轴松绳报警";
                    mode.HandlingSuggest = "升降轴松绳报警";
                    break;
                case 32:
                    mode.MESSAGE = deviceName + "升降轴干涉报警";
                    mode.HandlingSuggest = "检查货叉原点";
                    break;
                case 33:
                    mode.MESSAGE = deviceName + "升降轴正软限位报警";
                    mode.HandlingSuggest = "检查堆垛机升降轴的位置";
                    break;
                case 34:
                    mode.MESSAGE = deviceName + "升降轴负软限位报警";
                    mode.HandlingSuggest = "检查堆垛机升降轴的位置";
                    break;
                case 35:
                    mode.MESSAGE = deviceName + "升降轴正限位报警";
                    mode.HandlingSuggest = "检查堆垛机升降轴的位置";
                    break;
                case 36:
                    mode.MESSAGE = deviceName + "升降轴负限位报警";
                    mode.HandlingSuggest = "检查堆垛机升降轴的位置";
                    break;
                case 37:
                    mode.MESSAGE = deviceName + "升降轴变频器故障";
                    mode.HandlingSuggest = "检查堆垛机升降变频器";
                    break;
                case 38:
                    mode.MESSAGE = deviceName + "升降轴目标值超限位";
                    mode.HandlingSuggest = "检查目标值";
                    break;
                case 39:
                    mode.MESSAGE = deviceName + "升降轴速度反馈异常";
                    mode.HandlingSuggest = "检查行走速度";
                    break;
                case 40:
                    mode.MESSAGE = deviceName + "货叉运行超时报警";
                    mode.HandlingSuggest = "检查货叉电机变频器和轨道";
                    break;

                case 41:
                    mode.MESSAGE = deviceName + "货叉松绳报警";
                    mode.HandlingSuggest = "货叉轴松绳报警";
                    break;
                case 42:
                    mode.MESSAGE = deviceName + "货叉干涉报警";
                    mode.HandlingSuggest = "检查货叉原点";
                    break;
                case 43:
                    mode.MESSAGE = deviceName + "货叉正软限位报警";
                    mode.HandlingSuggest = "检查堆垛机货叉轴的位置";
                    break;
                case 44:
                    mode.MESSAGE = deviceName + "货叉负软限位报警";
                    mode.HandlingSuggest = "检查堆垛机货叉轴的位置";
                    break;
                case 45:
                    mode.MESSAGE = deviceName + "货叉正限位报警";
                    mode.HandlingSuggest = "检查堆垛机货叉轴的位置";
                    break;
                case 46:
                    mode.MESSAGE = deviceName + "货叉负限位报警";
                    mode.HandlingSuggest = "检查堆垛机货叉轴的位置";
                    break;
                case 47:
                    mode.MESSAGE = deviceName + "货叉变频器故障";
                    mode.HandlingSuggest = "检查堆垛机货叉变频器";
                    break;
                case 48:
                    mode.MESSAGE = deviceName + "货叉目标值超限位";
                    mode.HandlingSuggest = "检查目标值";
                    break;  
                case 49:
                    mode.MESSAGE = deviceName + "货叉速度反馈异常";
                    mode.HandlingSuggest = "检查货叉速度";
                    break;
                case 50:
                    mode.MESSAGE = deviceName + "货叉动作时行走升降轴不在目标范围内";
                    mode.HandlingSuggest = "检查升降是否移动";
                    break;

                case 51:
                    mode.MESSAGE = deviceName + "升降动作时行走和货叉不在目标范围内";
                    mode.HandlingSuggest = "检查货叉是否移动";
                    break;
                case 52:
                    mode.MESSAGE = deviceName + "升降距离超出范围";
                    mode.HandlingSuggest = "检查升降距离";
                    break;
                case 53:
                    mode.MESSAGE = deviceName + "取深仓位时浅仓位有货";
                    mode.HandlingSuggest = "检查浅仓位是否有货，或者探货光电";
                    break;
                case 54:
                    mode.MESSAGE = deviceName + "超高异常";
                    mode.HandlingSuggest = "检查货物外形高度";
                    break;
                case 55:
                    mode.MESSAGE = deviceName + "前超长";
                    mode.HandlingSuggest = "检查货物外形前超出";
                    break;
                case 56:
                    mode.MESSAGE = deviceName + "后超出";
                    mode.HandlingSuggest = "检查货物外形后超出"; 
                    break;
                case 57:
                    mode.MESSAGE = deviceName + "货叉放货时行走升降轴位置偏移过大";
                    mode.HandlingSuggest = "检查行走升降位置";
                    break;
                case 58:
                    mode.MESSAGE = deviceName + "输送机长时间未给允许取货信号";
                    mode.HandlingSuggest = "检查线体是否自动、站台是否有货和光电";
                    break;
                case 59:
                    mode.MESSAGE = deviceName + "输送机长时间未反馈取货完成信号";
                    mode.HandlingSuggest = "检查线体是否自动、站台是否有货和光电";
                    break;
                case 60:
                    mode.MESSAGE = deviceName + "输送机长时间未给允许放货信号";
                    mode.HandlingSuggest = "检查线体是否自动、站台是否有货和光电";
                    break;
                case 61:
                    mode.MESSAGE = deviceName + "输送机长时间未反馈放货完成信号";
                    mode.HandlingSuggest = "检查线体是否自动、站台是否有货和光电";
                    break;
                case 62:
                    mode.MESSAGE = deviceName + "货叉取货时行走升降轴位置偏移过大";
                    mode.HandlingSuggest = "检查货叉位置";
                    break;
                case 63:
                    mode.MESSAGE = deviceName + "和输送线通讯中断";
                    mode.HandlingSuggest = "检查和输送线通讯";
                    break;
            }
            return NotifyWmsReportTroubleStatus(mode);

            //return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 设备异常信息上报
        /// </summary>
        /// <param name="deviceErrMsg"></param>
        /// <returns></returns>
        private OperateResult NotifyWmsReportTroubleStatus(NotifyReportTroubleStatusCmdMode errMode)
        {
            string cmdPara = JsonConvert.SerializeObject(errMode);
            string interFaceName = "ReportTroubleStatus";
            NotifyElement element = new NotifyElement("", interFaceName, "上报设备异常信息WMS", null, cmdPara);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            OperateResult<object> result = UpperServiceHelper.WmsServiceInvoke(element);
            sw.Stop();
            LogMessage("wcs调用WMS ReportTroubleStatus 接口 耗时：" + sw.ElapsedMilliseconds + "毫秒", EnumLogLevel.Info, false);

          
            if (!result.IsSuccess)
            {
                string msg = string.Format("调用上层接口{0}失败，详情：\r\n {1}", interFaceName, result.Message);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            else
            {
                try
                {
                    CmdReturnMessageHeFei serviceReturn = (CmdReturnMessageHeFei)result.Content.ToString();
                    if (serviceReturn.IsSuccess)
                    {
                        return OperateResult.CreateSuccessResult("调用接口 " + interFaceName + "成功，并且WMS返回成功信息");
                    }
                    else
                    {
                        return OperateResult.CreateSuccessResult(string.Format("调用接口  " + interFaceName + " 成功，但是WMS返回失败，失败信息：{0}", serviceReturn.MESSAGE));
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            return result;
        }


        protected override OperateResult ParticularStart()
        {
            OperateResult registeResult = RegisteHandler();
            if (!registeResult.IsSuccess)
            {
                return registeResult;
            }
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
                        WorkerProperty = (SwitchingWorkerProperty)XmlSerializerHelper.DeserializeFromTextReader(sr, typeof(SwitchingWorkerProperty));
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
        private OperateResult RegisteHandler()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                foreach (AssistantDevice assistant in WorkerAssistants)
                {
                    if (assistant.Device.DeviceType.Equals(DeviceTypeEnum.SwitchDevice))
                    {
                        SwitchDeviceAbstract stationDevice = assistant.Device as SwitchDeviceAbstract;
                        if (stationDevice != null)
                        {
                            stationDevice.SwithValueChangeEvent += SwithValueChangeHandle;
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
            SwitchingWorkerDeviceViewModel viewModel = new SwitchingWorkerDeviceViewModel(this);
            viewModelResult.Content = viewModel;
            viewModelResult.IsSuccess = true;
            viewModelResult.ErrorCode = 0;
            return viewModelResult;

        }

        protected override Window CreateConfigView()
        {
            Window configView = new WorkerConfigView();
            WorkerConfigViewModel<ClSwitchingNotifyWmsErrrorWorker, SwitchingWorkerProperty> viewModel = new WorkerConfigViewModel<ClSwitchingNotifyWmsErrrorWorker, SwitchingWorkerProperty>(this, WorkerProperty);
            configView.DataContext = viewModel;
            return configView;
        }

        protected override OperateResult UpdateProperty()
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
            UCSwitchingWorkerDeviceView view = new UCSwitchingWorkerDeviceView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateDetailView()
        {
            OperateResult<UserControl> result = new OperateResult<UserControl>();
            SwitchingWorkerDetailView view = new SwitchingWorkerDetailView();
            result.IsSuccess = true;
            result.Content = view;
            return result;
        }

        protected override OperateResult<UserControl> CreateMonitorView()
        {
            UserControl view = new UserControl();
            return OperateResult.CreateFailedResult(view, "无界面");
        }
    }
}
