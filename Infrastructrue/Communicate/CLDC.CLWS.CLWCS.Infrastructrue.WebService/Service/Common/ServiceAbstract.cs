using System;
using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common
{
    /// <summary>
    /// Webservice的虚类
    /// </summary>
    public abstract class ServiceAbstract:IManageable
    {
        /// <summary>
        /// Webservice编号
        /// </summary>
        public int WebserviceId { get; set; }

        public string NameSpace { get; set; }

        public string ClassName { get; set; }

        public string ServiceType { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsShowUi { get; set; }

        public PackIconKind IconKind { get; set; }

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
                OperateResult startResult = StartService();
                return startResult;

            }
            catch (Exception ex)
            {
                initilizeResult.IsSuccess = false;
                initilizeResult.Message = OperateResult.ConvertException(ex);
            }
            return initilizeResult;
        }

        protected abstract OperateResult ParticularInitlize();

        protected abstract OperateResult InitilizeConfig();

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        protected abstract OperateResult StartService();
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public abstract OperateResult StopService();

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
