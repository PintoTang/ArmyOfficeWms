using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.ViewModel
{
    public class SendSwitchTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {
       public SendSwitchTaskCmd DataModel { get; set; }
       public SendSwitchTaskCmdViewModel(SendSwitchTaskCmd dataModel)
       {
           DataModel = dataModel;
       }
       private Dictionary<DoorActionTypeEnum, string> _dicDoorActionTypeList = new Dictionary<DoorActionTypeEnum, string>();
         /// <summary>
         /// 是否需要请求地址集合
         /// </summary>
       public Dictionary<DoorActionTypeEnum, string> DicDoorActionTypeList
         {
             get
             {
                 if (_dicDoorActionTypeList.Count == 0)
                 {
                     foreach (var value in Enum.GetValues(typeof(DoorActionTypeEnum)))
                     {
                         DoorActionTypeEnum em = (DoorActionTypeEnum)value;
                         _dicDoorActionTypeList.Add(em, em.GetDescription());
                     }
                 }
                 return _dicDoorActionTypeList;
             }
             set
             {
                 _dicDoorActionTypeList = value;
             }
         }
        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
