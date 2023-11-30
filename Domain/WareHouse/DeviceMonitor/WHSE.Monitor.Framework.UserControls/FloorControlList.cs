using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WHSE.Monitor.Framework.UserControls
{
	public class FloorControlList
	{
		#region IntanceCode
		private static FloorControlList instance;
		private FloorControlList()
		{

		}
		public static FloorControlList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new FloorControlList();
				}
				return instance;
			}

		}
		#endregion
		








		private List<FloorBean> _floorBeanList = new List<FloorBean>();

		public List<FloorBean> FloorBeanList
		{
			get { return _floorBeanList; }
			set { _floorBeanList = value; }
		}



		public DeviceBase FindDevice(string deviceName)
		{
			DeviceBase device = null;
			bool result = false;

			foreach (var floor in FloorBeanList)
			{
				if (result)
				{
					break;
				}
				foreach (var item in floor.Children)
				{
					if (result)
					{
						break;
					}
					item.Dispatcher.Invoke(new Action(() =>
					{
						if (item.DataContext != null)
						{
							Console.WriteLine("FindDevice :" + (item.DataContext as DeviceBase).DeviceName);
							if ((item.DataContext as DeviceBase).DeviceName == deviceName)
							{
								device = item.DataContext as DeviceBase;
								result = true;
							}
						}

					}));
				}
			}
			return device;


		}
		
	}
}
