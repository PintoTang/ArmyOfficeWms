using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace WHSE.Monitor.Framework.UserControls
{
	public class LoadAreaDevices
	{

		public void Load()
		{
			string NameSpace = ConfigurationManager.AppSettings["NameSpace"].ToString();
			LoadAreaFloor(NameSpace);
			FindDevice();
		}

		private void LoadAreaFloor(string NameSpace)
		{
			String path = Environment.CurrentDirectory + "\\Config\\" + ConfigurationManager.AppSettings["ConfigPath"].ToString() + "\\AreaConfig.xml";
			XElement xElement = XElement.Load(path);
			var xElementAreas = xElement.Elements();
			if (xElementAreas != null)
			{
				foreach (var areaElement in xElementAreas)
				{
					if (areaElement.Elements().Count() == 0)
					{
						Console.WriteLine("节点为空");
					}
					string areaName = areaElement.Attribute("AreaName").Value.ToString();
					string areaClassName = areaElement.Attribute("ClassName").Value.ToString();
					if ((AreaControlList.Instance.AreaBeanList.Where(x => x.AreaName == areaName).Count()) > 0)
					{
						continue;
					}
					CreateArea(NameSpace, areaName, areaClassName);
					CreateFloor(NameSpace, areaElement, areaName);

				}
			}
		}

		private void CreateArea(string NameSpace, string areaName, string areaClassName)
		{
			UserControl areaControl = CreateUserControl(NameSpace, areaClassName);
			AreaBean areaBean = new AreaBean
			{
				AreaName = areaName,
				AreaUserControl = areaControl
			};
			AreaControlList.Instance.AreaBeanList.Add(areaBean);
		}

		private void CreateFloor(string NameSpace, XElement areaElement, string areaName)
		{
			foreach (var floorElement in areaElement.Elements())
			{
				string floorName = floorElement.Attribute("FloorName").Value.ToString();
				int count = FloorControlList.Instance.FloorBeanList.Where(x => x.AreaName == areaName && x.FloorName == floorName).Count();
				if (count > 0)
				{
					continue;
				}
				string floorClassName = floorElement.Attribute("ClassName").Value.ToString();


				UserControl floorControl = CreateUserControl(NameSpace, floorClassName);
				FloorBean floorBean = new FloorBean
				{
					AreaName = areaName,
					FloorName = floorName,
					FloorClassName= floorClassName,
					FloorUserControl = floorControl
				};
				FloorControlList.Instance.FloorBeanList.Add(floorBean);

				AreaBean area = AreaControlList.Instance.AreaBeanList.Where(x => x.AreaName == areaName).FirstOrDefault();
				if (area!=null)
				{
					area.Children.Add(floorBean.FloorUserControl);
				}
				string strDefault = "";
				if (floorElement.Attribute("IsDefault") != null)
				{
					strDefault = floorElement.Attribute("IsDefault").Value;
				}
				
				

			}
		}

		public UserControl CreateUserControl(string nameSpace, string className)
		{
			Assembly assembly = Assembly.Load(nameSpace);
			object obj = assembly.CreateInstance(nameSpace + "." + className, true, BindingFlags.CreateInstance, null, null, null, null);
			UserControl uc = obj as UserControl;
			return uc;
		}



		private void FindDevice()
		{

			List<FloorBean> listFloor = FloorControlList.Instance.FloorBeanList;
			foreach (var areaBean in listFloor)
			{
				UserControl control = areaBean.FloorUserControl;
				FloorBean query = FloorControlList.Instance.FloorBeanList.Where(x => x.AreaName == areaBean.AreaName && areaBean.FloorName == areaBean.FloorName).FirstOrDefault();
				
				LookDevice((control.Content as Grid),query);
			}


		}



		private void LookDevice(Grid grid,FloorBean floor)
		{
			Console.WriteLine("查找设备");

			int count = VisualTreeHelper.GetChildrenCount(grid);
			for (int i = 0; i < count; i++)
			{
				var item = VisualTreeHelper.GetChild(grid, i);
				if (item is Grid)
				{
					LookDevice((item as Grid),floor);
					
				}
				else
				{
					PropertyInfo propertyInfo = item.GetType().GetProperty("UserControlName");
					if (propertyInfo != null)
					{
						if (floor != null)
						{							
							floor.Children.Add((UserControl)item);
						
						}

					}
				}
			}
		
		}
	}
}
