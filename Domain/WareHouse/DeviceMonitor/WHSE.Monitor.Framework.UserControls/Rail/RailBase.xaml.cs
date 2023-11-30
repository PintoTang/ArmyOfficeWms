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
	/// RailBase.xaml 的交互逻辑
	/// </summary>
	public partial class RailBase : UserControl
	{
		public RailBase()
		{
			InitializeComponent();
		}

		private double _interval = 20;

		public double Interval 
		{
			get { 
				return _interval; 
			} 
			set {
				_interval = value;
			}
		}






		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			double x = this.Width;
			int count = (int) (x / _interval);

			for (int i = 0; i < count; i++)
			
			{
				string xaml = System.Windows.Markup.XamlWriter.Save(line);
				Grid grid = System.Windows.Markup.XamlReader.Parse(xaml) as Grid;
				grid.Margin = new Thickness(_interval, 0, 0, 0);
				grid.SnapsToDevicePixels = true;
				sPanel.Children.Add(grid);
				

			}
		}

		private double _lineWidth;

		public double LineWidth
		{
			get { return _lineWidth; }
			set
			{
				_lineWidth = value;
				topline.Height = value;
				footline.Height = value;
				line.Width = value;

			}
		}




	}
}
