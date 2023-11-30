using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
   public class TaskExcuteMessage<TTask> where TTask:IDeviceTaskContent
    {
       public TaskExcuteMessage(DeviceBaseAbstract device, TTask task, TaskExcuteStatusType taskExcuteStatus)
       {
           ExcuteTaskStatus = taskExcuteStatus;
           ExcuteTask = task;
           ExcuteDevice = device;
           ExcuteStepStatus= TaskExcuteStepStatusEnum.Finished;
       }

       public TaskExcuteMessage(DeviceBaseAbstract device, TTask task)
       {
           ExcuteTaskStatus= TaskExcuteStatusType.NoException;
           ExcuteTask = task;
           ExcuteDevice = device;
       }

       public TaskExcuteMessage(DeviceBaseAbstract device, TTask task, TaskExcuteStatusType taskExcuteStatus, TaskExcuteStepStatusEnum stepStatus)
		{
			ExcuteTaskStatus = taskExcuteStatus;
            ExcuteTask = task;
			ExcuteDevice = device;
           ExcuteStepStatus = stepStatus;
		}

       /// <summary>
       /// 任务执行的进度
       /// </summary>
       public TaskExcuteStepStatusEnum ExcuteStepStatus { get; set; }

		/// <summary>
		/// 任务执行状态
		/// </summary>
		public TaskExcuteStatusType ExcuteTaskStatus { get; set; }

        public TTask ExcuteTask { get; set; }

		/// <summary>
		/// 指令设备
		/// </summary>
        public DeviceBaseAbstract ExcuteDevice { get; set; }

	}
}
