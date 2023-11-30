using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Framework.UserControls
{
	public class RgvViewModelList
	{
		#region IntanceCode
		private static RgvViewModelList instance;
		private RgvViewModelList()
		{

		}
		public static RgvViewModelList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new RgvViewModelList();
				}
				return instance;
			}

		}
		#endregion

		private List<RgvViewModel> _viewModelList = new List<RgvViewModel>();
		public List<RgvViewModel> ViewModelList
		{
			get { return _viewModelList; }
			set { _viewModelList = value; }
		}

	}
}
