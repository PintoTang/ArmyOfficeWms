using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.Monitor.DataModel;
using System.Collections.ObjectModel;

namespace WHSE.Monitor.Framework.UserControls
{
    public class DeviceInfoFormViewModel : INotifyPropertyChanged
    {
        public DeviceInfoFormViewModel(string DeviceName) { 
        
        
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private DeviceStateEnum _deviceStateEnun;
        public DeviceStateEnum DeviceState
        {
            get
            {
                return _deviceStateEnun;
            }
            set
            {
                _deviceStateEnun = value;
                PropertyChanged(this, new PropertyChangedEventArgs("DeviceState"));
            }
        }



		private ObservableCollection<PackageBean> _packageList=new ObservableCollection<PackageBean> ();
		public ObservableCollection<PackageBean> PackageList
		{
			get
			{

			
				return _packageList;
			}
			set
			{
				_packageList = value;
				
			}
		}
		

        
    }
}
