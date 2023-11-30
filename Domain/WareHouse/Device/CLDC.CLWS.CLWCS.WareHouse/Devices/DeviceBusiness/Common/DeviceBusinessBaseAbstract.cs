using System;
using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.ConfigManagerPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceBusiness.Common
{
    /// <summary>
    /// 设备业务控制的基类
    /// </summary>
    public abstract class DeviceBusinessBaseAbstract
    {

        public Action<string, EnumLogLevel, bool> LogMessageAction { get; set; }
        public void LogMessage(string msg, EnumLogLevel level, bool isNotifyUi)
        {
            if (LogMessageAction != null)
            {
                LogMessageAction(msg, level, isNotifyUi);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        internal OperateResult Initialize(int deviceId, DeviceName deviceName)
        {
            this.DeviceId = deviceId;
            this.DeviceName = deviceName;



            OperateResult initParticularResult = ParticularInitlize();
            if (!initParticularResult.IsSuccess)
            {
                return initParticularResult;
            }
            OperateResult initConfigResult = InitConfig();
            if (!initConfigResult.IsSuccess)
            {
                return initConfigResult;
            }

            OperateResult initStartResult = Start();
            if (!initStartResult.IsSuccess)
            {
                return initStartResult;
            }

            return OperateResult.CreateSuccessResult();
        }
        public  abstract OperateResult ParticularInitlize();

        protected Dictionary<Addr, List<string>> nextAddrDic = new Dictionary<Addr, List<string>>();
        private OperateResult InitConfig()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            OperateResult<Dictionary<Addr, List<string>>> getNextAddrResult = NextAddrConfig.GetNextAddrDicByWorkerName(DeviceName.FullName);
            if (!getNextAddrResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.Message = getNextAddrResult.Message;
                return result;
            }
            nextAddrDic = getNextAddrResult.Content;

            return ParticularConfig();
        }

        public  abstract OperateResult ParticularConfig();

        public abstract OperateResult Start();


        /// <summary>
        /// 设备标识名
        /// </summary>
        public DeviceName DeviceName { get; set; }

        public string ClassName { get; set; }

        public string NameSpace { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public virtual OperateResult<List<Addr>> ComputeNextAddr(Addr destAddr)
        {
            List<Addr> nextAddr = new List<Addr>();
            OperateResult<List<Addr>> result = OperateResult.CreateFailedResult(nextAddr, "无数据");
            try
            {
                bool isHas = false;
                foreach (Addr keyAddr in nextAddrDic.Keys)
                {
                    if (keyAddr.IsContain(destAddr))
                    {
                        foreach (string addr in nextAddrDic[keyAddr])
                        {
                            nextAddr.Add(new Addr(addr));
                        }
                        isHas = true;
                    }
                }
                if (!isHas)
                {
                    result.Message = string.Format("根据目的地址：{0} 没有找到转换的下步地址，目标地址为下步地址", destAddr);
                    nextAddr.Add(destAddr);
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }


    }
}
