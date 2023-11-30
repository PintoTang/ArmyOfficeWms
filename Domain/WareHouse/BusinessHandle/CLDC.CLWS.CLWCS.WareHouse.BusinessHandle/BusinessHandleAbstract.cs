using System;
using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.OrderHandle;

namespace CLDC.CLWS.CLWCS.WareHouse.BusinessHandle
{
    /// <summary>
    /// 业务处理虚拟类 主要处理各个项目的业务逻辑
    /// </summary>
    public abstract class BusinessHandleAbstract : IHandleOrderExcuteStatus
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 处理指令
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="order">指令</param>
        /// <param name="type">类型</param>
        /// <returns>OperateResult</returns>
        public abstract OperateResult HandleOrderChange(DeviceName deviceName, ExOrder order, TaskHandleResultEnum type);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public OperateResult Initilize()
        {
            OperateResult initilizeResult = OperateResult.CreateFailedResult();
            try
            {
                OperateResult initilizeConfigResult = InitilizeConfig();
                if (!initilizeConfigResult.IsSuccess)
                {
                    initilizeResult.IsSuccess = false;
                    initilizeResult.Message = initilizeConfigResult.Message;
                    return initilizeResult;
                }
                OperateResult particularInitlizeResult = ParticularInitlize();
                if (!particularInitlizeResult.IsSuccess)
                {
                    return particularInitlizeResult;
                }
              return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                initilizeResult.IsSuccess = false;
                initilizeResult.Message = OperateResult.ConvertException(ex);
            }
            return initilizeResult;
        }
        /// <summary>
        /// ParticularInitlize
        /// </summary>
        /// <returns></returns>
        protected abstract OperateResult ParticularInitlize();
        /// <summary>
        /// InitilizeConfig
        /// </summary>
        /// <returns></returns>
        protected abstract OperateResult InitilizeConfig();
        /// <summary>
        /// 获取调试助手
        /// </summary>
        /// <returns></returns>
        public abstract Window GetAssistantView();
        /// <summary>
        /// 获取显示界面
        /// </summary>
        /// <returns></returns>
        public abstract UserControl GetDetailView();

    }
}
