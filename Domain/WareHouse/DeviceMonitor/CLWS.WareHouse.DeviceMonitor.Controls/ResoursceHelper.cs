using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WHSE.Monitor.Framework.Model.DataModel;


namespace WHSE.Monitor.Framework.UserControls
{
	public class ResoursceHelper
	{
		/// <summary>
		/// 系统公共的资源
		/// </summary>
		public static ResourceDictionary CommResource
		{
			get
			{
				string packUri = @"/WHSE.Monitor.Framework.UserControls;component/CommonStyle/CommStyle.xaml";
				ResourceDictionary myResourceDictionary = Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
				return myResourceDictionary;
			}

		}
		public static ResourceDictionary FromResource(string resource)
		{
			try
			{
				string packUri = @"/WHSE.Monitor.Framework.UserControls;component/" + resource;
				ResourceDictionary myResourceDictionary = Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
				return myResourceDictionary;
			}
			catch (Exception ex)
			{

				throw ex;
			}
				
			

		}
	}
}
