using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WHSE.Monitor.Framework.UserControls
{
	public class FaultPanelViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;



		private int _lv2FaultCount;
		public int LV2FaultCount
		{
			get
			{
				return _lv2FaultCount;
			}
			set
			{
				_lv2FaultCount = value;
				PropertyChanged(this, new PropertyChangedEventArgs("LV2FaultCount"));
			}
		}



		private DataTable _faultDataTable;
		public DataTable FaultDataTable
		{
			get
			{
				return _faultDataTable;
			}
			set
			{
				_faultDataTable = value;
				PropertyChanged(this, new PropertyChangedEventArgs("FaultDataTable"));
			}
		}

		private DataTable _dataSource;
		public DataTable DataSource
		{
			get { return _dataSource; }
			set { _dataSource = value; }
		}

		private string[] _selectedFaultLevel;
		public string[] SelectedFaultLevel
		{
			get { return _selectedFaultLevel; }
			set { _selectedFaultLevel = value; }
		}



	}
}
