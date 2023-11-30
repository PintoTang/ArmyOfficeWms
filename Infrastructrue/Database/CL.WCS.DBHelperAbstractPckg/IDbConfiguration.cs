using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.DbHelper
{
   public interface IDbConfiguration
   {
       string GetConnectionString();
   }
}
