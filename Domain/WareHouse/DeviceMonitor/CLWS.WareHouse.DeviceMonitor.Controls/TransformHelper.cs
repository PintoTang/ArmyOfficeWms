using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WHSE.Monitor.Framework.UserControls
{
	public class TransformHelper
	{
		/// <summary>
		/// 变换子对象让其向上或向左显示
		/// </summary>
		/// <param name="parent">父控件</param>
		/// <param name="element">子对象</param>
		/// <returns>转换结果</returns>
		public static bool ResetAngle(UserControl parent, UIElement element)
		{
			bool result = false;
			if (parent != null)
			{
				TransformGroup tg = parent.RenderTransform as TransformGroup;
				if (tg != null)
				{
					RotateTransform rt = tg.Children[2] as RotateTransform;
					var angle = rt.Angle;
					angle = Math.Abs(angle);
					angle = angle % 360;
					double resAngle = 0;
					if (angle < 90)
					{
						resAngle = 0;
					}
					else if (angle >= 90 && angle < 270)
					{
						resAngle = 180;
					}
					RotateTransform newrt = new RotateTransform(resAngle);
					element.RenderTransformOrigin = parent.RenderTransformOrigin;
					element.RenderTransform = newrt;
					result= true;
				}
			}
			return result;
			

		}


		public static bool ResetAngleUp(UserControl parent, UIElement element)
		{
			bool result = false;
			if (parent != null)
			{
				TransformGroup tg = parent.RenderTransform as TransformGroup;
				if (tg != null)
				{
					RotateTransform rt = tg.Children[2] as RotateTransform;
					var angle = rt.Angle;
					angle = Math.Abs(angle);
					angle = angle % 360;
					double resAngle = 0;
					if (angle < 90)
					{
						resAngle = -90;
					}
					else if (angle >= 90 )
					{
						resAngle = 90;
					}
					RotateTransform newrt = new RotateTransform(resAngle);
					element.RenderTransformOrigin = parent.RenderTransformOrigin;
					element.RenderTransform = newrt;
					result = true;
				}
			}
			return result;


		}
	}
}
