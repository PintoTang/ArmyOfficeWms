using System;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Load.ViewModel
{
    public class DeviceParamsRackPlaceViewModel : DeviceDataBase
    {
        #region 属性
      

        #endregion


        private LoadPlaceViewModel DeviceViewModel { get; set; }
        /// <summary>
        /// 上一个对象 传递过来的 DataContext
        /// </summary>
        /// <param name="PreDataContext"></param>
        public DeviceParamsRackPlaceViewModel(object PreDataContext)
        {
            DeviceViewModel = PreDataContext as LoadPlaceViewModel;
            XmlNodeName = "RackPlaces";
            DeviceId = DeviceViewModel.Id;

            LoadXml_templateTwo();
        }

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


        /// <summary>
        /// 保存 （1、本地内存和xml）
        /// </summary>
        /// <param name="vmObj">对象ViewModel</param>
        public OperateResult Save(object vmObj)
        {
            OperateResult opResult = OperateResult.CreateFailedResult();
            try
            {
                SaveXml_templateTwo();
                opResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = ex.ToString();
            }
            return opResult;
        }

    }
}
