using System;
using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TaskDevice.RfidDoor.Common.ViewModel
{
    public class SendShowCmdViewModel : ViewModelBase, IInvokeCmd
    {
        public SendShowCmd DataModel { get; set; }
        public SendShowCmdViewModel(SendShowCmd dataModel)
        {
            DataModel = dataModel;
        }
        private Dictionary<MessageLevelTypeEnum, string> _dicMsgLevelList = new Dictionary<MessageLevelTypeEnum, string>();
        /// <summary>
        /// 地址集合
        /// </summary>
        public Dictionary<MessageLevelTypeEnum, string> DicMsgLevelList
        {
            get
            {
                if (_dicMsgLevelList.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(MessageLevelTypeEnum)))
                    {
                        MessageLevelTypeEnum em = (MessageLevelTypeEnum)value;
                        _dicMsgLevelList.Add(em, em.GetDescription());
                    }
                }
                return _dicMsgLevelList;
            }
            set
            {
                _dicMsgLevelList = value;
            }
        }

        public string GetCmd()
        {
            return DataModel.ToJson();
        }
    }
}
