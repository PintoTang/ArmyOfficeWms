using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.ConfigService
{
    public class FunctionMenuInfo
    {
        public int Id;
        public string Name;
        public string FunctionName;

        public List<FuncBusinessHandle> FuncBusinessHandleLst;
    }
    public class FuncBusinessHandle
    {
        public int Id;
        public string Name;
        public string Type;
        public string Class;
        public string NameSpace;
        public FuncBusinessHandleConfig FuncBusinessHandleConfig;

    }
    public class FuncBusinessHandleConfig
    {
        public List<FuncBusinessHandleConfigModule> Modules = new List<FuncBusinessHandleConfigModule>();
    }
    public class FuncBusinessHandleConfigModule
    {
        public string Name;
        public string Type;
        public string Class;
        public string NameSpace;
    }
}
