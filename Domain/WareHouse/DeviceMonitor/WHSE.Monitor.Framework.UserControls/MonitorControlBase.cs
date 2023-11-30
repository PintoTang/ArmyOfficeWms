using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WHSE.Monitor.Framework.UserControls
{
	public class MonitorControlBase
	{
		private Point _point;
		/// <summary>
		/// 控件在屏幕中的位置
		/// </summary>
		public Point ControlPoint
		{
			get { return _point; }
			set { _point = value; }
		}

		private double _width;
		/// <summary>
		/// 宽度
		/// </summary>
		public double Width
		{
			get { return _width; }
			set { _width = value; }
		}


		private double _height;
		/// <summary>
		/// 高度
		/// </summary>
		public double Height
		{
			get { return _height; }
			set { _height = value; }
		}


		private double _angle;
		/// <summary>
		/// 旋转角度
		/// </summary>
		public double Angle
		{
			get { return _angle; }
			set { _angle = value; }
		}


	}
}
