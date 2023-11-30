using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class HoisterViewModelList
	{
		#region IntanceCode
		private static HoisterViewModelList instance;
		private HoisterViewModelList()
		{

		}
		public static HoisterViewModelList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new HoisterViewModelList();
				}
				return instance;
			}

		}
		#endregion
		private List<HoisterViewModel> _viewModelList = new List<HoisterViewModel>();

		public List<HoisterViewModel> ViewModelList
		{
			get { return _viewModelList; }
			set { _viewModelList = value; }
		}
	}

		
}
