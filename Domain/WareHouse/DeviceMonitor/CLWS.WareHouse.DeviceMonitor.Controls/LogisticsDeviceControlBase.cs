using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace WHSE.Monitor.Framework.UserControls
{
	public class LogisticsDeviceControlBase : MonitorControlBase
	{
		private double _velocity;
		/// <summary>
		/// 运动速度
		/// </summary>
		public double Velocity
		{
			get { return _velocity; }
			set { _velocity = value; }
		}

		private Point _beginPonit;
		/// <summary>
		/// 运动的起点
		/// </summary>
		public Point BeginPonit
		{
			get { return _beginPonit; }
			set { _beginPonit = value; }
		}

		private Point _endPoint;
		/// <summary>
		/// 运动的终点
		/// </summary>
		public Point EndPonit
		{
			get { return _endPoint; }
			set { _endPoint = value; }
		}


		
		private object _moveLine;
		/// <summary>
		/// 动画路线
		/// </summary>
		public object MoveLine
		{
			get { return _moveLine; }
			set { _moveLine = value; }
		}



	}
}
