using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WHSE.Monitor.Framework.UserControls
{
	public class FloorBean
	{
		private string _areaName;

		public string AreaName
		{
			get { return _areaName; }
			set { _areaName = value; }
		}



		private string _floorName;

		public string FloorName
		{
			get { return _floorName; }
			set { _floorName = value; }
		}



		private string _floorClassName;

		public string FloorClassName
		{
			get { return _floorClassName; }
			set { _floorClassName = value; }
		}

		private UserControl _floorUserControl;

		public UserControl FloorUserControl
		{
			get { return _floorUserControl; }
			set { _floorUserControl = value; }
		}


		private List<UserControl> _children=new List<UserControl>();

		public List<UserControl> Children
		{
			get { return _children; }
			set { _children = value; }
		}
		
		
	}
}
