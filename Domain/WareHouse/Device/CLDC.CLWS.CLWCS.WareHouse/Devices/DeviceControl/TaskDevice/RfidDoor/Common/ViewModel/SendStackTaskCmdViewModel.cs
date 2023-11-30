using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.ViewModel
{
    public class SendStackTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {
       public SendStackTaskCmd DataModel { get; set; }
       public SendStackTaskCmdViewModel(SendStackTaskCmd dataModel)
       {
           DataModel = dataModel;
       }

       private Dictionary<BoxTypeEnum, string> _dicBoxTypeList = new Dictionary<BoxTypeEnum, string>();
       /// <summary>
       /// 地址集合
       /// </summary>
       public Dictionary<BoxTypeEnum, string> DicBoxTypeList
       {
           get
           {
               if (_dicBoxTypeList.Count == 0)
               {
                   foreach (var value in Enum.GetValues(typeof(BoxTypeEnum)))
                   {
                       BoxTypeEnum em = (BoxTypeEnum)value;
                       _dicBoxTypeList.Add(em, em.GetDescription());
                   }
               }
               return _dicBoxTypeList;
           }
           set
           {
               _dicBoxTypeList = value;
           }
       }

       private Dictionary<StackActionTypeEnum, string> _dicStackActionTypeList = new Dictionary<StackActionTypeEnum, string>();
       /// <summary>
       /// 地址集合
       /// </summary>
       public Dictionary<StackActionTypeEnum, string> DicStackActionTypeList
       {
           get
           {
               if (_dicStackActionTypeList.Count == 0)
               {
                   foreach (var value in Enum.GetValues(typeof(StackActionTypeEnum)))
                   {
                       StackActionTypeEnum em = (StackActionTypeEnum)value;
                       _dicStackActionTypeList.Add(em, em.GetDescription());
                   }
               }
               return _dicStackActionTypeList;
           }
           set
           {
               _dicStackActionTypeList = value;
           }
       }
        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
