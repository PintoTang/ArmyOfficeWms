using System;
using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Identify;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Identify;
using Infrastructrue.Ioc.DependencyFactory;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Identify.Model
{
    /// <summary>
    /// 识别信息设备的虚拟类
    /// </summary>
    public abstract class IdentityDeviceAbstract<T> : DeviceBaseAbstract where T : new()
    {

        //1.监控是否能获取识别信息的信号
        //2.获取识别信息
        //3.对信息进行初步过滤及处理
        //4.信息上报前处理业务
        //5.信息上报
        //6.信息上报后处理业务
        /// <summary>
        ///  设备初始化
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <param name="business"></param>
        /// <param name="control"></param>
        /// <returns></returns>

        
        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            this.DeviceBusiness = business as IdentifyDeviceBusinessAbstract<T>;
            this.DeviceControl = control as IdentifyDeviceControlAbstract<T>;

            if (DeviceBusiness == null)
            {
                string msg = string.Format("设备业务转换出错，期待类型：{0} 目标类型：{1}", "IdentifyDeviceBusinessAbstract<T>", business.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceControl == null)
            {
                string msg = string.Format("设备控制转换出错，期待类型：{0} 目标类型：{1}", "IdentifyDeviceControlAbstract<T>", control.GetType().FullName);

                return OperateResult.CreateFailedResult(msg, 1);
            }
            DeviceControl.OnReceiveBarcodeEvent += HandleMsgFromControl;

            return OperateResult.CreateSuccessResult();
        }

        protected internal IdentifyDeviceBusinessAbstract<T> DeviceBusiness { get; set; }

        protected internal IdentifyDeviceControlAbstract<T> DeviceControl { get; set; }

        public override void RefreshDeviceState()
        {
           IsHasTask= CurrentLiveData.Lenght > 0;
        }

        private T _currentContent = new T();
        /// <summary>
        /// 当前处理业务成功的内容
        /// </summary>
        public T CurrentContent { get { return _currentContent; } set { _currentContent = value; } }

        private DataCloneablePool<LiveData> _currentLiveData = new DataCloneablePool<LiveData>();
        /// <summary>
        /// 设备当前处理的数据
        /// </summary>
        public DataCloneablePool<LiveData> CurrentLiveData
        {
            get { return _currentLiveData; }
            set
            {
                _currentLiveData = value;
            }
        }

        /// <summary>
        /// 验证是否与当前值重复
        /// </summary>
        /// <param name="toCompare"></param>
        /// <returns></returns>
        public abstract bool IsRepeated(T toCompare);

        /// <summary>
        /// 保存内容到数据库
        /// </summary>
        /// <param name="content"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public abstract OperateResult SaveContentToDatabase(T content, HandleStatusEnum status);

        /// <summary>
        /// 保存内容到内存
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract OperateResult SetCurrentContent(T content);

        /// <summary>
        /// 删除指定的实时数据
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract OperateResult DeleteLiveDataContent(LiveData content);

        /// <summary>
        /// 记录当前的内容 同时保存到内存和数据库
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public OperateResult RecordContent(T content)
        {
            OperateResult setResult = SetCurrentContent(content);
            if (!setResult.IsSuccess)
            {
                LogMessage(string.Format("保存当前信息到内存失败：{0} 当前信息：{1}", setResult.Message, content.ToJson()), EnumLogLevel.Error, true);
            }
            OperateResult saveResult = SaveContentToDatabase(content, HandleStatusEnum.UnFinished);
            if (!saveResult.IsSuccess)
            {
                LogMessage(string.Format("保存当前信息到数据库失败：{0} 当前信息：{1}", saveResult.Message, content.ToJson()), EnumLogLevel.Error, true);
            }
            return saveResult;
        }

        /// <summary>
        /// 异步触发读取条码
        /// </summary>
        /// <param name="para"></param>
        public void GetIdentifyMessageAsync(params object[] para)
        {
            DeviceControl.GetIdentityMessageAsync(para);
        }

        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }

        /// <summary>
        /// 主动下发命令获取信息
        /// </summary>
        /// <returns>根据具体的设备获取返回不同类型</returns>
        public OperateResult GetIdentifyMessage(params object[] para)
        {
            OperateResult<T> getMsgResult = DeviceControl.GetIdentifyMessage(para);
            if (!getMsgResult.IsSuccess)
            {
                string errorMsg = string.Format("获取信息失败：{0}", getMsgResult.Message);
                LogMessage(errorMsg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(errorMsg, 1);
            }
            if (IsRepeated(getMsgResult.Content))
            {
                string repeatMsg = string.Format("获取到信息：{0}，信息与当前记录信息重复，不上报信息", getMsgResult.Content);
                LogMessage(repeatMsg, EnumLogLevel.Info, true);
                return OperateResult.CreateSuccessResult();
            }
            OperateResult handlerResult = IdentifyMsgHandler(getMsgResult.Content);
            if (!handlerResult.IsSuccess)
            {
                string errorMsg = string.Format("处理获取到的信息业务处理失败！\r\n 原因：{0}", handlerResult.Message);
                LogMessage(errorMsg, EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult(errorMsg, 1);
            }
            RecordContent(getMsgResult.Content);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 主动下发命令获取信息 并同步返回结果
        /// </summary>
        /// <returns>根据具体的设备获取返回不同类型</returns>
        public OperateResult<T> GetIdentifyMessageSync(params object[] para)
        {
            return DeviceControl.GetIdentifyMessage(para);
        }

        /// <summary>
        /// 获取到的身份信息处理
        /// </summary>
        /// <param name="identifyMsg"></param>
        private OperateResult IdentifyMsgHandler(T identifyMsg)
        {
            OperateResult<T> filterResult = IdentifyMsgFilter(identifyMsg);
            if (!filterResult.IsSuccess)
            {
                LogMessage(string.Format("信息过滤失败：{0}", filterResult.Message), EnumLogLevel.Error, true);
                return OperateResult.CreateFailedResult();
            }

            if (IsRepeated(filterResult.Content))
            {
                string repeatMsg = string.Format("获取到信息：{0}，信息与当前记录信息重复，不上报信息", filterResult.Content.ToJson());
                LogMessage(repeatMsg, EnumLogLevel.Info, true);
                return OperateResult.CreateSuccessResult();
            }

            OperateResult<T> beforeBusinessResult = BeforeNotifyIdetifyMsg(filterResult.Content);
            if (!beforeBusinessResult.IsSuccess)
            {
                LogMessage(string.Format("信息上报前业务处理失败：{0}", beforeBusinessResult.Message), EnumLogLevel.Error, true);
                return beforeBusinessResult;
            }

            OperateResult reportResult = NotifyIdetifyMsg(beforeBusinessResult.Content);
            if (!reportResult.IsSuccess)
            {
                return reportResult;
            }
            RecordContent(filterResult.Content);

            OperateResult afterBusinessResult = AfterNotifyIdetifyMsg(filterResult.Content);
            if (!beforeBusinessResult.IsSuccess)
            {
                LogMessage(string.Format("信息上报前业务处理失败：{0}", afterBusinessResult.Message), EnumLogLevel.Error, true);
                return beforeBusinessResult;
            }
            return OperateResult.CreateSuccessResult();
        }
        /// <summary>
        /// 上报身份信息前的业务处理
        /// </summary>
        /// <param name="identifyMsg"></param>
        private OperateResult<T> BeforeNotifyIdetifyMsg(T identifyMsg)
        {
            return DeviceBusiness.BeforeNotifyIdetifyMsg(identifyMsg);
        }

        /// <summary>
        /// 上报业务
        /// </summary>
        /// <param name="identifyMsg"></param>
        /// <param name="para"></param>
        private OperateResult NotifyIdetifyMsg(T identifyMsg, params object[] para)
        {
            if (IdentifyMsgCallbackHandler == null)
            {
                string msg = "收到条码枪上报的信息 但是IdentifyMsgCallbackHandler 尚未注册没往上报";
                return OperateResult.CreateFailedResult(msg, 1);
            }
            OperateResult callBackResult = IdentifyMsgCallbackHandler(this, identifyMsg, para);
            LogMessage(string.Format("上报信息结果：{0}", callBackResult.Message), EnumLogLevel.Info, true);
            return callBackResult;
        }

        /// <summary>
        /// 上报信息后的业务处理
        /// </summary>
        /// <param name="identifyMsg"></param>
        public virtual OperateResult AfterNotifyIdetifyMsg(T identifyMsg)
        {
            return DeviceBusiness.AfterNotifyIdetifyMsg(identifyMsg);
        }

        /// <summary>
        /// 获取到的身份信息进行过滤
        /// </summary>
        /// <param name="identifyMsg"></param>
        /// <returns></returns>
        private OperateResult<T> IdentifyMsgFilter(T identifyMsg)
        {
            return DeviceBusiness.IdentifyMsgFilter(identifyMsg);
        }
        /// <summary>
        /// 身份信息事件：上报设备名字、获取到的身份信息
        /// </summary>
        public Func<DeviceBaseAbstract, T, object[], OperateResult> IdentifyMsgCallbackHandler;

        private void HandleMsgFromControl(DeviceName deviceName, T barcode, params object[] para)
        {
            IdentifyMsgHandler(barcode);
        }

        public override OperateResult ParticularStart()
        {
            return OperateResult.CreateSuccessResult();
        }

    }
}
