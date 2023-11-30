using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.DataModelPckg
{
	/// <summary>
	/// 项目加载数据
	/// </summary>
	public class InstanceConfigData
	{
		public string LoadType
		{
			set;
			get;
		}

		public InstanceObjectData InstanceObjectData
		{
			set;
			get;
		}

		public DictionaryObjectData DictionaryObjectData
		{
			set;
			get;
		}

		public SingletonObjectData SingletonObjectData
		{
			set;
			get;
		}

		public MethodCallData MethodCallData
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 方法调用配置
	/// </summary>
	public class MethodCallData
	{
		/// <summary>
		/// 方法调用标识名称
		/// </summary>
		public string LoadName
		{
			set;
			get;
		}

		/// <summary>
		/// 方法名
		/// </summary>
		public string MethodName
		{
			set;
			get;
		}

		/// <summary>
		/// 方法调用入参集合
		/// </summary>
		public List<MethodCallParam> MethodCallParamList
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 方法调用入参
	/// </summary>
	public class MethodCallParam
	{
		/// <summary>
		/// 入参类型
		/// </summary>
		public string ParamType
		{
			set;
			get;
		}

		/// <summary>
		/// 入参值
		/// </summary>
		public string ParamValue
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 单例对象
	/// </summary>
	public class SingletonObjectData
	{
		/// <summary>
		/// 单例对象名称，用来标识单个配置项，唯一标识
		/// </summary>
		public string LoadName
		{
			set;
			get;
		}

		/// <summary>
		/// 创建对象的类名
		/// </summary>
		public string ClassName
		{
			set;
			get;
		}

		/// <summary>
		/// 创建对象的命名空间
		/// </summary>
		public string NameSpace
		{
			set;
			get;
		}

		/// <summary>
		/// 单例的方法名
		/// </summary>
		public string CreateMethodName
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 字典对象，实例化后提供给其它成员作为入参
	/// </summary>
	public class DictionaryObjectData
	{
		/// <summary>
		/// 字典名称，用来标识单个配置项，唯一标识
		/// </summary>
		public string LoadName
		{
			set;
			get;
		}

		/// <summary>
		/// 主键类型
		/// </summary>
		public string KeyType
		{
			set;
			get;
		}

		/// <summary>
		/// 值对象类型
		/// </summary>
		public string ValueType
		{
			set;
			get;
		}

		/// <summary>
		/// 字典成员对象集合
		/// </summary>
		public List<DictionaryParam> DictionaryParamList
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 字典成员对象
	/// </summary>
	public class DictionaryParam
	{
		/// <summary>
		/// 主键
		/// </summary>
		public string Key
		{
			set;
			get;
		}

		/// <summary>
		/// 值
		/// </summary>
		public string Value
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 实例化对象
	/// </summary>
	public class InstanceObjectData
	{
		/// <summary>
		/// 实体配置项名称，用来标识单个配置项，唯一标识
		/// </summary>
		public string LoadName
		{
			set;
			get;
		}

		/// <summary>
		/// 创建对象的类名
		/// </summary>
		public string ClassName
		{
			set;
			get;
		}

		/// <summary>
		/// 创建对象的命名空间
		/// </summary>
		public string NameSpace
		{
			set;
			get;
		}

		/// <summary>
		/// 加载模式，如真实\模拟
		/// </summary>
		public string LoadMode
		{
			set;
			get;
		}

		/// <summary>
		/// 是否是设备对象
		/// </summary>
		public bool IsDevice
		{
			set;
			get;
		}

		/// <summary>
		/// 是否需要在加载时判断设备当前状态，启用则正常加载，禁用则不加载
		/// </summary>
		public bool IsCheckStatus
		{
			set;
			get;
		}

		/// <summary>
		/// 构造函数入参对象集合
		/// </summary>List<ConstructParam>
		public List<ConstructParam> ConstructParamList
		{
			set;
			get;
		}

		/// <summary>
		/// 公共成员对象集合
		/// </summary>
		public List<SetterParam> SetterParamList
		{
			set;
			get;
		}

		/// <summary>
		/// 事件对象集合
		/// </summary>
		public List<EventParam> EventParamList
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 构造函数入参对象
	/// </summary>
	public class ConstructParam
	{
		/// <summary>
		/// 入参类型
		/// </summary>
		public string ParamType
		{
			set;
			get;
		}

		/// <summary>
		/// 入参值
		/// </summary>
		public string ParamValue
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 公共成员对象（需要实例化对象后再赋值的公共成员）
	/// </summary>
	public class SetterParam
	{
		/// <summary>
		/// 方法名称
		/// </summary>
		public string SetterName
		{
			set;
			get;
		}

		/// <summary>
		/// 对象类型
		/// </summary>
		public string SetterType
		{
			set;
			get;
		}

		/// <summary>
		/// 方法对象
		/// </summary>
		public string SetterValue
		{
			set;
			get;
		}
	}

	/// <summary>
	/// 事件对象
	/// </summary>
	public class EventParam
	{
		/// <summary>
		/// 事件名称
		/// </summary>
		public string EventName
		{
			set;
			get;
		}

		/// <summary>
		/// 绑定的方法名
		/// </summary>
		public string BindAppointMethodName
		{
			set;
			get;
		}

		/// <summary>
		/// 绑定的对象名
		/// </summary>
		public string BindAppointObjectName
		{
			set;
			get;
		}

		/// <summary>
		/// 绑定的方法所在的命名空间
		/// </summary>
		public string BindAppointNamespaceName
		{
			set;
			get;
		}

		/// <summary>
		/// 绑定的方法所在的对象类名
		/// </summary>
		public string BindAppointClassName
		{
			set;
			get;
		}
	}
}
