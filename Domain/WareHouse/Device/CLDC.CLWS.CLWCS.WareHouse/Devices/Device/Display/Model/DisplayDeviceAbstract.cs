using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Display;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.Model
{
    /// <summary>
    /// 显示设备
    /// </summary>
    public abstract class DisplayDeviceAbstract : DeviceBaseAbstract
    {
     

        private DisplayDeviceProperty _deviceProperty = new DisplayDeviceProperty();

        public DisplayDeviceProperty DeviceProperty
        {
            get
            {
                return _deviceProperty;
            }
            set { _deviceProperty = value; }
        }


        protected DisplayDeviceControlAbstract DeviceControl;
        protected DisplayDeviceBusinessAbstract DeviceBusiness;
        public override OperateResult ParticularInitlize(DeviceBusinessBaseAbstract business, DeviceControlBaseAbstract control)
        {
            DeviceBusiness = business as DisplayDeviceBusinessAbstract;
            DeviceControl = control as DisplayDeviceControlAbstract;
            if (DeviceControl == null)
            {
                string msg = string.Format("特定初始化出错，期待的控制类：DisplayDeviceControlAbstract 传入的类：{0}",
                    control.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            if (DeviceBusiness == null)
            {
                string msg = string.Format("特定初始化出错，期待的控制类：DisplayDeviceBusinessAbstract 传入的类：{0}",
                   control.GetType().FullName);
                return OperateResult.CreateFailedResult(msg, 1);
            }
            return OperateResult.CreateSuccessResult();
        }


        private bool _isNeedClearScreen = true;
        /// <summary>
        /// 是否需要清屏
        /// </summary>
        public bool IsNeedClearScreen { get; set; }

        private float _clearScreenInterval = 2.0f;
        /// <summary>
        /// 清屏的间隔，单位分钟
        /// </summary>
        public float ClearScreenInterval { get; set; }

        /// <summary>
        /// 屏幕默认显示信息
        /// </summary>
        public string DefaultContent
        {
            get { return _defaultContent; }
            set 
            { 
                _defaultContent = value;
                RaisePropertyChanged("DefaultContent");
            }
        }

        /// <summary>
        /// 当前显示的内容
        /// </summary>
        public string CurDisplayContent
        {
            get { return _curDisplayContent; }
            set { _curDisplayContent = value; }
        }

        /// <summary>
        /// 当前的标题
        /// </summary>
        public string CurTitleContent
        {
            get { return _curTitleContent; }
            set 
            { 
                _curTitleContent = value;
                RaisePropertyChanged("CurTitleContent");
            }
        }
        public string GetShowTitleContent
        {
            get
            {
                if (DeviceControl == null || !string.IsNullOrEmpty(DeviceControl.TitleContent)) return "鄂尔多斯1号库";
                return DeviceControl.TitleContent;
            }
        }

        private string _curTitleContent = string.Empty;

        private string _defaultContent = string.Empty;


        private string _curDisplayContent = string.Empty;

        public abstract OperateResult SendContent(string contentMsg);

        public abstract OperateResult SendTitle(string title);

        public abstract OperateResult ClearScreen();

        public override OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            return DeviceBusiness.ComputeNextAddr(destAddr);
        }

    }
}
