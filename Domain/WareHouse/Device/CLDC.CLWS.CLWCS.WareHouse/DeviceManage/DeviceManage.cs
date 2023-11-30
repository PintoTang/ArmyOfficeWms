using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.ConfigService;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Common;
using CLDC.CLWS.CLWCS.WareHouse.ViewModel;
using CLDC.Framework.Log.Helper;
using CLDC.Infrastructrue.UserCtrl.View.ProgressBar;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DeviceManage
{
    public class DeviceManage : IManage<DeviceBaseAbstract>
    {
        private string _logName = "初始化设备";
        private static DeviceManage _deviceManage;

        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static DeviceManage Instance
        {
            get
            {
                if (_deviceManage == null)
                    _deviceManage = new DeviceManage();
                return _deviceManage;
            }
        }

      


        /// <summary>
        /// 初始化所有设备
        /// </summary>
        /// <returns></returns>
        public OperateResult InitDevices()
        {
            OperateResult initilizeResult=OperateResult.CreateFailedResult();
            try
            {
                string strCurPath = System.IO.Directory.GetCurrentDirectory();
                string deviceXmlPath = strCurPath + @"\Config" + @"\DeviceConfig.xml";

                //加载设备配置文件
                OperateResult deviceLoadResult = LoadConfig(deviceXmlPath);
                if (!deviceLoadResult.IsSuccess)
                {
                    initilizeResult.Message = string.Format("加载设备失败，原因：\r\n{0}", deviceLoadResult.Message);
                    initilizeResult.IsSuccess = false;
                    return initilizeResult;
                }

                return AddDeviceToDeviceDataPool();
            }
            catch (Exception)
            {
                
                throw;
            }
          
        }

        /// <summary>
        /// 添加设备到  DeviceDataPool
        /// </summary>
        private OperateResult AddDeviceToDeviceDataPool()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            List<DeviceInfoConfig> deviceInfoLst = DeviceInfoConfigManage.Instance.GetAllDeviceInfoData();
            int deviceId = 0;
            try
            {
                double step = 0;
            
                int count = deviceInfoLst.Aggregate(0, (current, deviceInfo) => current + deviceInfo.DeviceLst.Count);
                double interval = Math.Ceiling(100.0 / count);
                foreach (var deviceInfo in deviceInfoLst)
                {
                    foreach (var deviceConfig in deviceInfo.DeviceLst)
                    {
                        ProgressBarEx.ReportProcess(step, string.Format("正在初始化设备：{0} ", deviceConfig.Name),"正在初始化设备，请稍后......");
                        step = step + interval;
                        LogHelper.WriteLog(_logName, string.Format("{0} 开始初始化设备{1} 命名空间：{2} 类名：{3}", DateTime.Now, deviceConfig.DeviceId.Trim(), deviceConfig.NameSpace, deviceConfig.Class), EnumLogLevel.Debug);
                        //int deviceId = 0;
                        deviceId = 0;
                        if (!string.IsNullOrEmpty(deviceConfig.DeviceId))
                        {
                            deviceId = int.Parse(deviceConfig.DeviceId.Trim());
                        }
                        else
                        {
                            return OperateResult.CreateFailedResult(string.Format("设备配置中:{0}，设备编号配置为空", deviceConfig.Name), 1);
                        }
                        DeviceBaseAbstract device = null;
                        DeviceBusinessBaseAbstract business = null;
                        DeviceControlBaseAbstract control = null;

                        if (!string.IsNullOrEmpty(deviceConfig.NameSpace) && !string.IsNullOrEmpty(deviceConfig.Class))
                        {
                            device = (DeviceBaseAbstract)Assembly.Load(deviceConfig.NameSpace).CreateInstance(deviceConfig.NameSpace + "." + deviceConfig.Class);
                            if (device == null)
                            {
                                string msg = string.Format("设备反射出错，命名空间：{0} 类名：{1} 设备编号：{2}", deviceConfig.NameSpace,
                                    deviceConfig.Class, deviceConfig.DeviceId);
                                return OperateResult.CreateFailedResult(msg, 1);
                            }
                            device.ClassName = deviceConfig.Class;
                            device.NameSpace = deviceConfig.NameSpace;
                            device.IsShowUi = deviceConfig.IsShowUi;
                        }
                        else
                        {
                            string msg = string.Format("设备配置中命名空间或者类名为空值，命名空间：{0} 类名：{1} 设备编号：{2}", deviceConfig.NameSpace, deviceConfig.Class, deviceConfig.DeviceId);
                            return OperateResult.CreateFailedResult(msg, 1);
                        }
                        if (!string.IsNullOrEmpty(deviceConfig.BusinessHandle.NameSpace) && !string.IsNullOrEmpty(deviceConfig.BusinessHandle.Class))
                        {
                            business = (DeviceBusinessBaseAbstract)Assembly.Load(deviceConfig.BusinessHandle.NameSpace)
                                .CreateInstance(deviceConfig.BusinessHandle.NameSpace + "." + deviceConfig.BusinessHandle.Class);
                            if (business == null)
                            {
                                string msg = string.Format("设备业务反射出错，命名空间：{0} 类名：{1} 设备编号：{2}", deviceConfig.BusinessHandle.NameSpace,
                                    deviceConfig.BusinessHandle.Class, deviceConfig.DeviceId);
                                return OperateResult.CreateFailedResult(msg, 1);
                            }
                            business.Name = deviceConfig.BusinessHandle.Name;
                            business.ClassName = deviceConfig.BusinessHandle.Class;
                            business.NameSpace = deviceConfig.BusinessHandle.NameSpace;
                        }
                        else
                        {
                            string msg = string.Format("设备业务配置中命名空间或者类名为空值，命名空间：{0} 类名：{1} 设备编号：{2}", deviceConfig.BusinessHandle.NameSpace,
    deviceConfig.BusinessHandle.Class, deviceConfig.DeviceId);
                            return OperateResult.CreateFailedResult(msg, 1);
                        }
                        if (!string.IsNullOrEmpty(deviceConfig.DeviceControlHandle.NameSpace) && !string.IsNullOrEmpty(deviceConfig.DeviceControlHandle.Class))
                        {
                            control = (DeviceControlBaseAbstract)Assembly.Load(deviceConfig.DeviceControlHandle.NameSpace)
                                    .CreateInstance(deviceConfig.DeviceControlHandle.NameSpace + "." + deviceConfig.DeviceControlHandle.Class);
                            if (control == null)
                            {
                                string msg = string.Format("设备控制反射出错，命名空间：{0} 类名：{1} 设备编号：{2}", deviceConfig.DeviceControlHandle.NameSpace, deviceConfig.DeviceControlHandle.Class, deviceConfig.DeviceId);
                                return OperateResult.CreateFailedResult(msg, 1);
                            }
                            control.Name = deviceConfig.DeviceControlHandle.Name;
                        }
                        else
                        {
                            string msg = string.Format("设备控制配置中命名空间或者类名为空值，命名空间：{0} 类名：{1} 设备编号：{2}", deviceConfig.DeviceControlHandle.NameSpace, deviceConfig.DeviceControlHandle.Class, deviceConfig.DeviceId);
                            return OperateResult.CreateFailedResult(msg, 1);
                        }

                        DeviceName deviceName = null;
                        string name = string.Empty;
                        DeviceTypeEnum deviceType = DeviceTypeEnum.LoadDevice;
                        Addr currAddress = null;
                        int workSize;
                        if (string.IsNullOrEmpty(deviceConfig.DeviceName))
                        {
                            string msg = string.Format("设备：{0} 设备名称(DeviceName)尚未配置", deviceId);
                            return OperateResult.CreateFailedResult(msg, 1);
                        }
                        deviceName = new DeviceName(deviceConfig.DeviceName);
                        if (string.IsNullOrEmpty(deviceConfig.Name))
                        {
                            string msg = string.Format("设备：{0} 设备名称(Name)尚未配置", deviceId);
                            return OperateResult.CreateFailedResult(msg, 1);
                        }
                        name = deviceConfig.Name;
                        if (string.IsNullOrEmpty(deviceInfo.DeviceType))
                        {
                            string msg = string.Format("设备：{0} 设备类别(DeviceType)尚未配置", deviceId);
                            return OperateResult.CreateFailedResult(msg, 1);
                        }
                        deviceType = (DeviceTypeEnum)Enum.Parse(typeof(DeviceTypeEnum), deviceInfo.DeviceType);

                        if (string.IsNullOrEmpty(deviceConfig.CurAddress))
                        {
                            string msg = string.Format("设备：{0} 设备当前地址(CurrAddress)尚未配置", deviceId);
                            return OperateResult.CreateFailedResult(msg, 1);

                        }
                        currAddress = new Addr(deviceConfig.CurAddress);
                        workSize = deviceConfig.WorkSize;

#if DEBUG
                        if (deviceId == 1001)
                        {

                        }
#endif
                        OperateResult deviceInit = device.Initialize(deviceId, deviceName, name, deviceType, currAddress, workSize, business, control);
                        if (!deviceInit.IsSuccess)
                        {
                            return OperateResult.CreateFailedResult(string.Format("设备：{0} 初始化出错 原因：\r\n {1}", device.Name, deviceInit.Message), 1);
                        }
                        OperateResult addDeviceResult = Add(device);
                        if (!addDeviceResult.IsSuccess)
                        {
                            string msg = string.Format("设备：{0} 添加到设备管理失败，原因：\r\n {1}", device.Name + device.Id, addDeviceResult.Message);
                            return OperateResult.CreateFailedResult(msg, 1); ;
                        }
                        OperateResult<WareHouseViewModelBase> createViewModelResult = device.CreateViewModel();
                        if (!createViewModelResult.IsSuccess)
                        {
                            string msg = string.Format("设备：{0} 添加到设备管理失败，原因：\r\n {1}", device.Name + device.Id, addDeviceResult.Message);
                            return OperateResult.CreateFailedResult(msg);
                        }
                        ViewModelManage.Instance.Add(createViewModelResult.Content);
                        LogHelper.WriteLog(_logName, string.Format("{0} 结束初始化设备{1} 命名空间：{2} 类名：{3}", DateTime.Now, deviceConfig.DeviceId.Trim(), deviceConfig.NameSpace, deviceConfig.Class), EnumLogLevel.Debug);
                        result.IsSuccess = true;
                        ProgressBarEx.ReportProcess(step, string.Format("结束初始化设备：{0} ", deviceConfig.Name));

                    }

                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
       
                result.ErrorCode = 1;
            }
            return result;
        }


        public OperateResult LoadConfig(string path)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {

                Context cc = new Context(new DeviceParent());
                cc.LoadXml(path);

                string strNodeParms = @"Configuration/Devices";
                cc.SetXmlNode(strNodeParms);
                cc.Request();
                if (cc.CurNextNodeInfo == null) return OperateResult.CreateFailedResult("加载失败 DeviceConfig.xml", 1);

                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
                result.ErrorCode = 1;
            }
            return result;
        }

        public List<DeviceBaseAbstract> GetAllData()
        {
            return _managedDataPool.DataPool;
        }

        public List<DeviceBaseAbstract> GetAllData(Predicate<DeviceBaseAbstract> prdicate)
        {
            return _managedDataPool.DataPool.FindAll(prdicate);
        }

        private readonly DataManageablePool<DeviceBaseAbstract> _managedDataPool = new DataManageablePool<DeviceBaseAbstract>();

        public DataManageablePool<DeviceBaseAbstract> ManagedDataPool
        {
            get { return _managedDataPool; }
        }

        public OperateResult Add(DeviceBaseAbstract device)
        {
            return _managedDataPool.AddPool(device);
        }

        public OperateResult Delete(int deviceId)
        {
            return _managedDataPool.RemovePool(deviceId);
        }

        public OperateResult Update(DeviceBaseAbstract device)
        {
            return _managedDataPool.UpdatePool(device);
        }

        public DeviceBaseAbstract FindDeivceByDeviceId(int deviceId)
        {
            OperateResult<DeviceBaseAbstract> findResult = _managedDataPool.FindData(deviceId);
            return findResult.Content;
        }

        public DeviceBaseAbstract FindDeviceByDeviceName(string deviceName)
        {
            OperateResult<DeviceBaseAbstract> findResult = _managedDataPool.FindData(d=>d.DeviceName.FullName.Equals(deviceName));
            return findResult.Content;
        }

        public DeviceBaseAbstract Find(Predicate<DeviceBaseAbstract> predicate)
        {
            OperateResult<DeviceBaseAbstract> findResult = _managedDataPool.FindData(predicate);
            return findResult.Content;
        }

        public List<DeviceBaseAbstract> FindDevicesByCurAddr(Addr curAddr)
        {
            OperateResult<List<DeviceBaseAbstract>> findResult = _managedDataPool.FindAllData(d => d.CurAddress.Equals(curAddr));
            if (findResult.IsSuccess)
            {
                return findResult.Content;
            }
            return new List<DeviceBaseAbstract>();
        }

        public List<DeviceBaseAbstract> FindDevicesByDeviceType(DeviceTypeEnum deviceType)
        {
            OperateResult<List<DeviceBaseAbstract>> findResult = _managedDataPool.FindAllData(d => d.DeviceType.Equals(deviceType));
            if (findResult.IsSuccess)
            {
                return findResult.Content;
            }
            return new List<DeviceBaseAbstract>();
        }

    }
}
