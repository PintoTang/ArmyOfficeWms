using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WHSE.Monitor.Framework.UserControls
{
public	class AreaBean
	{

		private string _areaAreaName;

		public string AreaName
		{
			get { return _areaAreaName; }
			set { _areaAreaName = value; }
		}


		private UserControl _areaUserControl;

		public UserControl AreaUserControl
		{
			get { return _areaUserControl; }
			set { _areaUserControl = value; }
		}

		private List<UserControl> _children=new List<UserControl> ();

		public List<UserControl> Children
		{
			get { return _children; }
			set { _children = value; }
		}


		


	}
}
