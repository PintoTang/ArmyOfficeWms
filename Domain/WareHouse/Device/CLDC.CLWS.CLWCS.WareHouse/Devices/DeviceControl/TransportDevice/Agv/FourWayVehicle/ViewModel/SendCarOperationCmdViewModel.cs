using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.ViewModel
{
    class SendCarOperationCmdViewModel: ViewModelBase, IInvokeCmd
    {
        public SendCarOperationCmd DataModel { get; set; }

        public SendCarOperationCmdViewModel(SendCarOperationCmd dataModel)
        {
            DataModel = dataModel;
        }
        private Dictionary<FourWayVehicleCarRunEnum, string> _dicCarWorkModeList = new Dictionary<FourWayVehicleCarRunEnum, string>();
        /// <summary>
        /// 杭叉车 工作状态集合
        /// </summary>
        public Dictionary<FourWayVehicleCarRunEnum, string> DicCarWorkModeList
        {
            get
            {
                if (_dicCarWorkModeList.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(FourWayVehicleCarRunEnum)))
                    {
                        FourWayVehicleCarRunEnum em = (FourWayVehicleCarRunEnum)value;
                        _dicCarWorkModeList.Add(em, em.GetDescription());
                    }
                }
                return _dicCarWorkModeList;
            }
            set
            {
                _dicCarWorkModeList = value;
            }
        }

        public string GetCmd()
        {
            return DataModel.ToString();
        }
    }
}
