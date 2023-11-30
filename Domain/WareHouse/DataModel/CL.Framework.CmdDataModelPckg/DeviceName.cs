using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Infrastructrue.Ioc.DependencyFactory;

namespace CL.Framework.CmdDataModelPckg
{
	/// <summary>
	/// 设备名称类
	/// </summary>
	public class DeviceName
	{
        private readonly IWmsWcsArchitecture _wmsWcsDataArchitecture;
		/// <summary>
		/// <para>无效值</para>
		/// </summary>
		public const int INVALID_VALUE = -0x7FFFFFFF;

		/// <summary>
		/// <para>Description: 设备名称类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">设备名称类型</param>
		/// <param name="number">设备编号</param>
		public DeviceName(string type, int number)
			: this(type + "#" + number)
		{

		}

		/// <summary>
		/// <para>Description: 设备名称类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">设备名称类型</param>
		/// <param name="number">设备编号</param>
		/// <param name="area">区域号</param>
		public DeviceName(string type, int number, string area)
			: this(type + "#" + number + "_" + area)
		{

		}

		/// <summary>
		/// <para>Description: 设备名称类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">设备名称类型</param>
		/// <param name="number">设备编号</param>
		/// <param name="area">区域号</param>
		/// <param name="wareHouse">库房号</param>
		public DeviceName(string type, int number, string area, string wareHouse)
			: this(type + "#" + number + "_" + area + "_" + wareHouse)
		{

		}

		/// <summary>
		/// <para>Description: 设备名称类的构造函数</para>
		/// <para>Exception--: 当输入的设备名称字符串不符合协议格式时，抛出异常。异常内容如下：</para>
		/// <para>-----------# 1、无效设备名称。错误的设备名称为：deviceName</para>
		/// <para>-----------# 2、无效设备名称,设备编号不为有效数字。错误的设备名称为：deviceName</para> 
		/// </summary>
		/// <param name="deviceName">命令发送委托</param>
		public DeviceName(string deviceName)
		{
			this.fullName = deviceName;

			string[] deviceNameResolve = deviceName.Split('#');

			if (deviceNameResolve.Count() != 2)
			{
				throw new Exception("无效设备名称。错误的设备名称为：" + deviceName);
			}

			this.type = deviceNameResolve[0];
			string[] nameValues = deviceNameResolve[1].Split('_');

			int nameValuesCount = nameValues.Count();
			if (nameValuesCount != 1 && nameValuesCount != 2 && nameValuesCount != 3)
			{
				throw new Exception("无效设备名称。错误的设备名称为：" + deviceName);
			}

			if (!int.TryParse(nameValues[0], out this.number))
			{
				throw new Exception("无效设备名称,设备编号不为有效数字。错误的设备名称为：" + deviceName);
			}

			if (nameValues.Count() == 2)
			{
				this.area = nameValues[1];
			}
			else if (nameValues.Count() == 3)
			{
				this.area = nameValues[1];
				this.wareHouse = nameValues[2];
			}

			this.coordinateName = deviceNameResolve[1];

            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();

		}

		/// <summary>
		/// <para>Description: 返回设备名称全名字符串。主要用于打印时可以在打印内容中直接+对象</para>
		/// </summary>
		public override string ToString()
		{
            if (_wmsWcsDataArchitecture != null)
            {
                OperateResult<string> getShowName = _wmsWcsDataArchitecture.WcsToShowName(FullName);
                if (getShowName.IsSuccess)
                {
                    return getShowName.Content;
                }
            }
			return this.FullName;
		}

		/// <summary>
		/// <para>Description: 和类object的GetHashCode()等效</para>
		/// </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
            result = prime * result + ((FullName == null) ? 0 : FullName.GetHashCode());
            result = prime * result + ((Type == null) ? 0 : Type.GetHashCode());
            result = prime * result + ((CoordinateName == null) ? 0 : CoordinateName.GetHashCode());
            result = prime * result + ((Area == null) ? 0 : Area.GetHashCode());
            result = prime * result + ((WareHouse == null) ? 0 : WareHouse.GetHashCode());
			return result;
		}

		/// <summary>
		/// <para>Description: 重写的Equals方法。只会判断设备名称是否在数值上相等。</para>
		/// <para>-----------# 不会死板地要求全名字符串完全匹配。</para>
		/// </summary>
		/// <param name="obj">与之比较的对象</param>
		public override bool Equals(object obj)
		{
			if (obj is DeviceName)
			{
				DeviceName deviceName = obj as DeviceName;

				if (this.type != deviceName.type)
				{
					return false;
				}

				if (this.number != deviceName.number)
				{
					return false;
				}

				if (this.area != deviceName.area)
				{
					return false;
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// <para>Description: 重写的==运算符。判断标准同重写的Equals方法。</para>
		/// </summary>
		/// <param name="lhs">比较的对象</param>
		/// <param name="rhs">比较的对象</param>
		public static bool operator ==(DeviceName lhs, DeviceName rhs)
		{
			object lhsObject = lhs;
			object rhsObject = rhs;

			if (lhsObject != null)
			{
				if (rhsObject == null)
				{
					return false;
				}
				else
				{
					return lhs.Equals(rhs);
				}
			}
			else
			{
				if (rhsObject == null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// <para>Description: 重写的!=运算符。判断标准是重写的Equals方法求反。</para>
		/// </summary>
		/// <param name="lhs">比较的对象</param>
		/// <param name="rhs">比较的对象</param>
		public static bool operator !=(DeviceName lhs, DeviceName rhs)
		{
			return !(lhs == rhs);
		}

		/// <summary>
		/// <para>Description: 克隆一个一样内容的新对象。</para>
		/// </summary>
		public DeviceName Clone()
		{
			return new DeviceName(this.FullName);
		}

		private string fullName;
		/// <summary>
		/// <para>Description: 克隆一个一样内容的新对象。</para>
		/// </summary>
		public string FullName
		{
			get
			{
				return fullName;
			}
		}

		private string type;
		/// <summary>
		/// <para>get: 获取设备名称类型。协议中“#”前面的部分</para>
		/// </summary>
		public string Type
		{
			get
			{
				return type;
			}
		}

		private string coordinateName;
		/// <summary>
		/// <para>get: 获取设备坐标全称。协议中“#”后面的部分</para>
		/// </summary>
		public string CoordinateName
		{
			get
			{
				return coordinateName;
			}
		}

		private int number;
		/// <summary>
		/// <para>get: 获取设备名称编号。协议中“#”后面的值</para>
		/// </summary>
		public int Number
		{
			get { return number; }
		}

		private string area;
		/// <summary>
		/// 库区
		/// </summary>
		public string Area
		{
			get
			{
				return area;
			}
		}

		private string wareHouse;
		/// <summary>
		/// <para>仓房编号</para>
		/// </summary>
		public string WareHouse
		{
			get
			{
				return wareHouse;
			}
		}

		/// <summary>
		/// <para>Description: 判断this对象是否是入参的前缀。（前半部分相同）</para>
		/// </summary>
		/// <param name="deviceFullName">被判断的设备名称字符串</param>
		public bool IsContain(string deviceFullName)
		{
			DeviceName deviceName = new DeviceName(deviceFullName);

			return IsContain(deviceName);
		}

		/// <summary>
		/// <para>Description: 判断this对象是否是入参的前缀。（前半部分相同）</para>
		/// </summary>
		/// <param name="deviceName">被判断的设备名称类对象</param>
		public bool IsContain(DeviceName deviceName)
		{
			if (this.Type != deviceName.Type)
			{
				return false;
			}

			if ((this.Number != INVALID_VALUE) && (this.Number != deviceName.Number))
			{
				return false;
			}

			return true;
		}
	}
}
