using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHSE.Monitor.Framework.Model.DataModel;

namespace WHSE.Monitor.Client.Model.DataPool
{
	public class DeviceStateBean
	{
		protected List<byte> _faultStateList = null;
		public List<byte> FaultStateList 
		{ 
			get
			{
				return _faultStateList;
			}
		}

		protected DateTime _lastRxFaultTime = new DateTime();
		public DateTime LastRxFaultTime
		{
			get
			{
				return _lastRxFaultTime;
			}
		}

		protected DeviceManualAutomaticEnum _deviceManualAutomaticEnum = DeviceManualAutomaticEnum.Automatic;
		public DeviceManualAutomaticEnum DeviceManualAutomaticEnum
		{
			get
			{
				return _deviceManualAutomaticEnum;
			}
		}
	}
}
