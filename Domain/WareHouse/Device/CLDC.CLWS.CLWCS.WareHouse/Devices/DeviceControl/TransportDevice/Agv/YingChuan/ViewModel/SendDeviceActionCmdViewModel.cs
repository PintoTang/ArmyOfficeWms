using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.YingChuan.ViewModel
{
   public class SendDeviceActionCmdViewModel: ViewModelBase, IInvokeCmd
    {
       public SendDeviceActionCmd DataModel { get; set; }

       public SendDeviceActionCmdViewModel(SendDeviceActionCmd dataModel)
        {
            DataModel = dataModel;
        }

       private Dictionary<ActionTypeEnum, string> _dicActionTypeList = new Dictionary<ActionTypeEnum, string>();
       /// <summary>
       /// 是否需要请求地址集合
       /// </summary>
       public Dictionary<ActionTypeEnum, string> DicAcitonTypeList
       {
           get
           {
               if (_dicActionTypeList.Count == 0)
               {
                   foreach (var value in Enum.GetValues(typeof(ActionTypeEnum)))
                   {
                       ActionTypeEnum em = (ActionTypeEnum)value;
                       _dicActionTypeList.Add(em, em.GetDescription());
                   }
               }
               return _dicActionTypeList;
           }
           set
           {
               _dicActionTypeList = value;
           }
       }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
