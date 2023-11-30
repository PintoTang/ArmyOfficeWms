using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.Model;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.ViewModel
{
   public class OrderAddCmdViewModel : ViewModelBase, IInvokeCmd
    {
       public OrderAddCmd DataModel { get; set; }

         public OrderAddCmdViewModel(OrderAddCmd dataModel)
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
            List<OrderAddCmd> orderAddCmdList=new List<OrderAddCmd>();
            orderAddCmdList.Add(DataModel);
            return orderAddCmdList.ToJson();
        }
    }
}
