using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructrue.Ioc.DependencyFactory
{
    public static class DependencyHelper
    {
        public static T GetService<T>()
        {
            return DependencyFactory.GetDependency().GetService<T>();
        }

        public static T GetService<T>(string flagName)
        {
            return DependencyFactory.GetDependency().GetService<T>(flagName);
        }

    }
}
