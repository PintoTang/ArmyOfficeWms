using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CL.WCS.Monitor.DataModel;

namespace WHSE.Monitor.Framework.UserControls
{
	public class SysGroupViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public event Action StateChanged;


		public SysGroupViewModel() {

			
		}
		private string _groupName;
		/// <summary>
		/// 系统组名称
		/// </summary>
		public string GroupName
		{
			get { return _groupName; }
			set { _groupName = value; }
		}


		private SysStateEnum _groupState;
		/// <summary>
		/// 系统组的连接状态
		/// </summary>
		public SysStateEnum GroupState
		{
			get
			{
				Application.Current.Dispatcher.Invoke(new Action(() => {
					int countOffline = Children.Where(x => (((SysInfoViewModel)x.DataContext).SysState == SysStateEnum.OFFLINE)).Count();

					if (countOffline == 0)
					{
						int countPause = Children.Where(x => (((SysInfoViewModel)x.DataContext).SysState == SysStateEnum.PAUSE)).Count();

						if (countPause > 0)
						{
							_groupState = SysStateEnum.PAUSE;
						}
						else
						{
							_groupState = SysStateEnum.RUNNING;
						}
					}
					else
					{
						_groupState = SysStateEnum.OFFLINE;
					}

				}));
				
			
				return _groupState;
			}
			set
			{
				_groupState = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("GroupState"));
				}

				if (StateChanged != null)
				{
					StateChanged();
				}
			
			}
		}




		private List<SysInfo> _children = new List<SysInfo>();
		public List<SysInfo> Children
		{
			get
			{
				return _children;
			}
			set
			{
				_children = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Children"));
				}
			}
		}


		public void ChangeGroupState(string name, SysStateEnum state)
		{
			GroupState = state;
		}

		public string NNN = DateTime.Now.ToString("fff");

	}
}

