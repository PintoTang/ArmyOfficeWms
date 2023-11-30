using System;
using System.Collections.Generic;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ExOrderSingalViewModel : ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ExOrder DataModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exOrder"></param>
        public ExOrderSingalViewModel(ExOrder exOrder)
        {
            DataModel = exOrder;
        }

        private readonly Dictionary<StatusEnum, string> _dicRunningStatus = new Dictionary<StatusEnum, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<StatusEnum, string> DicRunningStatus
        {
            get
            {
                if (_dicRunningStatus.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(StatusEnum)))
                    {
                        Enum em = (StatusEnum)Enum.Parse(typeof(StatusEnum), strName);
                        _dicRunningStatus.Add((StatusEnum)em, em.GetDescription());
                    }
                }
                return _dicRunningStatus;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }

        private readonly Dictionary<OrderTypeEnum, string> _dicTypeEnum = new Dictionary<OrderTypeEnum, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<OrderTypeEnum, string> DicTypeEnum
        {
            get
            {
                if (_dicTypeEnum.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(OrderTypeEnum)))
                    {
                        Enum em = (OrderTypeEnum)Enum.Parse(typeof(OrderTypeEnum), strName);
                        _dicTypeEnum.Add((OrderTypeEnum)em, em.GetDescription());
                    }
                }
                return _dicTypeEnum;
            }
        }


        private readonly Dictionary<FinishType, string> _dicFinishTypeEnum = new Dictionary<FinishType, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<FinishType, string> DicFinishTypeEnum
        {
            get
            {
                if (_dicFinishTypeEnum.Count == 0)
                {
                    foreach (string strName in System.Enum.GetNames(typeof(FinishType)))
                    {
                        Enum em = (FinishType)Enum.Parse(typeof(FinishType), strName);
                        _dicFinishTypeEnum.Add((FinishType)em, em.GetDescription());
                    }
                }
                return _dicFinishTypeEnum;
            }
        }
        

    }
}
