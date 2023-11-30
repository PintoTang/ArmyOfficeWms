using Infrastructrue.Ioc.Interface;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.Infrastructure.Ioc.Unity
{
    public class UnityDependency : IDependency, IDisposable
    {
        private readonly IUnityContainer _container;
        /// <summary>
        /// 实例化一个Unity注册类
        /// </summary>
        /// <param name="container"></param>
        public UnityDependency(IUnityContainer container = null)
        {
            this._container = new UnityContainer();
            if (container != null)
            {
                this._container = container;
            }
        }

        /// <summary>
        /// 根据名称获取服务 名称可选
        /// </summary>
        /// <typeparam name="T">获取服务的类型</typeparam>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public T GetService<T>(string name = null)
        {
            T result = default(T);
            result = string.IsNullOrEmpty(name) ? this._container.Resolve<T>() : this._container.Resolve<T>(name);
            return result;
        }

        /// <summary>
        /// 根据名称获取指定的类型的服务
        /// </summary>
        /// <param name="serviceType">指定的类型</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public object GetService(Type serviceType, string name = null)
        {
            return string.IsNullOrEmpty(name) ? this._container.Resolve(serviceType) : this._container.Resolve(serviceType, name);
        }
        /// <summary>
        /// 根据名称获取服务列表 名称可选
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public IEnumerable<T> GetServices<T>(string name = null)
        {
            List<T> result = new List<T>();
            result = this._container.ResolveAll<T>().ToList<T>();
            return result;
        }
        ///<summary>
        /// 根据名称获取指定的类型的服务列表
        /// </summary>
        /// <param name="serviceType">指定的类型</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType, string name = null)
        {
            var result = this._container.ResolveAll(serviceType).ToList<object>();
            return result;
        }

        /// <summary>
        /// 延迟加载指定类型的服务
        /// </summary>
        /// <typeparam name="T">名称</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public Lazy<T> GetLazyService<T>(string name = null)
        {
            return new Lazy<T>(() => this.GetService<T>(name));
        }
        /// <summary>
        /// 延迟加载指定类型的服务列表
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public Lazy<IEnumerable<T>> GetLazyServices<T>(string name = null)
        {
            return new Lazy<IEnumerable<T>>(() => this.GetServices<T>(name));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this._container.Dispose();
        }

        public IUnityContainer RegisterType<TFrom, TTo>(params InjectionMember[] injectionMembers)where TTo:TFrom
        {
           return this._container.RegisterType<TFrom, TTo>(injectionMembers);
        }

        public IUnityContainer RegisterType<TFrom, TTo>(string name, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
           return this._container.RegisterType<TFrom, TTo>(name);
        }

        public IUnityContainer RegisterType<TFrom, TTo>(string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return this._container.RegisterType<TFrom, TTo>(name, lifetimeManager);
        }

        public IUnityContainer RegisterInstance<TInterface>(TInterface instance)
        {
            return this._container.RegisterInstance<TInterface>(instance);
        }

        public IUnityContainer RegisterInstance<TInterface>(string name,TInterface instance)
        {
            return this._container.RegisterInstance<TInterface>(name,instance);
        }

        public IUnityContainer RegisterInstance<TInterface>(TInterface instance, LifetimeManager lifetimeManager)
        {
            return this._container.RegisterInstance<TInterface>(instance, lifetimeManager);
        }

        public IUnityContainer RegisterType<TFrom, TTo>(LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return this._container.RegisterType<TFrom, TTo>(lifetimeManager);
        }
    }
}
