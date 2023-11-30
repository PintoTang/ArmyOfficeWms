using Infrastructrue.Ioc.Interface;
using CL.WCS.Infrastructure.Ioc.Unity;

namespace Infrastructrue.Ioc.DependencyFactory
{
    public static class DependencyFactory
    {
		static DependencyFactory()
		{
			Dependency = new UnityDependency();
		}
	
		public static IDependency GetDependency()
		{
			return Dependency;
		}

		private static readonly IDependency Dependency;
	}
}
