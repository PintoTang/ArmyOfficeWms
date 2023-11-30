using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.ViewModel
{
   public class SendTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {
         public SendTaskCmd DataModel { get; set; }

         public SendTaskCmdViewModel(SendTaskCmd dataModel)
        {
            DataModel = dataModel;
        }
         private Dictionary<RequestTypeEnum, string> _dicRequestTypeList = new Dictionary<RequestTypeEnum, string>();
         /// <summary>
         /// 是否需要请求地址集合
         /// </summary>
         public Dictionary<RequestTypeEnum, string> DicRequestTypeList
         {
             get
             {
                 if (_dicRequestTypeList.Count == 0)
                 {
                     foreach (var value in Enum.GetValues(typeof(RequestTypeEnum)))
                     {
                         RequestTypeEnum em = (RequestTypeEnum)value;
                         _dicRequestTypeList.Add(em, em.GetDescription());
                     }
                 }
                 return _dicRequestTypeList;
             }
             set
             {
                 _dicRequestTypeList = value;
             }
         }
        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
