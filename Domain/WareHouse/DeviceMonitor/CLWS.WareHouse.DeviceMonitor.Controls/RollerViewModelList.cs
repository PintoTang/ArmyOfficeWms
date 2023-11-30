using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class RollerViewModelList
	{
		#region IntanceCode
		private static RollerViewModelList instance;
		private RollerViewModelList()
		{

		}
		public static RollerViewModelList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new RollerViewModelList();
				}
				return instance;
			}

		}
		#endregion
		private List<RollerViewModel> _rollerViewModelList = new List<RollerViewModel>();

		public List<RollerViewModel> ViewModelList
		{
			get
			{
				return _rollerViewModelList;
			}
			set
			{
				_rollerViewModelList = value;
			}
		}


		


	}
}
