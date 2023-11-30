using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.OPCClientAbsPckg
{
    public interface IOpcClientFactory
    {
        OPCClientAbstract Create();
    }
}
