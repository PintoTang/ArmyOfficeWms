using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructrue.Ioc.Interface
{
	/// <summary>
	/// 依赖注入的解析者
	/// </summary>
    public interface IDependency
    {
		/// <summary>
		/// 根据名称获取服务 名称可选
		/// </summary>
		/// <typeparam name="T">获取服务的类型</typeparam>
		/// <param name="name">名称</param>
		/// <returns></returns>
		T GetService<T>(string name = null);
		/// <summary>
		/// 根据名称获取指定的类型的服务
		/// </summary>
		/// <param name="serviceType">指定的类型</param>
		/// <param name="name">名称</param>
		/// <returns></returns>
		object GetService(Type serviceType, string name = null);
		/// <summary>
		/// 根据名称获取服务列表 名称可选
		/// </summary>
		/// <typeparam name="T">指定类型</typeparam>
		/// <param name="name">名称</param>
		/// <returns></returns>
		IEnumerable<T> GetServices<T>(string name = null);
		/// 根据名称获取指定的类型的服务列表
		/// </summary>
		/// <param name="serviceType">指定的类型</param>
		/// <param name="name">名称</param>
		/// <returns></returns>
		IEnumerable<object> GetServices(Type serviceType, string name = null);
		/// <summary>
		/// 延迟加载指定类型的服务
		/// </summary>
		/// <typeparam name="T">名称</typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		Lazy<T> GetLazyService<T>(string name = null);
		/// <summary>
		/// 延迟加载指定类型的服务列表
		/// </summary>
		/// <typeparam name="T">指定类型</typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		Lazy<IEnumerable<T>> GetLazyServices<T>(string name = null);

		 IUnityContainer  RegisterType<TFrom, TTo>(params InjectionMember[] injectionMembers) where TTo : TFrom;

		IUnityContainer RegisterType<TFrom, TTo>(LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom;

		IUnityContainer RegisterType<TFrom, TTo>(string name, params InjectionMember[] injectionMembers) where TTo : TFrom;

		IUnityContainer RegisterType<TFrom, TTo>(string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom;

		IUnityContainer RegisterInstance<TInterface>(TInterface instance);

        IUnityContainer RegisterInstance<TInterface>(string name,TInterface instance);

		IUnityContainer RegisterInstance<TInterface>(TInterface instance, LifetimeManager lifetimeManager);

	}
}
