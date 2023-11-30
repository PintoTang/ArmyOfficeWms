using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WHSE.Monitor.Framework.UserControls
{
	public class StackingCraneViewModelList
	{
		#region IntanceCode
		private static StackingCraneViewModelList instance;
		private StackingCraneViewModelList()
		{

		}
		public static StackingCraneViewModelList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new StackingCraneViewModelList();
				}
				return instance;
			}

		}
		#endregion
		private  List<StackingCraneViewModel> _viewModelList=new List<StackingCraneViewModel> ();

		public  List<StackingCraneViewModel> ViewModelList
		{
			get { return _viewModelList; }
			set { _viewModelList = value; }
		}

	}
}
