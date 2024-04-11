using IdGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Tools
{
    public class Snowflake
    {
        private static IdGenerator generator;
        private Snowflake() { }
        static Snowflake()
        {
            generator = new IdGenerator(0);
        }

        public static Int64 NewId()
        {
            return generator.CreateId();
        }

        public static IList<Int64> NewIdList(int count)
        {
            return generator.Take(count).ToList();
        }
    }
}
