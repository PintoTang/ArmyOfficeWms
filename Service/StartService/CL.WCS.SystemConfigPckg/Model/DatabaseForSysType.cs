using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.WCS.SystemConfigPckg.Model
{
   public enum DatabaseForSysType
    {
       [Description("Wcs系统本地库")]
       WcsLocal,
       [Description("自动化测试库")]
       AtsTest
    }
}
