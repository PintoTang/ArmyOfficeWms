using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.FourWayVehicle.ViewModel
{

    public class SendAddTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {

        public SendAddTaskCmd DataModel { get; set; }

        public SendAddTaskCmdViewModel(SendAddTaskCmd dataModel)
        {
            DataModel = dataModel;
        }
        private List<FourWayVehicleCarWalkingPath> _walkingPathList=new List<FourWayVehicleCarWalkingPath>();


        private List<FourWayVehicleCarWalkingPath> _locationFromWalkingPathList=new List<FourWayVehicleCarWalkingPath>();
       /// <summary>
       /// 任务起点
       /// </summary>
        public List<FourWayVehicleCarWalkingPath> LocationFromWalkingPathList
        {
            get 
            {
                if (_locationFromWalkingPathList.Count == 0)
                {
                    _locationFromWalkingPathList = FourWayVehicleCarWalkingPathManage.Instance.WalkingPathList;
                }
                return _locationFromWalkingPathList;
            }
            set
            {
                _locationFromWalkingPathList=value;
            }
        }

        private List<FourWayVehicleCarWalkingPath> _locationToWalkingPathList=new List<FourWayVehicleCarWalkingPath>();
       /// <summary>
       /// 任务终点
       /// </summary>
        public List<FourWayVehicleCarWalkingPath> LocationToWalkingPathList
        {
            get 
            {
                if (_locationToWalkingPathList.Count == 0)
                {
                    _locationToWalkingPathList = FourWayVehicleCarWalkingPathManage.Instance.WalkingPathList;
                }
                return _locationToWalkingPathList;
            }
            set
            {
                _locationToWalkingPathList=value;
            }
        }

      
        public string GetCmd()
        {
            return DataModel.ToString();
        }
    }
}