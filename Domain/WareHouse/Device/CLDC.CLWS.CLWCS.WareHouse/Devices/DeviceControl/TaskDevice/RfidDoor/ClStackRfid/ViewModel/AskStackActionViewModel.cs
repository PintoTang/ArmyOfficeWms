using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.CmdModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.ClStackRfid.ViewModel
{
    public class AskStackActionViewModel : ViewModelBase, IInvokeCmd
    {
        public AskStackActionCmd DataModel { get; set; }
        public AskStackActionViewModel(AskStackActionCmd dataModel)
       {
           DataModel = dataModel;
       }
        private Dictionary<StackPutPick, string> _dicBufferActionTypeList = new Dictionary<StackPutPick, string>();
         /// <summary>
         /// 是否需要请求地址集合
         /// </summary>
        public Dictionary<StackPutPick, string> DicBufferActionTypeList
         {
             get
             {
                 if (_dicBufferActionTypeList.Count == 0)
                 {
                     foreach (var value in Enum.GetValues(typeof(StackPutPick)))
                     {
                         StackPutPick em = (StackPutPick)value;
                         _dicBufferActionTypeList.Add(em, em.GetDescription());
                     }
                 }
                 return _dicBufferActionTypeList;
             }
             set
             {
                 _dicBufferActionTypeList = value;
             }
         }


        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
