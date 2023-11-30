using System;
using System.Windows;
using System.Windows.Controls;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWCS.Service.DataService
{
    public abstract class WcsDataServiceAbstract : IManageable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NameSpace { get; set; }

        public string ClassName { get; set; }

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
                return OperateResult.CreateSuccessResult();
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
