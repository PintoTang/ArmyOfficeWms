using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.HangCha.ViewModel
{

    public class SendAddTaskCmdViewModel : ViewModelBase, IInvokeCmd
    {

        public SendAddTaskCmd DataModel { get; set; }

        public SendAddTaskCmdViewModel(SendAddTaskCmd dataModel)
        {
            DataModel = dataModel;
        }
        private List<HangChaAgvCarWalkingPath> _walkingPathList=new List<HangChaAgvCarWalkingPath>();


        private List<HangChaAgvCarWalkingPath> _locationFromWalkingPathList=new List<HangChaAgvCarWalkingPath>();
       /// <summary>
       /// 任务起点
       /// </summary>
        public List<HangChaAgvCarWalkingPath> LocationFromWalkingPathList
        {
            get 
            {
                if (_locationFromWalkingPathList.Count == 0)
                {
                    _locationFromWalkingPathList = HangChaAgvCarWalkingPathManage.Instance.WalkingPathList;
                }
                return _locationFromWalkingPathList;
            }
            set
            {
                _locationFromWalkingPathList=value;
            }
        }

        private List<HangChaAgvCarWalkingPath> _locationToWalkingPathList=new List<HangChaAgvCarWalkingPath>();
       /// <summary>
       /// 任务终点
       /// </summary>
        public List<HangChaAgvCarWalkingPath> LocationToWalkingPathList
        {
            get 
            {
                if (_locationToWalkingPathList.Count == 0)
                {
                    _locationToWalkingPathList = HangChaAgvCarWalkingPathManage.Instance.WalkingPathList;
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