using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WHSE.Monitor.Framework.UserControls
{
	/// <summary>
	/// FlautPanel.xaml 的交互逻辑
	/// </summary>
	public partial class FaultPanel : UserControl
	{


		List<CheckBox> cbxList = new List<CheckBox>();
		public FaultPanel()
		{
			InitializeComponent();
			//DataContext =  FaultInfo.Instance.FaultDataSource;
			dgFault.DataContext = FaultInfo.Instance.DataSource;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			InitCheckBox();

			
			
		}

		private void InitCheckBox()
		{
			currentArea.IsChecked = true;
			cbxList.Add(LV2);
			cbxList.Add(LV3);
			cbxList.Add(LV4);
			cbxList.Add(LV5);
		}

		private void CbxAll_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in cbxList)
			{
				if (CbxAll.IsChecked == true)
				{
					item.IsChecked = true;
				}
				else
				{
					item.IsChecked = false;
				}

			}

		}
	}
}
