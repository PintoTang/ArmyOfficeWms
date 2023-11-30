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
	/// GoodsShelf.xaml 的交互逻辑
	/// </summary>
	public partial class GoodsShelf : UserControl
	{
		public GoodsShelf()
		{
			InitializeComponent();
		}
		private string _userControlName;
		/// <summary>
		/// 设备名称
		/// </summary>
		public string UserControlName
		{
			get { return _userControlName; }
			set { _userControlName = value; }
		}

		private int _gridNumber;
		/// <summary>
		/// 仓位数量（单层）
		/// </summary>
		public int GridNumber
		{
			get { return _gridNumber; }
			set { _gridNumber = value; }
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{


			if (CellWidth > 0)
			{
				LoadCellByCellWidth();
			}
			else
			{
				LoadCellsByGridNumber();
			}


		}

		private void LoadCellByCellWidth()
		{
		
			double width, height;
			double margin = CellMargin;
			width = sPanel.ActualWidth;
			height = CellWidth +CellMargin;

			CellNumber = (int)Math.Ceiling(sPanel.ActualHeight / height);

			for (int i = 0; i < CellNumber; i++)
			{
				Rectangle rectangle = new Rectangle
				{
					Height = CellWidth,
					Width = width,
					Margin = new Thickness(0, CellMargin, 0 , 0),
					StrokeThickness = 0,
					Fill = (Brush)this.FindResource("BorderBrush"),
					SnapsToDevicePixels=true
				};
				sPanel.Children.Add(rectangle);

			}
		}

		private void LoadCellsByGridNumber()
		{
			double width, height;
			int gridNumber;
			int margin = 20;
			if (GridNumber == 0)
			{
				//width = this.Width - margin * 2;
				//height = this.Width - margin * 3;
				width = sPanel.ActualWidth;
				height = sPanel.ActualWidth;
				gridNumber = (int)Math.Ceiling((sPanel.ActualHeight - margin) / sPanel.ActualWidth);
			}
			else
			{
				//width = this.Width - margin * 2;
				//height = (this.Height - 3 * margin) / (double)GridNumber;
				width = sPanel.ActualWidth;				
				height = (sPanel.ActualHeight) / (double)GridNumber;
				gridNumber = GridNumber;
			}

			for (int i = 0; i < gridNumber; i++)
			{
				Rectangle rectangle = new Rectangle
				{
					Height = height - margin,
					Width = width,
					//Margin = new Thickness(margin, i == 0 ? margin : 0, margin, margin),
					Margin = new Thickness(0, margin,  0, 0),
					StrokeThickness = 0,
					Fill = (Brush)this.FindResource("BorderBrush"),
					SnapsToDevicePixels = true
				};
				sPanel.Children.Add(rectangle);

			}
		}


		private double _cellWidth;
		/// <summary>
		/// 仓位的宽度
		/// </summary>
		public double CellWidth
		{
			get { return _cellWidth; }
			set { _cellWidth = value; }
		}

		private int _cellsNumber;

		/// <summary>
		/// 仓位的个数
		/// </summary>
		private int CellNumber
		{
			get { return _cellsNumber; }
			set { _cellsNumber = value; }
		}

		private double _cellMargin;

		public double CellMargin
		{
			get { return _cellMargin; }
			set { _cellMargin = value; }
		}
		
	

	}
}
