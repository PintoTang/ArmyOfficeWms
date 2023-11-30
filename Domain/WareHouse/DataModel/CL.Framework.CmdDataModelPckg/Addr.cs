using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using Infrastructrue.Ioc.DependencyFactory;

namespace CL.Framework.CmdDataModelPckg
{
	/// <summary>
	/// 地址类。用于解析协议格式的地址。
	/// </summary>
	public class Addr
	{
        /// <summary>
        /// WMS与WCS基础数据对象
        /// </summary>
        private readonly IWmsWcsArchitecture _wmsWcsDataArchitecture;
		/// <summary>
		/// <para>无效值</para>
		/// </summary>
		public const int INVALID_VALUE = -0x7FFFFFFF;

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="area">库区</param>
		public Addr(string type, string area)
			: this(type + ":" + area)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="area">库区</param>
		/// <param name="wareHouse">库房号</param>
		public Addr(string type, string area, string wareHouse)
			: this(type + ":" + area + "_" + wareHouse)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		public Addr(string type, int range)
			: this(type + ":" + range)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="area">库区</param>
		public Addr(string type, int range, string area)
			: this(type + ":" + range + "_" + area)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="area">库区</param>
		/// <param name="wareHouse">库房</param>
		public Addr(string type, int range, string area, string wareHouse)
			: this(type + ":" + range + "_" + area + "_" + wareHouse)
		{

		}
		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="area">库区</param>
		/// <param name="wareHouse">库房</param>

		public Addr(string type, int range, int row, string area, string wareHouse)
			: this(type + ":" + range + "_" + row + "_" + area + "_" + wareHouse)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		public Addr(string type, int range, int row)
			: this(type + ":" + range + "_" + row)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="area">库区</param>
		public Addr(string type, int range, int row, string area)
			: this(type + ":" + range + "_" + row + "_" + area)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="column">列</param>
		public Addr(string type, int range, int row, int column)
			: this(type + ":" + range + "_" + row + "_" + column)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="column">列</param>
		/// /// <param name="depth">仓库深浅位</param>
		public Addr(string type, int range, int row, int column, int depth)
			: this(type + ":" + range + "_" + row + "_" + column + "_" + depth)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="column">列</param>
		/// <param name="area">库区</param>
		public Addr(string type, int range, int row, int column, string area)
			: this(type + ":" + range + "_" + row + "_" + column + "_" + area)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="column">列</param>
		/// <param name="area">库区</param>
		/// <param name="wareHouse">库房</param>
		public Addr(string type, int range, int row, int column, string area, string wareHouse)
			: this(type + ":" + range + "_" + row + "_" + column + "_" + area + "_" + wareHouse)
		{

		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="column">列</param>
		/// <param name="depth">仓库深浅位</param>
		/// <param name="area">库区</param>
		public Addr(string type, int range, int row, int column, int depth, string area)
			: this(type + ":" + range + "_" + row + "_" + column + "_" + depth + "_" + area)
		{

		}

		/// <summary>
		///  <para>Description: 地址类的构造函数</para>
		///  <para>Exception--: 无</para>
		/// </summary>
		/// <param name="type">地址类型</param>
		/// <param name="range">排</param>
		/// <param name="row">层</param>
		/// <param name="column">列</param>
		/// <param name="depth">货位深浅</param>
		/// <param name="area">库区</param>
		/// <param name="wareHouse">库房</param>
		public Addr(string type, int range, int row, int column, int depth, string area, string wareHouse)
			: this(type + ":" + range + "_" + row + "_" + column + "_" + depth + "_" + area + "_" + wareHouse)
		{
		}

		/// <summary>
		/// <para>Description: 地址类的构造函数</para>
		/// <para>Exception--: 当输入的地址字符串不符合协议格式时，抛出异常。异常内容如下：</para>
		/// <para>-----------# 1、无效地址。错误的地址为：addr</para>
		/// <para>-----------# 2、无效地址,坐标无效。错误的地址为：addr</para> 
		/// <para>-----------# 3、无效地址,地址坐标内不是有效的数字字符串。错误的地址为：addr</para> 
		/// </summary>
		/// <param name="addr">命令发送委托</param>
		public Addr(string addr)
		{
			string areaInFullName = "";
			string wareHouseInFullName = "";
			this.fullName = addr;
			string[] addrResolve = addr.Split(':');
			if (addrResolve.Count() != 2)
			{
				throw new Exception("无效地址。错误的地址为：" + addr);
			}
			if (addrResolve[1].EndsWith("_"))
			{
				throw new Exception("格式错误,地址不能以下划线（_）结尾,错误地址为：" + fullName);
			}
			string typeStr = addrResolve[0];
			string[] addrCoordinate = addrResolve[1].Split('_');
			if (addrCoordinate.Count() > 6)
			{
				throw new Exception("无效地址,坐标无效。错误的地址为：" + addr);
			}
			int indexOfArea = addrCoordinate.Count();
			for (int i = 0; i < addrCoordinate.Count(); i++)
			{
				if (checkArea(addrCoordinate[i]))
				{
					indexOfArea = i;
					areaInFullName = addrCoordinate[i];
					break;
				}
				else if (i < coordinates.Count())
				{
					if (addrCoordinate[i] != "")
					{
						int tmp = 0;
						bool isSuccess = int.TryParse(addrCoordinate[i], out tmp);
						if (isSuccess)
						{
							coordinates[i] = tmp;
						}
						else
						{
							throw new Exception("无效地址,地址内的排/层/列/深度不是有效的数字字符串。错误的地址为：" + addr);
						}
					}
				}
				else
				{
					throw new Exception("无效地址,坐标维度超出范围。错误的地址为：" + fullName);
				}

			}

			if (indexOfArea == addrCoordinate.Count() - 2)
			{
				wareHouseInFullName = addrCoordinate[addrCoordinate.Count() - 1];
			}
			if (indexOfArea < addrCoordinate.Count() - 2)
			{
				throw new Exception("无效地址,地址内的仓库/库区出现意外的位置。错误的地址为：" + fullName);
			}

			this.coordinateName = addrResolve[1];
			paramHander(typeStr, coordinates[0], coordinates[1], coordinates[2], coordinates[3], areaInFullName, wareHouseInFullName);
            _wmsWcsDataArchitecture = DependencyHelper.GetService<IWmsWcsArchitecture>();
		}

		private bool checkArea(string addr)
		{
			if (Regex.IsMatch(addr, "^[a-zA-Z][0-9a-zA-Z]{0,2}"))
			{
				return true;
			}
			return false;
		}

		private bool checkWareHouse(string addr)
		{
			if (addr.Count() > 0 && addr.Count() <= 3)
			{
				return true;
			}
			return false;
		}

		private bool checkDepth(int depth)
		{
			if (depth >= 0 || depth == INVALID_VALUE)
			{
				return true;
			}
			return false;
		}

		private void paramHander(String type, int range, int row, int column,
			int depth, string area, string wareHouse)
		{
			string str = "";
			if (str.Equals(type))
			{
				throw new Exception("无效地址,地址类型不能为空。" + type);
			}
			if (!checkDepth(depth))
			{
				throw new Exception("无效地址,地址内的深度不可小于0,当前的仓位深浅度为：" + depth);
			}
			if (!str.Equals(area) && !checkArea(area))
			{
				throw new Exception("无效地址,库区地址格式错误。" + area);
			}
			if (!str.Equals(wareHouse) && !checkWareHouse(wareHouse))
			{
				throw new Exception("无效地址,库房地址格式错误,长度应在1到3之间,错误的库房地址为：" + wareHouse);
			}
			if (str.Equals(area) && !str.Equals(wareHouse))
			{
				throw new Exception("无效地址,指定库房时应同时指定库区。");
			}
			this.type = type;
			this.area = area;
			this.wareHouse = wareHouse;
		}

		/// <summary>
		/// <para>Description: 返回地址全名字符串。主要用于打印时可以在打印内容中直接+对象</para>
		/// </summary>
		public override string ToString()
		{
            if (_wmsWcsDataArchitecture!=null)
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
			const int prime = 29;
			int result = 1;
            result = prime * result + ((FullName == null) ? 0 : FullName.GetHashCode());
            result = prime * result + ((Type == null) ? 0 : Type.GetHashCode());
            result = prime * result + ((CoordinateName == null) ? 0 : CoordinateName.GetHashCode());
            result = prime * result + ((Area == null) ? 0 : Area.GetHashCode());
            result = prime * result + ((WareHouse == null) ? 0 : WareHouse.GetHashCode());
			return result;
		}

		/// <summary>
		/// <para>Description: 重写的Equals方法。只会判断地址内容是否在数值上相等。</para>
		/// <para>-----------# 不会死板地要求全名字符串完全匹配。</para>
		/// </summary>
		/// <param name="obj">与之比较的对象</param>
		public override bool Equals(object obj)
		{
			if (obj is Addr)
			{
				Addr addr = obj as Addr;

				if (this.type != addr.type)
				{
					return false;
				}

				int coordinatesCount = this.coordinates.Count();
				for (int i = 0; i < coordinatesCount; i++)
				{
					if (this.coordinates[i] != addr.coordinates[i])
					{
						return false;
					}
				}
				if (this.area != addr.area)
				{
					return false;
				}
				if (this.wareHouse != addr.wareHouse)
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
		public static bool operator ==(Addr lhs, Addr rhs)
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
		public static bool operator !=(Addr lhs, Addr rhs)
		{
			return !(lhs == rhs);
		}

		/// <summary>
		/// <para>Description: 克隆一个一样内容的新对象。</para>
		/// </summary>
		public Addr Clone()
		{
			return new Addr(this.FullName);
		}

		private int[] coordinates = new int[4] { INVALID_VALUE, INVALID_VALUE, INVALID_VALUE, INVALID_VALUE };

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
		/// <para>get: 获取地址类型。协议中“:”前面的部分</para>
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
		/// <para>get: 获取地址坐标全称。协议中“:”后面的部分</para>
		/// </summary>
		public string CoordinateName
		{
			get
			{
				return coordinateName;
			}
		}

		/// <summary>
		/// <para>get: 获取地址排号。协议中“:”后面的第1个值</para>
		/// </summary>
		public int Range
		{
			get { return coordinates[0]; }
		}

		/// <summary>
		/// <para>get: 获取地址层号。协议中“:”后面的第2个值</para>
		/// </summary>
		public int Row
		{
			get { return coordinates[1]; }
		}

		/// <summary>
		/// <para>get: 获取地址列号。协议中“:”后面的第3个值</para>
		/// </summary>
		public int Column
		{
			get { return coordinates[2]; }
		}

		/// <summary>
		/// <para>get: 获取深浅位。协议中“:”后面的第4个值</para>
		/// </summary>
		public int Depth
		{
			get
			{
				return coordinates[3];
			}
		}

		private string area = string.Empty;
		/// <summary>
		/// <para>get: 获取地址库区号。协议中“:”后面的第1个字母</para>
		/// </summary>
		public string Area
		{
			get
			{
				return area;
			}
		}

		private string wareHouse = string.Empty;

		/// <summary>
		/// <para>get: 获取地址仓库编号。协议中“:”后面的第2个字母</para>
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
		/// <param name="addrFullName">被判断的地址字符串</param>
		public bool IsContain(string addrFullName)
		{
			Addr addr = new Addr(addrFullName);
			return IsContain(addr);
		}

		/// <summary>
		/// <para>Description: 判断this对象是否是入参的前缀。（前半部分相同）</para>
		/// </summary>
		/// <param name="addr">被判断的地址类对象</param>
		/// <param name="isIgnoreAddrType">是否忽略地址类型</param>
		/// <returns></returns>
		public bool IsContain(Addr addr, bool isIgnoreAddrType = false)
		{
			if ((this.Column != INVALID_VALUE) && (this.Column != addr.Column))
			{
				return false;
			}

			if ((this.Row != INVALID_VALUE) && (this.Row != addr.Row))
			{
				return false;
			}

			if ((this.Range != INVALID_VALUE) && (this.Range != addr.Range))
			{
				return false;
			}

			if (this.Depth != INVALID_VALUE && (this.Depth != addr.Depth))
			{
				return false;
			}

			if (this.Area != string.Empty && (this.Area != addr.Area))
			{
				return false;
			}

			if (this.WareHouse != string.Empty && (this.WareHouse != addr.WareHouse))
			{
				return false;
			}

            if (!isIgnoreAddrType && (this.Type != addr.Type) && (this.Type != "*"))
			{
                return false;
			}

			return true;
		}
	}
}
