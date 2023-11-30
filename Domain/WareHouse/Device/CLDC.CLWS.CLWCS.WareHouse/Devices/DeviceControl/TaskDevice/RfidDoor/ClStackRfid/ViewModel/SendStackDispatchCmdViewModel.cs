using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.ViewModel
{
    public class SendStackDispatchCmdViewModel : ViewModelBase, IInvokeCmd
    {
        public SendStackDispatchCmd DataModel { get; set; }
       public SendStackDispatchCmdViewModel(SendStackDispatchCmd dataModel)
       {
           DataModel = dataModel;
       }

       private Dictionary<StackPositionEnum, string> _dicPositionEnumList = new Dictionary<StackPositionEnum, string>();
       /// <summary>
       /// 地址集合
       /// </summary>
       public Dictionary<StackPositionEnum, string> DicPositionEnumList
       {
           get
           {
               if (_dicPositionEnumList.Count == 0)
               {
                   foreach (var value in Enum.GetValues(typeof(StackPositionEnum)))
                   {
                       StackPositionEnum em = (StackPositionEnum)value;
                       _dicPositionEnumList.Add(em, em.GetDescription());
                   }
               }
               return _dicPositionEnumList;
           }
           set
           {
               _dicPositionEnumList = value;
           }
       }
        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
